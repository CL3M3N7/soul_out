using Godot;
using System;

public partial class ScoreUI : VBoxContainer
{
	[Export] public Texture2D Avatar0;
	[Export] public Texture2D Avatar1;
	[Export] public Texture2D Avatar2;
	[Export] public Texture2D Avatar3;
	
	public void SetAvatarAndScore(int avatar, int score)
	{
		if(!(GetChild(0) is TextureRect avatarTexture)) return;
		if(avatar == 0)
		{
			avatarTexture.Texture = Avatar0;
		}
		if(avatar == 1)
		{
			avatarTexture.Texture = Avatar1;
		}
		if(avatar == 2)
		{
			avatarTexture.Texture = Avatar2;
		}
		if(avatar == 3)
		{
			avatarTexture.Texture = Avatar3;
		}
		else
		{
			GD.PrintErr("This texture does not exist !");
		}
		
		if(!(GetChild(1) is Label scoreLabel)) return;
		scoreLabel.Text = score.ToString();
	}
}
