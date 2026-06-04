using Godot;

namespace SoulOut.Scripts.Levels;

public abstract partial class SONodeScene : Node2D
{
    [Signal] public delegate void OnEndSceneEventHandler();
}