using Godot;
using System;

public partial class GlobalZoneLava : Area2D
{
	private void _on_area_entered(Area2D area)
	{
		// On vérifie si l'Area qui touche la lave s'appelle bien "FeetArea"
		// et on récupère le parent (le Character) pour lui appliquer la chute
		if (area.Name == "FeetArea" && area.GetParent() is SOFightingCharacter joueur)
		{
			GD.Print($"Les pieds du joueur {joueur.PlayerController} ont touché la lave !");
			joueur.TomberDansLaLave(); // On appellera cette fonction juste après
		}
	}
}
