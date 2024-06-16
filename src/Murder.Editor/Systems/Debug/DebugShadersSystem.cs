using Bang.Contexts;
using Bang.Systems;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.ImGuiExtended;
using Murder.Utilities;

namespace Murder.Editor.Systems;

[DoNotPause]
[OnlyShowOnDebugView]
[Filter(ContextAccessorFilter.None)]
public class DebugShadersSystem : IGuiSystem
{
    private bool _show;
    private int _selectedShader;
    public void DrawGui(RenderContext render, Context context)
    {
        ImGui.BeginMainMenuBar();

        if (ImGui.BeginMenu("Show"))
        {
            ImGui.MenuItem("Shaders", "", ref _show);
            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();

        if (!_show)
        {
            return;
        }

        if (ImGui.BeginChild("shaders", new System.Numerics.Vector2(), ImGuiChildFlags.AlwaysAutoResize | ImGuiChildFlags.AutoResizeY))
        {
            DrawShaders();
        }
        ImGui.EndChild();
    }
    private void DrawShaders()
    {
        using var table = new TableMultipleColumns("Shaders", ImGuiTableFlags.Borders, [0, -1]);
        
        ImGui.TableNextColumn();

        for (int i = 0; i < Game.Data.CustomGameShaders.Length; i++)
        {
            if (Game.Data.CustomGameShaders[i] is Effect s)
            {
                if (ImGui.Selectable(s.Name, _selectedShader == i))
                {
                    _selectedShader = i;
                }
            }
        }

        ImGui.TableNextColumn();

        if (Game.Data.CustomGameShaders[_selectedShader] is Effect shader)
        { 
            foreach (var p in shader.Parameters)
            {
                switch (p.ParameterType)
                {
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Void:
                        ImGui.Text($"{p.Name}: {p.ParameterType}");
                        break;
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Bool:
                        bool boolValue = p.GetValueBoolean();
                        if (ImGui.Checkbox($"{p.Name}({p.ParameterType}): {boolValue}", ref boolValue))
                        {
                            p.SetValue(boolValue);
                        }
                        break;
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Int32:
                        {
                            string stringValue;
                            switch (p.ParameterClass)
                            {
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Scalar:
                                    stringValue = DrawFloatInt(p);
                                    break;
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Vector:
                                    stringValue = p.GetValueVector2().ToString();
                                    break;
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Matrix:
                                    stringValue = p.GetValueMatrix().ToString();
                                    break;
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Object:
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Struct:
                                default:
                                    stringValue = "unknown value";
                                    break;
                            }
                            ImGui.Text($"{p.Name}({p.ParameterType}): {stringValue}");
                        }
                        break;
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Single:
                        {
                            string stringValue;
                            switch (p.ParameterClass)
                            {
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Scalar:
                                    stringValue = DrawFloatField(p);
                                    break;
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Vector:
                                    if (p.Elements?.Count == 0)
                                    {
                                        if (p.ColumnCount == 2)
                                        {
                                            stringValue = DrawVector2Field(p);
                                        }
                                        else
                                        {
                                            stringValue = p.GetValueVector3().ToString();
                                        }
                                    }
                                    else
                                    {
                                        stringValue = $"Array {p.RowCount} values:\n";
                                        if (p.ColumnCount == 2)
                                        {
                                            stringValue = DrawVector2Field(p);
                                        }
                                        else
                                        {
                                            var array = p.GetValueVector3Array();
                                            foreach (var v in array)
                                            {
                                                stringValue += v.ToString() + "\n";
                                            }
                                        }
                                    }

                                    break;
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Matrix:
                                    stringValue = p.GetValueMatrix().ToString();
                                    break;
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Object:
                                case Microsoft.Xna.Framework.Graphics.EffectParameterClass.Struct:
                                default:
                                    stringValue = "unknown value";
                                    break;
                            }
                            if (!string.IsNullOrEmpty(stringValue))
                            {
                                ImGui.Text($"{p.Name}({p.ParameterType}): {stringValue}");
                            }
                        }
                        break;
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.String:
                        ImGui.Text($"{p.Name}({p.ParameterType}): {p.GetValueString()}");
                        break;
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Texture:
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Texture1D:
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Texture2D:
                        {
                            var texture = p.GetValueTexture2D();
                            if (texture != null)
                            {
                                ImGui.Text($"{p.Name}({p.ParameterType}): {texture.Name}({texture.Width}x{texture.Height}px)");
                            }
                        }
                        break;
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.Texture3D:
                        {
                            var texture = p.GetValueTexture3D();
                            ImGui.Text($"{p.Name}({p.ParameterType}): {texture.Name}({texture.Width}x{texture.Height}x{texture.Depth}px)");
                        }
                        break;
                    case Microsoft.Xna.Framework.Graphics.EffectParameterType.TextureCube:
                        {
                            var texture = p.GetValueTextureCube();
                            ImGui.Text($"{p.Name}({p.ParameterType}): {texture.Name}");
                        }
                        break;
                    default:
                        ImGui.Text($"{p.Name}({p.ParameterType}): Unknown value");
                        break;
                }
            }
        }
    }


    private static string DrawFloatInt(EffectParameter? p)
    {
        if ( p == null )
        {
            ImGui.Text("Null parameter");
            return string.Empty;
        }
        var floatValue = p.GetValueInt32();

        if (ShaderHelper.TryGetAnotationVector2(p, "slider") is System.Numerics.Vector2 minMax)
        {
            ImGui.Text(p.Name);
            ImGui.SameLine();
            ImGui.SliderInt($"##{p.Name}", ref floatValue, (int)minMax.X, (int)minMax.Y);

            p.SetValue(floatValue);
            return string.Empty;
        }

        return floatValue.ToString();
    }
    private static string DrawFloatField(EffectParameter? p)
    {
        if (p == null)
        {
            ImGui.Text("Null parameter");
            return string.Empty;
        }

        string stringValue = p.GetValueVector2().ToString();
        var floatValue = p.GetValueSingle();

        if (ShaderHelper.TryGetAnotationVector2(p, "slider") is System.Numerics.Vector2 minMax)
        {
            ImGui.Text(p.Name);
            ImGui.SameLine();
            ImGui.SliderFloat($"##{p.Name}", ref floatValue, minMax.X, minMax.Y);

            p.SetValue(floatValue);
            stringValue = string.Empty;
        }

        return stringValue;
    }
    private static string DrawVector2Field(EffectParameter? p)
    {
        if (p == null)
        {
            ImGui.Text("Null parameter");
            return string.Empty;
        }

        var vector2Value = p.GetValueVector2().ToSysVector2();
        string stringValue = $"{vector2Value.X:0.0000}, {vector2Value.Y:0.0000s}";

        if (ShaderHelper.TryGetAnotationVector2(p, "slider") is System.Numerics.Vector2 minMax)
        {
            ImGui.Text(p.Name);
            ImGui.SameLine();
            ImGui.SliderFloat2($"##{p.Name}", ref vector2Value, minMax.X, minMax.Y);

            p.SetValue(vector2Value.ToXnaVector2());
            stringValue = string.Empty;
        }

        return stringValue;
    }
}
