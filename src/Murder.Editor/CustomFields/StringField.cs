using InstallWizard;
using InstallWizard.Util;
using InstallWizard.Util.Attributes;
using Editor.Gui;
using Editor.Reflection;
using ImGuiNET;

namespace Editor.CustomFields
{
    [CustomFieldOf(typeof(string))]
    internal class StringField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;
            string text = (string)fieldValue! ?? String.Empty;

            if (AttributeExtensions.IsDefined(member, typeof(AtlasTextureAttribute)))
            {
                return ProcessAtlasTexture(text);
            }

            if (AttributeExtensions.IsDefined(member, typeof(SoundAttribute)))
            {
                return ProcessSound(text);
            }


            if (member.IsReadOnly)
            {
                ImGui.Text(text);
            }
            else
            {
                modified = ImGui.InputText("", ref text, 1024);
            }

            return (modified, text);
        }

        private (bool modified, object? result) ProcessAtlasTexture(string text)
        {
            bool modified = false;

            if (ImGui.BeginCombo("", text))
            {
                foreach (var value in Game.Data.FetchAtlas(InstallWizard.Data.AtlasId.Gameplay).GetAllEntries())
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
            ImGuiExtended.Image(text, 256, Game.Data.FetchAtlas(InstallWizard.Data.AtlasId.Gameplay));

            return (modified, text);
        }

        private (bool modified, object? result) ProcessSound(string text)
        {
            if (ImGuiExtended.DeleteButton("No Sound"))
            {
                return (modified: true, string.Empty);
            }
            ImGui.SameLine();
            if (SearchBox.SearchSounds(text) is string sound)
            {
                return (modified: true, sound);
            }

            return (modified: false, text);
        }
    }
}
