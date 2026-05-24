using Godot;
using System;

public partial class GameCamera : Camera2D
{
	private FastNoiseLite _noise = new FastNoiseLite();
	private double _shakeTimer = 0.0;
	private float _shakeIntensity = 0.0f;

	public override void _Ready()
	{
		// On configure le bruit (Noise) pour un tremblement naturel
		// et non pas juste de l'aléatoire pur.
		_noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
		_noise.Seed = (int)GD.Randi();
		_noise.Frequency = 0.1f;
	}

	public override void _Process(double delta)
	{
		if (_shakeTimer > 0)
		{
			_shakeTimer -= delta;

			// On crée un offset aléatoire mais lisse grâce au noise
			// et on l'applique sur l'offset de la caméra.
			Vector2 offset = new Vector2(
				(float)_noise.GetNoise2D(_shakeIntensity * 1000, (float)_shakeTimer * 500),
				(float)_noise.GetNoise2D(_shakeIntensity * 1000 + 50, (float)_shakeTimer * 500 + 50)
			);

			// On applique l'offset par rapport à l'intensité voulue
			Offset = offset * _shakeIntensity;

			// On fait diminuer l'intensité petit à petit pour que ça s'arrête en douceur
			_shakeIntensity = Mathf.MoveToward(_shakeIntensity, 0, (float)delta * 5.0f);
		}
		else
		{
			// Reset de l'offset si on ne shake plus
			Offset = Vector2.Zero;
		}
	}

	/// <summary>
	/// Lance un tremblement de caméra.
	/// </summary>
	/// <param name="intensity">La force du shake en pixels.</param>
	/// <param name="duration">La durée du shake en secondes.</param>
	public void Shake(float intensity = 5.0f, double duration = 0.2)
	{
		GD.Print($"[Camera] Shake lancé ! Intensité: {intensity}, Durée: {duration}");
		_shakeIntensity = intensity;
		_shakeTimer = duration;
	}
}
