# BloomFilter

**Namespace:** Murder.Core.Graphics \
**Assembly:** Murder.dll

```csharp
public class BloomFilter : IDisposable
```

https://github.com/Kosmonaut3d/BloomFilter-for-Monogame-and-XNA
            Version 1.1, 16. Dez. 2016
            
            Bloom / Blur, 2016 TheKosmonaut
            
            High-Quality Bloom filter for high-performance applications
            Based largely on the implementations in Unreal Engine 4 and Call of Duty AW
            For more information look for
            "Next Generation Post Processing in Call of Duty Advanced Warfare" by Jorge Jimenez
            http://www.iryoku.com/downloads/Next-Generation-Post-Processing-in-Call-of-Duty-Advanced-Warfare-v18.pptx
            
            The idea is to have several rendertargets or one rendertarget with several mip maps
            so each mip has half resolution (1/2 width and 1/2 height) of the previous one.
            
            32, 16, 8, 4, 2
            
            In the first step we extract the bright spots from the original image. If not specified otherwise thsi happens in full resolution.
            We can do that based on the average RGB value or Luminance and check whether this value is higher than our Threshold.
                BloomUseLuminance = true / false (default is true)
                BloomThreshold = 0.8f;
            
            Then we downscale this extraction layer to the next mip map.
            While doing that we sample several pixels around the origin.
            We continue to downsample a few more times, defined in
                BloomDownsamplePasses = 5 ( default is 5)
            
            Afterwards we upsample again, but blur in this step, too.
            The final output should be a blur with a very large kernel and smooth gradient.
            
            The output in the draw is only the blurred extracted texture. 
            It can be drawn on top of / merged with the original image with an additive operation for example.
            
            If you use ToneMapping you should apply Bloom before that step.

**Implements:** _[IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public BloomFilter(GraphicsDevice graphicsDevice, Effect bloomEffect, int width, int height, SurfaceFormat renderTargetFormat, QuadRenderer quadRenderer)
```

Initialize and load all needed components for the BloomEffect.

**Parameters** \
`graphicsDevice` [GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
\
`bloomEffect` [Effect](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Effect.html) \
\
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`renderTargetFormat` [SurfaceFormat](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.SurfaceFormat.html) \
\
`quadRenderer` [QuadRenderer](../../../Murder/Core/Graphics/QuadRenderer.html) \
\

### ⭐ Properties
#### BloomDownsamplePasses
```csharp
public int BloomDownsamplePasses;
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### BloomPreset
```csharp
public BloomPresets BloomPreset { get; public set; }
```

**Returns** \
[BloomPresets](../../../Murder/Core/Graphics/BloomPresets.html) \
#### BloomStreakLength
```csharp
public float BloomStreakLength { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### BloomStrengthMultiplier
```csharp
public float BloomStrengthMultiplier;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### BloomThreshold
```csharp
public float BloomThreshold { get; public set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### BloomUseLuminance
```csharp
public bool BloomUseLuminance;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
### ⭐ Methods
#### Draw(Texture2D, int, int)
```csharp
public Texture2D Draw(Texture2D inputTexture, int width, int height)
```

Main draw function

**Parameters** \
`inputTexture` [Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
\
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\

**Returns** \
[Texture2D](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.Texture2D.html) \
\

#### Dispose()
```csharp
public virtual void Dispose()
```

Dispose our RenderTargets. This is not covered by the Garbage Collector so we have to do it manually

#### UpdateResolution(int, int)
```csharp
public void UpdateResolution(int width, int height)
```

Update the InverseResolution of the used rendertargets. This should be the InverseResolution of the processed image
            We use SurfaceFormat.Color, but you can use higher precision buffers obviously.

**Parameters** \
`width` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\
`height` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
\



⚡