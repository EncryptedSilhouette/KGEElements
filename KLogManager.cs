using System.Text;

namespace Elements
{
    public class KLogManager
    {
        public const int DEBUG_LOG = 0;
        public const int ERROR_LOG = 1;

        private List<StringBuilder> _logs = new();

        public KLogManager()
        {
            _logs.Add(new StringBuilder());
            _logs.Add(new StringBuilder());
        }

        public int CreateLog()
        {
            _logs.Add(new StringBuilder());
            return _logs.Count - 1;
        }

#if RELEASE
        public void Log(int logID, string message) => _logs[logID].AppendLine(message);
#elif DEBUG

        public void Log(int logID, string message)
        {
            _logs[logID].AppendLine(message);
            Console.WriteLine($"Log ({logID}): {message}");
        }
#endif
        public void DebugLog(string message) => Log(DEBUG_LOG, message);

        public void ErrorLog(string message) => Log(ERROR_LOG, message);

        public string GetLog(int logID) => _logs[logID].ToString();

        public void ClearLog(int logID) => _logs[logID].Clear();
    }
}
