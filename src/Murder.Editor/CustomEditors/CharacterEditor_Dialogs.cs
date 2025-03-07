using Bang.Components;
using ImGuiNET;
using Murder.Assets;
using Murder.Assets.Sounds;
using Murder.Components;
using Murder.Core.Dialogs;
using Murder.Core.Sounds;
using Murder.Editor.CustomFields;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
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

            DialogueId id = new(info.ActiveSituation, info.ActiveDialog, lineIndex);
            Dialog dialog = situation.Value.Dialogs[id.DialogId];

            Line line = dialog.Lines[lineIndex].WithSpeaker(speaker).WithPortrait(portrait);
            dialog = dialog.WithLineAt(lineIndex, line);

            _script.SetSituation(situation.Value.WithDialogAt(id.DialogId, dialog));
            _script?.SetCustomPortraitAt(
                id: id,
                speaker, portrait);

            return line;
        }

        private Line? ModifyEventAt(DialogueId id, int lineIndex, string? @event)
        {
            if (_script is null || !FetchActiveSituation(out Situation? situation))
            {
                return null;
            }

            Dialog dialog = situation.Value.Dialogs[id.DialogId];

            Line line = dialog.Lines[lineIndex].WithEvent(@event);
            dialog = dialog.WithLineAt(lineIndex, line);

            _script.SetSituation(situation.Value.WithDialogAt(id.DialogId, dialog));
            _script?.SetEventInfoAt(
                id: id,
                @event);

            return line;
        }

        private DialogAction? ModifyComponentAt(ScriptInformation info, int actionIndex, IComponent c)
        {
            if (_script is null || !FetchActiveSituation(out Situation? situation))
            {
                return null;
            }

            DialogueId id = new(info.ActiveSituation, info.ActiveDialog, actionIndex);
            Dialog dialog = situation.Value.Dialogs[id.DialogId];

            if (dialog.Actions is null)
            {
                return null;
            }

            DialogAction action = dialog.Actions!.Value[actionIndex];
            action = action.WithComponent(c);

            _script.SetSituation(situation.Value.WithDialogAt(id.DialogId, dialog));
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
            DrawChance(dialog);

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

                    case FactKind.Float:
                        ImGui.TextColored(Game.Profile.Theme.Accent, criterion.FloatValue!.Value.ToString());
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
                    string value = LocalizationServices.GetLocalizedString(line.Text.Value, LocalizationFlags.ForceDefault);

                    float textHeight = (ImGui.GetItemRectSize().Y * .45f -
                        ImGui.CalcTextSize(value, wrapWidth: ImGui.GetContentRegionAvail().X).Y) / 2f;

                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + textHeight);

                    bool selected = info.Stage.EditorHook.SelectedDialogueLine?.Text?.Id == line.Text?.Id;
                    if (ImGuiHelpers.ColoredIconButton('\uf06e', $"highlight_{i}", isActive: selected))
                    {
                        if (selected)
                        {
                            info.Stage.EditorHook.SelectedDialogueLine = null;
                        }
                        else
                        {
                            Debug.Assert(_script is not null);
                            info.Stage.EditorHook.SelectedSituation = new(_script.Guid, info.ActiveSituation);
                            info.Stage.EditorHook.SelectedDialogueLine = line;
                            info.Stage.EditorHook.ForceUpdateDialogue = true;
                        }
                    }

                    ImGuiHelpers.HelpTooltip(selected ? "Hide line with game UI" : "Preview line with game UI");

                    ImGui.SameLine();
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

                    if (speakerGuid != Guid.Empty)
                    {
                        ImGui.SameLine();
                    }

                    ImGui.PushItemWidth(200);

                    ImGui.PushID($"portrait_kind_{i}");

                    SpeakerAsset? speaker = Game.Data.TryGetAsset<SpeakerAsset>(speakerGuid);
                    string? portrait = null;

                    // -- Select speaker portrait --
                    if (speaker is not null)
                    {
                        List<string> allPortraits = [.. speaker.Portraits.Keys.Order()];
                        int portraitIndex = line.Portrait is null ? -1 : allPortraits.IndexOf(line.Portrait);

                        ImGui.PushItemWidth(-1);

                        if (portraitIndex == -1 && allPortraits.Count > 0)
                        {
                            // Just set the first portrait as default.
                            portrait = speaker.DefaultPortrait ?? allPortraits[0];
                            line = ModifyPortraitAt(info, i, speakerGuid, portrait)!.Value;
                        }
                        else if (ImGui.Combo($"##portrait_{i}", ref portraitIndex, allPortraits.ToArray(), allPortraits.Count))
                        {
                            portrait = allPortraits[portraitIndex];
                            line = ModifyPortraitAt(info, i, speakerGuid, portrait)!.Value;
                        }

                        ImGui.PopItemWidth();
                    }

                    speaker ??= _script is null ? null : Game.Data.TryGetAsset<SpeakerAsset>(_script.Owner);

                    // -- Optional event sound --
                    if (speaker?.Events is not null && speaker.Events?.TryAsset is SpeakerEventsAsset speakerEvents)
                    {
                        DialogueId id = new(info.ActiveSituation, info.ActiveDialog, id: i);

                        bool chooseSound = true;

                        if (_script is not null &&
                            GetPortraitName(speaker, line) is string targetPortrait &&
                            speaker.Portraits[targetPortrait].Sound is SoundEventId portraitSound)
                        {
                            if (!_script.GetEventInfoFlags(id).HasFlag(LineInfoProperties.SkipDefaultPortraitSound))
                            {
                                chooseSound = false;

                                if (ImGuiHelpers.BlueIcon('\uf7d2', $"delete_sound_{i}"))
                                {
                                    _script.SetEventInfoAt(id, LineInfoProperties.SkipDefaultPortraitSound);
                                }

                                ImGui.SameLine();

                                if (ImGuiHelpers.IconButton('', $"play_sound_{i}")) EditorSoundServices.Play(portraitSound);
                                ImGui.SameLine();

                                if (ImGuiHelpers.IconButton('\uf04c', $"stop_sound_{i}")) SoundServices.StopAll(fadeOut: false);
                                ImGui.SameLine();

                                ImGui.TextColored(Game.Profile.Theme.Faded, portraitSound.Path);
                            }
                            else
                            {
                                if (ImGuiHelpers.BlueIcon('\uf1da', $"revert_sound_{i}"))
                                {
                                    _script.SetEventInfoAt(id, LineInfoProperties.None);

                                    // make sure this is no longer valid
                                    line = ModifyEventAt(id, i, @event: null) ?? line;
                                }

                                ImGui.SameLine();
                            }
                        }

                        if (chooseSound)
                        {
                            string[] names = [.. speakerEvents.Events.Keys.Order()];

                            string? name = line.Event;

                            if (name is not null &&
                                speakerEvents.Events.TryGetValue(name, out SpriteEventInfo spriteEventInfo) &&
                                spriteEventInfo.Sound is SoundEventId sound)
                            {
                                if (ImGuiHelpers.IconButton('', $"play_sound_{i}")) EditorSoundServices.Play(sound);
                                ImGui.SameLine();

                                if (ImGuiHelpers.IconButton('\uf04c', $"stop_sound_{i}")) SoundServices.StopAll(fadeOut: false);
                                ImGui.SameLine();

                                if (ImGuiHelpers.IconButton('\uf2ed', $"reset_sound_{i}", bgColor: Game.Profile.Theme.Faded))
                                {
                                    name = null;
                                    line = ModifyEventAt(id, i, name) ?? line;
                                }

                                ImGui.SameLine();
                            }

                            if (name is null)
                            {
                                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.Faded);
                                name = "Play an event...";
                            }
                            else
                            {
                                ImGui.PushStyleColor(ImGuiCol.Text, Game.Profile.Theme.White);
                            }

                            ImGui.PushItemWidth(-1);

                            if (StringField.ProcessStringCombo($"##portrait_sound_{i}", ref name, names))
                            {
                                _ = ModifyEventAt(id, i, name) ?? line;
                            }

                            ImGui.PopItemWidth();
                            ImGui.PopStyleColor();
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

            ImmutableDictionary<string, Situation> situations = _script.Situations;
            string[] situationNames = situations.Select(s => s.Key).ToArray();

            if (situationNames.Length == 0)
            {
                ImGui.TextColored(Game.Profile.Theme.Warning, "No situation to go next?");
                return;
            }

            if (_script.TryFetchSituation(dialog.GoTo) is Situation target)
            {
                if (ImGui.Button(target.Name))
                {
                    SwitchSituation(info, target);
                }
            }
            else if (dialog.IsExit)
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

            ImGui.SameLine();
        }

        private void DrawChance(Dialog dialog)
        {
            if (dialog.Chance != 1)
            {
                ImGuiHelpers.DisabledButton($" \uf522 {Calculator.RoundToInt(dialog.Chance * 100)}% ");
                ImGuiHelpers.HelpTooltip($"Chance of playing");
            }
        }
    }
}