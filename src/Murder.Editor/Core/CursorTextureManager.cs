﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Murder.Assets;
using Murder.Assets.Graphics;
using Murder.Core.Graphics;
using System.Collections.Immutable;

namespace Murder.Editor.EditorCore
{
    /// <summary>
    /// Manages texture being displayed within the ImGui.
    /// </summary>
    public class CursorTextureManager : IDisposable
    {
        private struct CursorInfo
        {
            public readonly Core.Graphics.Animation Animation;
            public readonly ImmutableArray<MouseCursor> Cursors;

            public CursorInfo(Animation animation, ImmutableArray<MouseCursor> cursors) =>
                (Animation, Cursors) = (animation, cursors);
        }

        private readonly ImmutableDictionary<CursorStyle, CursorInfo> _cursors;

        public CursorTextureManager(EditorAssets editorAssets)
        {
            var builder = ImmutableDictionary.CreateBuilder<CursorStyle, CursorInfo>();

            CursorInfo? cursors = AcquireCursorsForSprite(editorAssets.Normal);
            if (cursors is not null)
            {
                builder.Add(CursorStyle.Normal, cursors.Value);
            }

            cursors = AcquireCursorsForSprite(editorAssets.Hand);
            if (cursors is not null)
            {
                builder.Add(CursorStyle.Hand, cursors.Value);
            }

            cursors = AcquireCursorsForSprite(editorAssets.Point);
            if (cursors is not null)
            {
                builder.Add(CursorStyle.Point, cursors.Value);
            }

            cursors = AcquireCursorsForSprite(editorAssets.Eye);
            if (cursors is not null)
            {
                builder.Add(CursorStyle.Eye, cursors.Value);
            }

            _cursors = builder.ToImmutable();
        }

        private int _lastFrame = 0;
        private CursorStyle? _lastStyle = null;

        private float? _switchedTime = null;

        public void RenderCursor(CursorStyle style)
        {
            if (_switchedTime is null || style != _lastStyle)
            {
                _switchedTime = Game.NowUnscaled;
            }

            if (_cursors.TryGetValue(style, out CursorInfo info))
            {
                var anim = info.Animation.Evaluate(Game.NowUnscaled - _switchedTime.Value, Game.PreviousNowUnscaled, true, info.Animation.AnimationDuration);
                if (anim.Frame == _lastFrame && style == _lastStyle)
                {
                    return;
                }

                Mouse.SetCursor(info.Cursors[anim.Frame]);
                _lastFrame = anim.Frame;
            }

            _lastStyle = style;
        }

        private CursorInfo? AcquireCursorsForSprite(Guid guid)
        {
            if (Game.Data.TryGetAsset<SpriteAsset>(guid) is SpriteAsset asset)
            {
                return AcquireCursors(asset, string.Empty);
            }

            return null;
        }

        private CursorInfo? AcquireCursors(SpriteAsset sprite, string animationId)
        {
            if (!sprite.Animations.TryGetValue(animationId, out Animation animation))
            {
                return null;
            }

            var builder = ImmutableArray.CreateBuilder<MouseCursor>();

            foreach (int frame in animation.Frames)
            {
                AtlasCoordinates coordinate = sprite.GetFrame(frame);

                TextureAtlas? atlas = Game.Data.TryFetchAtlas(coordinate.AtlasId);
                if (atlas is null)
                {
                    continue;
                }

                using Texture2D texture = atlas.CreateTextureFromAtlas(coordinate, SurfaceFormat.Color, scale: Game.Profile.GameScale);
                builder.Add(MouseCursor.FromTexture2D(texture, sprite.Origin.X, sprite.Origin.Y));
            }

            return new(animation, builder.ToImmutable());
        }

        public void Dispose()
        {
            foreach (var (_, cursors) in _cursors)
            {
                foreach (var cursor in cursors.Cursors)
                {
                    cursor?.Dispose();
                }
            }

            _cursors.Clear();
        }
    }
}