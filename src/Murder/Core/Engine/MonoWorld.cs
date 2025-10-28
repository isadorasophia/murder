using Bang;
using Bang.Entities;
using Bang.StateMachines;
using Bang.Systems;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Core.Sounds;
using Murder.Diagnostics;
using Murder.Services;
using Murder.StateMachines;
using System;
using System.Reflection;

namespace Murder.Core;

/// <summary>
/// World implementation based in MonoGame.
/// </summary>
public partial class MonoWorld : World
{
    public readonly Camera2D Camera;

    public readonly Guid WorldAssetGuid;

    public MonoWorld(
        IList<(ISystem system, bool isActive)> systems,
        Camera2D camera,
        Guid worldAssetGuid) : base(systems) => (Camera, WorldAssetGuid) = (camera, worldAssetGuid);

    public override void Pause()
    {
        base.Pause();
        Game.Instance.Pause();

        SoundServices.Pause(SoundLayer.Sfx);
    }

    public override void Resume()
    {
        base.Resume();
        Game.Instance.Resume();

        SoundServices.Resume(SoundLayer.Sfx);
    }

    public void PreDraw()
    {
        // TODO: Do not make a copy every frame.
        foreach (var (systemId, (system, contextId)) in _cachedRenderSystems)
        {
            if (system is IMonoPreRenderSystem preRenderSystem)
            {
                if (DIAGNOSTICS_MODE)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                }

                preRenderSystem.BeforeDraw(Contexts[contextId]);

                if (DIAGNOSTICS_MODE)
                {
                    InitializeDiagnosticsCounters();

                    _stopwatch.Stop();
                    PreRenderCounters[systemId].Update(_stopwatch.Elapsed.TotalMicroseconds, Contexts[contextId].Entities.Length);
                }
            }
        }
    }

    public void Draw(RenderContext render)
    {
        // TODO: Do not make a copy every frame.
        foreach (var (systemId, (system, contextId)) in _cachedRenderSystems)
        {
            if (system is IMurderRenderSystem monoSystem)
            {
                if (DIAGNOSTICS_MODE)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                }

                monoSystem.Draw(render, Contexts[contextId]);

                if (DIAGNOSTICS_MODE)
                {
                    InitializeDiagnosticsCounters();

                    _stopwatch.Stop();
                    RenderCounters[systemId].Update(_stopwatch.Elapsed.TotalMicroseconds, Contexts[contextId].Entities.Length);
                }
            }
        }
    }

    public void DrawGui(RenderContext render)
    {
        // TODO: Do not make a copy every frame.
        foreach (var (systemId, (system, contextId)) in _cachedRenderSystems)
        {
            if (system is IGuiSystem monoSystem)
            {
                if (DIAGNOSTICS_MODE)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                }

                monoSystem.DrawGui(render, Contexts[contextId]);

                if (DIAGNOSTICS_MODE)
                {
                    InitializeDiagnosticsCounters();

                    _stopwatch.Stop();
                    GuiCounters[systemId].Update(_stopwatch.Elapsed.TotalMicroseconds, Contexts[contextId].Entities.Length);
                }
            }
        }
    }

    /// <summary>
    /// Absolutely do not persist this. This is just our pool of entities.
    /// </summary>
    private readonly List<int> _effectEntities = [];
    private int _nextFreeEffectEntityIndex = 0;

    public Coroutine RunCoroutine(IEnumerator<Wait> routine, CoroutineFlags flags = CoroutineFlags.None)
    {
        Entity entity = FetchNextFreeEffectEntity(out Coroutine coroutine);

        entity.SetStateMachine(
             new StateMachineComponent<CoroutineStateMachine>(new CoroutineStateMachine(routine)));

        if (flags.HasFlag(CoroutineFlags.DoNotPause))
        {
            entity.SetDoNotPause();
        }
        else
        {
            entity.RemoveDoNotPause();
        }

        // Immediately run the first tick!
        entity.GetStateMachine().Tick(Game.DeltaTime);
        return coroutine;
    }

    internal bool StopCoroutine(Coroutine coroutine)
    {
        int index = coroutine.Index;

        if (index >= _effectEntities.Count)
        {
            GameLogger.Warning($"How are we stopping a coroutine of {index} on a pool of {_effectEntities.Count}?");
            return false;
        }

        int entityId = _effectEntities[index];
        if (TryGetEntity(entityId) is not Entity pooledEntity)
        {
            return false;
        }

        pooledEntity.RemoveStateMachine();
        if (index < _nextFreeEffectEntityIndex)
        {
            _nextFreeEffectEntityIndex = index;
        }

        return true;
    }

    private Entity FetchNextFreeEffectEntity(out Coroutine coroutine)
    {
        int start = _nextFreeEffectEntityIndex;

        while (_nextFreeEffectEntityIndex < _effectEntities.Count)
        {
            int index = _nextFreeEffectEntityIndex;
            _nextFreeEffectEntityIndex = (_nextFreeEffectEntityIndex + 1) % _effectEntities.Count;

            coroutine = new(index);

            int entityId = _effectEntities[index];
            if (TryGetEntity(entityId) is not Entity pooledEntity)
            {
                pooledEntity = AddEntity();

                _effectEntities[index] = pooledEntity.EntityId;
                return pooledEntity;
            }

            if (!pooledEntity.HasStateMachine())
            {
                return pooledEntity;
            }

            // otherwise, the pooled entity is still busy, apparently. let's keep going, unless
            // we already circled all available entities, in which case we want to stop.

            if (_nextFreeEffectEntityIndex == start)
            {
                break;
            }
        }

        Entity e = AddEntity();
        _effectEntities.Add(e.EntityId);
        coroutine = new(_nextFreeEffectEntityIndex);

        _nextFreeEffectEntityIndex = _effectEntities.Count;

        return e;
    }
}