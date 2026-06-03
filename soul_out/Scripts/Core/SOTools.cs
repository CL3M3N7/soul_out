using SoulOut.Scripts.Characters.Modifiers;

namespace SoulOut.Scripts.Core;

public enum TypeCharacter
{
        Warrior,
        Archer,
        Pawn,
}

public enum TypeBuff
{
        None,
        Speed,
}

public enum TypeNerf
{
        None,
        Slowness,
}

public static class Modifiers
{
        public static Modifier CreateInstance(this TypeBuff buff) => buff switch
        {
                TypeBuff.Speed => new SpeedBuffModifier(),
                _ => null
        };
        public static Modifier CreateInstance(this TypeNerf nerf) => nerf switch
        {
                TypeNerf.Slowness => new SlownessNerfModifier(),
                _ => null
        };
}