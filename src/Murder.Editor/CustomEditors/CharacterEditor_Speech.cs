using ImGuiNET;
using System.Diagnostics;
using System.Collections.Immutable;
using Murder.Assets.Graphics;
using Murder.Core.Dialogs;
using Murder.Assets;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.CustomFields;
using Murder.Editor.Utilities;
using Bang.Components;

namespace Murder.Editor.CustomEditors
{
    public partial class CharacterEditor : CustomEditor
    {
        /// <summary>
        /// Show all the nodes within a dialog.
        /// </summary>
        /// <returns>Whether the nodes have been modified.</returns>
        private bool ShowDialogs(ref Situation situation)
        {
            Debug.Assert(_script != null);

            bool modified = false;
            if (ImGui.Button("Add dialog!"))
            {
                situation = situation.WithNewDialog(new());
                return true;
            }

            for (int i = 0; i < situation.Dialogs.Length; ++i)
            {
                ImGui.PushID($"dialog_{i}");

                bool changed = false;
                Dialog dialog = situation.Dialogs[i];

                ImGui.BeginGroup();

                {
                    using RectangleBox box = new(10, 5);

                    ImGuiHelpers.ColoredIconButton('\uf0c9', $"##move_{situation.Id}", isActive: true);
                    ImGui.SameLine();

                    DragDrop<int>.DragDropSource($"situations_{situation.Id}", "situation", i);

                    if (ImGuiHelpers.DeleteButton($"Delete_{situation.Id}"))
                    {
                        situation = situation.RemoveDialogAt(i);

                        ImGui.PopID();
                        return true;
                    }

                    // -- Stop after button --
                    ImGui.SameLine();
                    if (ImGuiHelpers.ColoredIconButton('\uf363', $"play_once_{situation.Id}", isActive: !dialog.PlayOnce))
                    {
                        dialog = dialog.FlipPlayOnce();
                        changed = true;
                    }

                    ImGuiHelpers.HelpTooltip(dialog.PlayOnce ? "Play once." : "Play multiple times.");

                    ImGui.SameLine();
                    if (dialog.Actions is null)
                    {
                        if (ImGui.Button("Add action"))
                        {
                            dialog = dialog.WithActions(ImmutableArray<DialogAction>.Empty);
                            changed = true;
                        }
                    }
                    else
                    {
                        ImGuiHelpers.SelectedButton("Add action");
                    }

                    ImGui.SameLine();
                    if (dialog.GoTo is null)
                    {
                        if (ImGui.Button("Go to"))
                        {
                            dialog = dialog.WithGoTo(0);
                            changed = true;
                        }
                    }
                    else
                    {
                        ImGuiHelpers.SelectedButton("Go to");
                    }

                    ImGui.Separator();

                    // -- Show all matching requirements --
                    changed |= DrawRequirements($"situation{situation.Id}_dialog{i}", ref dialog);
                    ImGui.Separator();

                    changed |= DrawLines($"situation{situation.Id}_dialog{i}", ref dialog);

                    if (dialog.Actions is not null || dialog.GoTo is not null)
                    {
                        ImGui.Separator();
                    }

                    {
                        using TableMultipleColumns table = new($"dialog_{situation.Id}", flags: ImGuiTableFlags.SizingFixedFit, -1, -1, 850.WithDpi());

                        ImGui.TableNextColumn();

                        changed |= DrawActionsForTable(situation.Id.ToString(), ref dialog);
                        changed |= DrawGoToForTable(situation.Id.ToString(), ref dialog);
                    }

                    if (changed)
                    {
                        situation = situation.WithDialogAt(i, dialog);
                        modified = true;
                    }
                }

                ImGui.EndGroup();

                if (DragDrop<int>.DragDropTarget($"situations_{situation.Id}", out int draggedId))
                {
                    situation = situation.ReorderDialogAt(previousIndex: draggedId, newIndex: i);
                    modified = true;
                }

                ImGui.PopID();
            }

            return modified;
        }

        private bool DrawRequirements(string id, ref Dialog dialog)
        {
            ImGui.Text("Requirements");

            bool changed = false;

            // -- Add new requirement --
            ImGui.SameLine();
            if (ImGuiHelpers.IconButton('\uf055', $"add_requirement_{id}"))
            {
                dialog = dialog.AddRequirement(new());
                changed = true;
            }

            ImGuiHelpers.HelpTooltip("Add a requirement to this dialog.");

            ImGui.SameLine();
            if (ImGuiHelpers.IconButton('\uf5cd', $"add_requirement_weight_{id}"))
            {
                dialog = dialog.AddRequirement(Criterion.Weight);
                changed = true;
            }

            ImGuiHelpers.HelpTooltip("Add a weight to this dialog.");

            ImGui.SameLine();
            if (ImGuiHelpers.IconButton('\uf49e', $"add_requirement_component_{id}"))
            {
                dialog = dialog.AddRequirement(Criterion.Component);
                changed = true;
            }
            
            ImGuiHelpers.HelpTooltip("Add a check for components.");

            if (dialog.Requirements.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "[No requirements]");
                return changed;
            }

            using TableMultipleColumns table = new($"criteria_{id}", flags: ImGuiTableFlags.SizingFixedFit, -1, 350.WithDpi(), 300.WithDpi(), 200.WithDpi());

            for (int i = 0; i < dialog.Requirements.Length; ++i)
            {
                Criterion criterion = dialog.Requirements[i];

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                // -- Delete --
                if (ImGuiHelpers.DeleteButton($"delete_criteria_{id}_{i}"))
                {
                    dialog = dialog.WithRequirements(dialog.Requirements.RemoveAt(i));
                    return true;
                }

                ImGui.TableNextColumn();

                if (criterion.Fact.Kind != FactKind.Weight)
                {
                    if (criterion.Fact.Kind == FactKind.Component)
                    {
                        // -- Facts across all blackboards --
                        if (SearchBox.SearchComponentType(t: criterion.Fact.ComponentType) is Type t)
                        {
                            criterion = criterion.WithFact(new(t));
                            changed = true;
                        }
                    }
                    else
                    {
                        // -- Facts across all blackboards --
                        if (SearchBox.SearchFacts($"{id}_criteria{i}", criterion.Fact) is Fact newFact)
                        {
                            criterion = criterion.WithFact(newFact);
                            changed = true;
                        }
                    }

                    ImGui.TableNextColumn();
                    ImGui.PushItemWidth(-1);

                    // -- Select match kind --
                    if (DrawCriteriaCombo($"criteria_kind_{id}_{i}", ref criterion))
                    {
                        changed = true;
                    }

                    ImGui.PopItemWidth();
                    ImGui.TableNextColumn();
                }
                else
                {
                    ImGui.TextColored(Game.Profile.Theme.HighAccent, "Weight");
                    ImGuiHelpers.HelpTooltip("This is the weight which will be applied when prioritizing dialogs for this situation.");
                    ImGui.SameLine();
                }
                
                ImGui.PushItemWidth(-1);

                // -- Select requirement value --
                if (criterion.Fact.Kind is not FactKind.Invalid)
                {
                    ImGui.PushID($"edit_requirement_{id}_{i}");

                    // Draw criterion kind
                    string targetFieldName = GetTargetFieldForFact(criterion.Fact.Kind);
                    if (CustomField.DrawValue(ref criterion, targetFieldName))
                    {
                        changed = true;
                    }

                    ImGui.PopID();
                }

                ImGui.PopItemWidth();

                // -- Synchronize --
                if (changed)
                {
                    dialog = dialog.WithRequirements(dialog.Requirements.SetItem(i, criterion));
                }
            }

            return changed;
        }

        private bool DrawLines(string id, ref Dialog dialog)
        {
            Debug.Assert(_script != null);

            bool changed = false;

            ImGui.Text("Lines");
            ImGui.SameLine();

            // -- Add new caption --
            if (ImGuiHelpers.IconButton('\uf086', $"add_caption_{id}"))
            {
                dialog = dialog.AddLine(new(_script.Owner, string.Empty));
                return true;
            }

            ImGuiHelpers.HelpTooltip("Create line with speaker");

            ImGui.SameLine();

            // -- Add new single caption --
            if (ImGuiHelpers.IconButton('\uf075', $"add_single_caption_{id}"))
            {
                dialog = dialog.AddLine(new(string.Empty));
                return true;
            }

            ImGuiHelpers.HelpTooltip("Create line without speaker");

            ImGui.SameLine();

            // -- Add new timer --
            if (ImGuiHelpers.IconButton('\uf017', $"timer_caption_{id}"))
            {
                dialog = dialog.AddLine(new(_script.Owner, 0));
                return true;
            }

            ImGuiHelpers.HelpTooltip("Add timer");

            // -- Empty lines --
            if (dialog.Lines.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "[No lines]");
            }

            using TableMultipleColumns table = new($"lines_{id}", flags: ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Reorderable, -1, 600.WithDpi());

            // -- Display lines --
            for (int i = 0; i < dialog.Lines.Length; ++i)
            {
                Line line = dialog.Lines[i];

                bool changedLine = false;

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.PushItemWidth(-1);

                ImGui.PushID($"line {i} at {id}");

                // -- Portrait --
                if (line.IsText)
                {
                    if (!TryDrawPortrait(line))
                    {
                        ImGui.TextColored(Game.Profile.Theme.Faded, "\n   No \n speaker!");
                    }
                }
                else
                {
                    // -- Delay --
                    ImGui.TextColored(Game.Profile.Theme.Faded, "[Timer]");
                }

                ImGui.PopID();

                ImGui.PopItemWidth();
                ImGui.TableNextColumn();

                // -- Text input --
                if (line.Text is not null)
                {
                    ImGui.Dummy(new());

                    int height = line.Speaker is not null ? 55.WithDpi() : 44.WithDpi();
                    string value = line.Text;
                    if (ImGui.InputTextMultiline($"##line_input_{id}_{i}", ref value, 128, new(-1, height)))
                    {
                        line = line.WithText(value);
                        changedLine = true;
                    }
                }

                ImGuiHelpers.ColoredIconButton('\uf0c9', $"##move_{id}_{i}", isActive: true);
                ImGui.SameLine();

                DragDrop<int>.DragDropSource($"lines_{id}", "line", i);

                if (DragDrop<int>.DragDropTarget($"lines_{id}", out int draggedId))
                {
                    dialog = dialog.ReorderLineAt(previousIndex: draggedId, newIndex: i);
                    changed = true;
                }

                // -- Delete line --
                if (ImGuiHelpers.DeleteButton($"delete_line_{id}_{i}"))
                {
                    dialog = dialog.WithLines(dialog.Lines.RemoveAt(i));

                    // Immediately return!
                    return true;
                }

                if (line.Delay is not null)
                {
                    ImGui.SameLine();
                    ImGui.PushItemWidth(150);

                    float delay = line.Delay.Value;
                    if (ImGui.InputFloat($"##delay_input_{id}_{i}", ref delay, .1f, 1, "%.1f") && delay >= 0)
                    {
                        line = line.WithDelay(delay);
                        changedLine = true;
                    }

                    ImGui.PopItemWidth();
                }

                if (line.IsText && line.Speaker is not null)
                {
                    ImGui.SameLine();
                    SearchBox.PushItemWidth(200);
                    ImGui.PushID($"speaker_{id}_{i}");

                    // -- Select speaker --
                    Guid speakerGuid = line.Speaker.Value;
                    if (SearchBox.SearchAsset(ref speakerGuid, typeof(SpeakerAsset)))
                    {
                        line = line.WithSpeaker(speakerGuid);
                        changedLine = true;
                    }

                    ImGui.PopID();
                    SearchBox.PopItemWidth();
                    ImGui.SameLine();
                    ImGui.PushItemWidth(200);

                    ImGui.PushID($"portrait_kind_{id}_{i}");

                    // -- Select speaker portrait --
                    if (Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid) is SpeakerAsset speaker)
                    {
                        List<string> allPortraits = speaker.Portraits.Keys.ToList();
                        int portraitIndex = allPortraits.IndexOf(line.Portrait!);

                        if (portraitIndex == -1)
                        {
                            // Just set the first portrait as default.
                            line = line.WithPortrait(allPortraits[0]);
                            changedLine = true;
                        }
                        else if (ImGui.Combo($"##{id}", ref portraitIndex, allPortraits.ToArray(), allPortraits.Count))
                        {
                            line = line.WithPortrait(allPortraits[portraitIndex]);
                            changedLine = true;
                        }
                    }
                    else
                    {
                        ImGui.TextColored(Game.Profile.Theme.Warning, "Invalid speaker!");
                    }

                    ImGui.PopID();

                    ImGui.PopItemWidth();
                }
                
                // -- Synchronize --
                if (changedLine)
                {
                    dialog = dialog.SetLineAt(i, line);
                    changed = true;
                }
            }

            return changed;
        }

        private bool TryDrawPortrait(Line line)
        {
            if (!line.IsText || line.Speaker is null)
            {
                return false;
            }

            Guid speakerGuid = line.Speaker.Value;

            if (Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid) is not SpeakerAsset speaker ||
                line.Portrait is null ||
                !speaker.Portraits.TryGetValue(line.Portrait, out Portrait portrait) ||
                Game.Data.TryGetAsset<AsepriteAsset>(portrait.Aseprite) is not AsepriteAsset aseprite)
            {
                return false;
            }

            EditorAssetHelpers.DrawPreview(aseprite, maxSize: 100.WithDpi(), portrait.AnimationId);
            return true;
        }

        private bool DrawActionsForTable(string id, ref Dialog dialog)
        {
            if (dialog.Actions is null)
            {
                return false;
            }

            bool changed = false;

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            if (ImGuiHelpers.DeleteButton($"delete_actions_{id}"))
            {
                dialog = dialog.WithActions(null);
                return true;
            }

            ImGui.TableNextColumn();

            // -- Field --
            ImGui.Text("Actions:");

            ImGui.TableNextColumn();

            // -- Add --
            if (ImGuiHelpers.IconButton('\uf51b', $"add_action_{id}", Game.Profile.Theme.White))
            {
                dialog = dialog.WithActions(dialog.Actions.Value.Add(new()));
                return true;
            }

            ImGuiHelpers.HelpTooltip("Modify a blackboard variable");

            ImGui.SameLine();

            // -- Add --
            if (ImGuiHelpers.IconButton('\uf0a1', $"add_interactive_{id}", Game.Profile.Theme.White))
            {
                dialog = dialog.WithActions(dialog.Actions.Value.Add(DialogAction.ComponentAction));
                return true;
            }

            ImGuiHelpers.HelpTooltip("Add component");

            // -- List all actions --
            for (int i = 0; i < dialog.Actions!.Value.Length; ++i)
            {
                DialogAction action = dialog.Actions.Value[i];

                using TableMultipleColumns table = new($"action_{id}_{i}", flags: ImGuiTableFlags.SizingFixedFit, -1, 350.WithDpi(), 200.WithDpi(), 200.WithDpi());

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                // -- Delete action --
                if (ImGuiHelpers.DeleteButton($"delete_action_{id}_{i}"))
                {
                    dialog = dialog.WithActions(dialog.Actions.Value.RemoveAt(i));
                    return true;
                }

                ImGui.TableNextColumn();
                
                if (action.Kind == BlackboardActionKind.Component)
                {
                    if (CustomField.DrawValue(ref action, nameof(action.ComponentsValue)))
                    {
                        changed = true;
                    }
                }
                else
                {
                    // -- Facts across all blackboards --
                    if (SearchBox.SearchFacts($"{id}_action_{i}", action.Fact) is Fact newFact)
                    {
                        action = action.WithFact(newFact);
                        changed = true;
                    }

                    ImGui.TableNextColumn();
                    ImGui.PushItemWidth(-1);

                    // -- Select action kind --
                    if (DrawActionCombo($"{id}_action_{i}_combo", ref action))
                    {
                        changed = true;
                    }

                    ImGui.PopItemWidth();
                    ImGui.TableNextColumn();
                    ImGui.PushItemWidth(-1);

                    // -- Select action value --
                    if (action.Fact is not null)
                    {
                        string targetFieldName = GetTargetFieldForFact(action.Fact.Value.Kind);
                        if (CustomField.DrawValue(ref action, targetFieldName))
                        {
                            changed = true;
                        }
                    }

                    ImGui.PopItemWidth();
                }

                // -- Synchronize --
                if (changed)
                {
                    dialog = dialog.WithActions(dialog.Actions.Value.SetItem(i, action));
                    changed = true;
                }
            }

            return changed;
        }

        private bool DrawGoToForTable(string id, ref Dialog dialog)
        {
            Debug.Assert(_script != null);
            bool changed = false;

            if (dialog.GoTo is null)
            {
                return false;
            }

            ImGui.TableNextRow();
            ImGui.TableNextColumn();

            if (ImGuiHelpers.DeleteButton($"delete_goto_{id}"))
            {
                dialog = dialog.WithGoTo(null);
                return true;
            }

            ImGui.TableNextColumn();

            // -- Field --
            ImGui.Text("Go to:");

            ImGui.TableNextColumn();
            ImGui.PushItemWidth(-1);

            ImmutableArray<(string name, int id)> situations = FetchAllSituations();
            string[] situationNames = situations.Select(s => s.name).ToArray();

            if (situationNames.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "No situation to go next?");
                return false;
            }

            int item = 0;
            if (_script.TryFetchSituation(dialog.GoTo.Value) is Situation target)
            {
                item = situations.IndexOf((target.Name, target.Id));
            }
            else
            {
                // Set a initial value for this.
                dialog = dialog.WithGoTo(situations[item].id);
                changed = true;
            }

            if (ImGui.Combo($"##{id}", ref item, situationNames, situationNames.Length))
            {
                dialog = dialog.WithGoTo(situations[item].id);
                changed = true;
            }

            ImGui.PopItemWidth();

            return changed;
        }
        
        private ImmutableArray<(string, int)> FetchAllSituations()
        {
            Debug.Assert(_script != null);
            return FetchAllSituations(_script);
        }

        public static ImmutableArray<(string, int)> FetchAllSituations(CharacterAsset asset)
        {
            var builder = ImmutableArray.CreateBuilder<(string, int)>();

            for (int i = 0; i < asset.Situations.Length; ++i)
            {
                builder.Add((asset.Situations[i].Name, asset.Situations[i].Id));
            }

            return builder.ToImmutable();
        }
        
        internal static string GetTargetFieldForFact(FactKind kind)
        {
            switch (kind)
            {
                case FactKind.Int:
                    return nameof(Criterion.IntValue);

                case FactKind.String:
                    return nameof(Criterion.StrValue);

                case FactKind.Weight:
                    return nameof(Criterion.IntValue);

                case FactKind.Bool:
                default:
                    return nameof(Criterion.BoolValue);
            }
        }

        internal static bool DrawCriteriaCombo(string id, ref Criterion criterion)
        {
            CriterionKind[] allKinds = criterion.FetchValidCriteriaKind();
            if (allKinds.Length == 0)
            {
                // This may be empty if the criterion has not been initialized yet.
                ImGui.TextColored(Game.Profile.Theme.Warning, "Choose a fact!");
                return false;
            }
            
            int index = Array.IndexOf(allKinds, criterion.Kind);
            if (index == -1)
            {
                criterion = criterion.WithKind(allKinds[0]);
                return true;
            }

            string[] kindToString = allKinds.Select(k => k.ToString()).ToArray();

            if (ImGui.Combo($"##{id}", ref index, kindToString, kindToString.Length))
            {
                criterion = criterion.WithKind(allKinds[index]);
                return true;
            }

            return false;
        }


        public static bool DrawActionCombo(string id, ref DialogAction action)
        {
            BlackboardActionKind[] allKinds = action.FetchValidActionKind();
            if (allKinds.Length == 0)
            {
                // This may be empty if the action has not been initialized yet.
                ImGui.TextColored(Game.Profile.Theme.Warning, "Choose a fact!");
                return false;
            }

            int index = Array.IndexOf(allKinds, action.Kind);
            if (index == -1)
            {
                action = action.WithKind(allKinds[0]);
                return true;
            }

            string[] kindToString = allKinds.Select(k => k.ToString()).ToArray();

            if (ImGui.Combo($"##{id}", ref index, kindToString, kindToString.Length))
            {
                action = action.WithKind(allKinds[index]);
                return true;
            }

            return false;
        }
    }
}
