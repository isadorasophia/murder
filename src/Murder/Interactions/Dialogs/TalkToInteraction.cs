using Bang.Entities;
using Bang.StateMachines;
using Bang;
using Murder.Assets;
using Murder.Attributes;
using Murder.Core.Dialogs;
using Bang.Interactions;

namespace Murder.Interactions
{
    public readonly struct TalkToInteraction : Interaction
    {
        [GameAssetId(typeof(CharacterAsset)), ShowInEditor]
        public readonly Guid Character = Guid.Empty;

        /// <summary>
        /// This is the starter situation for the interaction.
        /// </summary>
        public readonly int Situation = 0;

        [GameAssetId(typeof(PrefabAsset)), ShowInEditor]
        private readonly Guid _dialogPrefab = Guid.Empty;

        public TalkToInteraction() { }

        public void Interact(World world, Entity interactor, Entity interacted)
        {
            if (Game.Data.TryGetAsset<PrefabAsset>(_dialogPrefab) is PrefabAsset prefab)
            {
                Entity dialogPrefab = prefab.CreateAndFetch(world);

                // TODO: Generate extensions.
                // dialogPrefab.SetSituation(Character, Situation);
                // dialogPrefab.SetStateMachine(new StateMachineComponent<DialogStateMachine>());
            }
        }
    }
}
