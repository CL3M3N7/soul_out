using Godot;
using System;

public partial class GameManager : Node
{
	//TODO : ajouter les donnees de la partie
	private int _currentRound = 1;
	
	public void EndPhase(int tmp/*TODO : ajout classement*/)
	{
		_currentRound++;
		if(_currentRound%2 == 1)
		{
			//On sort d'un combat : appliquer bonus/malus
			//On passe sur un mini-jeu ajouter la logique de selection de mini-jeux
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://scenes/levels/level_test2.tscn");
		}
		else
		{
			//On sort d'un mini-jeu : modifier les scores
			//On passe sur un combat
			GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile, "res://scenes/levels/level_test.tscn");
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
}
