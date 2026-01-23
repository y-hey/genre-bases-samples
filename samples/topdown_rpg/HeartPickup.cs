using Godot;

namespace TopDownRpgSample;

/// <summary>
/// 回復アイテム（ハート）
/// プレイヤーが触れるとHPを回復する
/// </summary>
public partial class HeartPickup : Area2D
{
    [Export] public int HealAmount { get; set; } = 2;
    [Export] public float FloatAmplitude { get; set; } = 4.0f;
    [Export] public float FloatSpeed { get; set; } = 3.0f;

    private bool _isPickedUp;
    private float _baseY;
    private float _floatTime;
    private Tween? _spawnTween;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        // 出現アニメーション
        Scale = Vector2.Zero;
        _spawnTween = CreateTween();
        _spawnTween.TweenProperty(this, "scale", Vector2.One, 0.2f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);
    }

    public override void _Process(double delta)
    {
        if (_isPickedUp) return;

        // 出現アニメーション完了後に浮遊開始
        if (_spawnTween != null && _spawnTween.IsRunning())
        {
            return;
        }

        // 初回のみベース位置を記録
        if (_floatTime == 0)
        {
            _baseY = Position.Y;
        }

        // 浮遊アニメーション（サイン波）
        _floatTime += (float)delta * FloatSpeed;
        var offsetY = Mathf.Sin(_floatTime) * FloatAmplitude;
        Position = new Vector2(Position.X, _baseY + offsetY);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_isPickedUp) return;

        if (body is TopDownPlayer player && !player.IsDead)
        {
            _isPickedUp = true;
            player.Heal(HealAmount);

            // 取得エフェクト
            var tween = CreateTween();
            tween.TweenProperty(this, "scale", Vector2.One * 1.5f, 0.1f);
            tween.TweenProperty(this, "modulate:a", 0.0f, 0.1f);
            tween.TweenCallback(Callable.From(QueueFree));
        }
    }
}
