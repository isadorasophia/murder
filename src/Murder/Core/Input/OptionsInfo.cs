using Murder.Services;
using Murder.Utilities;

namespace Murder.Core.Input
{
    public readonly struct OptionsInfo
    {
        public MenuOption[]? Options { get; } = null;

        public int Length { get; init; } = 0;

        public OptionsInfo(MenuOption[] options) => (Options, Length) = (options, options.Length);

        public bool IsOptionAvailable(int option)
        {
            if (Options is null)
            {
                return true;
            }

            if (Options.Length <= option)
            {
                return false;
            }

            return Options[option].Selectable;
        }

        /// <param name="option">The currently selected option. If -1, it means that is being initialized.</param>
        /// <param name="direction">A sign number (1 or -1) with the direction.</param>
        /// <returns>The next option that is available.</returns>
        public int NextAvailableOption(int option, int direction)
        {
            int totalOptionsTried = 0;
            while (totalOptionsTried < Length)
            {
                option += direction;
                option = Calculator.WrapAround(option, 0, Length - 1);

                if (IsOptionAvailable(option))
                {
                    break;
                }

                totalOptionsTried++;
            }

            return option;
        }
    }
}