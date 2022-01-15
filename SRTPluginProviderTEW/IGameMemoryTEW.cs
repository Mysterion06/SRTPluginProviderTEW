using System;
using SRTPluginProviderTEW.Structs;
using SRTPluginProviderTEW.Structs.GameStructs;

namespace SRTPluginProviderTEW
{
    public interface IGameMemoryTEW
    {
        // Gamename
        string GameName { get; }

        // Player
        PlayerHP Player { get; set; }
        GameStats Stats { get; set; }
        EnemyHP[] EnemyHealth { get; set; }

        // Money
        int GreenGel { get; set; }

        // Versioninfo
        string VersionInfo { get; }

        // GameInfo
        string GameInfo { get; set; }


    }
}