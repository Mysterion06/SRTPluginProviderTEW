using SRTPluginBase;
using System;

namespace SRTPluginProviderTEW
{
    internal class PluginInfo : IPluginInfo
    {
        public string Name => "Game Memory Provider The Evil Within";

        public string Description => "A game memory provider plugin for The Evil Within.";

        public string Author => "Mysterion_06_ (Pointers & Coding) & Squirrelies (Provider of the SRTHost)";

        public Uri MoreInfoURL => new Uri("https://github.com/SpeedrunTooling/SRTPluginProviderTEW");

        public int VersionMajor => assemblyFileVersion.ProductMajorPart;

        public int VersionMinor => assemblyFileVersion.ProductMinorPart;

        public int VersionBuild => assemblyFileVersion.ProductBuildPart;

        public int VersionRevision => assemblyFileVersion.ProductPrivatePart;

        private System.Diagnostics.FileVersionInfo assemblyFileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}
