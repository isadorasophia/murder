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

namespace Murder.Editor.CustomEditors
{
    [CustomEditorOf(typeof(CharacterAsset))]
    public partial class CharacterEditor : CustomEditor
    {
        private CharacterAsset? _script;

        public override object Target => _script!;

        public override void OpenEditor(ImGuiRenderer imGuiRenderer, object target)
        {
            _script = (CharacterAsset)target;
        }

        public override void DrawEditor()
        {
            bool modified = false;
            Debug.Assert(_script != null);

            using (TableTwoColumns table = new($"fields_{_script.Guid}"))
            {
                CustomComponent.DrawMembersForTarget(_script, MembersForCharacter.Value);
            }

            if (ImGui.Button("Add situation!"))
            {
                ImGui.OpenPopup("new_situation");
            }

            if (ImGui.BeginPopup($"new_situation"))
            {
                modified |= DrawNewSituationInstanceModal();
                ImGui.EndPopup();
            }

            for (int i = 0; i < _script.Situations.Length; ++i)
            {
                Situation dialog = _script.Situations[i];

                if (ImGui.BeginPopup($"new_dialog_{dialog.Id}"))
                {
                    modified |= DrawAndRenameInstanceModal(dialog.Id);
                    ImGui.EndPopup();
                }

                if (ImGuiHelpers.TreeNodeWithIcon('\uf086', dialog.Name, ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.SpanFullWidth))
                {
                    if (ImGuiHelpers.DeleteButton($"Delete_{dialog.Id}"))
                    {
                        _script.RemoveSituationAt(dialog.Id);

                        modified = true;
                        break;
                    }

                    ImGui.SameLine();

                    if (ImGuiHelpers.IconButton('\uf304', $"rename_dialog_{dialog.Id}"))
                    {
                        ImGui.OpenPopup($"rename_{dialog.Id}");
                    }

                    if (ImGui.BeginPopup($"rename_{dialog.Id}"))
                    {
                        modified |= DrawAndRenameInstanceModal(dialog.Id);
                        ImGui.EndPopup();
                    }

                    ImGui.SameLine();

                    if (ShowDialogs(ref dialog))
                    {
                        _script.SetSituationAt(dialog.Id, dialog);
                        modified = true;
                    }

                    ImGui.TreePop();
                }
            }

            _script.FileChanged |= modified;
        }

        private static string? _cachedName;

        private bool DrawNewSituationInstanceModal()
        {
            Debug.Assert(_script != null);
            _cachedName ??= string.Empty;

            ImGui.Text("What is this situation name?");

            ImGui.InputText($"##newSituationName_new", ref _cachedName, 128, ImGuiInputTextFlags.AutoSelectAll);

            if (IsDialogNameSubmitted(_cachedName))
            {
                _script.AddNewSituation(_cachedName);

                _cachedName = null;
                ImGui.CloseCurrentPopup();

                return true;
            }

            return false;
        }

        private bool DrawAndRenameInstanceModal(int id)
        {
            Debug.Assert(_script != null);
            _cachedName ??= _script.TryFetchSituation(id)?.Name ?? string.Empty;

            if (ImGui.IsWindowAppearing())
                ImGui.SetKeyboardFocusHere();
            ImGui.InputText($"##newSituationName_{id}", ref _cachedName, 128, ImGuiInputTextFlags.AutoSelectAll);
            
            if (IsDialogNameSubmitted(_cachedName))
            {
                Situation situation = _script.Situations[id];
                _script.SetSituationAt(id, situation.WithName(_cachedName));

                _cachedName = null;
                ImGui.CloseCurrentPopup();

                return true;
            }

            return false;
        }

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
