namespace Elements
{
    public struct KCommandArgs
    {
        public bool IsOptional;
        public string ID;
        public string Description;
        public string[] Options;
    }

    public struct KCommand
    {
        public string ID;
        public string Description;
        public KCommandArgs[] Args;
        public Action<string[]> CommandAction;
    }

    public class KCommandManager
    {
        public static KCommand EXIT = new()
        {
            ID = "exit",
            Description = "Closes the application.",
            Args = [],
            CommandAction = (args) => KProgram.Running = false
        };

        public static KCommand PRINT_LOG = new()
        {
            ID = "print_log",
            Description = "Prints the log identified by ID.",
            Args = [],
            CommandAction = (args) => 
            {
                var text = KProgram.LogManager.GetLog(int.Parse(args[1]));
            }
        };

        public static KCommand PAUSE = new()
        {
            ID = "gamestate",
            Description = "Sets the gamestate.",
            Args =
            [
                new()
                {
                    ID = "state",
                    Description = "Updated game state"
                }
            ],
            CommandAction = (args) =>
            {
                switch (args[1])
                {
                    case "pause":
                        break;
                    default:
                        break;
                }
            }
        };

        //LinkedList<IKCommandHandler> CommandHandlers = new();
        private Dictionary<string, KCommand> _commands = new();
        private Queue<string> _calls = new();

        public KCommandManager()
        {
            _commands.Add(EXIT.ID, EXIT);
            _commands.Add(PAUSE.ID, PAUSE);
        }

        public void Update()
        {
            while (_calls.Count > 0)
            {
                var args = _calls.Dequeue().Split(" ");
                _commands[args[0]].CommandAction.Invoke(args);
            }
        }

        public void SubmitCommandString(string command) => _calls.Enqueue(command);
    }
}
