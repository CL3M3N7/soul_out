using System;
using Godot;

namespace SoulOut.scripts.Spawner;

[GlobalClass]
public partial class EntitySpawner : Node
{
    [Export] public PackedScene Entity;
    [Export] public float IntervalleMin { get; set; } = 0.5f;        // Minimum de secondes entre deux apparitions
    [Export] public float IntervalleMax { get; set; } = 2.0f;        // Maximum de secondes entre deux apparitions
    
    private Timer _spawnTimer;
    private Random _random = new Random();
    
    private CollisionShape2D _spawnAreaShape;

    public override void _Ready()
    {
        _spawnAreaShape = GetNode<CollisionShape2D>("AreaSpawn");
        
        _spawnTimer = new Timer();
        _spawnTimer.OneShot = true; // On gère le côté aléatoire à chaque fin de cycle
        _spawnTimer.Timeout += OnSpawnTimerTimeout;
        AddChild(_spawnTimer);
        
        ChooseTimeNextSpawn();
    }
    
    private void OnSpawnTimerTimeout()
    {
        GD.Print("test");
        SpawnEntity();
        ChooseTimeNextSpawn();
    }

    private void SpawnEntity()
    {
        if (Entity == null)
        {
            GD.PrintErr("Erreur : La scène 'SpotScene' n'est pas assignée dans l'inspecteur !");
            return;
        }
        
        Node2D entityInstantiated = Entity.Instantiate<Node2D>();
        entityInstantiated.Position = GetRandomPointInShape();
        
        GetParent().AddChild(entityInstantiated);
    }

    private void ChooseTimeNextSpawn()
    {
        // Calcul d'un temps aléatoire entre les bornes min et max
        float nextSpawnTime = (float)(_random.NextDouble() * (IntervalleMax - IntervalleMin) + IntervalleMin);
        _spawnTimer.WaitTime = nextSpawnTime;
        _spawnTimer.Start();
    }
    
    private Vector2 GetRandomPointInShape()
    {
        if (_spawnAreaShape.Shape is RectangleShape2D rectShape)
        {
            Vector2 extents = rectShape.Size / 2;
            
            float randomX = (float)GD.RandRange(-extents.X, extents.X);
            float randomY = (float)GD.RandRange(-extents.Y, extents.Y);
            
            return new Vector2(randomX, randomY) + _spawnAreaShape.Position;
        }
        else if (_spawnAreaShape.Shape is CircleShape2D circleShape)
        {
            float radius = circleShape.Radius;
            float angle = (float)GD.RandRange(0, Mathf.Tau); // Mathf.Tau = 2 * PI
            
            float distance = Mathf.Sqrt((float)GD.Randf()) * radius; 
            
            float x = Mathf.Cos(angle) * distance;
            float y = Mathf.Sin(angle) * distance;
            
            return new Vector2(x, y) + _spawnAreaShape.Position;
        }

        GD.PrintErr("Forme non supportée pour le spawn aléatoire.");
        return Vector2.Zero;
    }
}