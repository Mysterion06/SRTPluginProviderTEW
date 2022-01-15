using SRTPluginProviderTEW.Structs.GameStructs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SRTPluginProviderTEW.Structs
{
    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct  EnemyHP
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsTrigger)
                    return string.Format("TRIGGER", CurrentHP, MaximumHP, Percentage);
                else if (IsAlive)
                    return string.Format("{0} / {1} ({2:P1})", CurrentHP, MaximumHP, Percentage);
                else
                    return "DEAD / DEAD (0%)";
            }
        }
        public float MaximumHP { get => _maximumHP; }
        internal float _maximumHP;

        public float CurrentHP { get => _currentHP; }
        internal float _currentHP;

        public bool IsTrigger => MaximumHP is 0.5f or 1f or 5f or 10f or 20f or 49f or 50f or 100f or > 2500f;
        public bool IsAlive => !IsTrigger && MaximumHP > 0 && CurrentHP > 0 && CurrentHP <= MaximumHP && MaximumHP <= 2500;
        public float Percentage => ((IsAlive) ? (float)CurrentHP / (float)MaximumHP : 0f);
    }
}
