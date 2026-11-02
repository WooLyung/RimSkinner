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
        private GraphicSuit? graphicSuit = null;
        private int expandedSection;
        private SkinEditorContent content = new();

        private Vector2 buildingScroll;
        private Vector2 skinScroll;
        private Vector2 graphicScroll;

        public override Vector2 InitialSize => new Vector2(Mathf.Min(1100f, UI.screenWidth - 40f), Mathf.Min(750f, UI.screenHeight - 40f));

        public static void Open(ThingDef? buildingDef = null)
        {
            Find.WindowStack.Add(new SkinEditor(buildingDef));
        }

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
            {
                SelectBuilding(buildingDef);
                expandedSection = 1;
            }
        }

        public override void PostClose()
        {
            content.SelectSkin(null);
            base.PostClose();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Event.current.type == EventType.MouseDown)
                GUI.FocusControl(null);

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
            content.SelectSkin(SelectedSkin);
            DrawEditorButtons(new Rect(rect.x, rect.y, rect.width, 38f));
            rect.yMin += 44f;
            const float padding = 12f;
            const float gap = 6f;

            float infoWidth = rect.width - padding * 2f;
            string graphicPath = SelectedGraphicPath ?? "";
            GameFont previousFont = Text.Font;
            Text.Font = GameFont.Small;
            float pathHeight = Mathf.Max(24f, Text.CalcHeight(graphicPath, Mathf.Max(1f, infoWidth - Mathf.Min(90f, infoWidth * 0.25f) - 12f)));
            Text.Font = previousFont;

            Rect infoPanel = new Rect(rect.x, rect.y, rect.width, padding * 2f + 52f + pathHeight);
            Widgets.DrawMenuSection(infoPanel);
            Rect info = infoPanel.ContractedBy(padding);
            DrawSelectionInfo(new Rect(info.x, info.y, info.width, 24f), "RimSkinner.Editor.Buildings".Translate(), "RimSkinner.Editor.SelectedBuildings.Tooltip".Translate(), SelectedBuildingDef?.LabelCap.ToString(), SelectedBuildingDef?.defName);
            DrawSelectionInfo(new Rect(info.x, info.y + 26f, info.width, 24f), "RimSkinner.Editor.Skins".Translate(), "RimSkinner.Editor.SelectedSkins.Tooltip".Translate(), SelectedSkin?.Name, SelectedSkin?.Id);
            DrawSelectionInfo(new Rect(info.x, info.y + 52f, info.width, pathHeight), "RimSkinner.Editor.Graphics".Translate(), "RimSkinner.Editor.SelectedGraphics.Tooltip".Translate(), graphicPath, null);

            if (SelectedBuildingDef == null || SelectedSkin == null || SelectedGraphicPath == null)
            {
                PrintCenterMessage("RimSkinner.Editor.NoSelected".Translate());
                return;
            }

            if (graphicSuit == null)
                return;

            if (!graphicSuit.Slots.TryGetValue(SelectedGraphicPath, out GraphicSlot? slot) || slot == null)
                return;

            if (!SkinEditorContent.SupportedGraphics.Contains(slot.SourceData.graphicClass))
            {
                PrintCenterMessage("RimSkinner.Editor.UnsupportedGraphicClass".Translate());
                return;
            }

            if (!SelectedSkin.Graphics.TryGetValue(SelectedGraphicPath, out var selectedGraphic))
                return;
            content.InitContent(slot, selectedGraphic, SelectedBuildingDef, SelectedSkin, SelectedGraphicPath);
            content.DrawContent(new Rect(rect.x, infoPanel.yMax + gap, rect.width, Mathf.Max(0f, rect.yMax - infoPanel.yMax - gap)));

            void PrintCenterMessage(string message)
            {
                Rect messageRect = new Rect(rect.x, infoPanel.yMax + gap, rect.width, Mathf.Max(0f, rect.yMax - infoPanel.yMax - gap));
                TextAnchor previousAnchor = Text.Anchor;
                GameFont messageFont = Text.Font;
                try
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.Font = GameFont.Small;
                    Widgets.Label(messageRect.ContractedBy(12f), message);
                }
                finally
                {
                    Text.Anchor = previousAnchor;
                    Text.Font = messageFont;
                }
            }
        }

        private static readonly string[] EditorButtonKeys =
        {
            "ChangeId", "Rename", "Save", "Revert", "Reset", "Duplicate", "Delete", "Export", "Import"
        };

        private void DrawEditorButtons(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect inner = rect.ContractedBy(4f);
            const float gap = 4f;
            float width = (inner.width - gap * (EditorButtonKeys.Length - 1)) / EditorButtonKeys.Length;
            if (width <= 0f) 
                return;

            GameFont previousFont = Text.Font;
            TextAnchor previousAnchor = Text.Anchor;
            try
            {
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.MiddleCenter;

                if (SelectedSkin != null)
                {
                    for (int i = 0; i < EditorButtonKeys.Length; i++)
                    {
                        Rect button = new Rect(inner.x + i * (width + gap), inner.y, width, inner.height);
                        string label = ("RimSkinner.Editor.Actions." + EditorButtonKeys[i]).Translate();
                        TooltipHandler.TipRegion(button, ("RimSkinner.Editor.Actions." + EditorButtonKeys[i] + ".Tooltip").Translate());

                        if (Widgets.ButtonText(button, label.Truncate(width - 6f)))
                        {
                            switch (i)
                            {
                                case 0:
                                    EditSkinText(true);
                                    break;
                                case 1:
                                    EditSkinText(false);
                                    break;
                                case 2:
                                    content.Save();
                                    break;
                                case 3:
                                    content.Revert();
                                    break;
                                case 4:
                                    content.Reset();
                                    break;
                                case 5:
                                    DuplicateSkin();
                                    break;
                                case 6:
                                    DeleteSkin();
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < EditorButtonKeys.Length; i++)
                    {
                        Rect button = new Rect(inner.x + i * (width + gap), inner.y, width, inner.height);
                        string label = ("RimSkinner.Editor.Actions." + EditorButtonKeys[i]).Translate();
                        Widgets.DrawBoxSolid(button, new Color(0f, 0f, 0f, 0.2f));
                        Widgets.Label(button.ContractedBy(3f), label.Truncate(width - 6f));
                        TooltipHandler.TipRegion(button, ("RimSkinner.Editor.Actions." + EditorButtonKeys[i] + ".Tooltip").Translate());
                    }
                }
            }
            finally 
            {
                Text.Anchor = previousAnchor;
                Text.Font = previousFont; 
            }
        }
        private void EditSkinText(bool editId)
        {
            if (SelectedSkin == null) 
                return;

            SkinData selected = SelectedSkin;
            string action = editId ? "ChangeId" : "Rename";
            Find.WindowStack.Add(new SkinTextDialog(("RimSkinner.Editor.Actions." + action).Translate(), editId ? selected.Id : selected.Name, value =>
                {
                    string preName = selected.Name;

                    if (string.IsNullOrWhiteSpace(value))
                        return "RimSkinner.Editor.Actions.EmptyValue".Translate();

                    if (editId)
                    {
                        if (!SkinLibrary.TryChangeId(selected, value))
                            return "RimSkinner.Editor.Actions.DuplicateId".Translate();
                    }
                    else 
                        selected.Name = value;
                   
                    Messages.Message(("RimSkinner.Editor.Actions." + action + ".Completed").Translate(preName, value), MessageTypeDefOf.TaskCompletion, historical: false);
                    return null;
                }));
        }

        private void DuplicateSkin()
        {
            if (SelectedSkin == null || SelectedBuildingDef == null)
                return;

            var skins = SkinLibrary.GetSkins(SelectedBuildingDef);
            string prefix = SelectedSkin.Id + "_Copy";

            int number = 1;
            while (skins.Any(item => string.Equals(item.Id.Trim(), prefix + number, StringComparison.OrdinalIgnoreCase)))
                number++;

            SkinData copy = content.CreateDuplicate(prefix + number);
            copy.Name += $" ({number})";

            if (!SkinLibrary.TryAdd(copy)) 
                return;

            SelectedSkin = copy;
            content.SelectSkin(copy);
            buildingSkins = SkinLibrary.GetSkins(SelectedBuildingDef);
            skinScroll.y = buildingSkins.Count * 30f;

            Messages.Message("RimSkinner.Editor.Actions.Duplicate.Completed".Translate(copy.Name), MessageTypeDefOf.TaskCompletion, historical: false);
        }

        private void DeleteSkin()
        {
            if (SelectedSkin == null) return;
            SkinData selected = SelectedSkin;
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation(
                "RimSkinner.Editor.Actions.Delete.Confirm".Translate(selected.Name, selected.Id), () =>
                {
                    if (!SkinLibrary.Remove(selected)) 
                        return;
                    if (ReferenceEquals(SelectedSkin, selected))
                    {
                        SelectedSkin = null;
                        SelectedGraphicPath = null;
                        content.SelectSkin(null);
                    }
                    if (SelectedBuildingDef != null)
                        buildingSkins = SkinLibrary.GetSkins(SelectedBuildingDef);

                    Messages.Message("RimSkinner.Editor.Actions.Delete.Completed".Translate(selected.Name), MessageTypeDefOf.TaskCompletion, historical: false);
                }, destructive: true));
        }
        private void DrawSelectionInfo(Rect rect, string label, string tooltip, string? value, string? detail)
        {
            GameFont previousFont = Text.Font;
            Color previousColor = GUI.color;
            TextAnchor previousAnchor = Text.Anchor;

            try
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
                float labelWidth = Mathf.Min(90f, rect.width * 0.25f);
                GUI.color = new Color(0.65f, 0.65f, 0.65f);
                Widgets.Label(new Rect(rect.x, rect.y, labelWidth, 24f), label.Truncate(labelWidth));
                Rect valueRect = new Rect(rect.x + labelWidth + 12f, rect.y, rect.width - labelWidth - 12f, rect.height);
                GUI.color = Color.white;

                if (string.IsNullOrEmpty(detail))
                    Widgets.Label(valueRect, value ?? "");
                else
                {
                    float nameWidth = Mathf.Min(Text.CalcSize(value ?? "").x, valueRect.width * 0.6f);
                    Widgets.Label(new Rect(valueRect.x, valueRect.y, nameWidth, 24f), (value ?? "").Truncate(nameWidth));
                    Text.Font = GameFont.Tiny;
                    GUI.color = new Color(0.65f, 0.65f, 0.65f);
                    float detailWidth = Mathf.Max(1f, valueRect.width - nameWidth - 10f);
                    Widgets.Label(new Rect(valueRect.x + nameWidth + 10f, valueRect.y + 3f, detailWidth, 21f), detail.Truncate(detailWidth));
                }

                TooltipHandler.TipRegion(rect, tooltip);
            }
            finally
            {
                Text.Font = previousFont;
                Text.Anchor = previousAnchor;
                GUI.color = previousColor;
            }
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

            graphicSuit = ThingDefGraphics.GetGraphicSuit(def);
            buildingSkins = SkinLibrary.GetSkins(def);
            graphicPaths = graphicSuit.Slots.Keys.ToList();
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
                    SelectGraphicPath(item);
            }
            Widgets.EndScrollView();
        }

        private void SelectGraphicPath(string item)
        {
            SelectedGraphicPath = item;
            content.Clear();

            if (graphicSuit == null)
                return;
            if (SelectedBuildingDef == null)
                return;
            if (SelectedSkin == null)
                return;
            if (!graphicSuit.Slots.TryGetValue(SelectedGraphicPath, out GraphicSlot? slot) || slot == null)
                return;
            if (!SelectedSkin.Graphics.TryGetValue(SelectedGraphicPath, out SkinGraphicData? target) || target == null)
                return;

            content.InitContent(
                slot,
                target,
                SelectedBuildingDef,
                SelectedSkin,
                SelectedGraphicPath
            );
        }
    }
}
