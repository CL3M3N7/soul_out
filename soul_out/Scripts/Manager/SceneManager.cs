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
	
	public PackedScene MainScene;
	public Array<PackedScene> BattleScenes;
	public PackedScene JudgementScene;
	public Array<PackedScene> TrialScenes;
	public PackedScene ScoringScenes;
	public PackedScene ResultScene;
	
	private SceneType _currentScene = SceneType.MainScene;

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
		
		MainScene = listScene.MainScene;
		if (MainScene == null)
		{
			GD.PrintErr("[SceneManager] No main scene loaded.");
			throw new ArgumentException("No main scene loaded.");
		}
		
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
		
		ResultScene = listScene.ResultScene;
		if (ResultScene == null)
		{
			GD.PrintErr("[SceneManager] No result scenes loaded.");
			throw new ArgumentException("No result scenes loaded.");
		}
	}

	public void StartGame()
	{
		GameManager.Instance.Reset();
		GetTree().Root.GetNodeOrNull<Node>("Game")?.QueueFree();

		_gameNode = new Node();
		_gameNode.Name = "Game";
		GetTree().Root.AddChild(_gameNode);
		LoadMainScene();
	}
	
	private void LoadMainScene()
	{
		if (MainScene == null) return;
		
		Node instantiatedScene = MainScene.Instantiate<Node>();
		
		foreach (var child in _gameNode.GetChildren())
			child.QueueFree();
			
		_gameNode.AddChild(instantiatedScene);
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
		instantiatedScene.OnAddBuff += PactManager.Instance.AddBuff;
		instantiatedScene.OnAddNerf += PactManager.Instance.AddNerf;
		instantiatedScene.OnEndTrial += GameManager.Instance.AddScoreWithLeaderboard;
		
	}

	private void LoadScoringScene()
	{
		PackedScene nextScene = ScoringScenes;
		SONodeScene instantiatedScene = nextScene.Instantiate<SONodeScene>();
		LoadScene(instantiatedScene);
	}

	private void LoadResultScene()
	{
		PackedScene nextScene = ResultScene;
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
				LoadMainScene();
				break;
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
			case SceneType.ResultScene:
				LoadResultScene();
				break;
			default:
				GD.PrintErr("[SceneManager] Unknown scene type.");
				throw new ArgumentException("Unknown scene type.");
		}
	}
}
