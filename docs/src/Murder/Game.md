# Game

**Namespace:** Murder \
**Assembly:** Murder.dll

```csharp
public class Game : Game, IDisposable
```

**Implements:** _[Game](https://docs.monogame.net/api/Microsoft.Xna.Framework.Game.html), [IDisposable](https://learn.microsoft.com/en-us/dotnet/api/System.IDisposable?view=net-7.0)_

### ⭐ Constructors
```csharp
public Game(IMurderGame game, GameDataManager dataManager)
```

Creates a new game, there should only be one game instance ever.
            If <paramref name="dataManager" /> is not initialized, it will create the starting scene from [GameProfile](..//Murder/Assets/GameProfile.html).

**Parameters** \
`game` [IMurderGame](../..//Murder/IMurderGame.html) \
`dataManager` [GameDataManager](../..//Murder/Data/GameDataManager.html) \

```csharp
public Game(IMurderGame game)
```

**Parameters** \
`game` [IMurderGame](../..//Murder/IMurderGame.html) \

### ⭐ Properties
#### _gameData
```csharp
protected readonly GameDataManager _gameData;
```

**Returns** \
[GameDataManager](../..//Murder/Data/GameDataManager.html) \
#### _graphics
```csharp
protected readonly GraphicsDeviceManager _graphics;
```

**Returns** \
[GraphicsDeviceManager](https://docs.monogame.net/api/Microsoft.Xna.Framework.GraphicsDeviceManager.html) \
#### _logger
```csharp
protected GameLogger _logger;
```

Single logger of the game.

**Returns** \
[GameLogger](../..//Murder/Diagnostics/GameLogger.html) \
#### _pendingExit
```csharp
protected bool _pendingExit;
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### _pendingWorld
```csharp
protected MonoWorld _pendingWorld;
```

**Returns** \
[MonoWorld](../..//Murder/Core/MonoWorld.html) \
#### _pendingWorldTransition
```csharp
protected T? _pendingWorldTransition;
```

**Returns** \
[T?](https://learn.microsoft.com/en-us/dotnet/api/System.Nullable-1?view=net-7.0) \
#### _playerInput
```csharp
protected readonly PlayerInput _playerInput;
```

**Returns** \
[PlayerInput](../..//Murder/Core/Input/PlayerInput.html) \
#### _sceneLoader
```csharp
protected SceneLoader _sceneLoader;
```

Initialized in [Game.LoadContent](../murder/game.html#loadcontent).

**Returns** \
[SceneLoader](../..//Murder/Core/SceneLoader.html) \
#### ActiveScene
```csharp
public Scene ActiveScene { get; }
```

**Returns** \
[Scene](../..//Murder/Core/Scene.html) \
#### AlwaysUpdateBeforeFixed
```csharp
protected virtual bool AlwaysUpdateBeforeFixed { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Components
```csharp
public GameComponentCollection Components { get; }
```

**Returns** \
[GameComponentCollection](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameComponentCollection.html) \
#### Content
```csharp
public ContentManager Content { get; public set; }
```

**Returns** \
[ContentManager](https://docs.monogame.net/api/Microsoft.Xna.Framework.Content.ContentManager.html) \
#### Data
```csharp
public static GameDataManager Data { get; }
```

**Returns** \
[GameDataManager](../..//Murder/Data/GameDataManager.html) \
#### DeltaTime
```csharp
public static float DeltaTime { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Downsample
```csharp
public float Downsample;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### ElapsedDeltaTime
```csharp
public static float ElapsedDeltaTime { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### FixedDeltaTime
```csharp
public static float FixedDeltaTime { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Fullscreen
```csharp
public bool Fullscreen { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### GameScale
```csharp
public Vector2 GameScale { get; }
```

**Returns** \
[Vector2](../..//Murder/Core/Geometry/Vector2.html) \
#### GraphicsDevice
```csharp
public GraphicsDevice GraphicsDevice { get; }
```

**Returns** \
[GraphicsDevice](https://docs.monogame.net/api/Microsoft.Xna.Framework.Graphics.GraphicsDevice.html) \
#### HasCursor
```csharp
protected virtual bool HasCursor { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### Height
```csharp
public static int Height { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### InactiveSleepTime
```csharp
public TimeSpan InactiveSleepTime { get; public set; }
```

**Returns** \
[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/System.TimeSpan?view=net-7.0) \
#### InitialScene
```csharp
protected virtual Scene InitialScene { get; }
```

**Returns** \
[Scene](../..//Murder/Core/Scene.html) \
#### Input
```csharp
public static PlayerInput Input { get; }
```

**Returns** \
[PlayerInput](../..//Murder/Core/Input/PlayerInput.html) \
#### Instance
```csharp
public static Game Instance { get; private set; }
```

Singleton instance of the game. Be cautious when referencing this...

**Returns** \
[Game](../..//Murder/Game.html) \
#### IsActive
```csharp
public bool IsActive { get; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsFixedTimeStep
```csharp
public bool IsFixedTimeStep { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsMouseVisible
```csharp
public bool IsMouseVisible { get; public set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsPaused
```csharp
public bool IsPaused { get; private set; }
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### IsSkippingDeltaTimeOnUpdate
```csharp
public bool IsSkippingDeltaTimeOnUpdate { get; }
```

Whether the player is currently skipping frames (due to cutscene) and ignore
            the time while calling update methods.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### LaunchParameters
```csharp
public LaunchParameters LaunchParameters { get; }
```

**Returns** \
[LaunchParameters](https://docs.monogame.net/api/Microsoft.Xna.Framework.LaunchParameters.html) \
#### LONGEST_TIME_RESET
```csharp
public static const float LONGEST_TIME_RESET;
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### LongestRenderTime
```csharp
public float LongestRenderTime { get; private set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### LongestUpdateTime
```csharp
public float LongestUpdateTime { get; private set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### MaxElapsedTime
```csharp
public TimeSpan MaxElapsedTime { get; public set; }
```

**Returns** \
[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/System.TimeSpan?view=net-7.0) \
#### Now
```csharp
public static float Now { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### NowUnscaled
```csharp
public static float NowUnscaled { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Preferences
```csharp
public static GamePreferences Preferences { get; }
```

**Returns** \
[GamePreferences](../..//Murder/Save/GamePreferences.html) \
#### PreviousElapsedTime
```csharp
public float PreviousElapsedTime { get; }
```

Elapsed time in seconds from the previous update frame since the game started

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### PreviousNow
```csharp
public static float PreviousNow { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### PreviousNowUnscaled
```csharp
public static float PreviousNowUnscaled { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Profile
```csharp
public static GameProfile Profile { get; }
```

**Returns** \
[GameProfile](../..//Murder/Assets/GameProfile.html) \
#### Random
```csharp
public static Random Random;
```

**Returns** \
[Random](https://learn.microsoft.com/en-us/dotnet/api/System.Random?view=net-7.0) \
#### RenderTime
```csharp
public float RenderTime { get; private set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Save
```csharp
public static SaveData Save { get; }
```

**Returns** \
[SaveData](../..//Murder/Assets/SaveData.html) \
#### Services
```csharp
public GameServiceContainer Services { get; }
```

**Returns** \
[GameServiceContainer](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameServiceContainer.html) \
#### Sound
```csharp
public static ISoundPlayer Sound { get; }
```

**Returns** \
[ISoundPlayer](../..//Murder/Core/Sounds/ISoundPlayer.html) \
#### SoundPlayer
```csharp
public readonly ISoundPlayer SoundPlayer;
```

**Returns** \
[ISoundPlayer](../..//Murder/Core/Sounds/ISoundPlayer.html) \
#### StartedSkippingCutscene
```csharp
public bool StartedSkippingCutscene;
```

Whether the player started skipping.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \
#### TargetElapsedTime
```csharp
public TimeSpan TargetElapsedTime { get; public set; }
```

**Returns** \
[TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/System.TimeSpan?view=net-7.0) \
#### UnscaledDeltaTime
```csharp
public static float UnscaledDeltaTime { get; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### UpdateTime
```csharp
public float UpdateTime { get; private set; }
```

**Returns** \
[float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \
#### Width
```csharp
public static int Width { get; }
```

**Returns** \
[int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \
#### Window
```csharp
public GameWindow Window { get; }
```

**Returns** \
[GameWindow](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameWindow.html) \
### ⭐ Events
#### Activated
```csharp
public event EventHandler<TEventArgs> Activated;
```

**Returns** \
[EventHandler\<TEventArgs\>](https://learn.microsoft.com/en-us/dotnet/api/System.EventHandler-1?view=net-7.0) \
#### Deactivated
```csharp
public event EventHandler<TEventArgs> Deactivated;
```

**Returns** \
[EventHandler\<TEventArgs\>](https://learn.microsoft.com/en-us/dotnet/api/System.EventHandler-1?view=net-7.0) \
#### Disposed
```csharp
public event EventHandler<TEventArgs> Disposed;
```

**Returns** \
[EventHandler\<TEventArgs\>](https://learn.microsoft.com/en-us/dotnet/api/System.EventHandler-1?view=net-7.0) \
#### Exiting
```csharp
public event EventHandler<TEventArgs> Exiting;
```

**Returns** \
[EventHandler\<TEventArgs\>](https://learn.microsoft.com/en-us/dotnet/api/System.EventHandler-1?view=net-7.0) \
### ⭐ Methods
#### BeginDraw()
```csharp
protected virtual bool BeginDraw()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### LoadSceneAsync(bool)
```csharp
protected virtual Task LoadSceneAsync(bool waitForAllContent)
```

**Parameters** \
`waitForAllContent` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

**Returns** \
[Task](https://learn.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.Task?view=net-7.0) \

#### ApplyGameSettingsImpl()
```csharp
protected virtual void ApplyGameSettingsImpl()
```

#### BeginRun()
```csharp
protected virtual void BeginRun()
```

#### Dispose(bool)
```csharp
protected virtual void Dispose(bool isDisposing)
```

**Parameters** \
`isDisposing` [bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### Draw(GameTime)
```csharp
protected virtual void Draw(GameTime gameTime)
```

**Parameters** \
`gameTime` [GameTime](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameTime.html) \

#### DrawImGui(GameTime)
```csharp
protected virtual void DrawImGui(GameTime gameTime)
```

**Parameters** \
`gameTime` [GameTime](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameTime.html) \

#### EndDraw()
```csharp
protected virtual void EndDraw()
```

#### EndRun()
```csharp
protected virtual void EndRun()
```

#### ExitGame()
```csharp
protected virtual void ExitGame()
```

Exit the game. This is used to wrap any custom behavior depending on the game implementation.

#### Finalize()
```csharp
protected virtual void Finalize()
```

#### Initialize()
```csharp
protected virtual void Initialize()
```

#### LoadContent()
```csharp
protected virtual void LoadContent()
```

#### LoadContentImpl()
```csharp
protected virtual void LoadContentImpl()
```

#### OnActivated(Object, EventArgs)
```csharp
protected virtual void OnActivated(Object sender, EventArgs args)
```

**Parameters** \
`sender` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \
`args` [EventArgs](https://learn.microsoft.com/en-us/dotnet/api/System.EventArgs?view=net-7.0) \

#### OnDeactivated(Object, EventArgs)
```csharp
protected virtual void OnDeactivated(Object sender, EventArgs args)
```

**Parameters** \
`sender` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \
`args` [EventArgs](https://learn.microsoft.com/en-us/dotnet/api/System.EventArgs?view=net-7.0) \

#### OnExiting(Object, EventArgs)
```csharp
protected virtual void OnExiting(Object sender, EventArgs args)
```

**Parameters** \
`sender` [Object](https://learn.microsoft.com/en-us/dotnet/api/System.Object?view=net-7.0) \
`args` [EventArgs](https://learn.microsoft.com/en-us/dotnet/api/System.EventArgs?view=net-7.0) \

#### SetWindowSize(Point)
```csharp
protected virtual void SetWindowSize(Point screenSize)
```

**Parameters** \
`screenSize` [Point](../..//Murder/Core/Geometry/Point.html) \

#### UnloadContent()
```csharp
protected virtual void UnloadContent()
```

#### Update(GameTime)
```csharp
protected virtual void Update(GameTime gameTime)
```

**Parameters** \
`gameTime` [GameTime](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameTime.html) \

#### ApplyGameSettings()
```csharp
protected void ApplyGameSettings()
```

This will apply the game settings according to [Murder.Game._gameData](). /&gt;.

#### DoPendingExitGame()
```csharp
protected void DoPendingExitGame()
```

#### DoPendingWorldTransition()
```csharp
protected void DoPendingWorldTransition()
```

#### UpdateImpl(GameTime)
```csharp
protected void UpdateImpl(GameTime gameTime)
```

**Parameters** \
`gameTime` [GameTime](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameTime.html) \

#### CanResumeAfterSaveComplete()
```csharp
public bool CanResumeAfterSaveComplete()
```

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### QueueReplaceWorldOnCurrentScene(MonoWorld)
```csharp
public bool QueueReplaceWorldOnCurrentScene(MonoWorld world)
```

This is called when replacing the world for a current scene.
            Happened when transition from two different scenes (already loaded) as a world.

**Parameters** \
`world` [MonoWorld](../..//Murder/Core/MonoWorld.html) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### QueueWorldTransition(Guid)
```csharp
public bool QueueWorldTransition(Guid world)
```

**Parameters** \
`world` [Guid](https://learn.microsoft.com/en-us/dotnet/api/System.Guid?view=net-7.0) \

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### ResumeDeltaTimeOnUpdate()
```csharp
public bool ResumeDeltaTimeOnUpdate()
```

Resume game to normal game time.

**Returns** \
[bool](https://learn.microsoft.com/en-us/dotnet/api/System.Boolean?view=net-7.0) \

#### BeginImGuiTheme()
```csharp
public virtual void BeginImGuiTheme()
```

#### Dispose()
```csharp
public virtual void Dispose()
```

#### EndImGuiTheme()
```csharp
public virtual void EndImGuiTheme()
```

#### RefreshWindow()
```csharp
public virtual void RefreshWindow()
```

#### Exit()
```csharp
public void Exit()
```

#### FreezeFrames(int)
```csharp
public void FreezeFrames(int amount)
```

This will pause the game for <paramref name="amount" /> of frames.

**Parameters** \
`amount` [int](https://learn.microsoft.com/en-us/dotnet/api/System.Int32?view=net-7.0) \

#### Pause()
```csharp
public void Pause()
```

This will pause the game.

#### QueueExitGame()
```csharp
public void QueueExitGame()
```

This queues such that the game exit at the end of the update.
            We wait until the end of the update to avoid any access to a world that has been disposed on cleanup.

#### ResetElapsedTime()
```csharp
public void ResetElapsedTime()
```

#### Resume()
```csharp
public void Resume()
```

This will resume the game.

#### RevertSlowDown()
```csharp
public void RevertSlowDown()
```

#### Run()
```csharp
public void Run()
```

#### Run(GameRunBehavior)
```csharp
public void Run(GameRunBehavior runBehavior)
```

**Parameters** \
`runBehavior` [GameRunBehavior](https://docs.monogame.net/api/Microsoft.Xna.Framework.GameRunBehavior.html) \

#### RunOneFrame()
```csharp
public void RunOneFrame()
```

#### SetWaitForSaveComplete()
```csharp
public void SetWaitForSaveComplete()
```

#### SkipDeltaTimeOnUpdate()
```csharp
public void SkipDeltaTimeOnUpdate()
```

This will skip update times and immediately run the update calls from the game 
            until [Game.ResumeDeltaTimeOnUpdate](../murder/game.html#resumedeltatimeonupdate) is called.

#### SlowDown(float)
```csharp
public void SlowDown(float scale)
```

This will slow down the game time.
            TODO: What if we have multiple slow downs in the same run?

**Parameters** \
`scale` [float](https://learn.microsoft.com/en-us/dotnet/api/System.Single?view=net-7.0) \

#### SuppressDraw()
```csharp
public void SuppressDraw()
```

#### Tick()
```csharp
public void Tick()
```



⚡