using RimSkinner.Management;
using RimSkinner.Skin;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinEditor : Window
    {
        private static List<ThingDef>? buildingDefs = null;
        private static IReadOnlyList<ThingDef> BuildingDefs
        {
            get
            {
                if (buildingDefs == null)
                {
                    buildingDefs = DefDatabase<ThingDef>.AllDefsListForReading
                        .Where(def => def.category == ThingCategory.Building)
                        .Where(def => ThingDefGraphics.GetGraphicSuitTemporary(def).Slots.Count != 0)
                        .OrderBy(def => def.LabelCap.ToString())
                        .ThenBy(def => def.defName)
                        .ToList();
                }
                return buildingDefs.AsReadOnly();
            }
        }

        public ThingDef? SelectedBuildingDef { get; private set; }
        public SkinData? SelectedSkin { get; private set; }
        public string? SelectedGraphicPath { get; private set; }

        private IReadOnlyList<ThingDef> filteredBuildingDefs;
        private string buildingSearch = string.Empty;
        private IReadOnlyList<SkinData> buildingSkins = Array.Empty<SkinData>();
        private List<string> graphicPaths = new();
        private int expandedSection;
        private Vector2 buildingScroll;
        private Vector2 skinScroll;
        private Vector2 graphicScroll;
        public override Vector2 InitialSize => new Vector2(Mathf.Min(1100f, UI.screenWidth - 40f), Mathf.Min(750f, UI.screenHeight - 40f));

        public static void Open(ThingDef? buildingDef = null) => Find.WindowStack.Add(new SkinEditor(buildingDef));

        public SkinEditor(ThingDef? buildingDef = null)
        {
            filteredBuildingDefs = BuildingDefs;
            optionalTitle = "RimSkinner".Translate();
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;
            closeOnAccept = false;
            doCloseX = true;

            if (buildingDef != null)
                SelectBuilding(buildingDef);
        }

        public override void DoWindowContents(Rect inRect)
        {
            float width = Mathf.Min(315f, inRect.width * 0.315f);
            const float headerHeight = 32f;
            const float gap = 6f;
            float bodyHeight = Mathf.Max(0f, inRect.height - headerHeight * 3f - gap * 2f);
            float y = inRect.y;
            int activeSection = expandedSection;
            string[] titles = { "RimSkinner.Editor.Buildings".Translate(), "RimSkinner.Editor.Skins".Translate(), "RimSkinner.Editor.Graphics".Translate() };
            string[] tooltips = { "RimSkinner.Editor.Buildings.Tooltip".Translate(), "RimSkinner.Editor.Skins.Tooltip".Translate(), "RimSkinner.Editor.Graphics.Tooltip".Translate() };

            for (int section = 0; section < 3; section++)
            {
                bool expanded = section == activeSection;
                Rect header = new Rect(inRect.x, y, width, headerHeight);
                Widgets.DrawOptionUnselected(header);
                TooltipHandler.TipRegion(header, tooltips[section].Translate());

                Widgets.Label(new Rect(header.x + 10f, header.y + 5f, header.width - 20f, header.height - 5f), titles[section]);
                if (Widgets.ButtonInvisible(header, false))
                {
                    if (section == 2 && (SelectedBuildingDef == null || SelectedSkin == null))
                    {
                        Messages.Message("RimSkinner.Editor.Graphics.NoSelectedBuildingOrSkin".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                    }
                    else if (section == 1 && SelectedBuildingDef == null)
                    {
                        Messages.Message("RimSkinner.Editor.Skins.NoSelectedBuilding".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                    }
                    else
                    {
                        expandedSection = section;
                        GUI.FocusControl(null);
                    }
                }

                y += headerHeight;
                if (expanded && bodyHeight > 0f)
                {
                    Rect body = new Rect(inRect.x, y, width, bodyHeight);
                    switch (section)
                    {
                        case 0:
                            DrawBuildings(body);
                            break;
                        case 1:
                            DrawSkins(body);
                            break;
                        case 2:
                            DrawGraphics(body);
                            break;
                    }
                    y += bodyHeight;
                }

                y += gap;
            }

            DrawEditor(new Rect(inRect.x + width + gap, inRect.y, inRect.width - width - gap, inRect.height));
        }

        private void DrawEditor(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            if (SelectedBuildingDef == null || SelectedSkin == null || SelectedGraphicPath == null)
                return;
        }

        private void DrawBuildings(Rect rect)
        {
            Widgets.DrawMenuSection(rect);

            GUI.DrawTexture(new Rect(rect.x + 8f, rect.y + 10f, 20f, 20f), TexButton.Search);
            Rect searchRect = new Rect(rect.x + 36f, rect.y + 6f, rect.width - 44f, 28f);
            string search = Widgets.TextField(searchRect, buildingSearch);
            TooltipHandler.TipRegion(searchRect, "RimSkinner.Editor.Buildings.Search.Tooltip".Translate());

            if (search != buildingSearch)
            {
                buildingSearch = search;
                string query = search.Trim();
                filteredBuildingDefs = query.Length == 0 ? BuildingDefs : BuildingDefs
                    .Where(def => def.LabelCap.ToString().Contains(query, StringComparison.OrdinalIgnoreCase)
                        || def.defName.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                buildingScroll = Vector2.zero;
            }

            const float rowHeight = 30f;
            Rect outer = new Rect(rect.x + 1f, rect.y + 40f, rect.width - 2f, Mathf.Max(1f, rect.height - 41f));
            Rect view = new Rect(0f, 0f, Mathf.Max(1f, outer.width - 16f), filteredBuildingDefs.Count * rowHeight);
            Widgets.BeginScrollView(outer, ref buildingScroll, view);
         
            for (int i = 0; i < filteredBuildingDefs.Count; i++)
            {
                Rect row = new Rect(0f, i * rowHeight, view.width, rowHeight);
                if (row.yMax < buildingScroll.y || row.y > buildingScroll.y + outer.height)
                    continue;
                ThingDef def = filteredBuildingDefs[i];
                if (def == SelectedBuildingDef)
                    Widgets.DrawHighlightSelected(row);
                Widgets.DrawHighlightIfMouseover(row);
                Widgets.Label(new Rect(6f, row.y + 5f, row.width - 12f, row.height - 5f),
                    def.LabelCap.ToString().Truncate(Mathf.Max(1f, row.width - 12f)));
                TooltipHandler.TipRegion(row, $"{def.LabelCap} ({def.defName})");
                if (Widgets.ButtonInvisible(row))
                    SelectBuilding(def);
            }

            Widgets.EndScrollView();
        }
        private void SelectBuilding(ThingDef def)
        {
            if (def == SelectedBuildingDef)
                return;

            if (SelectedBuildingDef != null && SkinLibrary.GetSkins(SelectedBuildingDef).Count == 0)
                ThingDefGraphics.RemoveCachedGraphics(SelectedBuildingDef);

            SelectedBuildingDef = def;
            SelectedSkin = null;
            SelectedGraphicPath = null;
            skinScroll = Vector2.zero;
            graphicScroll = Vector2.zero;
            buildingSkins = SkinLibrary.GetSkins(def);
            graphicPaths = ThingDefGraphics.GetGraphicSuit(def).Slots.Keys.ToList();
        }

        private void DrawSkins(Rect rect)
        {
            const float rowHeight = 30f;
            Widgets.DrawMenuSection(rect);
            bool wasEnabled = GUI.enabled;
            GUI.enabled = wasEnabled && SelectedBuildingDef != null;
            Rect createRect = new Rect(rect.x + 6f, rect.y + 6f, rect.width - 12f, 28f);

            if (Widgets.ButtonText(createRect, "RimSkinner.Editor.Skins.CreateSkin".Translate()))
                CreateSkin(Mathf.Max(1f, rect.height - 41f));
            TooltipHandler.TipRegion(createRect, "RimSkinner.Editor.Skins.CreateSkin.Tooltip".Translate());

            GUI.enabled = wasEnabled;

            Rect outer = new Rect(rect.x + 1f, rect.y + 40f, rect.width - 2f, Mathf.Max(1f, rect.height - 41f));
            Rect view = new Rect(0f, 0f, Mathf.Max(1f, outer.width - 16f), buildingSkins.Count * rowHeight);
            Widgets.BeginScrollView(outer, ref skinScroll, view);
           
            for (int i = 0; i < buildingSkins.Count; i++)
            {
                Rect row = new Rect(0f, i * rowHeight, view.width, rowHeight);
                if (row.yMax < skinScroll.y || row.y > skinScroll.y + outer.height)
                    continue;

                var item = buildingSkins[i];
                if (item == SelectedSkin)
                    Widgets.DrawHighlightSelected(row);
                Widgets.DrawHighlightIfMouseover(row);
                Widgets.Label(new Rect(6f, row.y + 5f, row.width - 12f, row.height - 5f),
                    item.Name.Truncate(Mathf.Max(1f, row.width - 12f)));
                TooltipHandler.TipRegion(row, $"{item.Name} ({item.Id})");
                if (Widgets.ButtonInvisible(row))
                    SelectedSkin = item;
            }

            Widgets.EndScrollView();
        }
        private void CreateSkin(float listHeight)
        {
            if (SelectedBuildingDef == null)
                return;

            var skins = SkinLibrary.GetSkins(SelectedBuildingDef);
            int number = 1;
            while (skins.Any(skin => string.Equals(skin.Id.Trim(), $"NewSkin_{number}", StringComparison.OrdinalIgnoreCase)))
                number++;

            string id = $"NewSkin_{number}";
            SkinData created = SkinData.Create(SelectedBuildingDef.defName, id, $"{"RimSkinner.Editor.Skins.NewSkin".Translate()} {number}");
            if (!SkinLibrary.TryAdd(created))
                return;

            buildingSkins = SkinLibrary.GetSkins(SelectedBuildingDef);
            SelectedSkin = created;
            skinScroll.y = Mathf.Max(0f, buildingSkins.Count * 30f - listHeight);
        }

        private void DrawGraphics(Rect rect)
        {
            const float rowHeight = 30f;

            Widgets.DrawMenuSection(rect);
            Rect outer = new Rect(rect.x + 1f, rect.y + 1f, rect.width - 2f, rect.height - 2f);
            Rect view = new Rect(0f, 0f, Mathf.Max(1f, outer.width - 16f), graphicPaths.Count * rowHeight);
            Widgets.BeginScrollView(outer, ref graphicScroll, view);

            for (int i = 0; i < graphicPaths.Count; i++)
            {
                Rect row = new Rect(0f, i * rowHeight, view.width, rowHeight);
                if (row.yMax < graphicScroll.y || row.y > graphicScroll.y + outer.height)
                    continue;
                var item = graphicPaths[i];
                if (item == SelectedGraphicPath)
                    Widgets.DrawHighlightSelected(row);
                Widgets.DrawHighlightIfMouseover(row);
                Widgets.Label(new Rect(6f, row.y + 5f, row.width - 12f, row.height - 5f),
                    item.Truncate(Mathf.Max(1f, row.width - 12f)));
                TooltipHandler.TipRegion(row, item);
                if (Widgets.ButtonInvisible(row))
                    SelectedGraphicPath = item;
            }
            Widgets.EndScrollView();
        }
    }
}
