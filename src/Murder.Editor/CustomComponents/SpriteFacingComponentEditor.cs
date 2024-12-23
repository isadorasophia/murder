using ImGuiNET;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Helpers;
using Murder.Utilities;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.CustomComponents;

[CustomComponentOf(typeof(SpriteFacingComponent))]
public class SpriteFacingComponentEditor : CustomComponent
{
    private int _selectedSlice = 1;
    private int _lastSize = 0;
    private int _makeAuto = 0;
    protected override bool DrawAllMembersWithTable(ref object target)
    {
        bool fileChanged = false;
        SpriteFacingComponent sprite = (SpriteFacingComponent)target;

        if (_lastSize!= sprite.FacingInfo.Length)
        {
            _lastSize = sprite.FacingInfo.Length;
            _selectedSlice = Calculator.WrapAround(_selectedSlice, 0, sprite.FacingInfo.Length);
            _selectedSlice = Math.Max(1, _selectedSlice);
        }

        if (sprite.FacingInfo.Length == 0)
        {
            if (ImGui.Button("Auto 2 Directions"))
            {
                ImGui.OpenPopup("auto_sure");
                _makeAuto = 2;
            }
            ImGui.SameLine();
            if (ImGui.Button("Auto 4 Directions"))
            {
                ImGui.OpenPopup("auto_sure");
                _makeAuto = 4;
            }
            ImGui.SameLine();
            if (ImGui.Button("Auto 8 Directions"))
            {
                ImGui.OpenPopup("auto_sure");
                _makeAuto = 8;
            }

            bool _ = true;
            if (ImGui.BeginPopupModal("auto_sure", ref _, ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text("Are you sure you want to override this component?");
                if(ImGui.Button("yes"))
                {
                    fileChanged = true;
                    sprite = MakeAutomatic(_makeAuto);
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }

        if (ImGui.BeginTable($"field_{target.GetType().Name}", 2,
            ImGuiTableFlags.SizingFixedSame | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInnerH))
        {

            ImGui.TableSetupColumn("a", ImGuiTableColumnFlags.WidthFixed, -1, 0);
            ImGui.TableSetupColumn("b", ImGuiTableColumnFlags.WidthStretch, -1, 1);


            ImGui.TableNextColumn();

            ImGui.Dummy(new Vector2(100, 100));
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();
            var mid = Vector2.Lerp(min, max, 0.5f);

            var draw = ImGui.GetForegroundDrawList();
            uint faded = Color.ToUint(Game.Profile.Theme.Faded);
            uint highlight = Color.ToUint(Game.Profile.Theme.Accent);

            draw.AddCircle(mid, 50, faded);

            float offset = sprite.AngleStart * Calculator.TO_DEG;
            if (ImGui.DragFloat("Offset", ref offset, 0.3f, -180, 180))
            {
                fileChanged = true;
                sprite = sprite with
                {
                    AngleStart = offset * Calculator.TO_RAD
                };
            }

            DrawCircle(sprite, mid, draw, faded, highlight);

            ImGui.TableNextColumn();

            int slices = sprite.FacingInfo.Length;
            ImGui.Text("Suffix:");
            ImGui.SameLine();
            if (ImGui.InputInt("##slices", ref slices))
            {
                fileChanged = true;
                
            }

            if (sprite.FacingInfo.Length > 0)
            {
                ImGui.SliderInt("Selected Slice", ref _selectedSlice, 1, sprite.FacingInfo.Length + 1);

                if (_selectedSlice <= sprite.FacingInfo.Length)
                {
                    // This is a regular slice
                    DrawSelectedSlice(ref fileChanged, ref sprite);
                }
                else
                {
                    // This is the last (or default) slice
                    DrawDefaultSlice(ref fileChanged, ref sprite);
                }
            }
            else
            {
                // There's only the default suffix
                DrawDefaultSlice(ref fileChanged, ref sprite);
            }

            sprite = HandleSlicesChange(sprite, slices);

            if (fileChanged)
            {
                target = sprite;
            }
            
            ImGui.EndTable();
        }

        return fileChanged;
    }

    private SpriteFacingComponent MakeAutomatic(int totalSlices)
    {
        var facingInfos = ImmutableArray.CreateBuilder<FacingInfo>(totalSlices);
        float angleSize = 2 * MathF.PI / totalSlices; // Divide 360 degrees by the number of slices

        for (int i = 0; i < totalSlices - 1; i++)
        {
            // Adding default FacingInfo for each slice
            (string name, bool flip) = DirectionHelper.GetName(i, totalSlices, true);
            facingInfos.Add(new FacingInfo
            {
                AngleSize = angleSize,
                Suffix = name, // Example suffix, you can customize this
                Flip = flip // DefaultInitialization flip setting
            });
        }

        // Final slice is the default one
        {
            (string name, bool flip) = DirectionHelper.GetName(totalSlices - 1, totalSlices, true);
            return new SpriteFacingComponent
            {
                AngleStart = totalSlices > 2 ? (-angleSize / 2f) : 0,
                DefaultSuffix = name,
                DefaultFlip = flip,
                FacingInfo = facingInfos.ToImmutable()
            };
        }
    }


    private void DrawSelectedSlice(ref bool fileChanged, ref SpriteFacingComponent sprite)
    {
        int selected = Math.Clamp(_selectedSlice - 1, 0, sprite.FacingInfo.Length);

        float currentAngle = 0;
        for (int i = 0; i < selected; i++)
        {
            currentAngle += sprite.FacingInfo[i].AngleSize;
        }

        var info = sprite.FacingInfo[selected];

        float angle = info.AngleSize * Calculator.TO_DEG;
        if (ImGui.DragFloat($"##{selected}_angleSize", ref angle, 0.1f, 0, 360 - currentAngle * Calculator.TO_DEG))
        {
            fileChanged = true;
            sprite = sprite with
            {
                FacingInfo = sprite.FacingInfo.SetItem(selected, new FacingInfo()
                {
                    Suffix = info.Suffix,
                    AngleSize = angle * Calculator.TO_RAD,
                    Flip = info.Flip
                })
            };
        }

        string suffix = info.Suffix ?? string.Empty;
        if (ImGui.InputText($"##{selected}_suffix", ref suffix, 256))
        {
            fileChanged = true;
            sprite = sprite with
            {
                FacingInfo = sprite.FacingInfo.SetItem(selected, new FacingInfo()
                {
                    Suffix = suffix,
                    AngleSize = info.AngleSize,
                    Flip = info.Flip
                })
            };
        }

        bool flip = info.Flip;
        if (ImGui.Checkbox($"Flip##{selected}_flip", ref flip))
        {
            fileChanged = true;
            sprite = sprite with
            {
                FacingInfo = sprite.FacingInfo.SetItem(selected, new FacingInfo()
                {
                    Suffix = suffix,
                    AngleSize = info.AngleSize,
                    Flip = flip
                })
            };
        }
    }

    private void DrawDefaultSlice(ref bool fileChanged, ref SpriteFacingComponent sprite)
    {
        float currentAngle = 0;
        for (int i = 0; i < sprite.FacingInfo.Length; i++)
        {
            currentAngle += sprite.FacingInfo[i].AngleSize;
        }
        currentAngle = 360 - currentAngle * Calculator.TO_DEG;
        if (ImGui.DragFloat($"##angleSize_empty", ref currentAngle, 0, currentAngle, currentAngle))
        {

        }

        string suffix = sprite.DefaultSuffix ?? string.Empty;
        if (ImGui.InputText($"##default_suffix", ref suffix, 256))
        {
            fileChanged = true;
            sprite = sprite with
            {
                DefaultSuffix = suffix
            };
        }

        bool flip = sprite.DefaultFlip;
        if (ImGui.Checkbox("Flip", ref flip))
        {
            fileChanged = true;
            sprite = sprite with
            {
                DefaultFlip = flip
            };
        }
    }

    private void DrawCircle(SpriteFacingComponent sprite, Vector2 mid, ImDrawListPtr draw, uint faded, uint highlight)
    {
        float currentAngle = sprite.AngleStart;

        if (sprite.FacingInfo.Length + 1 == _selectedSlice)
        {
            draw.AddLine(mid, mid + Vector2Helper.Right.Rotate(sprite.AngleStart) * 50, highlight, 3);
        }
        else
        {
            draw.AddLine(mid, mid + Vector2Helper.Right.Rotate(sprite.AngleStart) * 50, faded);
        }
        
        for (int i = 0; i < sprite.FacingInfo.Length; i++)
        {
            var info = sprite.FacingInfo[i];

            Vector2 previous = Vector2Helper.Right.Rotate(currentAngle);
            Vector2 line = Vector2Helper.Right.Rotate(currentAngle + info.AngleSize);
            Vector2 text = Vector2Helper.Right.Rotate(Calculator.Lerp(currentAngle, currentAngle + info.AngleSize, 0.5f));

            string suffix = info.Suffix ?? string.Empty;
            draw.AddText(mid + text * 30 - new Vector2(suffix.Length * 4, 8), faded, suffix);
            
            if (i == _selectedSlice - 1)
            {
                draw.AddLine(mid, mid + previous * 50, highlight, 3);
                draw.AddLine(mid, mid + line * 50, highlight, 3);
            }
            else
            {
                if (sprite.FacingInfo.Length + 1 == _selectedSlice && i == sprite.FacingInfo.Length - 1)
                {
                    draw.AddLine(mid, mid + line * 50, highlight, 3);
                }
                else
                {
                    draw.AddLine(mid, mid + line * 50, faded);
                }
            }
            currentAngle += info.AngleSize;
        }

        {
            Vector2 text = Vector2Helper.Right.Rotate(Calculator.Lerp(currentAngle, 2 * MathF.PI, 0.5f) + sprite.AngleStart / 2f);
            draw.AddText(mid + text * 30 - new Vector2(sprite.DefaultSuffix.Length * 4, 8), faded, sprite.DefaultSuffix);
        }
    }

    private SpriteFacingComponent HandleSlicesChange(SpriteFacingComponent sprite, int slices)
    {
        if (sprite.FacingInfo.Length == slices)
        {
            return sprite;
        }
        else
        {
            var newSprite = sprite.Resize(_selectedSlice, Math.Max(0, slices));
            _selectedSlice += slices - sprite.FacingInfo.Length;

            return newSprite;
        }

    }
}
