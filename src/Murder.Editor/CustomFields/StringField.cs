using ImGuiNET;
using Murder.Attributes;
using Murder.Data;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities.Attributes;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(string))]
    internal class StringField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            string text = fieldValue as string ?? string.Empty;

            if (AttributeExtensions.IsDefined(member, typeof(AtlasTextureAttribute)))
            {
                return ProcessAtlasTexture(text);
            }

            if (AttributeExtensions.IsDefined(member, typeof(SimpleTextureAttribute)))
            {
                return ProcessTexture(text);
            }

            if (AttributeExtensions.IsDefined(member, typeof(AtlasTextureAttribute)))
            {
                return ProcessAtlasTexture(text);
            }

            if (AttributeExtensions.IsDefined(member, typeof(SoundAttribute)))
            {
                return ProcessSound(text);
            }

            string memberId = $"##{member.Name}";
            if (AttributeExtensions.IsDefined(member, typeof(ChildIdAttribute)))
            {
                return ProcessChildName(memberId, text);
            }

            if (member.IsReadOnly)
            {
                ImGui.Text(text);
            }
            else
            {
                if (AttributeExtensions.IsDefined(member, typeof(MultilineAttribute)))
                {
                    modified = ImGui.InputTextMultiline(memberId, ref text, 1024, new(-1, 75));
                }
                else
                {
                    modified = ImGui.InputText("", ref text, 1024);
                }
            }

            return (modified, text);
        }

        private (bool modified, object? result) ProcessTexture(string text)
        {
            bool modified = false;

            if (ImGui.BeginCombo("", text))
            {
                foreach (var value in Game.Data.AvailableUniqueTextures)
                {
                    if (ImGui.MenuItem(value))
                    {
                        text = value;
                        modified = true;
                    }
                }
                ImGui.EndCombo();
            }

            ImGui.SameLine();
            Architect.ImGuiTextureManager.DrawPreviewImage(text, 256, Game.Data.FetchAtlas(AtlasId.Gameplay));

            return (modified, text);
        }
        private (bool modified, object? result) ProcessAtlasTexture(string text)
        {
            bool modified = false;

            if (ImGui.BeginCombo("", text))
            {
                foreach (var value in Game.Data.FetchAtlas(AtlasId.Gameplay).GetAllEntries())
                {
                    if (ImGui.MenuItem(value.Name))
                    {
                        text = value.Name;
                        modified = true;
                    }
                }
                ImGui.EndCombo();
            }

            ImGui.SameLine();
            Architect.ImGuiTextureManager.DrawPreviewImage(text, 256, Game.Data.FetchAtlas(AtlasId.Gameplay));

            return (modified, text);
        }

        private (bool modified, object? result) ProcessSound(string text)
        {
            if (ImGuiHelpers.DeleteButton("No Sound"))
            {
                return (modified: true, string.Empty);
            }
            ImGui.SameLine();
            if (SearchBox.SearchSounds(text, "") is string sound)
            {
                return (modified: true, sound);
            }

            return (modified: false, text);
        }

        private static (bool modified, string result) ProcessChildName(string id, string text)
        {
            HashSet<string>? names = StageHelpers.GetChildNamesForSelectedEntity();
            if (names is null)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "Unable to find a selected entity :-(");
                return (false, text);
            }
            else if (names.Count() == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "No children found :-(");
                return (false, text);
            }

            bool modified = ProcessChildName(id, ref text, names);
            return (modified, text);
        }

        public static bool ProcessChildName(string id, ref string text, HashSet<string> names)
        {
            bool modified = false;

            if (ImGui.BeginCombo(id, text))
            {
                foreach (string name in names)
                {
                    if (ImGui.MenuItem(name))
                    {
                        text = name;
                        modified = true;
                    }
                }

                ImGui.EndCombo();
            }

            return modified;
        }
    }
}
