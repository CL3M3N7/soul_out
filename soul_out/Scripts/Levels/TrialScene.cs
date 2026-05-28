using Godot;
using Godot.Collections;

namespace SoulOut.Scripts.Levels;

public partial class TrialScene : SONodeScene
{
    [Signal] public delegate void OnEndTrialEventHandler(Array<int> leaderboard);
}