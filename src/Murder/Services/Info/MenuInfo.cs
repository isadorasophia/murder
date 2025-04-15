using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Services;
using Murder.Utilities;
using static Murder.Core.Input.PlayerInput;

namespace Murder.Core.Input
{
    public struct GenericMenuInfo<T>
    {
        public T[] Options = [];

        public int Scroll = 0;
        public float SmoothScroll = 0;

        public bool Canceled = false;
        public bool Disabled = false;
        public bool JustMoved = false;

        public float LastPressed = 0;
        public float LastMoved;

        public int OverflowX = 0;
        public int OverflowY = 0;
        public int PreviousSelection;

        /// <summary>
        /// Number of visible options on the screen, 8 is the default.
        /// </summary>
        public int VisibleItems = 8;

        public int Selection { get; private set; }

        public MenuSounds Sounds = new();

        /// <summary>
        /// Number of options in this menu
        /// </summary>
        public int Length => Options.Length;

        public GenericMenuInfo(T[] options)
        {
            Options = options;
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

                // TODO: Support disabled options.
                break;

                // totalOptionsTried++;
            }

            return option;
        }


        public void Select(int index, float now)
        {
            JustMoved = Selection != index;

            PreviousSelection = Selection;

            Selection = index;
            LastMoved = now;

            if (JustMoved)
            {
                _ = SoundServices.Play(Sounds.SelectionChange);
            }
        }
    }

    public struct MenuInfo
    {
        public int PreviousSelection;
        public int Selection { get; private set; }
        public float CreatedAt;
        public float LastMoved;
        public float LastPressed;
        public bool Canceled;
        public bool Disabled = false;
        public int OverflowX = 0;
        public int OverflowY = 0;
        public bool JustMoved = false;
        public int Scroll = 0;

        public float SmoothScroll = 0;

        /// <summary>
        /// Number of options in this menu
        /// </summary>
        public int Length => !HasOptions ? 0 : Options.Length;

        /// <summary>
        /// Number of visible options on the screen, 8 is the default.
        /// </summary>
        public int VisibleItems = 8;

        /// <summary>
        /// This is the allowed size in view, used to calculate size of elements in the screen.
        /// </summary>
        public int CapacityToShowUi = 8;

        public MenuSounds Sounds = new();

        public MenuOption[] Options = [];

        /// <summary>
        /// Optional icons to be displayed near the options.
        /// </summary>
        public Portrait[] Icons = [];

        public bool HasOptions => Options != null && Options.Length > 0;

        public float LargestOptionText
        {
            get
            {
                float largestTextWidth = 0;
                foreach (var item in Options)
                {
                    largestTextWidth = Math.Max(MurderFonts.PixelFont.GetLineWidth(item.Text), largestTextWidth);
                }
                return largestTextWidth;
            }
        }

        public MenuInfo()
        {
            CreatedAt = Game.NowUnscaled;
            LastMoved = Game.NowUnscaled;
        }

        public MenuInfo(params MenuOption[] options) : this()
        {
            Options = options;

            if (options.Length > 0 && !options[0].Enabled)
            {
                Selection = NextAvailableOption(0, 1);
            }
        }

        public MenuInfo(IEnumerable<MenuOption> options) : this(options.ToArray()) { }

        public MenuInfo(params string[] options) : this()
        {
            Options = options.Select(s => new MenuOption(s, true)).ToArray();
        }

        public MenuInfo(IEnumerable<string> options) : this()
        {
            Options = options.Select(s => new MenuOption(s, true)).ToArray();
        }

        public MenuInfo(int size) : this()
        {
            Resize(size);
        }

        public void Clamp()
        {
            Selection = Math.Max(0, Math.Min(Selection, Options.Length - 1));
        }

        public MenuInfo Disable(bool disabled)
        {
            Disabled = disabled;
            return this;
        }

        public void SnapRight(int width)
        {
            Selection = (Calculator.FloorToInt(Selection / width) + 1) * width - 1;
        }

        public void SnapLeft(int width)
        {
            Selection = (Calculator.FloorToInt(Selection / width)) * width;
        }

        public string GetSelectedOptionText()
        {
            return Options[Selection].Text;
        }
        public string GetOptionText(int index)
        {
            return Options[index % Options.Length].Text;
        }

        internal bool IsEnabled(int index)
        {
            return Options[index % Options.Length].Enabled;
        }

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

            return Options[option].Enabled;
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
                if (direction == 0)
                {
                    direction = 1;
                }
                option = Calculator.WrapAround(option, 0, Length - 1);

                if (IsOptionAvailable(option))
                {
                    break;
                }

                totalOptionsTried++;
            }

            return option;
        }

        public (int option, bool wrapped) NextAvailableOptionHorizontal(in int option, int width, int direction)
        {
            int startRow = Calculator.FloorToInt(option / (float)width);
            int startCollumn = option % width;

            int totalCollumns = width;
            int totalAttempts = 0;

            bool wrapped = false;
            while (totalAttempts < width)
            {
                int collumn = Calculator.WrapAround(startCollumn + direction * totalAttempts, 0, width - 1);
                // Did we wrap around?
                if (collumn > startCollumn && direction < 0)
                {
                    wrapped = true;
                }
                if (collumn < startCollumn && direction > 0)
                {
                    wrapped = true;
                }

                for (int i = 0; i < totalCollumns; i++)
                {
                    int checkRow = startRow - i;
                    if (checkRow >= 0)
                    {
                        int nextOption = checkRow * width + collumn;
                        if (nextOption > 0 && nextOption < Length)
                        {
                            if (IsOptionAvailable(nextOption))
                            {
                                return (nextOption, wrapped);
                            }
                        }
                    }

                    checkRow = startRow + i;
                    if (checkRow < totalCollumns)
                    {
                        int nextOption = checkRow * width + collumn;
                        if (nextOption > 0 && nextOption < Length)
                        {
                            if (IsOptionAvailable(nextOption))
                            {
                                return (nextOption, wrapped);
                            }
                        }
                    }
                }

                totalAttempts++;
            }

            return (option, false);
        }

        public (int option, bool wrapped) NextAvailableOptionVertical(in int option, int width, int direction)
        {
            // If we didn't find an option in the current column, closest to the current selection,
            // the first option available in the next row.
            int initialRow = Calculator.FloorToInt(option / width);
            int totalRows = Calculator.CeilToInt(Length / width);

            int collumn = option % width;

            int totalAttempts = 0;
            bool wrapped = false;
            while (totalAttempts < totalRows)
            {
                int row = Calculator.WrapAround(initialRow + direction * totalAttempts, 0, totalRows);
                // Did we wrap around?
                if (row > initialRow && direction < 0)
                {
                    wrapped = true;
                }
                if (row < initialRow && direction > 0)
                {
                    wrapped = true;
                }

                // First we try the first one imediatelly below or above the current selection.
                for (int i = 0; i < width; i++)
                {
                    int checkCollumn = collumn - i;
                    if (checkCollumn >= 0)
                    {
                        int nextOption = row * width + checkCollumn;
                        if (nextOption > 0 && nextOption < Length)
                        {
                            if (IsOptionAvailable(nextOption))
                            {
                                return (nextOption, wrapped);
                            }
                        }
                    }

                    checkCollumn = collumn + i;
                    if (checkCollumn < width)
                    {
                        int nextOption = row * width + checkCollumn;
                        if (nextOption > 0 && nextOption < Length)
                        {
                            if (IsOptionAvailable(nextOption))
                            {
                                return (nextOption, wrapped);
                            }
                        }
                    }
                }

                totalAttempts++;
            }

            return (option, false);
        }

        /// <summary>
        /// Resets the menu info selector to the first available option.
        /// </summary>
        public void Reset()
        {
            Selection = 0;
            LastMoved = Game.NowUnscaled;
            PreviousSelection = -1;
        }

        public void Resize(int size)
        {
            Options = new MenuOption[size];
            for (int i = 0; i < size; i++)
            {
                Options[i] = new MenuOption(true);
            }
        }

        public void Cancel()
        {
            // _ = SoundServices.Play(Sounds.Cancel);
        }

        public void Press(float now)
        {
            if (Options[Selection].SoundOnClick)
            {
                _ = SoundServices.Play(Sounds.MenuSubmit);
            }

            LastPressed = now;
        }

        public void Select(int index) => Select(index, Game.NowUnscaled, true);

        public void Select(int index, float now, bool updateScroll = true)
        {
            if (updateScroll)
            {
                if (index < Scroll)
                {
                    Scroll = index;
                }
                else if (index >= Scroll + VisibleItems)
                {
                    Scroll = index - VisibleItems + 1;
                }
            }

            JustMoved = Selection != index;

            PreviousSelection = Selection;

            Selection = index;
            LastMoved = now;
            LastPressed = now;


            if (Selection >= Options.Length)
            {
                Selection = Options.Length - 1;
            }
            if (Selection < 0)
            {
                Selection = 0;
            }

            if (JustMoved && (Selection >= Options.Length || Options[Selection].Enabled))
            {
                _ = SoundServices.Play(Sounds.SelectionChange);
            }
        }
    }
}