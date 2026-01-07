#region Preamble Dev 0.0.1
//Apologies for the lack of comments, that will be improved.
//This project exists for a number or purposes.
//  1.) This is a hobby project, but also an item to add to my Portfolio as a developer.
//  2.) This is a goal I set for myself; To finish a project and make a game using all that I have learned throughout my years as a developer.
//  3.) This is a art showcase I wish for others to see.
//      I want people to be able to observe a fully made game, in the hopes that it may spark their interest.

//A personal philosophy of mine is art is anything you can put your soul into; blood, sweat, and tears, but most importantly passion.
//All works contain traces of their artist, and programming is no different. 
//The way people write code is different for everyone.
//One of the reason I like programming as more of a hobby is I can explore so many ways of programming and find my own way.

//Now I subscribed to a few programming philosophies in the creation of this project. 
//  1.) Simplify. 
//  2.) Prioritize using the stack; Reduce, cache, and recycle objects.
//  3.) Use classes for data management, use structs for data.
//  4.) Use arrays and array pools over other collections.
//  5.) AVOID null at all costs. Exception for delegates; use a null check (D?.Invoke()).
//  6.) Provide as much data upfront.
//  7.) Optimize when and where needed. 
//  8.) Avoid inheritance.
//  9.) Have a common area to access other parts of the program.
//      This is the purpose of the static class KProgram; to act as a mediator between systems.
// 10.) Otherwise avoid static objects and singletons except for default/predetermined/hardcoded objects.

//Additonal documentation will be added in the future.
#endregion

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
    //Other values are initialized in the static constructor for this class.
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
    public static KResourceManager ResourceManager; //Needs finilization.
    public static KInputManager InputManager;       //Needs a rework.
    public static KCommandManager CommandManager;   //Unfinished.
    public static KRenderManager RenderManager;     //Finished. 
    public static KGameManager GameManager;         //WIP
    public static KLogManager LogManager;           //Unfinished.
    public static KDebugger Debugger;               //Unfinished.
    public static KCLI CLI;                         //Unfinished.

    //Events for extension. 
    public static event Action? OnInit;             
    public static event Action? OnLoad;
    public static event Action? OnStart;
    public static event Action? OnStop;
    public static event Action? OnDeinit;

    //Getters and Setters
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

    static KProgram() //Static constructor; initializes static members.
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

        //Managers
        ResourceManager = new("config.csv");
        InputManager = new();
        CommandManager = new();
        RenderManager = new(Window, ResourceManager);
        GameManager = new(ResourceManager, RenderManager, InputManager);
        LogManager = new();
        Debugger = new();
        CLI = new(CommandManager, ResourceManager);
    }

    public static void Main(string[] args) //Program entry point.
    {
        InitAndLoad();
        OnStart?.Invoke();

        StartGameLoop();

        OnStop?.Invoke();
        Deinit();
    }

    #region Game-loop

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

        GC.Collect(); //Runs the garbage collector before starting the loop.

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

            //Loops when lagging behind on updates.
            //These updates are not rendered. 
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

        ResourceManager.Load();
        OnLoad?.Invoke();

        #region Rendering initilization

        var windowViews = new View[]
        {
            Window.DefaultView,
        };

        var drawLayers = new KDrawLayer[2];

        //Default render layer.
        drawLayers[0] = new KDrawLayer(new VertexBuffer(16384, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic));
        drawLayers[0].States.Texture = ResourceManager.TextureAtlases["atlas"].Texture; 

        drawLayers[1] = KRenderManager.CreateTextLayer(ResourceManager.Fonts[0]); //Default text layer.

        RenderManager.Init(ResourceManager.Fonts[0], windowViews, drawLayers);
        GameManager.Init();

        #endregion

        OnInit?.Invoke();
    }

    private static void Deinit()
    {
        InputManager.Deinit(Window);
        OnDeinit?.Invoke();
    }

    private static void Update(in uint currentUpdate)
    {
        InputManager.Update();      //Must update first or else inputs will be cleared before processed.
        Window.DispatchEvents();    //Processes new input events here. Do NOT change the order of these two.

        CLI.Update(InputManager);
        CommandManager.Update();

        Debugger.Update();
        GameManager.Update(currentUpdate);
    }

    private static void FrameUpdate(in uint currentUpdate, in uint currentFrame, in double deltaTime)
    {
        GameManager.FrameUpdate(RenderManager);
        CLI.FrameUpdate(RenderManager);
        RenderManager.FrameUpdate();
        Debugger.FrameUpdate();
    }

    //Here for now. May move later.
    public static int GetIndex(int column, int row, int width) => column + row * width;
    public static void GetIndex(int column, int row, int width, out int index) => index = column + row * width;

    public static (int c, int r) GetPosition(int index, int width) => (index % width, index / width);
    public static void GetPosition(int index, int width, out int column, out int row) => (column, row) = GetPosition(index, width);

    public static double Hypotenuse(int x1, int y1, int x2, int y2)
    {
        int a = x2 - x1;
        int b = y2 - y1;
        return Math.Sqrt(a * a + b * b);
    }
}

//Ignore.
#region Old code

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