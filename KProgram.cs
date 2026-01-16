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
using Elements.Game;
using Elements.Rendering;
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
    private static string s_configPath;

    public static bool Running = false;
    public static uint UpdateTarget;
    public static double UpdateInterval;
    public static Vector2u TargetResolution;
    public static RenderWindow Window;
    public static Random RNG;
    public static KInputManager InputManager;       //Needs a rework.
    public static KRenderManager RenderManager;     //Finished. 
    public static KGameManager GameManager;         //WIP
    public static KLogManager LogManager;           //Unfinished.
    public static VideoMode[] VideoModes;
    public static Font[] Fonts = [];
    public static KTextureAtlas[] TextureAtlases = [];

    //Events for extension. 
    public static event Action? OnInit;
    public static event Action? OnLoad;
    public static event Action? OnStart;
    public static event Action? OnStop;
    public static event Action? OnDeinit;

    //Getters and Setters
    public static uint FontSize
    {
        get; set;
    }

    public static float AspectX
    {
        get; set;
    }

    public static float AspectY
    {
        get; set;
    }

    public static float AspectRatio => AspectX / AspectY;

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
        VideoModes = VideoMode.FullscreenModes;

        //configs
        s_configPath = "config.csv";
        Title = "Elements";
        FontSize = 14;
        UpdateTarget = 30;
        FrameLimit = UpdateTarget;
        TargetResolution = Window.Size;

        //Defaults
        UpdateInterval = MS_PER_SECOND / UpdateTarget;

        //Managers
        InputManager = new();
        RenderManager = new(Window, new VertexBuffer(4096, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Stream));
        GameManager = new(RenderManager, InputManager);
        LogManager = new();
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

        var lines = File.ReadAllLines(s_configPath);

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
                        VSync = values[1] == bool.TrueString;
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
                        Console.WriteLine("failed to read \"resolution\"");
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

        #region Draw layers

        var drawLayers = new KDrawLayer[2]; //Draw layers.

        drawLayers[0] = new KDrawLayer //Default render layer.
        {
            Buffer = new VertexBuffer(16384, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
            States = new(TextureAtlases[0].Texture),
            RenderTexture = new(Window.Size.X, Window.Size.Y),
            DrawBounds = new(0, 0, Window.Size.X, Window.Size.Y)
        };

        drawLayers[1] = new KDrawLayer //Default text layer.
        {
            Buffer = new VertexBuffer(16384, PrimitiveType.Quads, VertexBuffer.UsageSpecifier.Dynamic),
            States = new(Fonts[0].GetTexture(12)),
            RenderTexture = new(Window.Size.X, Window.Size.Y),
            DrawBounds = new(0, 0, Window.Size.X, Window.Size.Y)
        };

        #endregion

        RenderManager.Init(drawLayers);
        InputManager.Init(Window);
        GameManager.Init();

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

        GameManager.Update(currentUpdate);
    }

    private static void FrameUpdate(in uint currentUpdate, in uint currentFrame, in double deltaTime)
    {
        GameManager.FrameUpdate(RenderManager);
        RenderManager.FrameUpdate();
    }

    public static KTextureAtlas LoadAtlas(string filePath)
    {
        //Reads from texture data file.
        var lines = File.ReadAllLines(filePath);

        //Loads the whole atlas as a sprite at index 0.
        Texture texture = new(lines[0]);
        List<KSprite> sprites =
        [
            new KSprite
            {
                ID = string.Empty,
                TextureCoords = new FloatRect
                {
                    Left = 0,
                    Top = 0,
                    Width = texture.Size.X,
                    Height = texture.Size.Y,
                },
                Rotocenter = new Vector2f
                {
                    X = 0,
                    Y = 0
                }
            }
        ];

        #region Data entry legend
        //This is how data is layed out in csv texture data files:
        //index 0: sprite name
        //index 1: sprite id
        //index 2: sprite pos x1
        //index 3: sprite pos y1
        //index 4: sprite y2
        //index 5: sprite y2
        //index 6: sprite rotation center x1
        //index 7: sprite rotation center y1
        #endregion

        for (int i = 1; i < lines.Length; i++) //Iterate over lines
        {
            var contents = lines[i].Split(','); //Split line into values

            if (contents[0] == "sprite")
            {
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
            }
        }

        return new KTextureAtlas(texture, sprites.ToArray());
    }

    //Leaving utility functions here for now, may move later.

    #region Math Utilities

    //Math functions.
    public static int GetIndex(int column, int row, int width) => column + row * width;
    public static void GetIndex(int column, int row, int width, out int index) => index = column + row * width;

    public static (int c, int r) GetPosition(int index, int width) => (index % width, index / width);
    public static void GetPosition(int index, int width, out int column, out int row) => (column, row) = GetPosition(index, width);

    public static float Hypotenuse(float x1, float y1, float x2, float y2)
    {
        float a = x1 - x2;
        float b = y1 - y2;
        return MathF.Sqrt(a * a + b * b);
    }
    #endregion

    #region Collision Utilities

    //Collision functions.
    public static bool CheckCirclePointCollision(float centerX, float centerY, float radius, float posX, float posY)
    {
        double distX = posX - centerX;
        double distY = posY - centerY;
        return distX * distX + distY * distY <= radius * radius;
    }

    public static bool CheckRectPointCollision(float x, float y, float width, float height, float posX, float posY) =>
        posX >= x && posX <= x + width &&
        posY >= y && posY <= y + height;

    public static bool CheckRectPointCollision(in FloatRect rectangle, float posX, float posY) =>
        posX >= rectangle.Left && posX <= rectangle.Left + rectangle.Width &&
        posY >= rectangle.Top && posY <= rectangle.Top + rectangle.Height;

    #endregion
}

#if DEBUG
//Ignore.
#region Old code

#if false

 //Collision functions.
    public static bool CheckCirclePointCollision(in float centerX, in float centerY, in float radius, in float posX, in float posY)
    {
        double distX = posX - centerX;
        double distY = posY - centerY;
        //checks hypotenuse without sqrt. Squareing negates negatives.
        //  a^2   +         b^2              c^2
        if (distX * distX + distY * distY <= radius * radius) return true;
        else return false;
    }

    public static bool CheckRectPointCollision(in float centerX, in float centerY, in float width, in float height, in float posX, in float posY)
    {
        float halfWidth = width / 2;
        float halfHeight = height / 2;

        //if within x bounds
        if (posX >= centerX - halfWidth && posX <= centerX + halfWidth)
        {
            //if within y bounds
            if (posY >= centerY - halfHeight && posY <= centerY + halfHeight) return true;
        }
        return false;
    }

    public static bool CheckRectPointCollision(in KRectangle rectangle, in float posX, in float posY) =>
        CheckRectPointCollision(
            rectangle.Transform.PosX, rectangle.Transform.PosY,
            rectangle.Width * rectangle.Transform.ScaleX, rectangle.Height * rectangle.Transform.ScaleY,
            posX, posY);
//Other shit

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

#endif