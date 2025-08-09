using Elements.Core.Systems;
using System.Diagnostics;

public class KProgram
{
    //Using doubles for floating point precision.
    const double MS_PER_SECOND = 1000.0d;

    public static bool Running;
    public static double UpdateTarget;
    public static double UpdateInterval;
    public static KWindowManager WindowManager;
    public static KInputManager InputManager;

    public static event Action? OnStart;
    public static event Action? OnStop;

    static KProgram()
    {
        Running = false;
        UpdateTarget = 60;
        UpdateInterval = MS_PER_SECOND / UpdateTarget;
        WindowManager = new();
        InputManager = new();
    }

    public static void Main(string[] args)
    {
        _ = args; //gets rid of the warning

        ulong ups = 0;
        ulong fps = 0;
        ulong currentUpdate = 0;
        ulong currentFrame = 0;
        double unprocessedTime = 0;
        double newTime = 0;
        
        Init();
        Load();
        OnStart?.Invoke();

        Running = true;
        Stopwatch debugTimer = Stopwatch.StartNew();
        Stopwatch loopTimer = Stopwatch.StartNew();

        double lastTime = loopTimer.ElapsedTicks;

        while (Running)
        {
            //Keeps track of time between updates.
            newTime = loopTimer.ElapsedTicks;
            unprocessedTime += (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
            lastTime = newTime;

            //Debug tracker.
            if (debugTimer.ElapsedMilliseconds / MS_PER_SECOND >= 1)
            {
                debugTimer.Restart();
#if DEBUG
                Console.WriteLine($"ups: {ups}, fps: {fps}");
#endif
                ups = fps = 0;
            }

            //Maintains target FPS.
            if (unprocessedTime < UpdateInterval) continue;

            //Update and loop if lagging.
            do
            {
                ups++;
                currentUpdate++;
                unprocessedTime -= UpdateInterval;
                Update(currentUpdate);
            }
            while (unprocessedTime >= UpdateInterval);

            //Update and draw frame.
            fps++;
            currentFrame++;
            FrameUpdate(currentUpdate, currentFrame);
        }

        OnStop?.Invoke();
        Deinit();
    }

    private static void Init()
    {
        WindowManager.Window.Closed += (_, _) => Running = false;

        WindowManager.Window.KeyPressed += (_, e) => 
        {
            Console.WriteLine($"Press:{e.Code}");
        };

        WindowManager.Window.KeyPressed += (_, e) => 
        {
            Console.WriteLine($"Release:{e.Code}");
        };

        InputManager.Init(WindowManager);
    }

    private static void Deinit()
    {
        InputManager.Deinit(WindowManager);
    }

    private static void Load()
    {

    } 

    private static void Update(in ulong currentUpdate)
    {

        Console.WriteLine($"up:{currentUpdate}");
        InputManager.Update();
    }

    private static void FrameUpdate(in ulong currentUpdate, in ulong currentFrame)
    {
        InputManager.FrameUpdate();
        WindowManager.FrameUpdate();
    }
}