﻿using Bang.Components;
using Murder.Core;
using Murder.Utilities.Attributes;

namespace Murder.Components
{
    /// <summary>
    /// This is a struct that points to a singleton class.
    /// Reactive systems won't be able to subscribe to this component.
    /// </summary>
    [Unique]
    [RuntimeOnly]
    public readonly struct MapComponent : IModifiableComponent
    {
        public readonly Map Map;

        public readonly int Width => Map.Width;

        public readonly int Height => Map.Height;

        public MapComponent(int width, int height)
        {
            Map = new(width, height);
        }

        public void Subscribe(Action notification)
        { }

        public void Unsubscribe(Action notification)
        { }
    }
}