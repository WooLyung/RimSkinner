using RimSkinner.Management;
using RimSkinner.Skin;
using RimSkinner.WorldComponents;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RimSkinner.Core
{
    public static class Graphic_Patcher
    {
        private sealed record Slot(string Path, GraphicData Data);
        private static readonly ConditionalWeakTable<ThingDef, Dictionary<Graphic, Slot?>> slots = new();
        private static readonly ConditionalWeakTable<SkinGraphicData, Dictionary<Graphic, Replacement>> replacements = new();

        public static Graphic? Resolve(Graphic original, Thing? thing)
        {
            if (thing is not Building || Find.World == null || (original.GetType() != typeof(Graphic_Single) && original.GetType() != typeof(Graphic_Multi)))
                return null;
            if (!WorldComponent_SkinAssignments.Instance.Assignments.TryGetValue(thing, out string? id))
                return null;

            SkinData? skin = null;
            foreach (var candidate in SkinLibrary.GetSkins(thing.def))
            {
                if (string.Equals(candidate.Id, id, StringComparison.OrdinalIgnoreCase))
                {
                    skin = candidate;
                    break;
                }
            }
            if (skin == null || !skin.IsCompatible)
                return null;

            var lookup = slots.GetValue(thing.def, _ => new());
            if (!lookup.TryGetValue(original, out Slot? slot))
            {
                var suit = ThingDefGraphics.GetGraphicSuit(thing.def);
                foreach (var pair in suit.Slots)
                {
                    if (ReferenceEquals(pair.Value.SourceGraphic, original))
                    {
                        slot = new Slot(pair.Key, pair.Value.SourceData);
                        break;
                    }
                }

                if (slot == null)
                {
                    foreach (var pair in suit.Slots)
                    {
                        if (!ReferenceEquals(pair.Value.SourceData, original.data)) 
                            continue;
                        if (slot != null) 
                        { 
                            slot = null; 
                            break; 
                        }
                        slot = new Slot(pair.Key, pair.Value.SourceData);
                    }
                }

                lookup[original] = slot;
            }

            if (slot == null || !skin.Graphics.TryGetValue(slot.Path, out var values) || values is not (SkinGraphicSingle or SkinGraphicMulti))
                return null;
            if ((original is Graphic_Multi) != (values is SkinGraphicMulti)) 
                return null;

            var cache = replacements.GetValue(values, _ => new());
            if (!cache.TryGetValue(original, out var result))
            {
                result = new Replacement(original, slot.Data, values);
                cache.Add(original, result);
            }

            return result;
        }

        public static void Dirty(Thing thing)
        {
            if (thing.Spawned && !thing.Destroyed) thing.DirtyMapMesh(thing.Map);
        }

        private sealed class Replacement : Graphic
        {
            private readonly Material[] materials = new Material[4];
            private readonly Graphic source;
            private readonly SkinGraphicData values;
            public override Material MatSingle => materials[values is SkinGraphicMulti ? 2 : 0];
            public override Material MatNorth => materials[0];
            public override Material MatEast => materials[1];
            public override Material MatSouth => materials[2];
            public override Material MatWest => materials[3];
            public override Material MatAt(Rot4 rot, Thing? thing = null) => materials[rot.AsInt];
            public override bool EastFlipped => data.allowFlip && source.EastFlipped && (values is not SkinGraphicMulti multi || multi.TextureEast == null);
            public override bool WestFlipped => data.allowFlip && source.WestFlipped && (values is not SkinGraphicMulti multi || multi.TextureWest == null);
            public override bool ShouldDrawRotated => data.drawRotated && (values is SkinGraphicSingle || source.ShouldDrawRotated && values is SkinGraphicMulti m && m.TextureEast == null && m.TextureWest == null);
            public override float DrawRotatedExtraAngleOffset => source.DrawRotatedExtraAngleOffset;

            public Replacement(Graphic source, GraphicData original, SkinGraphicData values)
            {
                this.source = source;
                this.values = values;
                data = new GraphicData();
                data.CopyFrom(original);
                data.drawSize = values.DrawSize ?? original.drawSize;
                data.drawOffset = values.DrawOffset ?? original.drawOffset;
                data.drawOffsetNorth = values.DrawOffsetNorth ?? original.drawOffsetNorth;
                data.drawOffsetEast = values.DrawOffsetEast ?? original.drawOffsetEast;
                data.drawOffsetSouth = values.DrawOffsetSouth ?? original.drawOffsetSouth;
                data.drawOffsetWest = values.DrawOffsetWest ?? original.drawOffsetWest;
                data.drawRotated = values.DrawRotated ?? original.drawRotated;
                data.allowFlip = values.AllowFlip ?? original.allowFlip;
                data.flipExtraRotation = values.FlipExtraRotation ?? original.flipExtraRotation;
                data.textureRepeats = values.TextureRepeats ?? original.textureRepeats;
                data.allowAtlasing = values.AllowAtlasing ?? original.allowAtlasing;
                data.renderQueue = values.RenderQueue ?? original.renderQueue;
                data.overlayOpacity = values.OverlayOpacity ?? original.overlayOpacity;
                data.addTopAltitudeBias = values.AddTopAltitudeBias ?? original.addTopAltitudeBias;
                data.ignoreThingDrawColor = values.IgnoreThingDrawColor ?? original.ignoreThingDrawColor;
                data.shaderType = values.ShaderTypeDefName == null ? original.shaderType : DefDatabase<ShaderTypeDef>.GetNamedSilentFail(values.ShaderTypeDefName) ?? original.shaderType;
                drawSize = data.drawSize;
                color = data.color = values.Color ?? (data.ignoreThingDrawColor ? original.color : source.color);
                colorTwo = data.colorTwo = values.ColorTwo ?? (data.ignoreThingDrawColor ? original.colorTwo : source.colorTwo);
                path = source.path;
                maskPath = source.maskPath;

                for (int i = 0; i < 4; i++)
                {
                    Material mat = source.MatAt(new Rot4(i));
                    Texture2D? texture = null, mask = null;

                    if (values is SkinGraphicSingle single)
                    {
                        texture = single.Texture;
                        mask = single.Mask;
                    }
                    else if (values is SkinGraphicMulti multi)
                    {
                        texture = i switch { 0 => multi.TextureNorth, 1 => multi.TextureEast, 2 => multi.TextureSouth, _ => multi.TextureWest };
                        mask = i switch { 0 => multi.MaskNorth, 1 => multi.MaskEast, 2 => multi.MaskSouth, _ => multi.MaskWest };
                    }

                    var request = new MaterialRequest
                    {
                        mainTex = texture ?? (Texture2D)mat.mainTexture,
                        maskTex = mask ?? (mat.HasProperty(ShaderPropertyIDs.MaskTex) ? mat.GetTexture(ShaderPropertyIDs.MaskTex) as Texture2D : null),
                        shader = data.shaderType?.Shader ?? mat.shader,
                        color = color,
                        colorTwo = colorTwo,
                        renderQueue = values.RenderQueue ?? mat.renderQueue,
                        shaderParameters = original.shaderParameters
                    };

                    materials[i] = MaterialPool.MatFrom(request);
                }
            }
        }
    }
}