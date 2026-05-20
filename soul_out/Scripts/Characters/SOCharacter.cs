using Godot;

public partial class SOCharacter : CharacterBody2D
{
	[ExportGroup("SOCharacter Parameter")]
	[Export]
	public int PlayerController { get; set; } = 0;

	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	public AnimatedSprite2D CharacterSprite = null;

	public override void _Ready()
	{
		CharacterSprite = GetNode<CharacterSprite>("Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (CharacterSprite != null)
		{
			CharacterSprite.FlipH = Velocity.X switch
			{
				> 0 => false,
				< 0 => true,
				_ => CharacterSprite.FlipH
			};
			CharacterSprite.Play(Velocity != Vector2.Zero ? "run" : "idle");
		}
		else
		{
			GD.PrintErr("[Character] No animated sprite attached!");
		}

		MoveAndSlide();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		Vector2 direction = Input.GetVector(
			$"SOMoveLeft_{PlayerController}", $"SOMoveRight_{PlayerController}",
			$"SOMoveUp_{PlayerController}", $"SOMoveDown_{PlayerController}"
		);

		if (direction != Vector2.Zero)
		{
			Velocity = direction * Speed;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
	}
}
