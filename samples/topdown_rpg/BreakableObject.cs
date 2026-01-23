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

    private Area2D? _hitbox;
    private ColorRect? _visual;
    private bool _isBroken;

    public override void _Ready()
    {
        _hitbox = GetNodeOrNull<Area2D>("Hitbox");
        _visual = GetNodeOrNull<ColorRect>("Visual");

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (_visual == null) return;

        switch (Type)
        {
            case ObjectType.Grass:
                _visual.Color = new Color(0.2f, 0.7f, 0.2f);
                break;
            case ObjectType.Pot:
                _visual.Color = new Color(0.6f, 0.4f, 0.2f);
                break;
            case ObjectType.Bush:
                _visual.Color = new Color(0.1f, 0.5f, 0.1f);
                break;
        }
    }

    public void Break()
    {
        if (_isBroken) return;
        _isBroken = true;

        // アイテムドロップ
        if (GD.Randf() < HeartDropChance)
        {
            SpawnHeart();
        }

        // 破壊エフェクト
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.15f);
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void SpawnHeart()
    {
        var heartScene = GD.Load<PackedScene>("res://samples/topdown_rpg/HeartPickup.tscn");
        if (heartScene != null)
        {
            var heart = heartScene.Instantiate<Node2D>();
            heart.GlobalPosition = GlobalPosition;
            GetParent().CallDeferred("add_child", heart);
        }
    }
}
