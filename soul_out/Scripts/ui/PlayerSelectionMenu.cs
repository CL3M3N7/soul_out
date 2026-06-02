using Godot;
using System;
using SoulOut.Scripts.Manager;

public partial class PlayerSelectionMenu : Control
{
	private Button _startButton;
	
	private Control[] _slots = new Control[4];
	private Label[] _statusLabels = new Label[4];
	private TextureRect[] _playerSprites = new TextureRect[4]; 
	
	private bool[] _playerJoined = new bool[4];

	public override void _Ready()
	{
		GD.Print("[SELECTION] --- INITIALISATION DU MENU DE SELECTION ---");
		GameManager.Instance.NumberOfPlayers = 0;

		_startButton = GetNode<Button>("StartButton");
		if (_startButton == null) GD.PrintErr("[SELECTION] ERREUR: StartButton introuvable !");
		
		_startButton.Disabled = true;
		_startButton.Pressed += OnStartPressed;

		for (int i = 0; i < 4; i++)
		{
			string panelPath = $"HBoxContainer/PlayerPanel{i}/Slot";
			_slots[i] = GetNode<Control>(panelPath);
			
			if (_slots[i] == null)
			{
				GD.PrintErr($"[SELECTION] ERREUR: Le slot {i} à l'adresse '{panelPath}' est introuvable !");
				continue;
			}

			_statusLabels[i] = _slots[i].GetNode<Label>("StatusLabel");
			_playerSprites[i] = _slots[i].GetNode<TextureRect>("PlayerSprite");
			
			if (_playerSprites[i] != null)
			{
				GD.Print($"[SELECTION] Tentative de cacher le sprite du Joueur {i+1}...");
				_playerSprites[i].Hide(); 
				GD.Print($"[SELECTION] Sprite du Joueur {i+1} est-il visible ? -> {_playerSprites[i].Visible}");
			}
			else
			{
				GD.PrintErr($"[SELECTION] ERREUR: PlayerSprite introuvable dans le slot {i} !");
			}

			_playerJoined[i] = false;
		}
		GD.Print("[SELECTION] --- FIN DE L'INITIALISATION ---");
	}

	public override void _Input(InputEvent @event)
	{

		for (int i = 0; i < 4; i++)
		{
			string actionName = $"SOActionButton0_{i}";
			
			// On vérifie si l'action existe bien dans les paramètres du projet
			if (!InputMap.HasAction(actionName))
			{
				GD.PrintErr($"[INPUT ERREUR] L'action '{actionName}' n'existe pas dans ta Configuration du Projet !");
				continue;
			}

			if (@event.IsActionPressed(actionName))
			{
				GD.Print($"[INPUT REUSSI] La manette {i} a appuyé sur {actionName} !");
				JoinPlayer(i);
			}
		}
	}

	private void JoinPlayer(int playerIndex)
	{
		GD.Print($"[JOIN] Tentative d'ajout du joueur {playerIndex}. Déjà joint ? {_playerJoined[playerIndex]}");
		if (_playerJoined[playerIndex] == true) return;
		
		_playerJoined[playerIndex] = true;
		GameManager.Instance.NumberOfPlayers += 1;
		GD.Print($"[JOIN] Nombre total de joueurs enregistrés : {GameManager.Instance.NumberOfPlayers}");

		if (_statusLabels[playerIndex] != null)
			_statusLabels[playerIndex].Text = $"Joueur {playerIndex + 1}\nPrêt !";
			
		if (_playerSprites[playerIndex] != null)
			_playerSprites[playerIndex].Show();
		
		if (GameManager.Instance.NumberOfPlayers >= 2) 
		{
			GD.Print("[JOIN] Assez de joueurs ! Activation du bouton Start.");
			_startButton.Disabled = false;
			
			if (playerIndex == 0) 
			{
				GD.Print("[JOIN] Focus donné au bouton Start pour le Joueur 1.");
				_startButton.GrabFocus();
			}
		}
	}

	private void OnStartPressed()
	{
		GD.Print("[START] Bouton Start cliqué ! Changement de scène...");
		SceneManager.Instance.ChangeScene();
	}
}
