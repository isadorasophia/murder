using Bang.Components;
using Bang.Entities;
using Murder.Attributes;
using Murder.Core.Geometry;
using System.Numerics;

namespace Murder.Components
{
    public readonly struct RectPositionComponent : IParentRelativeComponent
    {
        private readonly IComponent? _parentBox;
        public bool HasParent => _parentBox is not null;

        [Bang.Serialize]
        private readonly float _paddingTop;
        [Bang.Serialize]
        private readonly float _paddingLeft;
        [Bang.Serialize]
        private readonly float _paddingBottom;
        [Bang.Serialize]
        private readonly float _paddingRight;

        public readonly Vector2 Size;

        [Slider]
        public readonly Vector2 Origin;
        public RectPositionComponent(float top, float left, float bottom, float right, Vector2 size, Vector2 origin, IComponent? parent)
        {
            _paddingTop = top;
            _paddingLeft = left;
            _paddingBottom = bottom;
            _paddingRight = right;
            Size = size;
            Origin = origin;
            _parentBox = parent;
        }

        public RectPositionComponent WithSize(Vector2 size) =>
            new(_paddingTop, _paddingLeft, _paddingBottom, _paddingRight, size, Origin, _parentBox);

        public void OnParentModified(IComponent parentComponent, Entity childEntity)
        {
            RectPositionComponent parentBox = (RectPositionComponent)parentComponent;

            childEntity.ReplaceComponent(new RectPositionComponent(_paddingTop, _paddingLeft, _paddingBottom, _paddingRight, Size, Origin, parentBox));
        }
        public Rectangle GetBox(Entity? entity, Point screenSize, Point? referenceSize = null)
        {
            // If we don't have a parent, we're at the screen, set it to the screen size
            // Otherwise, we're a child of a parent, so we need to get the parent's box
            Rectangle rect;
            referenceSize ??= screenSize;

            Vector2 scale = new Vector2(
                (float)screenSize.X / referenceSize.Value.X,
                (float)screenSize.Y / referenceSize.Value.Y);

            float scaleMin = Math.Min(scale.X, scale.Y);

            if (_parentBox is RectPositionComponent positionComponent)
            {
                rect = positionComponent.GetBox(entity, screenSize, referenceSize);
            }
            else
            {
                rect = new(0, 0, screenSize.X, screenSize.Y);
            }

            // Set the box size
            var width = (Size.X > 0 ? Size.X : rect.Width) * scaleMin;
            var height = (Size.Y > 0 ? Size.Y : rect.Height) * scaleMin;

            // Now we need to get the padding
            Rectangle box = new Rectangle(
                rect.X
                    + _paddingLeft * (1 - Origin.X) * scaleMin
                    + Origin.X * rect.Width
                    - Origin.X * width
                    - _paddingRight * Origin.X * 2 * scale.X,
                rect.Y
                    + _paddingTop * (1 - Origin.Y * 2) * scaleMin
                    + Origin.Y * rect.Height
                    - Origin.Y * height
                    - _paddingBottom * Origin.Y * 2 * scale.Y,
                rect.Width - (_paddingLeft * scaleMin + _paddingRight * scaleMin),
                rect.Height - (_paddingTop * scaleMin + _paddingBottom * scaleMin)
                );

            // Now we need to get the origin
            box.Top += Origin.Y * rect.Height - Origin.Y * box.Height;
            box.Left += Origin.X * rect.Width - Origin.X * box.Width;

            // Make sure we keep the correct size
            if (Size.X > 0)
                box.Width = width;

            if (Size.Y > 0)
                box.Height = height;

            // And we are done!
            return box;
        }

        public RectPositionComponent AddPadding(RectPositionComponent b) =>
            new(
                _paddingTop + b._paddingTop,
                _paddingLeft + b._paddingLeft,
                _paddingBottom + b._paddingBottom,
                _paddingRight + b._paddingRight,
                Size, Origin, _parentBox);

        public IParentRelativeComponent WithoutParent() => new RectPositionComponent(_paddingTop, _paddingLeft, _paddingBottom, _paddingRight, Size, Origin, default);
    }

}