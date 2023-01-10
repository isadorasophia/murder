using ImGuiNET;
using Murder.Assets.Graphics;
using Murder.Core.Geometry;
using Murder.Core.Particles;
using Murder.Editor.CustomComponents;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Editor.Utilities;
using Murder.Utilities;
using Newtonsoft.Json.Linq;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(Particle))]
    internal class ParticleField : CustomField
    {
        public override (bool modified, object? result) ProcessInput(EditorMember member, object? fieldValue)
        {
            bool modified = false;

            Particle particle = (Particle)fieldValue!;

            using TableMultipleColumns table = new($"particle", flags: ImGuiTableFlags.SizingFixedFit, -1, 400);

            ImGui.TableNextColumn();

            ImGui.TextColored(Game.Profile.Theme.Faded, "\uf5aa");

            ImGuiHelpers.HelpTooltip("Texture of the particles.");

            ImGui.TableNextColumn();

            ParticleTexture texture = particle.Texture;
            if (DrawValue(ref texture, nameof(ParticleTexture.Kind)))
            {
                particle = particle.WithTexture(texture);
                modified = true;
            }

            switch (texture.Kind)
            {
                case ParticleTextureKind.Rectangle:
                    if (DrawValue(ref texture, nameof(ParticleTexture.Rectangle)))
                    {
                        particle = particle.WithTexture(texture);
                        modified = true;
                    }

                    break;

                case ParticleTextureKind.Circle:
                    if (DrawValue(ref texture, nameof(ParticleTexture.Circle)))
                    {
                        particle = particle.WithTexture(texture);
                        modified = true;
                    }

                    break;
                    
                case ParticleTextureKind.Asset:
                    Guid asset = texture.Asset;
                    if (SearchBox.SearchAsset(ref asset, typeof(AsepriteAsset)))
                    {
                        particle = particle.WithTexture(new(asset));
                        modified = true;
                    }
                    
                    break;
            }
            
            ImGui.TableNextRow();
            //ImGui.TableNextColumn();

            if (modified)
            {
                fieldValue = particle;
            }
            
            modified |= CustomComponent.DrawAllMembers(
                fieldValue, 
                exceptForMembers: new HashSet<string>() { nameof(Particle.Texture) });

            return (modified, fieldValue);
        }
    }
}
