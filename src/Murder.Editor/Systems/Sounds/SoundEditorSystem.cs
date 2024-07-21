using Bang;
using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using ImGuiNET;
using Murder.Components;
using Murder.Core.Graphics;
using Murder.Editor.Attributes;
using Murder.Editor.Components;
using Murder.Editor.Utilities;
using Murder.Services;
using Murder.Utilities;
using Murder.Utilities.Attributes;
using System.Collections.Immutable;
using System.Numerics;

namespace Murder.Editor.Systems.Sounds
{
    [SoundEditor]
    [PrefabEditor]
    [Filter(ContextAccessorFilter.AnyOf, typeof(SoundComponent), typeof(MusicComponent), typeof(SoundParameterComponent))]
    internal class SoundEditorSystem : GenericSelectorSystem, IStartupSystem, IUpdateSystem, IMurderRenderSystem, IGuiSystem
    {
        private Type[]? _filter = null;
        private float _lastMove;
        private Vector2 _previousCursorPosition;

        private ImmutableArray<Entity> FetchEntities(World world) => _filter is null || _filter.Length == 0 ?
            ImmutableArray<Entity>.Empty : world.GetEntitiesWith(ContextAccessorFilter.AnyOf, _filter);

        public void Start(Context context)
        {
            _filter = ReflectionHelper.FetchComponentsWithAttribute<SoundPlayerAttribute>();
        }

        public void Update(Context context)
        {
            ImmutableArray<Entity> allEntities = context.Entities.AddRange(FetchEntities(context.World));
            Update(context.World, allEntities, clearOnlyWhenSelectedNewEntity: false);
        }

        public void Draw(RenderContext render, Context context)
        {
            World world = context.World;

            ImmutableArray<Entity> entities = context.Entities;
            DrawImpl(render, world, entities);

            EditorHook hook = world.GetUnique<EditorComponent>().EditorHook;
            foreach (Entity e in entities)
            {
                if (!e.HasTransform()) continue;

                bool isSelected = hook.IsEntitySelected(e.EntityId);

                Guid asset = Game.Profile.EditorAssets.SoundImage;

                Vector2 position = e.GetGlobalTransform().Vector2;
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    position = collider.GetBoundingBox(position).CenterPoint;
                }

                if (e.HasMusic() || e.HasSoundParameter())
                {
                    asset = Game.Profile.EditorAssets.MusicImage;
                }

                RenderSprite(render, asset, position, isSelected);
            }

            HashSet<int> entitiesDrawn = new();

            // Now, draw all sound player entities.
            ImmutableArray<Entity> soundPlayerEntities = FetchEntities(world);
            foreach (Entity e in soundPlayerEntities)
            {
                if (!e.HasTransform()) continue;

                // Make sure we are only drawing once, or things will get weird.
                if (entitiesDrawn.Contains(e.Parent ?? -1)) continue;

                bool isAlreadyRendered = false;
                foreach (int child in e.Children)
                {
                    if (entitiesDrawn.Contains(child))
                    {
                        isAlreadyRendered = true;
                        break;
                    }
                }

                if (isAlreadyRendered) continue;

                bool isSelected = hook.IsEntitySelected(e.EntityId) || hook.IsEntitySelected(e.Parent ?? -1);

                Guid asset = Game.Profile.EditorAssets.SoundImage;

                Vector2 position = e.GetGlobalTransform().Vector2;
                if (e.TryGetCollider() is ColliderComponent collider)
                {
                    position = collider.GetBoundingBox(position).CenterPoint;
                }

                if (!isSelected)
                {
                    RenderSprite(render, asset, position, false);
                }

                entitiesDrawn.Add(e.EntityId);
            }
        }

        private void RenderSprite(RenderContext render, Guid asset, Vector2 position, bool isHighlighted)
        {
            RenderServices.DrawSprite(
                render.GameUiBatch,
                asset,
                position,
                new DrawInfo() { Origin = Vector2Helper.Center, Sort = 0, Outline = isHighlighted ? Color.White : null },
                AnimationInfo.Default);
        }

        public void DrawGui(RenderContext render, Context context)
        {
            var hook = context.World.GetUnique<EditorComponent>().EditorHook;
            ShowAllPossibleSelections(hook, ref _lastMove, ref _previousCursorPosition);
        }
    }
}