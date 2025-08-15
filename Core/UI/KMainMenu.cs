namespace Elements.Core.UI
{
    public struct KMainMenu
    {
        KDrawData drawData;

        KButton[] buttons;

        public void Init()
        {
            buttons = new KButton[3]
            {
                new(),
                new(),
                new()
            };
        }
        public void Exit()
        {

        }
    }
}
