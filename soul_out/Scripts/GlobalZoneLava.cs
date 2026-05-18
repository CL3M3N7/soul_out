using Godot;
using System;

public partial class ZoneLaveGlobale : Area2D
{
	private void _on_body_entered(Node2D body)
	{
		// Check if it's a player who collides with the lavaZone
		if (body is Character joueur)
		{
			GD.Print($"Le joueur {joueur.Name} est tombé dans la lave !");
			
			// TODO: Implement death or hp lost by falling in lava
		}
	}
}
