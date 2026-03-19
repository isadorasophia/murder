using Bang.Components;

namespace Murder.Messages;

/// <summary>
/// Generic struct for notifying that an entity that is about to be replaced.
/// </summary>
public readonly struct OnBeforeReplaceMessage : IMessage { }

/// <summary>
/// Generic struct for notifying that an entity was replaced.
/// </summary>
public readonly struct OnReplacedMessage : IMessage { }