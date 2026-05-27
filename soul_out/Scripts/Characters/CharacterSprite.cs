using Godot;
using SoulOut.Scripts.Core;

public partial class CharacterSprite : AnimatedSprite2D
{
	[ExportCategory("Player One (Blue)")]
	[Export] public SpriteFrames BlueWarrior;
	[Export] public SpriteFrames BlueArcher;
	[Export] public SpriteFrames BluePawn;
	
	[ExportCategory("Player Two (Red)")]
	[Export] public SpriteFrames RedWarrior;
	[Export] public SpriteFrames RedArcher;
	[Export] public SpriteFrames RedPawn;
	
	[ExportCategory("Player Three (Yellow)")]
	[Export] public SpriteFrames YellowWarrior;
	[Export] public SpriteFrames YellowArcher;
	[Export] public SpriteFrames YellowPawn;
	
	[ExportCategory("Player Four (Purple)")]
	[Export] public SpriteFrames PurpleWarrior;
	[Export] public SpriteFrames PurpleArcher;
	[Export] public SpriteFrames PurplePawn;
	
	public CharacterSprite(){}
	
	public override void _Ready()
	{
		int playerController = GetParent<SOCharacter>().PlayerController;
		TypeCharacter typeCharacter = GetParent<SOCharacter>().TypeCharacter;
		ChangeSprite(playerController,typeCharacter);
	}
	
	public override void _Process(double delta)
	{
	}

	public void ChangeSprite(int playerControllerNumber, TypeCharacter type)
	{
		switch (playerControllerNumber, type)
		{
			case (0, TypeCharacter.Warrior):
				this.SpriteFrames = BlueWarrior;
				break;
			case (1, TypeCharacter.Warrior):
				this.SpriteFrames = RedWarrior;
				break;
			case (2, TypeCharacter.Warrior):
				this.SpriteFrames = YellowWarrior;
				break;
			case (3, TypeCharacter.Warrior):
				this.SpriteFrames = PurpleWarrior;
				break;
			
			case (0, TypeCharacter.Archer):
				this.SpriteFrames = BlueArcher;
				break;
			case (1, TypeCharacter.Archer):
				this.SpriteFrames = RedArcher;
				break;
			case (2, TypeCharacter.Archer):
				this.SpriteFrames = YellowArcher;
				break;
			case (3, TypeCharacter.Archer):
				this.SpriteFrames = PurpleArcher;
				break;
			
			case (0, TypeCharacter.Pawn):
				this.SpriteFrames = BluePawn;
				break;
			case (1, TypeCharacter.Pawn):
				this.SpriteFrames = RedPawn;
				break;
			case (2, TypeCharacter.Pawn):
				this.SpriteFrames = YellowPawn;
				break;
			case (3, TypeCharacter.Pawn):
				this.SpriteFrames = PurplePawn;
				break;
			
			default:
				break;
		}
	}
}
