using Elements;
using Elements.Drawing;
using Elements.Extensions;
using Elements.Game;
using SFML.Graphics;
using SFML.Window;
using System.Diagnostics;

//TODO
//Resizing the window causes the visuals to warp.
//This is probably due to the RenderTexture streching and compressing to fit the new aspect ratio.
//Need to figure out a layout manager.
//interpolated rendering.

public class KProgram
{
    //Using doubles for floating point precision.
    const double MS_PER_SECOND = 1000.0d;

    private static bool s_vSync;
    private static uint s_frameLimit;
    private static string s_title;

    public static bool Running;
    public static uint UpdateTarget;
    public static double UpdateInterval;
    public static Random RNG;
    public static RenderWindow Window;
    public static KResourceManager ResourceManager;
    public static KInputManager InputManager;
    public static KCommandManager CommandManager;
    public static KRenderManager DrawManager;
    public static KGameManager GameManager;
    public static KLogManager LogManager;
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

    //TODO
    public static float ScreenTopLeft;
    public static float ScreenTopRight;
    public static float ScreenBottomRight;
    public static float ScreenBottomLeft;

    public static float ScreenCenterX => Window.Size.X / 2;

    public static float ScreenCenterY => Window.Size.Y / 2;

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

        s_vSync = false;
        s_title = string.Empty; //removes warning.
        Window = new(VideoMode.DesktopMode, s_title);
        Window.Closed += (_, _) => Running = false;

        //configs
        Title = "Elements";
        UpdateTarget = 30;
        FrameLimit = UpdateTarget;

        //Defaults
        Running = false;
        UpdateInterval = MS_PER_SECOND / UpdateTarget;

        //System managers
        ResourceManager = new("config.csv");
        InputManager = new();
        CommandManager = new();
        DrawManager = new(Window);
        GameManager = new(DrawManager, InputManager);
        LogManager = new();

        CLI = new(CommandManager);
    }

    public static void Main(string[] args)
    {
        InitAndLoad();
        OnStart?.Invoke();

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

        #region Draw layers

        DrawManager.Init(
        new View[]
        {
            Window.GetView()
        },
        new KDrawLayer[]
        {
            //Tilemap layer
            new KDrawLayer()
            {
                CameraID = 0,
                States = new(ResourceManager.TextureAtlases["atlas"].Texture),
                RenderTexture = Window.CreateRenderTexture(),
                Buffer = new(1028, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
            },
            //Entity layer
            new KDrawLayer()
            {
                CameraID = 0,
                States = new(ResourceManager.TextureAtlases["atlas"].Texture),
                RenderTexture = Window.CreateRenderTexture(),
                Buffer = new(1028, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
            },
            //Text layer
            new KDrawLayer()
            {
                CameraID = 0,
                States = new(ResourceManager.Fonts["roboto_black"].GetTexture(12)),
                RenderTexture = Window.CreateRenderTexture(),
                Buffer = new(1028, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
            }
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

        GameManager.Update(currentUpdate);
    }

    private static void FrameUpdate(in uint currentUpdate, in uint currentFrame, in double deltaTime)
    {
        GameManager.FrameUpdate(DrawManager, currentUpdate, currentFrame, deltaTime);
        DrawManager.FrameUpdate();
    }
}