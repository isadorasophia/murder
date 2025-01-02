using Bang.Components;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Components.Graphics;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Messages;
using Murder.Helpers;
using Murder.Utilities;

namespace Murder.Editor.Systems;

/// <summary>
/// This system will draw selected entities and drag them through the map.
/// </summary>
[DoNotPause]
[OnlyShowOnDebugView]
[Requires(typeof(EditorSystem))]
[WorldEditor(startActive: true)]
[Filter(ContextAccessorFilter.None)] // Skip cutscene and sounds.
public class EntitiesShortcutsSystem : GenericSelectorSystem, IUpdateSystem
{
    public void Update(Context context)
    {
        if (context.World.TryGetUnique<EditorComponent>() is not EditorComponent editorComponent)
        {
            return;
        }

        var hook = editorComponent.EditorHook;

        if (hook.UsingGui || ImGui.IsAnyItemActive())
        {
            return;
        }

        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.R))
        {
            foreach (var entity in hook.AllSelectedEntities)
            {
                Direction currentDirection = entity.Value.TryGetFacing()?.Direction ?? Direction.Right;
                currentDirection = currentDirection.Rotate90Degrees();
                entity.Value.SetFacing(currentDirection);

                entity.Value.SendMessage(new AssetUpdatedMessage(typeof(FacingComponent)));

                if (entity.Value.TryGetSprite() is SpriteComponent sprite)
                {
                    if (!sprite.RotateWithFacing)
                    {
                        entity.Value.SetSprite(sprite with { RotateWithFacing = true });
                        entity.Value.SendMessage(new AssetUpdatedMessage(typeof(SpriteComponent)));
                    }
                }
            }
        }

        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.H))
        {
            foreach (var entity in hook.AllSelectedEntities)
            {
                if (entity.Value.TryGetFlipSprite() is FlipSpriteComponent flipSprite)
                {
                    ImageFlip flip = flipSprite.Orientation.FlipHorizontal();
                    if (flip == ImageFlip.None)
                    {
                        entity.Value.RemoveFlipSprite();
                    }
                    else
                    {
                        entity.Value.SetFlipSprite(flip);
                    }
                }
                else
                {
                    entity.Value.SetFlipSprite(ImageFlip.Horizontal);
                }

                entity.Value.SendMessage(new AssetUpdatedMessage(typeof(FlipSpriteComponent)));
            }
        }

        if (Game.Input.Shortcut(Microsoft.Xna.Framework.Input.Keys.V) && !Game.Input.Down(InputHelpers.OSActionModifier))
        {
            foreach (var entity in hook.AllSelectedEntities)
            {
                if (entity.Value.TryGetFlipSprite() is FlipSpriteComponent flipSprite)
                {
                    ImageFlip flip = flipSprite.Orientation.FlipVertical();
                    if (flip == ImageFlip.None)
                    {
                        entity.Value.RemoveFlipSprite();
                    }
                    else
                    {
                        entity.Value.SetFlipSprite(flip);
                    }
                }
                else
                {
                    entity.Value.SetFlipSprite(ImageFlip.Vertical);
                }

                entity.Value.SendMessage(new AssetUpdatedMessage(typeof(FlipSpriteComponent)));
            }
        }
    }
}