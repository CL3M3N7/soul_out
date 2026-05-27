using Godot;
using System;

public partial class BladeArea : Area2D
{
	private void _on_area_entered(Area2D area)
	{
		// On vérifie si l'Area qui touche la lave s'appelle bien "FeetArea"
		// et on récupère le parent (le Character) pour lui appliquer la chute
		if (area.Name == "DamageArea" && area.GetParent() is SOFightingCharacter joueur)
		{
			GD.Print($"Le joueur {joueur.PlayerController} a été touché !");
			joueur.TakeDamage(1, this); // On appellera cette fonction juste après
		}
	}
}
