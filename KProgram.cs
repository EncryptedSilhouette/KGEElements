#region Preamble Dev 0.0.1
//Apologies for the lack of comments, that will be improved.

//This project exists for a number or purposes.
//  1.) This is a hobby project, but also an item to add to my Portfolio as a developer.
//  2.) This is a goal I set for myself; To finish a project and make a game using all that I have learned throughout my years as a developer.
//  3.) This is a art showcase I wish for others to see; I want people to be able to observe a fully made game, in the hopes that it may spark their interest.

//I subscribed to a few programming philosophies in the creation of this project:
//  1.) Simplify. 
//  2.) Prioritize using the stack. Reduce, cache, and recycle objects when needed.
//  3.) Use structs for data; Use classes for data management.
//  4.) Use arrays and array pools over other collections.
//  5.) AVOID null at all costs. Exception for delegates; use a null check (D?.Invoke()).
//  6.) Provide as much data upfront.
//  7.) Optimize when and where needed. 
//  8.) Avoid inheritance.
//  9.) Have a common area to access other parts of the program.
//      This is the purpose of the static class KProgram; to act as a mediator between systems.
// 10.) Otherwise avoid static objects and singletons except for default/predetermined/hardcoded objects.

//The structure for this program is as follows:
//  Types belonging to this assembly are prefixed with 'K'.
//  Much of the code follows C#'s coding-style and coding-conventions: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
//  KProgram is a static class that serves as the the first layer and foundation for this app. 
//      This class takes care of loading and initialization, and contains refrences to managers, gobal variables, and serves as the entry point of the program.
//  To Be Continued...
#endregion

using Elements;
using Elements.Rendering;
using Elements.Game;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;

public record struct KTextureAtlas(Texture Texture, KSprite[] Sprites);
public record struct KSprite(string ID, Vector2f Rotocenter, FloatRect TextureCoords);

public static class KProgram
{
    //Other values are initialized in the static constructor for this class.
    const double MS_PER_SECOND = 1000.0d; //Using double for floating point precision.

    private static bool s_vSync = false;
    private static uint s_frameLimit;
    private static string s_title = string.Empty;

    public static bool Running = false;
    public static uint UpdateTarget;
    public static double UpdateInterval;
    public static Vector2u TargetResolution;
    public static string ConfigPath;
    public static Random RNG;
    public static RenderWindow Window;
    public static KInputManager InputManager;       //Needs a rework.
    public static KCommandManager CommandManager;   //Unfinished.
    public static KRenderManager RenderManager;     //Finished. 
    public static KGameManager GameManager;         //WIP
    public static KLogManager LogManager;           //Unfinished.
    public static KCLI CLI;                         //Unfinished.
    public static Font[] Fonts = [];
    public static KTextureAtlas[] TextureAtlases = [];

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
        ConfigPath = "config.csv";

        //Defaults
        UpdateInterval = MS_PER_SECOND / UpdateTarget;

        //Managers
        InputManager = new();
        CommandManager = new();
        RenderManager = new(Window);
        GameManager = new(RenderManager, InputManager);
        LogManager = new();
        CLI = new(CommandManager);
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
        //Loading
        #region Load Configs

        var lines = File.ReadAllLines(ConfigPath);

        for (int i = 0; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');

            switch (values[0])
            {
                case "title":
                    try 
                    { 
                        Title = values[1]; 
                    }
                    catch (Exception) 
                    { 
                        Console.WriteLine("failed to read \"title\""); 
                    }
                    break;

                case "frame_target":
                    try 
                    { 
                        FrameLimit = Convert.ToUInt32(values[1]); 
                    }
                    catch (Exception) 
                    { 
                        Console.WriteLine("failed to read \"frame_target\""); 
                    }
                    break;

                case "vsync":
                    try 
                    { 
                        VSync = bool.TrueString == values[1]; 
                    }
                    catch (Exception) 
                    { 
                        Console.WriteLine("failed to read \"vsync\""); 
                    }
                    break;

                case "resolution":
                    try
                    {
                        TargetResolution = new Vector2u(Convert.ToUInt32(values[1]), Convert.ToUInt32(values[2]));
                    }
                    catch (Exception) 
                    { 
                        Console.WriteLine("failed to read \'resolution\'"); 
                    }
                    break;

                case "atlases":
                    //Creates a new Texture atlas for each atlas defined after "atlases" at current line.
                    TextureAtlases = new KTextureAtlas[values.Length - 1];

                    for (int j = 1; j < values.Length; j++)
                    {
                        TextureAtlases[j - 1] = LoadAtlas(values[j]);
                    }
                    break;

                case "fonts":
                    Fonts = new Font[values.Length - 1];

                    for (int j = 0; j < Fonts.Length; j++)
                    {
                        Fonts[j] = new Font(values[j + 1]);

                        LogManager.DebugLog($"Loading font{j}: {Path.GetFileNameWithoutExtension(values[j + 1])}.");
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion

        OnLoad?.Invoke();

        #region Rendering initilization

        //Draw layers.
        var drawLayers = new KDrawLayer[2];
        //Default render layer.
        drawLayers[0] = new KDrawLayer(new VertexBuffer(16384, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic));
        drawLayers[0].States.Texture = TextureAtlases[0].Texture; 

        //Default text layer.
        drawLayers[1] = KRenderManager.CreateTextLayer(Fonts[0]);

        RenderManager.Init(Fonts[0], drawLayers);
        GameManager.Init();

        #endregion

        InputManager.Init(Window);

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

        GameManager.Update(currentUpdate);
    }

    private static void FrameUpdate(in uint currentUpdate, in uint currentFrame, in double deltaTime)
    {
        GameManager.FrameUpdate(RenderManager);
        CLI.FrameUpdate(RenderManager);
        RenderManager.FrameUpdate();
    }

    public static KTextureAtlas LoadAtlas(string filePath)
    {
        //Reads from texture data file.
        var lines = File.ReadAllLines(filePath);

        //loads texture file at first line. 
        Texture texture = new(lines[0]);
        List<KSprite> sprites = new();

        #region Data entry legend
        //index 0: sprite name
        //index 1: sprite id
        //index 2: sprite pos x
        //index 3: sprite pos y
        //index 4: sprite width
        //index 5: sprite height
        //index 6: sprite rotation center x
        //index 7: sprite rotation center y
        #endregion

        for (int i = 0; i < lines.Length; i++) //Iterate over lines
        {
            var contents = lines[i].Split(','); //Split line into values

            switch (contents[0]) 
            {
                case "sprite":
                    sprites.Add(new KSprite
                    {
                        ID = contents[1],
                        TextureCoords = new FloatRect
                        {
                            Left = Convert.ToInt32(contents[2]),
                            Top = Convert.ToInt32(contents[3]),
                            Width = Convert.ToInt32(contents[4]),
                            Height = Convert.ToInt32(contents[5]),
                        },
                        Rotocenter = new Vector2f 
                        {
                            X = Convert.ToInt32(contents[6]),
                            Y = Convert.ToInt32(contents[7])
                        }
                    });
                    break;
            }
        }
        return new KTextureAtlas(texture, sprites.ToArray());
    }

    //Math functions. Here for now, may move later.
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
var contents = File.ReadAllLines(filePath);
List<KTextureAtlas> atlases = new(contents.Length - 1);

//First line should be the texture path.
KTextureAtlas atlas = new()
{
    Texture = new(contents[0]),
    Sprites = []
};

//Add the full texture as default sprite.
sprites.Add(new((0, 0), (atlas.Texture.Size.X, atlas.Texture.Size.Y)));

LogManager.DebugLog($"Loading texture atlas: {Path.GetFileNameWithoutExtension(filePath)}.");

for (int i = 1; i < contents.Length; i++)
{
    var values = contents[i].Split(',');

    switch (values[0])
    {
        case "sprite":
            sprites.Add(new FloatRect()
            {
                Left = Convert.ToInt32(values[2]),
                Top = Convert.ToInt32(values[3]),
                Width = Convert.ToInt32(values[4]),
                Height = Convert.ToInt32(values[5])
            });
            LogManager
                .DebugLog($"Added sprite: {values[1]} to atlas: {Path.GetFileNameWithoutExtension(filePath)}.");
            break;

        default:
            break;
    }
}

TextureAtlases.Add(Path.GetFileNameWithoutExtension(contents[0]), atlas);



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
