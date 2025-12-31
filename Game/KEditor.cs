namespace Elements.Game
{
    public class KEditor
    {
        enum TileType : byte
        {
            EMPTY = 0,
            WALL = 1,
            FLOOR = 2,
        }

        enum TileElement : byte
        {
            EMPTY = 0,
            WALL = 1,
            FLOOR = 2,
        }

        public static int GetIndex(int row, int column, byte width) => column + row * width;

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
            Array.Fill(Map, (byte) TileType.EMPTY);
        }

        public void Update()
        {
            for (int i = 0; i < Map.Length; i++)
            {
                byte tile = Map[i];

                switch (tile)
                {
                    case (byte)TileType.EMPTY:
                        break;
                    case (byte)TileType.WALL:
                        break;
                    case (byte)TileType.FLOOR:
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
                    case (byte)TileType.EMPTY:
                        break;
                    case (byte)TileType.WALL:
                        break;
                    case (byte)TileType.FLOOR:
                        break;
                    default:
                        break;
                }
            }
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
