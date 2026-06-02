using Godot;
using System;

namespace SoulOut.Scripts.Characters;

public partial class SpellCastCharacter : SOCharacter
{
	public int SpellScore { get; private set; }= 0;
	[Export] public AnimatedSprite2D _spellsprite;
	[Signal] public delegate void SpellCastedEventHandler(int newScore);
	
	public override void _Ready()
	{
		_spellsprite.Play();
	}
	
	public override void _PhysicsProcess(double delta)
	{}
	
	public override void _UnhandledInput(InputEvent @event)
	{
		Velocity = Vector2.Zero;
		if (@event.IsActionPressed($"SOActionButton0_{PlayerController}"))
		{
			SpellScore ++;
			_spellsprite.SpeedScale = SpellScore;
		}
	}
}
