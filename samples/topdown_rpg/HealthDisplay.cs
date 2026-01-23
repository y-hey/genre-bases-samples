using Godot;

namespace TopDownRpgSample;

/// <summary>
/// HP表示UI
/// ハートアイコンでプレイヤーのHPを表示
/// </summary>
public partial class HealthDisplay : HBoxContainer
{
    [Export] public TopDownPlayer? Player { get; set; }

    private const int HeartSize = 32;
    private const int HeartSpacing = 4;

    public override void _Ready()
    {
        if (Player != null)
        {
            Player.HealthChanged += OnHealthChanged;
            UpdateHearts(Player.CurrentHealth, Player.MaxHealth);
        }
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        UpdateHearts(currentHealth, maxHealth);
    }

    private void UpdateHearts(int currentHealth, int maxHealth)
    {
        // 既存のハートをクリア
        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        // ハートを2つ分（1ハート = HP2）で表示
        var heartCount = maxHealth / 2;
        var fullHearts = currentHealth / 2;
        var halfHeart = currentHealth % 2 == 1;

        for (int i = 0; i < heartCount; i++)
        {
            var heartContainer = new Control();
            heartContainer.CustomMinimumSize = new Vector2(HeartSize, HeartSize);

            var heart = new ColorRect();
            heart.Size = new Vector2(HeartSize - 4, HeartSize - 4);
            heart.Position = new Vector2(2, 2);

            if (i < fullHearts)
            {
                // 満タンハート
                heart.Color = new Color(1.0f, 0.2f, 0.2f);
            }
            else if (i == fullHearts && halfHeart)
            {
                // 半分ハート
                heart.Color = new Color(1.0f, 0.5f, 0.5f);
            }
            else
            {
                // 空ハート
                heart.Color = new Color(0.3f, 0.3f, 0.3f);
            }

            heartContainer.AddChild(heart);
            AddChild(heartContainer);
        }
    }
}
