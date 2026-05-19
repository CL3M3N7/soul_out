using Godot;
using System;

public partial class GameManager : Node
{
	//TODO : ajouter les donnees de la partie
	private int _currentRound = 0;
	private int _maxRound = 10; 	//10 round = 5 mini-jeux + 5 combats
	
	public void EndPhase(int tmp/*TODO : ajout classement*/)
	{
		_currentRound++;
		if(_currentRound%2 == 1)
		{
			//On sort d'un combat : appliquer bonus/malus
			EndMiniGame();
			//On passe sur un mini-jeu ajouter la logique de selection de mini-jeux
			if(_currentRound < _maxRound)
			{
				GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://scenes/levels/level_test.tscn");
			}
			else
			{
				EndGame();
			}
		}
		else
		{
			//On sort d'un mini-jeu : modifier les scores
			EndBattle();
			//On passe sur un combat
			if(_currentRound < _maxRound)
			{
				GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://scenes/levels/CombatScene.tscn");
			}
			else
			{
				EndGame();
			}
			
		}
	}
	
	public void EndMiniGame(/*TODO : recevoir classement*/)
	{
		// TODO incremente les scores
	}
	
	public void EndBattle(/*TODO : recevoir classement*/)
	{
		// TODO applique le bonus et le malus
	}
	
	public void EndGame()
	{
		//TODO logique de fin de partie
	}
}
