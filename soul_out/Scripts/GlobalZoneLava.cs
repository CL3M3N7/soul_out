using Godot;
using System;

public partial class GlobalZoneLava : Area2D
{
	public override void _Ready()
	{
		BodyEntered += OnPlayerFall;
	}
	
	private void OnPlayerFall(Node2D body)
	{
		// Check if it's a player who collides with the lavaZone
		if (body is SOCharacter joueur)
		{
			GD.Print($"Le joueur {joueur.Name} est tombé dans la lave !");
			
			// TODO: Implement death or hp lost by falling in lava
			
			CombatScene combatScene = GetParent().GetParentOrNull<CombatScene>();
			if(combatScene != null)
			{
				combatScene.LastPlayerReamin();//combatScene.OnPlayerDeath();
			}
		}
	}
}
