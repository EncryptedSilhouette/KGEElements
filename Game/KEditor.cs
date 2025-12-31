namespace Elements.Game
{
    public class KEditor
    {
        enum TileType : byte
        {
            GROUND = 0,
            WALL = 1,
            WATER = 2,
        }

        enum TileFlavor : byte
        {
           CREATION = 0,
           EARTH = 1,
           WIND = 2,
           FIRE = 3,
           WATER = 4,
        }

        public static void GetIndex(int row, int column, byte width, out int index) => index = column + row * width;

        public static void GetPosition(int index, int width, out int row, out int column)
        {
            row = index / width;
            column = index % width;
        }

        public static double Hypotenuse(int x1, int y1, int x2, int y2)
        {
            int a = x2 - x1;
            int b = y2 - y1;
            return Math.Sqrt(a * a + b * b);
        }

        private Random _random = new Random();

        public byte[] Map;
        public byte[] FlavorMap;

        public KEditor()
        {
            Map = [];
            FlavorMap = [];
        }

        public void Init(byte Width, byte Height)
        {
            Map = new byte[Width*Height];
            Array.Fill(Map, (byte) TileType.GROUND);
        }

        public void Update()
        {
            for (int i = 0; i < Map.Length; i++)
            {
                byte tile = Map[i];

                switch (tile)
                {
                    case (byte)TileType.WATER:
                        break;
                    case (byte)TileType.GROUND:
                        break;
                    case (byte)TileType.WALL:
                        break;
                    default:
                        break;
                }
            }
        }

        public void FrameUpdate()
        {
            for (int i = 0; i < FlavorMap.Length; i++)
            {
                byte tile = FlavorMap[i];

                switch (tile)
                {
                    case (byte)TileFlavor.EARTH:
                        break;
                    case (byte)TileFlavor.WIND:
                        break;
                    case (byte)TileFlavor.FIRE:
                        break;
                    case (byte)TileFlavor.WATER:
                        break;
                    default:
                        //Creation 
                        break;
                }
            }
        }

        public void GenerateMap(byte rows, byte columns)
        {
            var radius = 10;
            var distance = =;
            Map = new byte[rows * columns]; //cache pls
            Array.Fill(Map, (byte) TileType.GROUND);

            FlavorMap = new byte[rows * columns]; //cache pls
            Array.Fill(FlavorMap, (byte) TileFlavor.CREATION);

            int[] nodes = new int[10];
            Array.Fill(nodes, 0);

            //Create Nodes
            for (int i = 0; i < nodes.Length; i++)
            {
                var index = _random.Next(rows*columns);
                var leastIntrusive = index;

                GetPosition(index, columns, out int ra, out int ca); //Position of current node.

                for (int j = 0; j < nodes.Length; j++)
                {
                    if (nodes[i] == nodes[j]) continue;

                    GetPosition(nodes[j], columns, out int rb, out int cb); //Gets position of other node.

                    if (Hypotenuse(ra, ca, rb, cb) > leastIntrusive) 
                }

                nodes[i] = leastIntrusive;
            }




            //for (int i = 0; i < FlavorMap.Length; i++)
            //{
            //    for (int i = 0; i < length; i++)
            //    {

            //    }
            //}
        }

        //public void TileLogic()
        //{
        //    switch (tile)
        //    {
        //        case (byte)TileType.EMPTY:
        //            break;
        //        case (byte)TileType.WALL:
        //            break;
        //        case (byte)TileType.FLOOR:
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}
