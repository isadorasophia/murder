using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using Murder.Core.Input;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Editor.CustomFields;


[CustomFieldOf(typeof(InputButton))]
public class InputButtonField : CustomField
{
    public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
    {
        InputButton? inputButton = (InputButton?)fieldValue;
        bool modified = false;

        if (inputButton != null)
        {
            ImGui.Text(member.Name);

            var source = inputButton.Value.Source;
            if (ImGuiHelpers.DrawEnumField<InputSource>(member.Name + "_source", ref source))
            {
                modified = true;
            }

            Keys? keys = inputButton.Value.Keyboard;
            Buttons? buttons = inputButton.Value.Gamepad;
            MouseButtons? mouseButtons = inputButton.Value.Mouse;
            GamepadAxis? gamepadAxis = inputButton.Value.Axis;

            switch (source)
            {
                case InputSource.None:
                    break;
                case InputSource.Keyboard:
                    Keys chosenKey = keys ?? Keys.None;
                    if (ImGuiHelpers.DrawEnumFieldWithSearch<Keys>(member.Name + "_keyboard", ref chosenKey))
                    {
                        modified = true;
                    }
                    keys = chosenKey;
                    break;
                case InputSource.Mouse:
                    MouseButtons chosenMouse = mouseButtons ?? MouseButtons.Left;
                    if (ImGuiHelpers.DrawEnumFieldWithSearch<MouseButtons>(member.Name + "_mouse", ref chosenMouse))
                    {
                        modified = true;
                    }
                    mouseButtons = chosenMouse;
                    break;
                case InputSource.Gamepad:
                    Buttons chosenButton = buttons ?? Buttons.BigButton;
                    if (ImGuiHelpers.DrawEnumFieldWithSearch<Buttons>(member.Name + "_gamepad", ref chosenButton))
                    {
                        modified = true;
                    }
                    buttons = chosenButton;
                    break;
                case InputSource.GamepadAxis:
                    GamepadAxis chosenAxis = gamepadAxis ?? GamepadAxis.LeftThumb;
                    if (ImGuiHelpers.DrawEnumFieldWithSearch<GamepadAxis>(member.Name + "_gamepadAxis", ref chosenAxis))
                    {
                        modified = true;
                    }
                    gamepadAxis = chosenAxis;
                    break;
            }

            if (modified)
            {
                inputButton = new InputButton(source, keys, buttons, mouseButtons, gamepadAxis);
            }

            return (modified, inputButton);
        }

        return (false, fieldValue);
    }
}
