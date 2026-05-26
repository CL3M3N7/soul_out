using Godot;
using System;

public partial class Fireball : Area2D
{
	[Export] public float Speed = 350f;
	[Export] public int Damage = 1;
	[Export] public float Lifetime = 4.0f;

	public Vector2 Direction = Vector2.Right; // Direction par défaut, sera modifiée par le spawner
	
	private AudioStreamPlayer2D _sfxExplosion;

	public override void _Ready()
	{
		// On connecte le signal de collision à notre fonction locale
		AreaEntered += OnAreaEntered;
		
		// Optionnel : si ton sprite est orienté vers la droite, on l'oriente vers sa direction de voyage
		Rotation = Direction.Angle();
		
		_sfxExplosion = GetNode<AudioStreamPlayer2D>("SfxExplosion");
		
		GetTree().CreateTimer(Lifetime).Timeout += () => DestroyFireball(false);
	}

	public override void _PhysicsProcess(double delta)
	{
		// Avancement rectiligne uniforme
		GlobalPosition += Direction * Speed * (float)delta;
	}

	private void OnAreaEntered(Area2D area)
	{
		// Détection du joueur (on vérifie la FeetArea comme pour tes geysers)
		if (area.Name == "FeetArea" && area.GetParent() is CombatCharacter joueur)
		{
			// On inflige les dégâts en passant 'this' (la boule de feu) comme attaquant pour déclencher le KNOCKBACK !
			joueur.TakeDamage(Damage, this);
			
			// La boule de feu explose/s'autodétruit après avoir touché un joueur
			DestroyFireball(true);
		}
		// Si elle touche un mur ou un élément de décor qui est une Area2D (ex: les bords de la map)
		else if (area.Name == "MapBounds" || area.Name == "WallArea") 
		{
			DestroyFireball(false);
		}
	}

	private void DestroyFireball(bool IsAPlayer = false)
{
	if (GodotObject.IsInstanceValid(this))
	{
		// --- SÉCURITÉ AUDIO POUR L'EXPLOSION ---
		if (_sfxExplosion != null && !_sfxExplosion.Playing && _sfxExplosion.Stream != null && IsAPlayer)
		{
			// 1. On cache le visuel et on coupe les collisions pour ne pas re-blesser quelqu'un
			Hide();
			SetDeferred("monitoring", false);
			SetDeferred("monitorable", false);
			SetPhysicsProcess(false); // On arrête de faire avancer la boule

			// 2. On joue le son
			_sfxExplosion.Play();

			// 3. On attend que le son se termine proprement avant de supprimer définitivement le nœud
			_sfxExplosion.Finished += () => QueueFree();
		}
		else
		{
			// Si pas de son configuré, on supprime direct
			QueueFree();
		}
	}
}

	// Sécurité : Si la boule de feu rate tout le monde et sort de l'écran, on la supprime pour éviter de surcharger la RAM
	private void _on_visible_on_screen_notifier_2d_screen_exited()
	{
		QueueFree();
	}
}
