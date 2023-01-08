using Bang.Contexts;
using Bang.Systems;
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
    [Filter(ContextAccessorFilter.None)]
    public class CursorSystem : IMonoRenderSystem, IStartupSystem
    {
        private AsepriteAsset? _cursorTexture;
        private AsepriteAsset? _handCursorTexture;
        private AsepriteAsset? _pointerCursorTexture;
        private AsepriteAsset? _eyeCursorTexture;

        public void Draw(RenderContext render, Context context)
        {
            EditorHook hook = context.World.GetUnique<EditorComponent>().EditorHook;

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
        }

        public void Start(Context context)
        {
            _cursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.EditorAssets.Normal)!;
            _handCursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.EditorAssets.Hand)!;
            _pointerCursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.EditorAssets.Point)!;
            _eyeCursorTexture = Game.Data.TryGetAsset<AsepriteAsset>(Game.Profile.EditorAssets.Eye)!;
        }

        private void RenderCursor(RenderContext render, EditorHook hook, AsepriteAsset? cursorTexture)
        {
            if (cursorTexture != null)
            {
                RenderServices.RenderSprite(
                    spriteBatch: render.UiBatch,
                    pos: hook.CursorScreenPosition,
                    rotation: 0f,
                    scale: Vector2.One,
                    animationId: string.Empty,
                    ase: cursorTexture,
                    animationStartedTime: 0,
                    color: Color.White,
                    blend: RenderServices.BLEND_NORMAL);
            }
        }
    }
}
