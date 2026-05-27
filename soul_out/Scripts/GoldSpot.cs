using Godot;
using System;

public partial class GoldSpot : Node2D
{
	public int GoldValue;
	private Random valueGenerator = new Random();
	
	public override void _Ready()
	{
		Spawn();
	}
	
	public void Spawn()
	{
		double tmp = valueGenerator.NextDouble();
		if(tmp < 0.50)
		{
			GoldValue = 1;
			//sprite associe
		}
		else
		{
			if(tmp < 0.85)
			{
				GoldValue = 2;
				//sprite associe
			}
			else
			{
				GoldValue = 3;
				//sprite associe
			}
		}
	}
	public void Collected()
	{
		QueueFree();
	}
}
