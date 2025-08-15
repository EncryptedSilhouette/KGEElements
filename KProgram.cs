using Elements.Core.Systems;
using System.Diagnostics;

public class KProgram
{
    //Using doubles for floating point precision.
    const double MS_PER_SECOND = 1000.0d;

    public static bool Running;
    public static double UpdateTarget;
    public static double UpdateInterval;
    public static KResourceManager ResourceManager;
    public static KWindowManager WindowManager;
    public static KInputManager InputManager;

    public static event Action? OnStart;
    public static event Action? OnStop;

    static KProgram()
    {
        Running = false;
        UpdateTarget = 30;
        UpdateInterval = MS_PER_SECOND / UpdateTarget;
        ResourceManager = new();
        WindowManager = new();
        InputManager = new();
    }

    public static void Main(string[] args)
    {
        _ = args; //gets rid of the warning

        uint ups = 0;
        uint fps = 0;
        uint currentUpdate = 0;
        uint currentFrame = 0;
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
                //Console.WriteLine($"ups: {ups}, fps: {fps}");
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
            while (unprocessedTime >= UpdateInterval && Running);

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
        Console.WriteLine("init");

        InputManager.Init(WindowManager);
    }

    private static void Deinit()
    {
        InputManager.Deinit(WindowManager);
    }

    private static void Load()
    {
        ResourceManager.Load();
    } 

    private static void Update(in uint currentUpdate)
    {
        InputManager.Update();  //Must update first, else inputs will be cleared immediately.
        WindowManager.Update(); //Need to call dispatch events in case of game lagging.
    }

    private static void FrameUpdate(in uint currentUpdate, in uint currentFrame)
    {
        WindowManager.FrameUpdate();
    }
}