using Elements.Rendering;
using SFML.Graphics;

namespace Elements.Game.Map
{
    public class KGameMap
    {
        public const int TILE_SIZE = 32;

        private Random _random;
        private int[] _resources;
        private Vertex[] _vBuffer;

        public int PosX;
        public int PosY;
        public int Rows;
        public int Columns;
        public int TileWidth;
        public int TileHeight;
        public KTileMap TileMap;
        public KGameNode[] Nodes;

        public KGameMap(int posX, int posY, int tileWidth, int tileHeight)
        {
            _random = new();
            _resources = [];
            _vBuffer = [];

            (PosX, PosY, Rows, Columns, TileWidth, TileHeight) = (posX, posY, 0, 0, tileWidth, tileHeight);

            Nodes = [];
        }

        public void Init(KTextureAtlas textureAtlas, int posX, int posY, int rows, int columns, int resourceCount)
        {
            (PosX, PosY, Rows, Columns) = (posX, posY, rows, columns);

            Nodes = new KGameNode[rows * columns];
            Array.Fill(Nodes, new KGameNode 
            { 
                Handle = 0, 
                Type = KTileType.NONE,
                Flavor = KTileFlavor.CREATION,
            });

            GenerateGameMap(rows, columns, resourceCount);
            CreateTileMap(textureAtlas);
        }

        public void Update()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                
            }
        }

        public void FrameUpdate(KRenderManager renderer)
        {
            renderer.SubmitDrawQuad(_vBuffer, (uint)_vBuffer.Length, 0);
        }

        public void GenerateGameMap(int rows, int columns, int resourceCount = 10)
        {
            Nodes = new KGameNode[rows * columns];
            Array.Fill(Nodes, new KGameNode { Handle = 0, Type = KTileType.NONE, Flavor = KTileFlavor.CREATION });

            _resources = new int[resourceCount];

            for (int i = 0; i < _resources.Length; i++)
            {
                int index = _random.Next(rows * columns);
                var flavor = (KTileFlavor)_random.Next((int)KTileFlavor.COUNT);

                _resources[i] = index;
                Nodes[index] = new KGameNode
                {
                    Handle = i,
                    Type = KTileType.RESOURCE,
                    Flavor = flavor,
                };
            }

            bool loop = true;
            int spread = 1;
            do
            {
                loop = false;
                Console.WriteLine($"spread: {spread}:");
                for (int i = 0; i < _resources.Length; i++)
                {
                    int handle = _resources[i];
                    KProgram.GetPosition(handle, Columns, out int col, out int row);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"tile:{i}, c:{col}, r:{row} ");
                    //Sets outer ring start positon at the following row: (r - spread).    
                    row -= spread;
                    Console.WriteLine($"st_row:{row} ");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    //Fill starting position.
                    int index = KProgram.GetIndex(col, row, columns);
                    if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                    {
                        Nodes[index].Flavor = Nodes[handle].Flavor;
                        Nodes[index].Type = KTileType.GROUND;
                    }

                    //Fills in the boarder, moving clockwise, starting at 12:00.
                    for (int j = 0; j < spread; j++) //Q1
                    {
                        col += 1;
                        row += 1;
                        index = KProgram.GetIndex(col, row, columns);

                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = KTileType.GROUND;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }
                    }
                    for (int j = 0; j < spread; j++) //Q2
                    {
                        col -= 1;
                        row += 1;
                        index = KProgram.GetIndex(col, row, columns);

                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = KTileType.GROUND;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }
                    }
                    for (int j = 0; j < spread; j++) //Q3
                    {
                        col -= 1;
                        row -= 1;
                        index = KProgram.GetIndex(col, row, columns);

                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = KTileType.GROUND;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }
                    }
                    for (int j = 0; j < spread; j++) //Q4
                    {
                        col += 1;
                        row -= 1;
                        index = KProgram.GetIndex(col, row, columns);

                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = KTileType.GROUND;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }
                    }
                }
                //Tile Check
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].Type == KTileType.NONE)
                    {
                        loop = true;
                        break;
                    }
                }

                spread++;
            }
            while (loop && spread < Nodes.Length);
        }

        public void CreateTileMap(KTextureAtlas textureAtlas)
        {
            Vertex[] vertices = new Vertex[Nodes.Length * 4];

            for (int i = 0; i < Nodes.Length; i++)
            {
                FloatRect rec = Nodes[i].Type switch
                {
                    KTileType.RESOURCE => textureAtlas.Sprites["tile_resource"],
                    _ => textureAtlas.Sprites["tile_0"],
                };

                Color color = Nodes[i].Flavor switch
                {
                    KTileFlavor.EARTH => new Color(139, 69, 19),
                    KTileFlavor.WIND => new Color(135, 206, 235),
                    KTileFlavor.FIRE => new Color(255, 69, 0),
                    KTileFlavor.WATER => new Color(0, 191, 255),
                    _ => Color.White,
                };
                //Remove when done
                if (Nodes[i].Type == KTileType.RESOURCE)
                {
                    color = Color.Black;
                }

                vertices[i * 4] = new Vertex
                {
                    Position = (PosX + TileWidth * (i % Columns), 
                                PosY + TileHeight * (i / Columns)),
                    Color = color,
                    TexCoords = (rec.Left, rec.Top),
                };
                vertices[i * 4 + 1] = new Vertex
                {
                    Position = (PosX + TileWidth * (i % Columns) + TileWidth,
                                PosY + TileHeight * (i / Columns)),
                    Color = color,
                    TexCoords = (rec.Left + rec.Width, rec.Top),
                };
                vertices[i * 4 + 2] = new Vertex
                {
                    Position = (PosX + TileWidth * (i % Columns) + TileWidth,
                                PosY + TileHeight * (i / Columns) + TileHeight),
                    Color = color,
                    TexCoords = (rec.Left + rec.Width, rec.Top + rec.Height),
                };
                vertices[i * 4 + 3] = new Vertex
                {
                    Position = (PosX + TileWidth * (i % Columns),
                                PosY + TileHeight * (i / Columns) + TileHeight),
                    Color = color,
                    TexCoords = (rec.Left, rec.Top + rec.Height),
                };
            }
            _vBuffer = vertices;
        }
    }
}
