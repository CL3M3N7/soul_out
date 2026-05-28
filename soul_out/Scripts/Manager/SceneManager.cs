using System;
using Godot;
using Godot.Collections;
using SoulOut.Scripts.Levels;
using SoulOut.Scripts.Resource;

namespace SoulOut.Scripts.Manager;

public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }

	public enum SceneType
	{
		MainScene,
		BattleScene,
		JudgementScene,
		TrialScene,
		ScoringScene,
		ResultScene
	}
	
	private Node _gameNode;
	
	public Array<PackedScene> BattleScenes;
	public PackedScene JudgementScene;
	public Array<PackedScene> TrialScenes;
	public PackedScene ScoringScenes;
	
	private SceneType _currentScene = SceneType.BattleScene;

	public override void _Ready()
	{
		if (Instance != null)
		{
			GD.PrintErr("[SceneManger] Multiple instances of SceneManger.");
			throw new ArgumentException("Multiple instances of SceneManger.");
		}

		Instance = this;
		FindScenes();

		// Todo : remove later
		CallDeferred(nameof(StartGame));
	}

	public void FindScenes()
	{
		ListScene listScene = GD.Load<ListScene>("res://resources/list_scene.tres");
		BattleScenes = listScene.BattleScenes;
		if (BattleScenes.Count == 0)
		{
			GD.PrintErr("[SceneManager] No battle scenes loaded.");
			throw new ArgumentException("No battle scenes loaded.");
		}
		
		JudgementScene = listScene.JudgementScene;
		if (JudgementScene == null)
		{
			GD.PrintErr("[SceneManager] No judgement scene loaded.");
			throw new ArgumentException("No judgement scene loaded.");
		}
		
		TrialScenes = listScene.TrialScenes;
		if (TrialScenes.Count == 0)
		{
			GD.PrintErr("[SceneManager] No trial scenes loaded.");
			throw new ArgumentException("No trial scenes loaded.");
		} 
		
		ScoringScenes = listScene.ScoringScenes;
		if (ScoringScenes == null)
		{
			GD.PrintErr("[SceneManager] No scoring scenes loaded.");
			throw new ArgumentException("No scoring scenes loaded.");
		}
	}

	public void StartGame()
	{
		GameManager.Instance.Reset();
		GetTree().Root.GetNodeOrNull<Node>("Game")?.QueueFree();

		_gameNode = new Node();
		_gameNode.Name = "Game";
		GetTree().Root.AddChild(_gameNode);
		LoadBattleScene();
	}

	public void LoadScene(SONodeScene instantiatedScene)
	{
		foreach (var child in _gameNode.GetChildren())
			child.QueueFree();
		_gameNode.AddChild(instantiatedScene);
		instantiatedScene.OnEndScene += ChangeScene;
	}

	private void LoadBattleScene()
	{
		PackedScene nextScene = BattleScenes.PickRandom();
		BattleScene instantiatedScene = nextScene.Instantiate<BattleScene>();
		LoadScene(instantiatedScene);
		GD.Print("test");
		Callable.From(() => {
			instantiatedScene.BattleManager.OnEndBattle += PactManager.Instance.RegisterBattleLeaderboard;
		}).CallDeferred();
	}

	private void LoadJudgementScene()
	{
		PackedScene nextScene = JudgementScene;
		SONodeScene instantiatedScene = nextScene.Instantiate<SONodeScene>();
		LoadScene(instantiatedScene);
	}

	private void LoadTrialScene()
	{
		PackedScene nextScene = TrialScenes.PickRandom();
		TrialScene instantiatedScene = nextScene.Instantiate<TrialScene>();
		LoadScene(instantiatedScene);
		Callable.From(() => {
			instantiatedScene.OnEndTrial += GameManager.Instance.AddScoreWithLeaderboard;
		}).CallDeferred();
		
	}

	private void LoadScoringScene()
	{
		PackedScene nextScene = ScoringScenes;
		SONodeScene instantiatedScene = nextScene.Instantiate<SONodeScene>();
		LoadScene(instantiatedScene);
	}

	public void ChangeScene()
	{
		_currentScene = FoundNextSceneType();
		LoadNextScene();
	}

	private SceneType FoundNextSceneType()
	{
		switch (_currentScene)
		{
			case SceneType.MainScene:
				return SceneType.BattleScene;
			case SceneType.BattleScene:
				return SceneType.JudgementScene;
			case SceneType.JudgementScene:
				return SceneType.TrialScene;
			case SceneType.TrialScene:
				// Inutile d'afficher le score s'il s'agit du dernier mini-jeu
				 return GameManager.Instance.IsLastGame() ? SceneType.ResultScene : SceneType.ScoringScene;
			case SceneType.ScoringScene:
				return SceneType.BattleScene;
			default:
				return SceneType.MainScene;
		}
	}

	private void LoadNextScene()
	{
		switch (_currentScene)
		{
			case SceneType.MainScene:
				GD.PrintErr("[SceneManager] MainScene not implemented.");
				throw new ArgumentException("MainScene not implemented.");
			case SceneType.BattleScene:
				LoadBattleScene();
				break;
			case SceneType.JudgementScene:
				LoadJudgementScene();
				break;
			case SceneType.TrialScene:
				LoadTrialScene();
				break;
			case SceneType.ScoringScene:
				LoadScoringScene();
				break;
			default:
				GD.PrintErr("[SceneManager] Unknown scene type.");
				throw new ArgumentException("Unknown scene type.");
		}
	}
}
