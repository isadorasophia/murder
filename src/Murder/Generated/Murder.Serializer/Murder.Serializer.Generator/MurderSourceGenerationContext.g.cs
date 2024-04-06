using System.Text.Json.Serialization;

namespace Murder.Serialization;

/// <summary>
/// Serialization context for all the types within Murder. You may find here all the:
///  - Components
///  - State machines
///  - Interactions
///  - Game assets
///  
/// And any private fields that these types have.
/// </summary>
[JsonSerializable(typeof(Murder.Components.PositionComponent))] // start with something to test!
[JsonSerializable(typeof(Murder.Components.MoveToComponent))] // start with something to test!
public partial class MurderSourceGenerationContext : JsonSerializerContext, IMurderSerializer
{
}
