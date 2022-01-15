using System.Runtime.InteropServices;

namespace SRTPluginProviderTEW.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x600)]

    public struct GameStats
    {
        [FieldOffset(0x10)] private int greenGel;
        [FieldOffset(0x98)] private int igt;
        
        public int GreenGel => greenGel;

        public int IGT => igt;
    }
}