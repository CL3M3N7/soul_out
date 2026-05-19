using Godot;

public partial class SOCharacter : CharacterBody2D
{
	[ExportGroup("Character Parameter")]
	[Export]
	public int PlayerController { get; set; } = 0;
	
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D _animatedSprite2D = null;

	public override void _Ready()
	{
		_animatedSprite2D = GetNode<CharacterSprite>("Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
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
			velocity.X = 0;
			velocity.Y = 0;
		}
		
		if (_animatedSprite2D != null)
		{
			_animatedSprite2D.FlipH = velocity.X switch
			{
				> 0 => false,
				< 0 => true,
				_ => _animatedSprite2D.FlipH
			};

			_animatedSprite2D.Play(velocity != Vector2.Zero ? "run" : "idle");
		}
		else
		{
			GD.PrintErr("[Character] No animated sprite attached!");
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
}