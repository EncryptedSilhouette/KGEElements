namespace Elements.Systems
{
    public interface IKCommandHandler
    {
        
    }

    public class KCommandManager : IKCommandHandler
    {
        public static KCommand EXIT = new()
        {
            ID = "exit",
            Description = "Closes the application.",
            Args = [],
            CommandAction = (args) => KProgram.Running = false
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

        LinkedList<IKCommandHandler> CommandHandlers = new();
        Dictionary<string, KCommand> Commands = new();
        Queue<string> calls = new();

        public KCommandManager()
        {
            Commands.Add(EXIT.ID, EXIT);
            Commands.Add(PAUSE.ID, PAUSE);
        }

        public void Update()
        {
            while (calls.Count > 0)
            {
                var args = calls.Dequeue().Split(" ");
                Commands[args[0]].CommandAction.Invoke(args);
            }
        }
    }
}
