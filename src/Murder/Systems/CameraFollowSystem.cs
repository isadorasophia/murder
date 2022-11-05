using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Core.Graphics;
using Murder.Core.Geometry;
using Murder.Core;
using Murder.Components;
using Murder.Utilities;
using Murder;

namespace InstallWizard.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(CameraFollowComponent), typeof(PositionComponent))]
    internal class CameraFollowSystem : IFixedUpdateSystem
    {
        bool _firstStarted = false;

        public ValueTask FixedUpdate(Context context)
        {
            if (!context.HasAnyEntity || context.Entity is not Entity cameraman)
            {
                // Unable to find any entity to follow? Do not update the camera position in that case.
                return default;
            }
            if (!context.Entity.GetCameraFollow().Enabled)
                return default;

            Camera2D camera = ((MonoWorld)context.World).Camera;

            PositionComponent cameramanPosition = cameraman.GetGlobalPosition();
            PositionComponent targetPosition = context.Entity.GetGlobalPosition();

            if (!_firstStarted)
            {
                _firstStarted = true;
                cameramanPosition = targetPosition;
                cameraman.AddOrReplaceComponent(cameramanPosition);
            }
            else
            {
                Point deadzone = new(24, 24);
                var delta = (cameramanPosition.Pos - targetPosition.Pos);
                float lerpAmount = 1 - MathF.Pow(0.01f, Game.FixedDeltaTime);
                float lerpedX = cameramanPosition.X;
                float lerpedY = cameramanPosition.Y;
                if (MathF.Abs(delta.X) > deadzone.X)
                {
                    lerpedX = Calculator.Lerp(cameramanPosition.X, targetPosition.X + deadzone.X * MathF.Sign(delta.X), lerpAmount);
                }
                if (MathF.Abs(delta.Y) > deadzone.Y)
                {
                    lerpedY = Calculator.Lerp(cameramanPosition.Y, targetPosition.Y + deadzone.Y * MathF.Sign(delta.Y), lerpAmount);
                }

                cameramanPosition = new(lerpedX, lerpedY);
                cameraman.AddOrReplaceComponent(cameramanPosition);
            }
            
            Vector2 finalPosition = cameramanPosition.Pos - new Vector2(camera.Width, camera.Height) / 2;

            // Make sure that the camera stay in the dungeon limits.
            if (context.World.TryGetUnique<MapComponent>() is MapComponent map && map.Map != null)
            {
                finalPosition = ClampBounds(map.Width, map.Height, camera, finalPosition);
            }

            camera.Position = finalPosition;

            return default;
        }

        private Vector2 ClampBounds(int width, int height, Camera2D camera, Vector2 position)
        {
            if (position.X < 0) position.X = 0;
            if (position.Y < 0) position.Y = 0;

            var maxWidth = width * Grid.CellSize;
            var maxHeight = height * Grid.CellSize;

            if (position.X + camera.Bounds.Width > maxWidth) position.X = maxWidth - camera.Bounds.Width;
            if (position.Y + camera.Bounds.Height > maxHeight) position.Y = maxHeight - camera.Bounds.Height;

            return position;
        }
    }
}
