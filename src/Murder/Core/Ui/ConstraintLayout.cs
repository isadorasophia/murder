using System.Collections.Immutable;
using Bang;
using Bang.Entities;
using Bang.Systems;
using Murder.Attributes;
using Murder.Core.Geometry;
using Murder.Core.Graphics;
using Murder.Core.Physics;
using Murder.Diagnostics;
using System.ComponentModel;
using Point = Murder.Core.Geometry.Point;
using Rectangle = Murder.Core.Geometry.Rectangle;
using IComponent = Bang.Components.IComponent;

namespace Murder.Core.Ui;

/// <summary>
/// Describes constraints on how an entity must be positioned on the screen.
/// </summary>
public interface IConstraint
{
    // Adding these annotations because, for now, we don't want users to override these values. 
    // If they choose to do so, it's at their own peril.
    /// <summary>
    /// Order in which this constraint should be applied (lower gets applied first). 
    /// </summary>
    [HideInEditor, EditorBrowsable(EditorBrowsableState.Never)]
    int Order { get; }

    /// <summary>
    /// Priority this constraint will have when laying elements down (higher overrides lower).
    /// </summary>
    [HideInEditor, EditorBrowsable(EditorBrowsableState.Never)]
    int Priority { get; }
}

/// <summary>
/// Holds the constraints necessary to render an entity on the right place.
/// </summary>
public readonly struct ConstraintsComponent() : IComponent
{
    /// <summary>
    /// List of constraints applicable to the entity possessing this component.
    /// </summary>
    public readonly ImmutableArray<IConstraint> Constraints = ImmutableArray<IConstraint>.Empty;
}

/// <summary>
/// Updates the entity's position and Box collider based on its layout constraints.
/// </summary>
[Watch(typeof(ConstraintsComponent))]
public sealed class LayoutConstraintSystem : IReactiveSystem
{
    
    /// <inheritdoc cref="IReactiveSystem"/>
    public void OnAdded(World world, ImmutableArray<Entity> entities)
    {
        Camera2D camera = ((MonoWorld)world).Camera;
        LayoutEntities(camera.Size, entities);
    }

    /// <inheritdoc cref="IReactiveSystem"/>
    public void OnRemoved(World world, ImmutableArray<Entity> entities)
    {
    }

    /// <inheritdoc cref="IReactiveSystem"/>
    public void OnModified(World world, ImmutableArray<Entity> entities)
    {
        Camera2D camera = ((MonoWorld)world).Camera;
        LayoutEntities(camera.Size, entities);
    }

    private readonly record struct ConstraintSetValue(int Value, int ConstraintPriority);

    private static void LayoutEntities(Point screenDimensions, ImmutableArray<Entity> entities)
    {
        foreach (var entity in entities)
        {
            var constraints = entity
                .GetConstraints()
                .Constraints;

            if (constraints.IsEmpty)
            {
                continue;
            }

            ConstraintSetValue x = new(), y = new(), width = new(), height = new();

            foreach (var constraint in constraints.OrderBy(c => c.Order))
            {
                var constraintPriority = constraint.Priority;
                switch (constraint)
                {
            }

            LogWarningForUnsetProperties(entity, x, y, width, height);
            
            var position = new Point(x.Value, y.Value);
            entity.SetPosition(position);

            var bounds = new Rectangle(0, 0, width.Value, height.Value);
            var shape = new BoxShape(bounds);
            var existingCollider = entity.TryGetCollider();
            entity.SetCollider(
                shape,
                existingCollider?.Layer ?? CollisionLayersBase.TRIGGER, 
                existingCollider?.DebugColor ?? Color.Magenta
            );
        }
    }

    private static void LogWarningForUnsetProperties(
        Entity entity,
        ConstraintSetValue x,
        ConstraintSetValue y,
        ConstraintSetValue width,
        ConstraintSetValue height
    )
    {
        if (height.Value == 0)
        {
            GameLogger.Warning($"Constraints failed to calculate the height of entity with id {entity.EntityId}. Setting it to 0.");
        }
        if (width.Value == 0)
        {
            GameLogger.Warning($"Constraints failed to calculate the width of entity with id {entity.EntityId}. Setting it to 0.");
        }
        if (x.ConstraintPriority == 0)
        {
            GameLogger.Warning($"Constraints failed to calculate the x position of entity with id {entity.EntityId}. Setting it to 0.");
        }
        if (y.ConstraintPriority == 0)
        {
            GameLogger.Warning($"Constraints failed to calculate the y position of entity with id {entity.EntityId}. Setting it to 0.");
        }
    }
}