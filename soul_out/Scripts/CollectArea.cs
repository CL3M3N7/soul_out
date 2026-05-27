using Godot;
using System;

public partial class CollectArea : Area2D
{
	private void _on_area_entered(Area2D area)
	{
		// On vérifie si l'Area qui touche s'appelle bien "GoldArea"
		if (area.Name == "GoldArea" && area.GetParent() is GoldSpot spot)
		{
			GetParentOrNull<CollectCharacter>().CollectGold(spot.GoldValue);
			spot.QueueFree();
		}
	}
}
