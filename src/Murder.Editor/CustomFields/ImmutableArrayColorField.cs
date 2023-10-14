﻿using Murder.Core.Graphics;
using Murder.Editor.ImGuiExtended;
using Murder.Editor.Reflection;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Murder.Editor.CustomFields
{
    [CustomFieldOf(typeof(ImmutableArray<Color>))]
    internal class ImmutableArrayColorField : ImmutableArrayField<Color>
    {
        protected override bool Add(in EditorMember member, [NotNullWhen(true)] out Color element)
        {
            element = default!;

            if (ImGuiHelpers.IconButton('', $"{member.Name}_add", Game.Data.GameProfile.Theme.Accent))
            {
                element = Color.White;
                return true;
            }

            return false;
        }

        protected override bool DrawElement(ref Color element, EditorMember member, int _)
        {
            bool modified = false;
            System.Numerics.Vector4 vector4Color = element.ToSysVector4();

            if (AttributeExtensions.TryGetAttribute<PaletteColorAttribute>(member, out var paletteColor))
            {
                CoreColorField.DrawPalettePicker(member, element, ref modified, ref vector4Color);
            }
            else
            {
                (modified, vector4Color) =
                     Vector4Field.ProcessInputImpl(member, new(element.R, element.G, element.B, element.A));
            }

            if (modified)
            {
                element = new Color(vector4Color.X, vector4Color.Y, vector4Color.Z, vector4Color.W);
            }

            return modified;
        }
    }
}