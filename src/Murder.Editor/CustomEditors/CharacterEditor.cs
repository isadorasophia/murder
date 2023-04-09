using ImGuiNET;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Immutable;
using Murder.Core.Dialogs;
using Murder.Editor.Attributes;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Core.Graphics;
using Murder.Assets;

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(CharacterAsset))]
    public partial class CharacterEditor : CustomEditor
    {
        private CharacterAsset? _script;

        public override object Target => _script!;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, RenderContext _, object target)
        {
            _script = (CharacterAsset)target;
        }

        public override void DrawEditor()
        {
            // _script.FileChanged |= modified;
        }

        private static string? _cachedName;

        /// <summary>
        /// Check if a name was successfully submitted by the user.
        /// </summary>
        private bool IsDialogNameSubmitted(string name)
        {
            Debug.Assert(_script != null);

            bool isValidName = !_script.Situations.Any(d => d.Name == name);
            if (isValidName)
            {
                if (ImGui.Button("Ok!") || Game.Input.Pressed(Keys.Escape) || Game.Input.Pressed(Keys.Enter))
                {
                    return true;
                }
            }
            else
            {
                ImGuiHelpers.DisabledButton("Ok!");
                ImGui.TextColored(Game.Profile.Theme.Warning, "Dialog node has to be unique.");
            }

            return false;
        }

        protected static readonly Lazy<ImmutableArray<(string, EditorMember)>> MembersForCharacter = new(() =>
        {
            Dictionary<string, EditorMember> members =
                typeof(CharacterAsset).GetFieldsForEditor().ToDictionary(f => f.Name, f => f);

            var builder = ImmutableArray.CreateBuilder<(string, EditorMember)>();
            builder.Add((nameof(CharacterAsset.Owner), members[nameof(CharacterAsset.Owner)]));

            return builder.ToImmutable();
        });
    }
}
