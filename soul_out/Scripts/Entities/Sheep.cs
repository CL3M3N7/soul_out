using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace SoulOut.scripts.Entities;

public partial class Sheep : CharacterBody2D
{
	[Export] public AnimatedSprite2D Sprite;
	[Export] public Area2D PlayerDetectorArea;


	private Array<SOCharacter> _detectedCharacters = new Array<SOCharacter>();
	bool is_fleeing = false;

	public override void _PhysicsProcess(double delta)
	{
		if (is_fleeing)
		{
			FleePlayers();
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		
		Sprite.FlipH = Velocity.X switch
		{
			> 0 => false,
			< 0 => true,
			_ => Sprite.FlipH
		};
		Sprite.Play(Velocity != Vector2.Zero ? "move" : "idle");
		
		MoveAndSlide();
	}
	
	public void OnPlayerDetectorAreaEnter(Area2D body)
	{
		if (body.GetParent() is SOCharacter character)
		{
			_detectedCharacters.Add(character);
			is_fleeing = true;
		}
	}

	public void OnPlayerDetectorAreaExit(Area2D body)
	{
		if (body.GetParent() is SOCharacter character)
		{
			_detectedCharacters.Remove(character);
			if (_detectedCharacters.Count == 0) is_fleeing = false;
		}
	}

	public void FleePlayers()
	{
		Vector2 fleeingDirection = Vector2.Zero;
		
		foreach (var character in _detectedCharacters)
		{
			Vector2 nextDirection = Position - character.Position;
			fleeingDirection += nextDirection / (nextDirection.LengthSquared()/(150*150));
		}
		
		float length =  Mathf.Clamp(fleeingDirection.Length(),0,800);
		
		fleeingDirection = fleeingDirection.Normalized();
		Velocity = fleeingDirection * length;
	}
}
