using Godot;
using Godot.Collections;

namespace SoulOut.Scripts.Resource;

[Tool]
[GlobalClass]
public partial class ListScene : Godot.Resource
{
	[Export] public PackedScene MainScene;
	[Export] public Array<PackedScene> BattleScenes;
	[Export] public PackedScene JudgementScene;
    [Export] public Array<PackedScene> TrialScenes;
	[Export] public PackedScene ScoringScenes;
}