using Elements;
using Elements.Dev;
using Elements.Rendering;
using Elements.Game;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;

public static class KProgram
{
    //Using doubles for floating point precision.
    const double MS_PER_SECOND = 1000.0d;

    private static bool s_vSync = false;
    private static uint s_frameLimit;
    private static string s_title = string.Empty;

    public static bool Running = false;
    public static uint UpdateTarget;
    public static double UpdateInterval;
    public static Vector2u TargetResolution;
    public static Random RNG;
    public static RenderWindow Window;
    public static KResourceManager ResourceManager;
    public static KInputManager InputManager;
    public static KCommandManager CommandManager;
    public static KRenderManager RenderManager;
    public static KGameManager GameManager;
    public static KLogManager LogManager;
    public static KDebugger Debugger;
    public static KCLI CLI;

    public static event Action? OnInit;
    public static event Action? OnLoad;
    public static event Action? OnStart;
    public static event Action? OnStop;
    public static event Action? OnDeinit;

    public static bool VSync
    {
        get => s_vSync;
        set
        {
            Window.SetVerticalSyncEnabled(value);
            Window.SetFramerateLimit(value ? 0 : s_frameLimit);
            s_vSync = value;
        }
    }

    public static uint FrameLimit
    {
        get => s_frameLimit;
        set
        {
            s_frameLimit = value > UpdateTarget ? value : UpdateTarget;
            if (!s_vSync) Window.SetFramerateLimit(value);
        }
    }

    public static string Title
    {
        get => s_title;
        set
        {
            s_title = value;
            Window.SetTitle(value);
        }
    }

    static KProgram()
    {
        RNG = new();
        Window = new(VideoMode.DesktopMode, s_title);
        Window.Closed += (_, _) => Running = false;

        //configs
        Title = "Elements";
        UpdateTarget = 30;
        FrameLimit = UpdateTarget;
        TargetResolution = Window.Size;

        //Defaults
        UpdateInterval = MS_PER_SECOND / UpdateTarget;

        //System managers
        ResourceManager = new("config.csv");
        InputManager = new();
        CommandManager = new();

        RenderManager = new(Window);
        GameManager = new();
        LogManager = new();
        Debugger = new();
        CLI = new(CommandManager);
    }

    public static void Main(string[] args)
    {
        InitAndLoad();
        OnStart?.Invoke();

        LogManager.DebugLog("Working");

        StartGameLoop();

        OnStop?.Invoke();
        Deinit();
    }

     #region capped frame loop

#if false
    private static void StartGameLoop()
    {
        uint ups = 0;
        uint fps = 0;
        uint currentUpdate = 0;
        uint currentFrame = 0;
        double unprocessedTime = 0;
        double newTime = 0;
        double lastTime;

        Running = true;
        Stopwatch debugTimer = Stopwatch.StartNew();
        Stopwatch loopTimer = Stopwatch.StartNew();
        lastTime = loopTimer.ElapsedTicks;

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
                Console.Write($"\rups: {ups}, fps: {fps}");
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
    }
#endif

    #endregion

    #region uncapped frame loop

    private static void StartGameLoop()
    {
        uint ups = 0;
        uint fps = 0;
        uint currentUpdate = 0;
        uint currentFrame = 0;
        double delta = 0;
        double unprocessedTime = 0;
        double newTime = 0;
        double lastTime = 0;

        GC.Collect();

        Running = true;
        Stopwatch debugTimer = Stopwatch.StartNew();
        Stopwatch loopTimer = Stopwatch.StartNew();
        lastTime = loopTimer.ElapsedTicks;

        while (Running)
        {
            //Keeps track of time between updates.
            newTime = loopTimer.ElapsedTicks;
            delta = (newTime - lastTime) / TimeSpan.TicksPerMillisecond;
            unprocessedTime += delta;
            lastTime = newTime;

            //Debug tracker.
            if (debugTimer.ElapsedMilliseconds / MS_PER_SECOND >= 1)
            {
                debugTimer.Restart();
#if DEBUG
                Console.Write($"\rups: {ups}, fps: {fps}");
#endif
                ups = fps = 0;
            }
            
            while (unprocessedTime >= UpdateInterval && Running)
            {
                ups++;
                currentUpdate++;
                unprocessedTime -= UpdateInterval;
                Update(currentUpdate);
            }

            //Update and draw frame.
            fps++;
            currentFrame++;
            FrameUpdate(currentUpdate, currentFrame, delta / UpdateInterval);
        }
    }

    #endregion

    private static void InitAndLoad()
    {
        InputManager.Init(Window);

        //Load
        Load();

        #region Rendering initilization

        RenderManager.Init(
            new View[]
            {
                Window.DefaultView,
            },
            new RenderLayer[]
            {
                new RenderLayer() //Default layer.
                {
                    Camera = 0,
                    LineColor = Color.Yellow,
                    BackgroundColor = Color.Black,
                    //States = new(ResourceManager.TextureAtlases["atlas"].Texture),
                    States = RenderStates.Default,
                    RenderTexture = new(TargetResolution.X, TargetResolution.Y),
                    Buffer = new(256, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
                },
            },
            new KTextRenderer[]
            {
                new(ResourceManager.Fonts["roboto_black"], RenderManager)
            });

        #endregion

        OnInit?.Invoke();
    }

    private static void Deinit()
    {
        InputManager.Deinit(Window);
        OnDeinit?.Invoke();
    }

    private static void Load()
    {
        ResourceManager.Load();
        OnLoad?.Invoke();
    } 

    private static void Update(in uint currentUpdate)
    {
        InputManager.Update();      //Must update first or else inputs will be cleared before processed.
        Window.DispatchEvents();    //Processes new input events here. Do NOT change the order of these two.

        CLI.Update(InputManager);
        CommandManager.Update();

        Debugger.Update();
        //GameManager.Update(currentUpdate);
    }

    private static void FrameUpdate(in uint currentUpdate, in uint currentFrame, in double deltaTime)
    {
        GameManager.FrameUpdate(RenderManager);
        CLI.FrameUpdate(RenderManager);
        RenderManager.FrameUpdate();
        Debugger.FrameUpdate();
    }
}