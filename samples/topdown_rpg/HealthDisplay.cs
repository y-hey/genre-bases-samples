using Godot;

namespace TopDownRpgSample;

/// <summary>
/// HP表示UI
/// ハートアイコンでプレイヤーのHPを表示
/// GC負荷を避けるため、ノードを事前生成して再利用
/// </summary>
public partial class HealthDisplay : HBoxContainer
{
    [Export] public TopDownPlayer? Player { get; set; }

    private const int HeartSize = 32;
    private const int MaxHearts = 10;

    // 事前生成したハートノードをキャッシュ（GC対策）
    private readonly Control[] _heartContainers = new Control[MaxHearts];
    private readonly ColorRect[] _heartRects = new ColorRect[MaxHearts];
    private int _currentHeartCount;

    // 色定数（毎回new Colorしない）
    private static readonly Color FullHeartColor = new(1.0f, 0.2f, 0.2f);
    private static readonly Color HalfHeartColor = new(1.0f, 0.5f, 0.5f);
    private static readonly Color EmptyHeartColor = new(0.3f, 0.3f, 0.3f);

    public override void _Ready()
    {
        // ハートノードを事前生成
        for (int i = 0; i < MaxHearts; i++)
        {
            var container = new Control();
            container.CustomMinimumSize = new Vector2(HeartSize, HeartSize);
            container.Visible = false;

            var heart = new ColorRect();
            heart.Size = new Vector2(HeartSize - 4, HeartSize - 4);
            heart.Position = new Vector2(2, 2);
            heart.Color = EmptyHeartColor;

            container.AddChild(heart);
            AddChild(container);

            _heartContainers[i] = container;
            _heartRects[i] = heart;
        }

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
        var heartCount = maxHealth / 2;
        var fullHearts = currentHealth / 2;
        var halfHeart = currentHealth % 2 == 1;

        // 必要な数だけ表示し、色を更新
        for (int i = 0; i < MaxHearts; i++)
        {
            if (i < heartCount)
            {
                _heartContainers[i].Visible = true;

                if (i < fullHearts)
                {
                    _heartRects[i].Color = FullHeartColor;
                }
                else if (i == fullHearts && halfHeart)
                {
                    _heartRects[i].Color = HalfHeartColor;
                }
                else
                {
                    _heartRects[i].Color = EmptyHeartColor;
                }
            }
            else
            {
                _heartContainers[i].Visible = false;
            }
        }

        _currentHeartCount = heartCount;
    }
}
