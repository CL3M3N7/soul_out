using System;
using Godot;

namespace SoulOut.Scripts.Manager;

public partial class GameManager : Node
{
    public static GameManager Instance { get; private set; }
    
    [Signal] public delegate void ScoreUpdateEventHandler(int playerSlot, int newScore);
    [Signal] public delegate void ChangeRoundEventHandler(int round);

    public int NumberOfPlayers { get; private set; } = 4; // On peut le changer depuis le menu principal
    private int[] _playerScore;

    private int _currentRound = 0;
    private int _totalRounds = 10;
    
    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("[GameManager] Multiple instances of GameManager.");
            throw new ArgumentException("Multiple instances of GameManager.");
        }

        Instance = this;
        Reset();
    }

    public void Reset()
    {
        _currentRound = 0;
        _playerScore = new int[NumberOfPlayers];
    }
    
    public void AddPlayerScore(int playerSlot, int score)
    {
        _playerScore[playerSlot] += score;
        EmitSignal(nameof(ScoreUpdateEventHandler), playerSlot, _playerScore[playerSlot]);
    }

    public int[] GetAllPlayerScore()
    {
        return _playerScore;
    }

    public bool IsLastGame()
    {
        return _currentRound >= _totalRounds;
    }

    public void NextRound()
    {
        _currentRound++;
        EmitSignal(nameof(ChangeRoundEventHandler), _currentRound);
    }
}