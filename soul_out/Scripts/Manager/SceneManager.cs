using System;
using Godot;
using Godot.Collections;
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
    public Array<PackedScene> TrialScenes;
    
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
        
        TrialScenes = listScene.TrialScenes;
        if (TrialScenes.Count == 0)
        {
            GD.PrintErr("[SceneManager] No trial scenes loaded.");
            throw new ArgumentException("No trial scenes loaded.");
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

    public void LoadScene(PackedScene packedScene)
    {
        Node instantiatedScene = packedScene.Instantiate();
        foreach (var child in _gameNode.GetChildren())
            child.QueueFree();
        _gameNode.AddChild(instantiatedScene);
    }

    private void LoadBattleScene()
    {
        PackedScene nextScene = BattleScenes.PickRandom();
        LoadScene(nextScene);
    }

    private void LoadTrialScene()
    {
        PackedScene nextScene = TrialScenes.PickRandom();
        LoadScene(nextScene);
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
                // TODO: implémenter scène de transition
                // return SceneType.JudgementScene;
                return SceneType.TrialScene;
            case SceneType.JudgementScene:
                return SceneType.TrialScene;
            case SceneType.TrialScene:
                // Inutile d'afficher le score s'il s'agit du dernier mini-jeu
                // TODO: implémenter scène de transition
                // return GameManager.Instance.IsLastGame() ? SceneType.ResultScene : SceneType.ScoringScene;
                return GameManager.Instance.IsLastGame() ? SceneType.ResultScene : SceneType.BattleScene;
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
                GD.PrintErr("[SceneManager] JudgementScene not implemented.");
                throw new ArgumentException("JudgementScene not implemented.");
            case SceneType.TrialScene:
                LoadTrialScene();
                break;
            case SceneType.ScoringScene:
                GD.PrintErr("[SceneManager] ScoringScene not implemented.");
                throw new ArgumentException("ScoringScene not implemented.");
            default:
                GD.PrintErr("[SceneManager] Unknown scene type.");
                throw new ArgumentException("Unknown scene type.");
        }
    }
}