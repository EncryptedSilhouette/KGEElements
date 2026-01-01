namespace Elements.Game.Map
{
    [Flags]
    public enum KGameNodeFlags: ulong
    {
        NONE = 0,
        RESOURCE = 1 << 0
    }

    public struct KGameNode
    {
        public int Handle;
        public KTileType Type; 
        public KTileFlavor Flavor;
        public KGameNodeFlags Flags;

        public KGameNode(int handle) =>
            (Handle, Flags) = (handle, KGameNodeFlags.NONE);

        public KGameNode(int handle, KGameNodeFlags flags) =>
            (Handle, Flags) = (handle, flags);    
    }
}
