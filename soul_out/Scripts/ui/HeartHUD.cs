using Godot;
using System;

public partial class HeartHUD : HBoxContainer
{
	// On exporte les textures pour pouvoir les changer facilement ou changer de couleur par joueur
	[Export] public Texture2D HeartFull;
	[Export] public Texture2D HeartEmpty;

	/// <summary>
	/// Met à jour l'affichage des cœurs selon les points de vie actuels.
	/// </summary>
	public void OnPlayerHealthChanged(int currentHealth)
	{
		// On parcourt tous les enfants du conteneur (les TextureRect)
		for (int i = 0; i < GetChildCount(); i++)
		{
			if (GetChild(i) is TextureRect heart)
			{
				// Si l'index de l'enfant est inférieur aux PV restants, le cœur est plein, sinon vide.
				if (i < currentHealth)
				{
					heart.Texture = HeartFull;
				}
				else
				{
					heart.Texture = HeartEmpty;
				}
			}
		}
	}
}
