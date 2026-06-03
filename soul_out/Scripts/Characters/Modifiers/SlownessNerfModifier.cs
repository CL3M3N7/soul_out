using Godot;

namespace SoulOut.Scripts.Characters.Modifiers;

public partial class SlownessNerfModifier : Modifier
{
    public override void _Ready()
    {
        if (GetParent() is SOCharacter character) character.Speed *= 0.7f;
    }
}