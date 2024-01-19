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
/// Constraint to help sizing an entity. If you apply two <see cref="SnapToScreenConstraint"/> with opposite <see cref="SnapToScreenConstraint.SnapPosition"/>s
/// (e.g.: Top and bottom) this constraint will be ignored.
/// </summary>
/// <param name="Height">The height the entity should have.</param>
public readonly record struct HeightConstraint(int Height) : IConstraint
{
    /// <inheritdoc cref="IConstraint"/>
    public int Order => 1;

    /// <inheritdoc cref="IConstraint"/>
    public int Priority => 10;
}

/// <summary>
/// Constraint to help sizing an entity. If you apply two <see cref="SnapToScreenConstraint"/> with opposite <see cref="SnapToScreenConstraint.SnapPosition"/>s
/// (e.g.: Top and bottom) this constraint will be ignored.
/// </summary>
/// <param name="Width">The width the entity should have.</param>
public readonly record struct WidthConstraint(int Width) : IConstraint
{
    /// <inheritdoc cref="IConstraint"/>
    public int Order => 1;
        
    /// <inheritdoc cref="IConstraint"/>
    public int Priority => 10;
}

/// <summary>
/// Constraint to center an entity. This, by default, has lower priority than <see cref="SnapToScreenConstraint"/>.
/// Must be used along with <see cref="HeightConstraint"/> and/or <see cref="WidthConstraint"/> to size the element.
/// </summary>
/// <param name="Orientation">In which orientation we should center. Use <see cref="Orientation.Any"/> for absolute centering. </param>
public readonly record struct CenterInScreenConstraint(Orientation Orientation) : IConstraint
{
    /// <inheritdoc cref="IConstraint"/>
    public int Order => 2;
    
    /// <inheritdoc cref="IConstraint"/>
    public int Priority => 10;
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
                    case HeightConstraint heightConstraint:
                        // This is a pretty straightforward constraint: It sets the height if it hasn't been previously set by a more powerful constraint.
                        if (height.ConstraintPriority > 0 && height.ConstraintPriority > constraintPriority)
                        {
                            GameLogger.Warning($"Ignoring height constraint for entity with Id {entity.EntityId}");
                        }
                        else
                        {
                            height = new (heightConstraint.Height, constraintPriority);
                        }
                        break;
                    case WidthConstraint widthConstraint:
                        // This is a pretty straightforward constraint: It sets the width if it hasn't been previously set by a more powerful constraint.
                        if (width.ConstraintPriority > 0 && width.ConstraintPriority > constraintPriority)
                        {
                            GameLogger.Warning($"Ignoring width constraint for entity with Id {entity.EntityId}");
                        }
                        else
                        {
                            width = new (widthConstraint.Width, constraintPriority);
                        }
                        break;
                    case CenterInScreenConstraint centerInScreenConstraint:
                    {
                        // This will try centering the entity, and it can only do so if the width or height are known.
                        var tryHorizontalCentering = centerInScreenConstraint.Orientation != Orientation.Vertical;
                        var tryVerticalCentering = centerInScreenConstraint.Orientation != Orientation.Horizontal;
                        
                        if (tryHorizontalCentering)
                        {
                            // If x was previously set, we just ignore this constraint.
                            if (x.ConstraintPriority > 0 && x.ConstraintPriority > constraintPriority)
                            {
                                GameLogger.Warning($"Ignoring center constraint for the X axis of entity with id {entity.EntityId}");
                            }
                            // If there's no width calculated up to this point, there's nothing we can do.
                            else if (width.ConstraintPriority == 0)
                            {
                                GameLogger.Warning($"Ignoring center constraint for entity with id {entity.EntityId} because its width could not be measured. Did you also include a WidthConstraint?");
                            }
                            else
                            {
                                x = new((int)((screenDimensions.X - width.Value) / 2.0f), constraintPriority);
                            }
                        }
                        
                        if (tryVerticalCentering)
                        {
                            // If y was previously set, we just ignore this constraint.
                            if (y.ConstraintPriority > 0 && y.ConstraintPriority > constraintPriority)
                            {
                                GameLogger.Warning($"Ignoring center constraint for the Y axis of entity with id {entity.EntityId}");
                            }
                            // If there's no height calculated up to this point, there's nothing we can do.
                            else if (height.ConstraintPriority == 0)
                            {
                                GameLogger.Warning($"Ignoring center constraint for entity with id {entity.EntityId} because its height could not be measured. Did you also include a HeightConstraint?");
                            }
                            else
                            {
                                y = new((int)((screenDimensions.Y - height.Value) / 2.0f), constraintPriority);
                            }
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(constraint));
                }
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