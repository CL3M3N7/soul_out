using Godot;

namespace SoulOut.Scripts.Characters.Modifiers;

public partial class SpeedBuffModifier : Modifier
{
    public override void _Ready()
    {
        if (GetParent() is SOCharacter character) character.Speed *= 1.5f;
    }
}