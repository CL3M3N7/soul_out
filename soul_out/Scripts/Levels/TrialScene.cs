using Godot;
using Godot.Collections;

namespace SoulOut.Scripts.Levels;

public partial class TrialScene : GameplayScene
{
    [Signal] public delegate void OnEndTrialEventHandler(Array<int> leaderboard);

    public override void SetHUD(SOCharacter character)
    {
        throw new System.NotImplementedException();
    }
}