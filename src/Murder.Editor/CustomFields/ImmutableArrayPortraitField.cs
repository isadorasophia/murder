using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Components;
using Murder.Core;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<PortraitInfo>))]
    internal class ImmutableArrayPortraitField : ImmutableArrayField<PortraitInfo>
    {
        protected override bool AllowReorder => true;

        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out PortraitInfo element)
        {
            if (ImGui.Button("New portrait"))
            {
                element = new();
                return true;
            }

            element = default;
            return false;
        }

        protected override void Reorder(ref ImmutableArray<PortraitInfo> elements)
        {
            elements = [.. elements.OrderBy(p => p.Name)];
        }

        protected override bool DrawElement(ref PortraitInfo element, EditorMember member, int index)
        {
            bool modified = false;

            string id = $"speaker_{index}";
            using TableMultipleColumns table = new(id, flags: ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Resizable, 65, 400, 300, 400);

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            if (Game.Data.TryGetAsset<SpriteAsset>(element.Portrait.Sprite) is SpriteAsset ase)
            {
                EditorAssetHelpers.DrawPreview(ase, maxSize: 256, element.Portrait.AnimationId);
                ImGui.TableNextColumn();
            }

            ImGui.Dummy(new System.Numerics.Vector2(20, 0));

            ImGui.PushItemWidth(-1);

            string name = element.Name;
            if (DrawValue(ref element, nameof(PortraitInfo.Name)))
            {
                modified = true;
            }

            Portrait portrait = element.Portrait;
            if (DrawValue(ref portrait, nameof(Portrait.Sprite)))
            {
                element = element with { Portrait = element.Portrait.WithSprite(portrait.Sprite) };
                modified = true;
            }

            // Draw combo box for the animation id
            string animation = element.Portrait.AnimationId;
            if (EditorAssetHelpers.DrawComboBoxFor(element.Portrait.Sprite, ref animation))
            {
                element = element with { Portrait = element.Portrait.WithAnimationId(animation) };
                modified = true;
            }
            ImGui.PopItemWidth();

            ImGui.TableNextColumn();
            ImGui.Dummy(new System.Numerics.Vector2(20, 0));

            if (portrait.Sprite != Guid.Empty)
            {
                ImGui.Text("\uf0c3");
                ImGui.SameLine();

                int flags = (int)element.Properties;
                if (ImGuiHelpers.DrawEnumFieldAsFlags(id, typeof(PortraitProperties), ref flags))
                {
                    element = element with { Properties = (PortraitProperties)flags };
                    modified = true;
                }
            }

            ImGui.Text("\uf001");
            ImGui.SameLine();
            if (DrawValue(ref element, nameof(PortraitInfo.Sound)))
            {
                modified = true;
            }

            ImGui.TableNextColumn();

            ImGui.Dummy(new System.Numerics.Vector2(20, 0));

            ImGui.Text("\uf3c9");
            ImGuiHelpers.HelpTooltip("Listen to portrait events...");
            ImGui.SameLine();

            if (element.Events is EventListenerEditorComponent eventsListener &&
                Game.Data.TryGetAsset<SpriteAsset>(element.Portrait.Sprite) is SpriteAsset sprite)
            {
                bool deleted = ImGuiHelpers.IconButton('', $"##delete_events_{element.Name}", Game.Profile.Theme.White, Game.Profile.Theme.BgFaded);
                ImGuiHelpers.HelpTooltip("Restore default value");
                ImGui.SameLine();

                if (deleted)
                {
                    modified = true;
                    element = element with { Events = null };
                }
                else
                {
                    (string[] Animations, HashSet<string> Events)? properties =
                        StageHelpers.GetSpriteEventsForAsset(sprite);

                    modified |= EventListenerEditor.DrawEventsListener(
                        ref eventsListener, drawPlayAnimations: false, properties?.Animations, properties?.Events);

                    if (modified)
                    {
                        element = element with { Events = eventsListener };
                    }
                }
            }
            else
            {
                ImGui.PushID($"events_{element.Name}");
                modified |= DrawValue(ref element, nameof(PortraitInfo.Events));
                ImGui.PopID();
            }

            return modified;
        }
    }
}