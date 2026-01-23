using Godot;

namespace TopDownRpgSample;

/// <summary>
/// 回復アイテム（ハート）
/// プレイヤーが触れるとHPを回復する
/// </summary>
public partial class HeartPickup : Area2D
{
    [Export] public int HealAmount { get; set; } = 2;

    private bool _isPickedUp;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        // 出現アニメーション
        Scale = Vector2.Zero;
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One, 0.2f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);

        // ゆらゆら浮遊アニメーション
        var floatTween = CreateTween();
        floatTween.SetLoops();
        floatTween.TweenProperty(this, "position:y", Position.Y - 5, 0.5f)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine);
        floatTween.TweenProperty(this, "position:y", Position.Y + 5, 0.5f)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (_isPickedUp) return;

        if (body is TopDownPlayer player)
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
