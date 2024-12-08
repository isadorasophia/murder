namespace Murder.Core.Graphics;

[Flags]
public enum ImageFlip
{
    None = 0,
    Horizontal = 1,
    Vertical = 1 << 1,
    Both = Horizontal | Vertical
}


public static class ImageFlipServices
{
    public static ImageFlip FlipHorizontal(this ImageFlip flip)
    {
        if (flip == ImageFlip.Horizontal)
        {
            return ImageFlip.None;
        }
        else if (flip == ImageFlip.Vertical)
        {
            return ImageFlip.Both;
        }
        else if (flip == ImageFlip.Both)
        {
            return ImageFlip.Vertical;
        }
        else
        {
            return ImageFlip.Horizontal;
        }
    }

    public static ImageFlip FlipVertical(this ImageFlip flip)
    {
        if (flip == ImageFlip.Vertical)
        {
            return ImageFlip.None;
        }
        else if (flip == ImageFlip.Horizontal)
        {
            return ImageFlip.Both;
        }
        else if (flip == ImageFlip.Both)
        {
            return ImageFlip.Horizontal;
        }
        else
        {
            return ImageFlip.Vertical;
        }
    }
}