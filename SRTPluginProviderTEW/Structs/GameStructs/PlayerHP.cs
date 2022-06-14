using System.Runtime.InteropServices;

namespace SRTPluginProviderTEW.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x250C)]

    public struct PlayerHP
    {
        [FieldOffset(0x8C0)] private float maxHP;
        [FieldOffset(0x8C4)] private float currentHP;

        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public bool IsAlive => CurrentHP != 0 && MaxHP != 0 && CurrentHP > 0 && CurrentHP <= MaxHP;
        public float PercentageHP => IsAlive ? (float)CurrentHP / (float)MaxHP : 0f;
        public PlayerState HealthState
        {
            get =>
                !IsAlive ? PlayerState.Dead :
                PercentageHP >= 0.66f ? PlayerState.Fine :
                PercentageHP >= 0.33f ? PlayerState.Caution :
                PlayerState.Danger;
        }
        public string CurrentHealthState => HealthState.ToString();
    }

    public enum PlayerState
    {
        Dead,
        Fine,
        Caution,
        Danger
    }
}