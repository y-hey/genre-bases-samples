using Godot;

namespace TopDownRpgSample;

/// <summary>
/// 壊せるオブジェクト（草、壺など）
/// プレイヤーの攻撃で破壊され、アイテムをドロップする可能性がある
/// </summary>
public partial class BreakableObject : StaticBody2D
{
    public enum ObjectType
    {
        Grass,
        Pot,
        Bush
    }

    [Export] public ObjectType Type { get; set; } = ObjectType.Grass;
    [Export] public float HeartDropChance { get; set; } = 0.2f;

    // 色定数（GC対策）
    private static readonly Color GrassColor = new(0.2f, 0.7f, 0.2f);
    private static readonly Color PotColor = new(0.6f, 0.4f, 0.2f);
    private static readonly Color BushColor = new(0.1f, 0.5f, 0.1f);

    private Area2D? _hitbox;
    private ColorRect? _visual;
    private bool _isBroken;
    private Vector2 _spawnPosition;

    public override void _Ready()
    {
        _hitbox = GetNodeOrNull<Area2D>("Hitbox");
        _visual = GetNodeOrNull<ColorRect>("Visual");

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (_visual == null) return;

        _visual.Color = Type switch
        {
            ObjectType.Grass => GrassColor,
            ObjectType.Pot => PotColor,
            ObjectType.Bush => BushColor,
            _ => GrassColor
        };
    }

    public void Break()
    {
        if (_isBroken) return;
        _isBroken = true;

        // アイテムドロップ（位置を先に記録）
        if (GD.Randf() < HeartDropChance)
        {
            _spawnPosition = GlobalPosition;
            CallDeferred(nameof(SpawnHeart));
        }

        // 破壊エフェクト
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.15f);
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void SpawnHeart()
    {
        var heartScene = GD.Load<PackedScene>("res://samples/topdown_rpg/HeartPickup.tscn");
        if (heartScene == null) return;

        var heart = heartScene.Instantiate<Node2D>();
        GetParent().AddChild(heart);
        heart.GlobalPosition = _spawnPosition;
    }
}
