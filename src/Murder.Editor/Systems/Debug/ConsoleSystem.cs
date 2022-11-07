using Bang.Contexts;
using Bang.Systems;
using Murder.Diagnostics;
using Murder.Core.Graphics;
using Murder.Editor.Services.Console;

namespace Murder.Editor.Systems
{
    [DoNotPause]
    public class ConsoleSystem : IStartupSystem, IGuiSystem
    {
        private GameLogger _logger = null!;

        public ValueTask Start(Context context)
        {
            _logger = GameLogger.GetOrCreateInstance();

            return default;
        }

        public ValueTask DrawGui(RenderContext render, Context context)
        {
            // Outside of the game, also display the console.
            _logger.DrawConsole((string input) =>
            {
                return CommandServices.Parse(context.World, input);
            });

            return default;
        }
    }
}
