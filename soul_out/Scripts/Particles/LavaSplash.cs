using Godot;
using System;

public partial class LavaSplash : AnimatedSprite2D
{
	public override void _Ready()
	{
		// On lance l'animation dès que l'effet apparaît
		Play("default");
		
		// On connecte le signal de fin d'animation à notre fonction de destruction
		AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished()
	{
		// On déconnecte le signal par sécurité et on détruit le nœud
		AnimationFinished -= OnAnimationFinished;
		QueueFree();
	}
}
