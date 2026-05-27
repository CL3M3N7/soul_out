using Godot;
using System;

public partial class GoldHUD : Label
{
	public void OnPlayerGoldChanged(int newGold)
	{
		Text = newGold.ToString();
	}
}
