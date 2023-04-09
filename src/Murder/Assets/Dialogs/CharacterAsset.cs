using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace Murder.Assets
{
    public class CharacterAsset : GameAsset
    {
        public override char Icon => '\uf075';
        public override string EditorFolder => "#\uf518Story\\#\uf075Characters";

        [GameAssetId(typeof(SpeakerAsset))]
        public readonly Guid Owner = Guid.Empty;

        /// <summary>
        /// List of tasks or events that the <see cref="Situations"/> may do.
        /// </summary>
        [JsonProperty]
        private SortedList<int, Situation> _situations = new();

        /// <summary>
        /// List of all the components that are modified within the dialog.
        /// </summary>
        [JsonProperty]
        private readonly Dictionary<DialogActionId, IComponent> _components = new();

        private ImmutableArray<Situation>? _cachedSituations;

        public ImmutableArray<Situation> Situations
        {
            get
            {
                _cachedSituations ??= _situations.Values.ToImmutableArray();
                return _cachedSituations.Value;
            }
        }

        /// <summary>
        /// Set the situation to a list. 
        /// This is called when updating the scripts with the latest data.
        /// </summary>
        public void SetSituations(SortedList<int, Situation> situations)
        {
            _situations = situations;
        }

        public void RemoveCustomComponents(IEnumerable<DialogActionId> actionIds)
        {
            foreach (DialogActionId id in actionIds)
            {
                _components.Remove(id);
            }
        }

        public ImmutableDictionary<DialogActionId, IComponent> Components => _components.ToImmutableDictionary();
    }
}
