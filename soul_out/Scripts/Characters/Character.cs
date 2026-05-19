using Godot;

public partial class Character : CharacterBody2D
{
	[ExportGroup("Character Parameter")]
	[Export]
	public int PlayerController { get; set; } = 0;
	
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Handle Jump.
		/*if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
	{
		velocity.Y = JumpVelocity;
	}*/
		
		Vector2 direction = Input.GetVector(
			$"SOMoveLeft_{PlayerController}", $"SOMoveRight_{PlayerController}",
			$"SOMoveUp_{PlayerController}", $"SOMoveDown_{PlayerController}"
		);
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
