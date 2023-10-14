﻿using Bang.Components;
using ImGuiNET;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Murder.Editor.CustomEditors
{
    public partial class CharacterEditor : CustomEditor
    {
        private void SelectNode(int id)
        {
            if (_script?.Guid is not Guid guid)
            {
                return;
            }

            ScriptInformation info = ActiveEditors[guid];
            info.CachedDialogs[info.ActiveSituation] = id;

            info.Stage.EditorHook.SelectedNode = id;
        }

        private Line? ModifyPortraitAt(ScriptInformation info, int lineIndex, Guid speaker, string? portrait)
        {
            if (_script is null || !FetchActiveSituation(out Situation? situation))
            {
                return null;
            }

            DialogItemId id = new(info.ActiveSituation, info.ActiveDialog, lineIndex);
            Dialog dialog = situation.Value.Dialogs[id.DialogId];

            Line line = dialog.Lines[lineIndex].WithSpeaker(speaker).WithPortrait(portrait);
            dialog = dialog.WithLineAt(lineIndex, line);

            _script.SetSituationAt(info.ActiveSituation, situation.Value.WithDialogAt(id.DialogId, dialog));
            _script?.SetCustomPortraitAt(
                id: id,
                speaker, portrait);

            return line;
        }

        private DialogAction? ModifyComponentAt(ScriptInformation info, int actionIndex, IComponent c)
        {
            if (_script is null || !FetchActiveSituation(out Situation? situation))
            {
                return null;
            }

            DialogItemId id = new(info.ActiveSituation, info.ActiveDialog, actionIndex);
            Dialog dialog = situation.Value.Dialogs[id.DialogId];

            if (dialog.Actions is null)
            {
                return null;
            }

            DialogAction action = dialog.Actions!.Value[actionIndex];
            action = action.WithComponent(c);

            _script.SetSituationAt(info.ActiveSituation, situation.Value.WithDialogAt(id.DialogId, dialog));
            _script?.SetCustomComponentAt(
                id: id,
                c);

            return action;
        }

        private void DrawCurrentDialog(ScriptInformation info, float height)
        {
            int dialogId = info.ActiveDialog;
            if (dialogId == -1 || _script?.TryFetchSituation(info.ActiveSituation) is not Situation situation)
            {
                return;
            }

            ImGui.BeginChild("dialog_table", new System.Numerics.Vector2(-1, height));

            //ImGui.Text("\uf4ad");
            //ImGuiHelpers.HelpTooltip("Active dialog");

            if (dialogId >= situation.Dialogs.Length)
            {
                // Probably due to hot reload, this has happened.
                ImGui.EndChild();
                return;
            }

            ImGui.TreePush("##dialog");

            Dialog dialog = situation.Dialogs[dialogId];

            DrawRequirements(dialog);
            DrawLines(info, dialog);

            ImGui.Spacing();

            DrawGoTo(info, dialog);
            DrawActions(info, dialog);
            DrawAmount(dialog);

            ImGui.TreePop();
            ImGui.EndChild();
        }

        private void DrawRequirements(Dialog dialog)
        {
            if (dialog.Requirements.Length == 0)
            {
                // ImGui.TextColored(Game.Profile.Theme.Faded, "Always");
                return;
            }

            ImGui.Text("(");
            ImGui.SameLine();

            for (int i = 0; i < dialog.Requirements.Length; ++i)
            {
                Criterion criterion = dialog.Requirements[i].Criterion;

                CriterionNodeKind? nextNodeKind = null;
                if (i < dialog.Requirements.Length - 1)
                {
                    nextNodeKind = dialog.Requirements[i + 1].Kind;
                }

                switch (criterion.Fact.Kind)
                {
                    case FactKind.Component:
                        break;

                    default:
                        ImGui.Text(criterion.Fact.Name);
                        break;
                }

                ImGui.SameLine();

                switch (criterion.Kind)
                {
                    case CriterionKind.Different:
                        ImGui.TextColored(Game.Profile.Theme.Accent, "is not");
                        break;

                    case CriterionKind.Is:
                        ImGui.TextColored(Game.Profile.Theme.Accent, "is");
                        break;

                    case CriterionKind.Less:
                        ImGui.TextColored(Game.Profile.Theme.Accent, "<");
                        break;

                    case CriterionKind.LessOrEqual:
                        ImGui.TextColored(Game.Profile.Theme.Accent, "<=");
                        break;

                    case CriterionKind.Bigger:
                        ImGui.TextColored(Game.Profile.Theme.Accent, ">");
                        break;

                    case CriterionKind.BiggerOrEqual:
                        ImGui.TextColored(Game.Profile.Theme.Accent, ">=");
                        break;
                }

                ImGui.SameLine();

                switch (criterion.Fact.Kind)
                {
                    case FactKind.Int:
                        ImGui.TextColored(Game.Profile.Theme.Accent, criterion.IntValue!.Value.ToString());
                        break;

                    case FactKind.Bool:
                        ImGui.TextColored(Game.Profile.Theme.HighAccent, criterion.BoolValue!.Value.ToString().ToLower());
                        break;

                    case FactKind.String:
                        ImGui.TextColored(Game.Profile.Theme.HighAccent, criterion.StrValue!.ToString());
                        break;
                }

                ImGui.SameLine();

                if (nextNodeKind is not null)
                {
                    ImGui.Text(nextNodeKind.Value.ToString().ToLower());
                }
            }

            ImGui.Text(")");
            ImGui.Spacing();
        }

        private void DrawLines(ScriptInformation info, Dialog dialog)
        {
            // -- Empty lines --
            if (dialog.Lines.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "[No lines]");
            }

            using TableMultipleColumns table = new("lines_dialog", flags: ImGuiTableFlags.SizingFixedFit, 0, -1);

            // -- Display lines --
            for (int i = 0; i < dialog.Lines.Length && table.Opened; ++i)
            {
                Line line = dialog.Lines[i];

                ImGui.TableNextRow();
                ImGui.TableNextColumn();

                ImGui.PushItemWidth(-1);

                ImGui.PushID($"line {i}");

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
                    // Centralizes the text vertically.
                    float textHeight = (ImGui.GetItemRectSize().Y -
                        ImGui.CalcTextSize(line.Text, wrapWidth: ImGui.GetWindowContentRegionMax().X - ImGui.GetCursorPosX()).Y) / 2f;

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + textHeight);

                    string value = line.Text;

                    ImGui.TextWrapped(value);
                }

                if (line.Delay is not null)
                {
                    ImGui.SameLine();
                    ImGui.PushItemWidth(150);

                    float delay = line.Delay.Value;
                    ImGui.TextWrapped(delay.ToString());

                    ImGui.PopItemWidth();
                }

                if (line.IsText)
                {
                    SearchBox.PushItemWidth(125);
                    ImGui.PushID($"speaker_{i}");

                    // -- Select speaker --
                    Guid speakerGuid = line.Speaker ?? Guid.Empty;
                    if (SearchBox.SearchAsset(ref speakerGuid, typeof(SpeakerAsset), defaultText: "Default speaker"))
                    {
                        line = ModifyPortraitAt(info, i, speakerGuid, line.Portrait)!.Value;
                    }

                    ImGui.PopID();
                    SearchBox.PopItemWidth();
                    ImGui.SameLine();
                    ImGui.PushItemWidth(200);

                    ImGui.PushID($"portrait_kind_{i}");

                    // -- Select speaker portrait --
                    if (Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid) is SpeakerAsset speaker)
                    {
                        List<string> allPortraits = speaker.Portraits.Keys.ToList();
                        int portraitIndex = line.Portrait is null ? -1 : allPortraits.IndexOf(line.Portrait);

                        if (portraitIndex == -1 && allPortraits.Count > 0)
                        {
                            // Just set the first portrait as default.
                            _ = ModifyPortraitAt(info, i, speakerGuid, speaker.DefaultPortrait ?? allPortraits[0])!.Value;
                        }
                        else if (ImGui.Combo($"##portrait_{i}", ref portraitIndex, allPortraits.ToArray(), allPortraits.Count))
                        {
                            _ = ModifyPortraitAt(info, i, speakerGuid, allPortraits[portraitIndex])!.Value;
                        }
                    }
                }
            }
        }

        private void DrawGoTo(ScriptInformation info, Dialog dialog)
        {
            Debug.Assert(_script != null);

            if (dialog.GoTo is null)
            {
                return;
            }

            // -- Field --
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Go to...");
            ImGui.SameLine();

            ImmutableArray<(string name, int id)> situations = FetchAllSituations(_script);
            string[] situationNames = situations.Select(s => s.name).ToArray();

            if (situationNames.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "No situation to go next?");
                return;
            }

            if (_script.TryFetchSituation(dialog.GoTo.Value) is Situation target)
            {
                if (ImGui.Button(target.Name))
                {
                    SwitchSituation(info, target);
                }
            }
            else if (dialog.GoTo.Value == -1)
            {
                ImGui.TextColored(Game.Profile.Theme.Faded, "\uf52b");
                ImGuiHelpers.HelpTooltip("Exit");
            }

            ImGui.Spacing();
        }

        private void DrawActions(ScriptInformation info, Dialog dialog)
        {
            if (dialog.Actions is null)
            {
                return;
            }

            // -- Field --
            ImGui.AlignTextToFramePadding();

            ImGui.Text("\uf005");
            ImGuiHelpers.HelpTooltip("Triggered once this dialog is complete");

            Dictionary<string, Fact> facts = AssetsFilter.GetAllFactsFromBlackboards();

            // -- List all actions --
            for (int i = 0; i < dialog.Actions!.Value.Length; ++i)
            {
                DialogAction action = dialog.Actions.Value[i];
                if (action.Kind == BlackboardActionKind.Component)
                {
                    ImGui.Spacing();

                    string targetFieldName = nameof(DialogAction.ComponentValue);
                    if (CustomField.DrawValue(ref action, targetFieldName))
                    {
                        ModifyComponentAt(info, i, action.ComponentValue!);
                    }
                }
                else
                {
                    if (i == 0 && dialog.Actions!.Value.Length == 1)
                    {
                        ImGui.SameLine();
                    }

                    // -- Facts across all blackboards --
                    if (!facts.ContainsKey(action.Fact.EditorName))
                    {
                        ImGui.TextColored(Game.Profile.Theme.Faded, action.Fact.EditorName);
                        ImGuiHelpers.HelpTooltip("This fact was not found in the blackboard");
                    }
                    else
                    {
                        ImGui.TextColored(Game.Profile.Theme.HighAccent, action.Fact.EditorName);
                    }

                    ImGui.SameLine();

                    switch (action.Kind)
                    {
                        case BlackboardActionKind.Add:
                            ImGui.Text("+=");
                            break;

                        case BlackboardActionKind.Minus:
                            ImGui.Text("-=");
                            break;

                        case BlackboardActionKind.Set:
                            ImGui.Text("=");
                            break;
                    }

                    ImGui.SameLine();

                    string value = GetTargetStringValueForAction(action);
                    ImGui.Text(value);
                }

            }

            return;
        }

        private void DrawAmount(Dialog dialog)
        {
            if (dialog.PlayUntil == -1)
            {
                ImGuiHelpers.DisabledButton($" \uf534 ");
                ImGuiHelpers.HelpTooltip($"Plays forever");
            }
            else
            {
                ImGuiHelpers.DisabledButton($" {dialog.PlayUntil} ");
                ImGuiHelpers.HelpTooltip($"Plays {dialog.PlayUntil} time(s)");
            }
        }
    }
}