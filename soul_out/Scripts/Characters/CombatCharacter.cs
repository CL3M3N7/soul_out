using Godot;
using System;

public partial class CombatCharacter : Character
{
	public int Health {get ; private set;}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Health = 3;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(/*"Attack"*/))
		{
			Attack();
		}
	}
	
	public void Attack()
	{
		//put attack logic
	}
	
	public void TakeDamage()
	{
		Health -= 1;
		if(Health <= 0)
		{
			//Maybe put a death event ?
			Die();
		}
	}
	
	private void Die()
	{
		//Death logic
	}
}
