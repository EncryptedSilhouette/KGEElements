using Elements.Core;
using Elements.Rendering;
using SFML.Graphics;
using SFML.System;

namespace Elements.Game.Map
{
    public class KGameMap
    {
        public const int TILE_SIZE = 32;

        private Random _random;

        public int PosX;
        public int PosY;
        public int Rows;
        public int Columns;
        public KTileMap TileMap;
        public KGameNode[] Nodes;

        public KGameMap(int posX, int posY, int rows, int columns)
        {
            _random = new();

            (PosX, PosY, Rows, Columns) = (posX, posY, rows, columns);

            Nodes = new KGameNode[rows * columns];
            Array.Fill(Nodes, new KGameNode
            {
                Handle = 0,
                Flags = KGameNodeFlags.NONE
            });
        }

        public void Init(int posX, int posY, int rows, int columns, int resourceCount)
        {
            (PosX, PosY, Rows, Columns) = (posX, posY, rows, columns);

            Nodes = new KGameNode[rows * columns];
            Array.Fill(Nodes, new KGameNode 
            { 
                Handle = 0, 
                Flags = KGameNodeFlags.NONE 
            });

            GenerateGameMap(rows, columns, resourceCount);
        }

        public void Update()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                //Nodes[i]
            }
        }

        public void FrameUpdate()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                //Nodes[i]
            }
        }

        public void GenerateGameMap(int rows, int columns, int resourceCount = 10)
        {
            Span<int> resources = stackalloc int[10];

            Nodes = new KGameNode[rows * columns];
            Array.Fill(Nodes, new KGameNode { Handle = 0, Flags = KGameNodeFlags.NONE });

            for (int i = 0; i < Nodes.Length; i++)
            {
                int index = resources[i] = _random.Next(rows * columns);
                var flavor = (KTileFlavor)_random.Next((int)KTileFlavor.COUNT);

                Nodes[index] = new KGameNode
                {
                    Handle = i,
                    Type = (flavor != KTileFlavor.WATER) ? KTileType.GROUND : KTileType.WATER,
                    Flavor = flavor,
                    Flags = KGameNodeFlags.RESOURCE,
                };
            }

            bool loop = true;
            for (int rad = 0; loop && rad < Nodes.Length; rad++) //r: radius (outer ring lv)
            {
                for (int j = 0; j < resources.Length; j++) //Iterate over all resources
                {
                    int handle = resources[j];

                    KProgram.GetPosition(handle, columns, out int r, out int c);

                    for (int k = 1 + 2 * rad; k > 0; k--)
                    {
                        int index = KProgram.GetIndex(r - k, c + k, c);
                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = Nodes[handle].Type;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }

                        index = KProgram.GetIndex(r - k, c - k, c);
                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = Nodes[handle].Type;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }

                        index = KProgram.GetIndex(r + k, c - k, c);
                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = Nodes[handle].Type;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }

                        index = KProgram.GetIndex(r + k, c + k, c);
                        if (index >= 0 && index < Nodes.Length && Nodes[index].Type == KTileType.NONE)
                        {
                            Nodes[index].Type = Nodes[handle].Type;
                            Nodes[index].Flavor = Nodes[handle].Flavor;
                        }
                    }
                }

                //Check all the nodes
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].Type == KTileType.NONE)
                    {
                        loop = true;
                        break;
                    }
                }
            }
        }

        public void CreateTileMap(KTextureAtlas textureAtlas)
        {
            RenderTexture renderTexture = new((uint)Columns, (uint)Rows);
            List<Vertex> vertices = new (Nodes.Length * 4);
            for (int i = 0; i < Nodes.Length; i += 4)
            {
                Color color = Nodes[i].Flavor switch
                {
                    KTileFlavor.EARTH => new Color(139, 69, 19),
                    KTileFlavor.WIND => new Color(135, 206, 235),
                    KTileFlavor.FIRE => new Color(255, 69, 0),
                    KTileFlavor.WATER => new Color(0, 191, 255),
                    _ => Color.White,
                };

                KRectangle texRect = Nodes[i].Type switch
                {
                    KTileType.GROUND => textureAtlas.Sprites[""],
                    KTileType.WALL => new(16, 0),
                    KTileType.WATER => new(32, 0),
                    _ => new(),
                };

                vertices[i] = new()
                {
                    TexCoords = new(0, 0),
                    Position = new(0, 0),
                    Color = color
                };
                vertices[i + 1] = new()
                {
                    TexCoords = new(0, 0),
                    Position = new(0, 0),
                    Color = color
                };
                vertices[i + 2] = new()
                {
                    TexCoords = new(0, 0),
                    Position = new(0, 0),
                    Color = color
                };
                vertices[i + 3] = new()
                {
                    TexCoords = new(0, 0),
                    Position = new(0, 0),
                    Color = color
                };
            }

            //buffer
            vertices.ToArray();
        }

        public void GetTileTexCoords()
        {

        }
    }
}
