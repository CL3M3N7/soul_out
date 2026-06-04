using Godot;
using System;

public partial class ScoreUI : VBoxContainer
{
	[Export] public Texture2D Avatar0;
	[Export] public Texture2D Avatar1;
	[Export] public Texture2D Avatar2;
	[Export] public Texture2D Avatar3;
	
	[Export] public Texture2D Ribbon0;
	[Export] public Texture2D Ribbon1;
	[Export] public Texture2D Ribbon2;
	[Export] public Texture2D Ribbon3;
	
	public void SetAvatarAndScore(int avatar, int score)
	{
		if(!(GetChild(0) is TextureRect avatarTexture)) return;
		if(!(GetChild(1) is TextureRect ribbonTexture)) return;
		if(avatar == 0)
		{
			avatarTexture.Texture = Avatar0;
			ribbonTexture.Texture = Ribbon0;
		}
		if(avatar == 1)
		{
			avatarTexture.Texture = Avatar1;
			ribbonTexture.Texture = Ribbon1;
		}
		if(avatar == 2)
		{
			avatarTexture.Texture = Avatar2;
			ribbonTexture.Texture = Ribbon2;
		}
		if(avatar == 3)
		{
			avatarTexture.Texture = Avatar3;
			ribbonTexture.Texture = Ribbon3;
		}
		else
		{
			GD.PrintErr("This texture does not exist !");
		}
		
		if(!(GetChild(2) is Label scoreLabel)) return;
		scoreLabel.Text = score.ToString();
	}
}
