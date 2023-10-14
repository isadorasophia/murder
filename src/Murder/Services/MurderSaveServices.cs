using Bang;
using Bang.Entities;
using Murder.Assets;
using Murder.Core.Dialogs;
using Murder.Save;

namespace Murder.Services
{
    public static class MurderSaveServices
    {
        public static SaveData CreateOrGetSave()
        {
            if (Game.Data.TryGetActiveSaveData() is not SaveData save)
            {
                // Right now, we are creating a new save if one is already not here.
                save = Game.Data.CreateSave("_default");
            }

            return save;
        }

        public static SaveData? TryGetSave() => Game.Data.TryGetActiveSaveData();

        public static void DoAction(BlackboardTracker tracker, DialogAction action)
        {
            Fact fact = action.Fact;

            switch (fact.Kind)
            {
                case FactKind.Bool:
                    tracker.SetBool(fact.Blackboard, fact.Name, action.Kind, action.BoolValue!.Value);
                    break;

                case FactKind.Int:
                    tracker.SetInt(fact.Blackboard, fact.Name, action.Kind, action.IntValue!.Value);
                    break;

                case FactKind.String:
                    tracker.SetString(fact.Blackboard, fact.Name, action.StrValue!);
                    break;
            }
        }

        public static void RecordAndMaybeDestroy(this Entity entity, World world, bool destroy)
        {
            CreateOrGetSave().RecordRemovedEntityFromWorld(world, entity);

            if (destroy)
            {
                entity.Destroy();
            }
        }

        public static bool CanLoadSave() => Game.Data.CanLoadSaveData();

        public static Guid? LoadSaveAndFetchTargetWorld()
        {
            Game.Data.LoadSaveAsCurrentSave();

            if (TryGetSave() is not SaveData save)
            {
                return null;
            }

            return save.CurrentWorld;
        }
    }
}