using Bang.Contexts;
using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Data;
using Murder.Editor;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;

namespace Murder.Systems
{
    [OnlyShowOnDebugView]
    public class CursorSystem : IMonoRenderSystem
    {
        private AsepriteAsset? _cursorTexture;
        private AsepriteAsset? _handCursorTexture;
        private AsepriteAsset? _pointerCursorTexture;
        private AsepriteAsset? _eyeCursorTexture;

        private bool _initialized = false;

        public ValueTask Draw(RenderContext render, Context context)
        {
            if (!_initialized)
            {
                _cursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.Cursors.Normal)!;
                _handCursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.Cursors.Hand)!;
                _pointerCursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.Cursors.Point)!;
                _eyeCursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.Cursors.Eye)!;

                _initialized = true;
            }

            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

            Point cursorPosition = render.Camera.GetCursorWorldPosition(hook.Offset, hook.StageSize);
            if (render.Camera.Bounds.Contains(cursorPosition))
            {
                Game.Instance.IsMouseVisible = false;
            }
            else
            {
                Game.Instance.IsMouseVisible = true;
            }

            switch (hook.Cursor)
            {
                case EditorHook.CursorStyle.Normal:
                    RenderCursor(render, hook, _cursorTexture);
                    break;
                case EditorHook.CursorStyle.Hand:
                    RenderCursor(render, hook, _handCursorTexture);
                    break;
                case EditorHook.CursorStyle.Point:
                    RenderCursor(render, hook, _pointerCursorTexture);
                    break;
                case EditorHook.CursorStyle.Eye:
                    RenderCursor(render, hook, _eyeCursorTexture);
                    break;
                default:
                    break;
            }

            return default;
        }

        private void RenderCursor(RenderContext render, EditorHook hook, AsepriteAsset? cursorTexture)
        {
            if (cursorTexture != null)
            {
                RenderServices.RenderSprite(
                    spriteBatch: render.UiBatch,
                    atlasId: AtlasId.Editor,
                    pos: hook.CursorScreenPosition,
                    rotation: 0f,
                    scale: Vector2.One * Architect.Instance.DPIScale/100f * 1.5f,
                    animationId: string.Empty,
                    ase: cursorTexture,
                    animationStartedTime: 0,
                    color: Color.White);
            }
        }
    }
}
