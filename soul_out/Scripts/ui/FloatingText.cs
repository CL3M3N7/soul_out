using Godot;
using System;

public partial class FloatingText : Label
{
	[Export] public float Speed = 50f; // Vitesse de montée du texte
	private Vector2 _velocity = Vector2.Up;

	public override void _Ready()
	{
		AnimationPlayer animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animPlayer.Play("up_and_pop");
		animPlayer.AnimationFinished += OnAnimationFinished;
	}

	public override void _Process(double delta)
	{
		Position += _velocity * Speed * (float)delta;
	}

	private void OnAnimationFinished(StringName animName)
	{
		QueueFree();
	}
}
