using Godot;
using System;
using SoulOut.Scripts.Manager;

public partial class MainMenu : Control
{
	private Button _startButton;
	private Button _optionsButton;
	private Button _quitButton;
	
	// Optionnel : un panel qui contient tes options (à cacher/afficher)
	private Control _optionsMenu; 
	[Export] private Control _mainMenuPanel;
	[Export] private Control _playerSelectionPanel;

	public override void _Ready()
	{
		// 2. On récupère les nœuds de la scène
		// ATTENTION : Remplace "VBoxContainer/StartButton" par le vrai chemin de tes boutons dans ta scène Godot !
		_startButton = GetNode<Button>("VBoxContainer/StartButton");
		_optionsButton = GetNode<Button>("VBoxContainer/OptionsButton");
		_quitButton = GetNode<Button>("VBoxContainer/QuitButton");
		
		
		// _optionsMenu = GetNode<Control>("OptionsMenuPanel");
		// _optionsMenu.Hide(); // On s'assure qu'il est caché au lancement

		// 3. LA MAGIE DU C# : On connecte les signaux de clic aux fonctions
		_startButton.Pressed += OnStartButtonPressed;
		_optionsButton.Pressed += OnOptionsButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
		
		if(_startButton != null)
		{
			_startButton.GrabFocus();
		}
	}
	
	private void OnStartButtonPressed()
	{
		GD.Print("Start !!");
		
		// Pour lancer ta page de connexion des joueurs, on charge une nouvelle scène :
		// (Remplace le chemin par celui de ta scène de sélection de joueurs)
		// GetTree().ChangeSceneToFile("res://Scenes/PlayerSelection.tscn");
		_playerSelectionPanel.Show();
		_mainMenuPanel.Hide();
	}

	private void OnOptionsButtonPressed()
	{
		GD.Print("Menu des options ouvert !");
		
		// Si ton menu d'options est dans la même scène, tu l'affiches simplement :
		// _optionsMenu.Show();
		
		// (Tu pourras mettre un bouton "Retour" dans tes options qui fera _optionsMenu.Hide(); )
	}

	private void OnQuitButtonPressed()
	{
		GD.Print("Fermeture du jeu...");
		
		// La fonction officielle de Godot pour fermer l'application proprement
		GetTree().Quit();
	}
}
