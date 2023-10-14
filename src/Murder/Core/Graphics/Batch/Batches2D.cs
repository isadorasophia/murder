﻿namespace Murder.Core.Graphics;

/// <summary>
/// A list of all available <see cref="Batch2D"/> in your <see cref="RenderContext"/>.
/// If extend this class it will automatically show the new constants on the "Target SpriteBatch" in the inspector.
/// Variables have "BachId" and "Batch" trimmed
/// </summary>
public class Batches2D
{
    // TODO: Use an attribute to se the name of the variable in the inspector instead of trimming
    public const int GameplayBatchId = 0;

    public const int FloorBatchId = 1;

    public const int LightBatchId = 2;

    public const int GameUiBatchId = 3;

    public const int ReflectionAreaBatchId = 4;

    public const int ReflectedBatchId = 5;

    public const int UiBatchId = 6;

    public const int DebugFxBatchId = 7;

    public const int DebugBatchId = 8;
}