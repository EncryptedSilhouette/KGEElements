namespace Elements.Systems
{
    public struct KCommandArgs
    {
        public bool IsOptional;
        public string ID;
        public string Description;
        public string[] Options;
    }

    //Example
    //exit -> close everthing
    public class KCommand
    {
        public string ID;
        public string Description;
        public KCommandArgs[] Args;
        public Action<string[]> CommandAction;
    }
}
