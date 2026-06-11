using Godot;
using System;

public partial class TrialMusicSafeZone : Area2D
{
	public bool _isOccupied = false;
	
	public void onBodyEntered(Node2D body)
	{
		if(!_isOccupied)
		{
			if (body is MusicChairCharacter character)
			{
				GD.Print($"Tu es safe {character}");
				character.isSafe = true;
				_isOccupied = true;
			}
		}
	}
	
	public void onBodyExited(Node2D body)
	{
		if(_isOccupied)
		{
			if (body is MusicChairCharacter character)
			{
				GD.Print($"Tu n'es plus safe {character}");
				character.isSafe = false;
				_isOccupied = false;
			}
		}
	}
}
