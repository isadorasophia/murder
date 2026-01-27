using Murder.Core.Graphics;
using Murder.Data;

namespace Murder.Core;

public interface IPreloadGame : IDisposable
{
    /// <summary>
    /// Called when a scene is unavailable due to loading of assets.
    /// Only assets at <see cref="GameDataManager.PreloadContent"/> are available.
    /// </summary>
    public void Update();

    /// <summary>
    /// Called when a scene is unavailable due to loading of assets.
    /// Only assets at <see cref="GameDataManager.PreloadContent"/> are available.
    /// </summary>
    /// <param name="context">Borrows the RenderContext from the world (currently busy loading).</param>
    public void Draw(RenderContext context);

    /// <summary>
    /// Try to wrap it up whatever it is doing.
    /// </summary>
    public bool WrapItUp();
}
