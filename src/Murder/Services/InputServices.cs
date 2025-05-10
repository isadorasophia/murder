using Murder.Assets.Graphics;
using Murder.Core;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Input;
using Murder.Services.Info;
using Murder.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Murder.Services;

public static class InputServices
{
    public static MenuInfo CreateBindingsMenuInfo(string? exitText)
    {
        var builder = ImmutableArray.CreateBuilder<MenuOption>();

        foreach (var id in Game.Input.AllButtons)
        {
            var virtualButton = Game.Input.GetOrCreateButton(id);

            string text = $"{id}: ";

            foreach (var button in virtualButton.Buttons)
            {
                text += button.ToString();
            }

            builder.Add(new MenuOption(text));
        }


        if (exitText != null)
        {
            builder.Add(new MenuOption(exitText));
        }


        return new MenuInfo(builder.DrainToImmutable());
    }

    public static DrawMenuInfo DrawBindingsMenu(Batch2D batch,
        in Point position,
        in DrawMenuStyle style,
        in MenuInfo menuInfo,
        in int maxItemsPerLine,
        int totalMenuWidth,
        float sort = .1f)
    {
        PixelFont font = Game.Data.GetFont(style.Font);
        int lineHeight = font.LineHeight + style.ExtraVerticalSpace;

        int maxSelectionWidth = 0;

        Point finalPosition = new(Math.Max(position.X, 0), Math.Max(position.Y, 0));
        Point textFinalPosition = new(Math.Max(position.X, 0), Math.Max(position.Y, 0));

        int collumns = Calculator.CeilToInt(menuInfo.Length / (float)maxItemsPerLine);
        int itemMaxWidth = (int)(totalMenuWidth / collumns);

        Vector2 CalculateText(int line, int column) => new Point(
            Calculator.RoundToInt(column * itemMaxWidth - (totalMenuWidth / 2f)),
            Calculator.FloorToInt(lineHeight * (line + 1.25f))) + textFinalPosition;

        Vector2 CalculateSelector(int line, int column) => new Point(
            (column + 0.5f) * itemMaxWidth - (totalMenuWidth / 2f) - 8,
            lineHeight * (line + 1.5f)) + finalPosition;

        for (int i = 0; i < menuInfo.Length; i++)
        {
            int column = Calculator.FloorToInt(i / (float)maxItemsPerLine);
            int line = i % maxItemsPerLine;

            var label = menuInfo.GetOptionText(i);
            Vector2 labelPosition = CalculateText(line, column);

            Color currentColor;
            Color? currentShadow;

            if (menuInfo.IsEnabled(i))
            {
                currentColor = i == menuInfo.Selection ? style.SelectedColor : style.Color;
                currentShadow = style.Shadow;
            }
            else
            {
                currentColor = style.Shadow;
                currentShadow = null;
            }

            Point textSize = RenderServices.DrawText(batch, style.Font, label ?? string.Empty, labelPosition, itemMaxWidth, new DrawInfo(sort)
            {
                Origin = new Vector2(0,0),
                Color = currentColor,
                Shadow = currentShadow,
                Debug = false
            });

            if (textSize.X > maxSelectionWidth)
            {
                maxSelectionWidth = textSize.X;
            }

            // We did not implement vertical icon menu with other offsets.
            if (i < menuInfo.Icons.Length && style.Origin.X == 0)
            {
                float bounceX = i != menuInfo.Selection ? 0 :
                    Ease.BackOut(Calculator.ClampTime(Game.NowUnscaled - menuInfo.LastMoved, 0.5f)) * 3 - 3;

                Portrait portrait = menuInfo.Icons[i];
                if (MurderAssetHelpers.GetSpriteAssetForPortrait(portrait) is (SpriteAsset sprite, string animation))
                {
                    RenderServices.DrawSprite(
                        batch,
                        sprite,
                        labelPosition - new Point(15 - bounceX, 0),
                        new DrawInfo(sort: sort),
                        new AnimationInfo(animation));
                }
            }
        }

        // Get the line and collumn of the selector
        int selectorColumn = Calculator.FloorToInt(menuInfo.Selection / (float)maxItemsPerLine);
        int selectorLine = menuInfo.Selection % maxItemsPerLine;

        Vector2 selectorPosition = CalculateSelector(selectorLine, selectorColumn) + new Vector2(0, MathF.Floor(lineHeight / 2f) - 3);

        selectorColumn = Calculator.FloorToInt(menuInfo.PreviousSelection / (float)maxItemsPerLine);
        selectorLine = menuInfo.PreviousSelection % maxItemsPerLine;
        Vector2 previousSelectorPosition = CalculateSelector(selectorLine, selectorColumn) + new Vector2(0, lineHeight / 2f - 2);

        Vector2 easedPosition;
        if (style.SelectorMoveTime == 0)
            easedPosition = selectorPosition;
        else
            easedPosition = Vector2.Lerp(previousSelectorPosition, selectorPosition,
            Ease.Evaluate(Calculator.ClampTime(Game.NowUnscaled - menuInfo.LastMoved, style.SelectorMoveTime), style.Ease));

         return new DrawMenuInfo()
        {
            SelectorPosition = selectorPosition,
            PreviousSelectorPosition = previousSelectorPosition,
            SelectorEasedPosition = easedPosition.Point(),
            MaximumSelectionWidth = Math.Min(Calculator.RoundToInt(totalMenuWidth/(collumns + 0.5f)), maxSelectionWidth)
        };
    }
}
