using Godot;
using System;
using System.Collections.Generic;
using SoulOut.Scripts.Manager;

public partial class MainMenu : Control
{
	private HSlider _masterSlider;
	private HSlider _musicSlider;
	private HSlider _sfxSlider;

	// On stocke les index de nos bus audio pour aller plus vite
	private int _masterBusIndex;
	private int _musicBusIndex;
	private int _sfxBusIndex;

	private Button _startButton;
	private Button _optionsButton;
	private Button _quitButton;
	
	private Button _quitOptionsButton;
	
	private OptionButton _resolutionButton;

	// Liste des résolutions que l'on veut proposer
	private List<Vector2I> _resolutions = new List<Vector2I>()
	{
		new Vector2I(1920, 1080),
		new Vector2I(1600, 900),
		new Vector2I(1366, 768),
		new Vector2I(1280, 720)
	};
	
	// Optionnel : un panel qui contient tes options (à cacher/afficher)
	private Panel _optionsMenu; 
	private VBoxContainer _mainMenuButtons;
	[Export] private Control _mainMenuPanel;
	[Export] private Control _playerSelectionPanel;

	public override void _Ready()
	{
		//DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed, 0);
		
		// 1. Récupération des index des bus
		_masterBusIndex = AudioServer.GetBusIndex("Master");
		_musicBusIndex = AudioServer.GetBusIndex("Music");
		_sfxBusIndex = AudioServer.GetBusIndex("SFX");

		// 2. Récupération de tes nœuds Sliders 
		_masterSlider = GetNode<HSlider>("OptionsPanel/MusicVBoxContainer/GlobalMusicContainer/Slider");
		_musicSlider = GetNode<HSlider>("OptionsPanel/MusicVBoxContainer/SfxMusicContainer/Slider");
		_sfxSlider = GetNode<HSlider>("OptionsPanel/MusicVBoxContainer/MusicContainer/Slider");

		// 3. On initialise la position des sliders par rapport au volume actuel du jeu
		_masterSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_masterBusIndex));
		_musicSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_musicBusIndex));
		_sfxSlider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_sfxBusIndex));

		// 4. On connecte le signal de changement de valeur
		_masterSlider.ValueChanged += OnMasterSliderValueChanged;
		_musicSlider.ValueChanged += OnMusicSliderValueChanged;
		_sfxSlider.ValueChanged += OnSfxSliderValueChanged;
		
		_startButton = GetNode<Button>("ButtonMenuContainer/StartButton");
		_optionsButton = GetNode<Button>("ButtonMenuContainer/OptionsButton");
		_quitButton = GetNode<Button>("ButtonMenuContainer/QuitButton");
		
		_quitOptionsButton = GetNode<Button>("OptionsPanel/QuitOptionsButton");
		
		_mainMenuButtons = GetNode<VBoxContainer>("ButtonMenuContainer");
		_mainMenuButtons.Show();
		_optionsMenu = GetNode<Panel>("OptionsPanel");
		_optionsMenu.Hide(); // On s'assure qu'il est caché au lancement

		_startButton.Pressed += OnStartButtonPressed;
		_optionsButton.Pressed += OnOptionsButtonPressed;
		_quitButton.Pressed += OnQuitButtonPressed;
		
		_quitOptionsButton.Pressed += OnQuitOptionsButtonPressed;
		
		if(_startButton != null)
		{
			_startButton.GrabFocus();
		}
		
		_resolutionButton = GetNode<OptionButton>("OptionsPanel/ResolutionHBoxContainer/ResolutionButton");

		// 1. On vide le bouton au cas où, puis on ajoute nos options textuelles
		_resolutionButton.Clear();
		foreach (var res in _resolutions)
		{
			_resolutionButton.AddItem($"{res.X} x {res.Y}");
		}

		// 2. On connecte le signal quand le joueur change de sélection
		_resolutionButton.ItemSelected += OnResolutionSelected;
		
		// 3. Optionnel : Sélectionner par défaut la résolution actuelle de la fenêtre
		Vector2I currentSize = DisplayServer.WindowGetSize();
		int index = _resolutions.IndexOf(currentSize);
		if (index != -1) _resolutionButton.Select(index);
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
		
		_optionsMenu.Show();
		_mainMenuButtons.Hide();
		_quitOptionsButton.GrabFocus();
	}

	private void OnQuitButtonPressed()
	{
		GD.Print("Fermeture du jeu...");
		
		// La fonction officielle de Godot pour fermer l'application proprement
		GetTree().Quit();
	}
	
	private void OnQuitOptionsButtonPressed()
	{
		_optionsMenu.Hide();
		_mainMenuButtons.Show();
		_startButton.GrabFocus();
	}
	
	private void OnMasterSliderValueChanged(double value)
	{
		// On convertit la valeur du slider (0 à 1) en Décibels, puis on l'applique au bus
		float dbValue = (float)Mathf.LinearToDb(value);
		AudioServer.SetBusVolumeDb(_masterBusIndex, dbValue);
		
		_musicSlider.Value = value;
		_sfxSlider.Value = value;
	}

	private void OnMusicSliderValueChanged(double value)
	{
		if (value > _masterSlider.Value)
		{
			_musicSlider.Value = _masterSlider.Value;
			value = _musicSlider.Value; // On bride la valeur
		}
		
		float dbValue = (float)Mathf.LinearToDb(value);
		AudioServer.SetBusVolumeDb(_musicBusIndex, dbValue);
	}

	private void OnSfxSliderValueChanged(double value)
	{
		if (value > _masterSlider.Value)
		{
			_sfxSlider.Value = _masterSlider.Value;
			value = _sfxSlider.Value; // On bride la valeur
		}
		
		float dbValue = (float)Mathf.LinearToDb(value);
		AudioServer.SetBusVolumeDb(_sfxBusIndex, dbValue);
	}
	
	private void OnResolutionSelected(long index)
	{
		// On récupère le Vector2I correspondant à l'index choisi
		Vector2I selectedRes = _resolutions[(int)index];
		
		// On applique la nouvelle taille à la fenêtre de jeu
		DisplayServer.WindowSetSize(selectedRes);
		
		// Astuce : On recentre la fenêtre sur l'écran du joueur pour éviter qu'elle se décale
		Vector2I screenSize = DisplayServer.ScreenGetSize();
		DisplayServer.WindowSetPosition((screenSize - selectedRes) / 2);
	}
}
