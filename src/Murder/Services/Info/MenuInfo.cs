using Murder.Core.Sounds;
using Murder.Services;
using Murder.Utilities;

namespace Murder.Core.Input
{
    public struct GenericMenuInfo<T>
    {
        public T[] Options = new T[0];

        public int Scroll = 0;

        public bool Canceled = false;
        public bool Disabled = false;
        public bool JustMoved = false;

        public float LastPressed = 0;
        public float LastMoved;

        public int Overflow = 0;
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

        public void Select(int index) => Select(index, Game.NowUnscaled);

        public void Select(int index, float now)
        {
            if (index < Scroll)
            {
                Scroll = index;
            }
            else if (index >= Scroll + VisibleItems)
            {
                Scroll = index - VisibleItems + 1;
            }

            JustMoved = Selection != index;

            PreviousSelection = Selection;

            Selection = index;
            LastMoved = now;
            LastPressed = now;

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
        public float LastMoved;
        public float LastPressed;
        public bool Canceled;
        public bool Disabled = false;
        public int Overflow = 0;
        public bool JustMoved = false;
        public int Scroll = 0;

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

        public void Clamp(int max)
        {
            Selection = Math.Max(0, Math.Min(Selection, max));
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
                option = Calculator.WrapAround(option, 0, Length - 1);

                if (IsOptionAvailable(option))
                {
                    break;
                }

                totalOptionsTried++;
            }

            return option;
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

        public void Select(int index) => Select(index, Game.NowUnscaled);

        public void Select(int index, float now)
        {
            if (index < Scroll)
            {
                Scroll = index;
            }
            else if (index >= Scroll + VisibleItems)
            {
                Scroll = index - VisibleItems + 1;
            }

            JustMoved = Selection != index;

            PreviousSelection = Selection;

            Selection = index;
            LastMoved = now;
            LastPressed = now;

            if (JustMoved)
            {
                _ = SoundServices.Play(Sounds.SelectionChange);
            }
        }
    }
}