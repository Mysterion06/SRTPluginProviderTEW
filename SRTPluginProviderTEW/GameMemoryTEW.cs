using System;
using System.Globalization;
using System.Runtime.InteropServices;
using SRTPluginProviderTEW.Structs;
using SRTPluginProviderTEW.Structs.GameStructs;
using System.Diagnostics;
using System.Reflection;

namespace SRTPluginProviderTEW
{
    public class GameMemoryTEW : IGameMemoryTEW
    {

        // Gamename
        public string GameName => "TEW";

        // Player
        public PlayerHP Player { get => _player; set => _player = value; }
        internal PlayerHP _player;

        public GameStats Stats { get => _stats; set => _stats = value; }
        internal GameStats _stats;

        public int GreenGel { get => _greenGel; set => _greenGel = value; }
        internal int _greenGel;

        // Enemy HP
        public EnemyHP[] EnemyHealth { get => _enemyHealth; set => _enemyHealth = value; }
        internal EnemyHP[] _enemyHealth;

        // Versioninfo
        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        // GameInfo
        public string GameInfo { get => _gameInfo; set => _gameInfo = value; }
        internal string _gameInfo;
    }
}
