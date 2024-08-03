using Bang;
using Bang.Components;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Murder.Serialization;
using System.Collections.Immutable;

namespace Murder.Assets
{
    public readonly struct LineInfo
    {
        public readonly Guid Speaker { get; init; } = Guid.Empty;
        public readonly string? Portrait { get; init; } = null;

        public string? Event { get; init; } = null;

        public LineInfo() { }
    }

    public class CharacterAsset : GameAsset
    {
        public override char Icon => '\uf075';
        public override string EditorFolder => "#\uf518Story\\#\uf075Characters";

        [GameAssetId(typeof(SpeakerAsset), allowInheritance: true)]
        public readonly Guid Owner = Guid.Empty;

        /// <summary>
        /// Portrait which will be shown by default from <see cref="Owner"/>.
        /// </summary>
        public readonly string? Portrait = null;

        /// <summary>
        /// Notes regarding this script. This will give context when localizing it.
        /// </summary>
        public readonly string? LocalizationNotes = null;

        /// <summary>
        /// List of tasks or events that the <see cref="Situations"/> may do.
        /// </summary>
        [Serialize]
        private SortedList<int, Situation> _situations = new();

        /// <summary>
        /// List of all the components that are modified within a dialog.
        /// </summary>
        [Serialize]
        private readonly ComplexDictionary<DialogItemId, IComponent> _components = new();

        /// <summary>
        /// List of all the portraits that are modified within a dialog.
        /// </summary>
        [Serialize]
        private readonly ComplexDictionary<DialogItemId, (Guid Speaker, string? Portrait)> _portraits = new();

        /// <summary>
        /// List of all the lines that have custom information within a dialogue.
        /// </summary>
        [Serialize]
        private readonly ComplexDictionary<DialogItemId, LineInfo> _info = [];

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
            FileChanged = true;

            _cachedSituations = null;
        }

        /// <summary>
        /// Set the situation on <paramref name="index"/> to <paramref name="situation"/>.
        /// </summary>
        public void SetSituationAt(int index, Situation situation)
        {
            _situations[index] = situation;
            FileChanged = true;

            _cachedSituations = null;
        }

        public Situation? TryFetchSituation(int id)
        {
            if (_situations.TryGetValue(id, out Situation value))
            {
                return value;
            }

            return null;
        }

        public void SetCustomComponentAt(DialogItemId id, IComponent c)
        {
            _components[id] = c;
            FileChanged = true;
        }

        public void SetCustomPortraitAt(DialogItemId id, Guid speaker, string? portrait)
        {
            _portraits[id] = (speaker, portrait);
            FileChanged = true;
        }

        public void SetEventInfoAt(DialogItemId id, string? @event)
        {
            _info[id] = new() { Event = @event };
            FileChanged = true;
        }

        public void RemoveCustomComponents(IEnumerable<DialogItemId> actionIds)
        {
            foreach (DialogItemId id in actionIds)
            {
                _components.Remove(id);
            }

            FileChanged = true;
        }

        public void RemoveCustomPortraits(IEnumerable<DialogItemId> actionIds)
        {
            foreach (DialogItemId id in actionIds)
            {
                _portraits.Remove(id);
            }

            FileChanged = true;
        }

        public ImmutableDictionary<DialogItemId, IComponent> Components => _components.ToImmutableDictionary();

        public ImmutableDictionary<DialogItemId, (Guid Speaker, string? Portrait)> Portraits => _portraits.ToImmutableDictionary();

        public ImmutableDictionary<DialogItemId, LineInfo> Info => _info.ToImmutableDictionary();
    }
}