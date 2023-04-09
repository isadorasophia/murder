namespace Murder.Core.Dialogs
{
    public readonly struct DialogActionId
    {
        public readonly int SituationId = 0;
        public readonly int DialogId = 0;
        public readonly int ActionId = 0;

        public DialogActionId(int situation, int dialog, int action)
        {
            SituationId = situation;
            DialogId = dialog;
            ActionId = action;
        }
    }
}
