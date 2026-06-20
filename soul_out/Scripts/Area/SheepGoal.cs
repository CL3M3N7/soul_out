using Godot;
using System;
using Godot.Collections;
using SoulOut.Scripts.Characters;
using SoulOut.scripts.Entities;
using SoulOut.Scripts.Manager;

public partial class SheepGoal : Area2D
{
	[Export] public int IndexPlayer;
	[Export] public Node PlayerNode;
	
	[Export] public PackedScene FloatingTextScene { get; set; }

	public override void _Ready()
	{
		// 1. On vérifie si ce script est bien lu par le jeu au lancement
		GD.Print("--- DÉMARRAGE DU SHEEPGOAL ---");
		GD.Print($"Nom du noeud : {Name}");
		
		// 2. On vérifie que la zone n'est pas désactivée
		GD.Print($"Est-ce que l'Area2D écoute ? Monitoring = {Monitoring}");
		
		// 3. On vérifie les masques de collision (c'est souvent là que l'éditeur Godot nous ment)
		GD.Print($"Masque de collision de l'Area2D : {CollisionMask}");


		GetNode<Sprite2D>("House").Visible = GameManager.Instance.NumberOfPlayers > IndexPlayer;
		
		BodyEntered += OnAreaEnter;
	}
	
	public void OnAreaEnter(Node2D body)
	{
		GD.Print($"[SUCCÈS] Quelque chose est entré : {body.Name}");
	   
		// ATTENTION : J'ai corrigé cette ligne ! 
		// body.GetParent() ne marchera pas si le 'body' EST le Sheep.
		// Si ton Sheep est le CharacterBody2D, on vérifie directement :
		if (body is Sheep sheep)
		{
			GD.Print("Le mouton est validé !");
			Array<Node> children = PlayerNode.GetChildren();
			foreach (Node child in children)
			{
				GD.Print(child.Name);
				if (child is SOTrialCharacter character && character.PlayerController == IndexPlayer)
				{
					character.Score += 1;
					
					FloatingText popup = FloatingTextScene.Instantiate<FloatingText>();
					switch(IndexPlayer)
					{
						case 0:
							popup.Modulate = Colors.Blue;
							break;
						case 1:
							popup.Modulate = Colors.Red;
							break;
						case 2:
							popup.Modulate = Colors.Yellow;
							break;
						case 3:
							popup.Modulate = Colors.Purple;
							break;
						default:
							break;
					}
					popup.SetText($"+1");
					popup.GlobalPosition = new Vector2(-20, -80);
					AddChild(popup);
					
					sheep.QueueFree();
				}
			}
		}
		else 
		{
			GD.Print("L'objet entré n'est pas un Sheep.");
		}
	}
}
