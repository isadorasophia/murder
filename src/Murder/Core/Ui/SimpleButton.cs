using Murder.Assets.Graphics;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Services;

namespace Murder.Core.Ui
{
    public class SimpleButton
    {
        public enum ButtonState
        {
            Normal,
            Disabled,
            Hover,
            Down
        }
        
        [Tooltip("Make sure to have the animations: normal, disabled, hovered and down."), GameAssetId<SpriteAsset>]
        public Guid Images;
        public ButtonState State;
        public Rectangle Rectangle;

        public SimpleButton(Guid images, Rectangle rectangle) :
            this(images, ButtonState.Normal, rectangle)
        {
        }
        
        public SimpleButton(Guid images, ButtonState state, Rectangle rectangle)
        {
            Images = images;
            State = state;
            Rectangle = rectangle;
        }

        public void Update(Point cursorPosition, bool cursorClicked, bool cursorDown, Action action)
        {

            if (Rectangle.Contains(cursorPosition))
            {
                if (State != ButtonState.Down)
                {
                    if (cursorClicked)
                        State = ButtonState.Down;
                    else
                        State = ButtonState.Hover;
                }

                if (State == ButtonState.Down && !cursorDown)
                {
                    State = ButtonState.Hover;
                    action?.Invoke();
                    // Button Clicked!
                }
            }
            else if (!cursorDown)
            {
                if (State == ButtonState.Down)
                {
                    State = ButtonState.Normal;
                }
                
                if (State != ButtonState.Down)
                {
                    State = ButtonState.Normal;
                }
            }

        }
        public void Draw(Batch2D batch, DrawInfo drawInfo)
        {
            RenderServices.DrawSprite(batch, Images, Rectangle.TopLeft,  drawInfo, new AnimationInfo(State.ToString().ToLowerInvariant()));
        }

        public void UpdatePosition(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }
    }
}
