using Godot;
using System;

public partial class GoldSpot : Node2D
{
	[Export] public Texture2D[] Sprites { get; set; }
	private Sprite2D _spriteNode;
	public int GoldValue;
	private Random valueGenerator = new Random();
	
	public override void _Ready()
	{
		_spriteNode = GetNode<Sprite2D>("Sprite2D");
		Spawn();
	}
	
	public void Spawn()
	{
		double tmp = valueGenerator.NextDouble();
		
		if(tmp < 0.60)
		{
			GoldValue = 1;
			_spriteNode.Texture = Sprites[tmp < 0.30 ? 0 : 1];
		}
		else
		{
			if(tmp < 0.90)
			{
				GoldValue = 2;
				_spriteNode.Texture = Sprites[tmp < 0.75 ? 2 : 3];
			}
			else
			{
				GoldValue = 3;
				_spriteNode.Texture = Sprites[tmp < 0.95 ? 4 : 5];
			}
		}
	}
}
