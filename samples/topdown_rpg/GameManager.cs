using Godot;

namespace TopDownRpgSample;

/// <summary>
/// ゲーム全体の管理クラス
/// プレイヤーの状態監視、ゲームオーバー処理など
/// </summary>
public partial class GameManager : Node
{
    [Export] public TopDownPlayer? Player { get; set; }
    [Export] public Control? GameOverScreen { get; set; }
    [Export] public Label? EnemyCountLabel { get; set; }

    private int _totalEnemies;
    private int _defeatedEnemies;

    public override void _Ready()
    {
        if (Player != null)
        {
            Player.PlayerDied += OnPlayerDied;
        }

        // 敵の数をカウント
        CallDeferred(nameof(SetupEnemies));

        if (GameOverScreen != null)
        {
            GameOverScreen.Visible = false;
        }
    }

    private void SetupEnemies()
    {
        var enemies = GetTree().GetNodesInGroup("enemies");
        _totalEnemies = enemies.Count;
        _defeatedEnemies = 0;

        foreach (var node in enemies)
        {
            if (node is TopDownEnemy enemy)
            {
                enemy.EnemyDied += OnEnemyDied;
            }
        }

        UpdateEnemyCount();
    }

    private void OnPlayerDied()
    {
        ShowGameOver();
    }

    private void OnEnemyDied(TopDownEnemy enemy)
    {
        _defeatedEnemies++;
        UpdateEnemyCount();

        if (_defeatedEnemies >= _totalEnemies)
        {
            GD.Print("全ての敵を倒した！");
        }
    }

    private void UpdateEnemyCount()
    {
        if (EnemyCountLabel != null)
        {
            var remaining = _totalEnemies - _defeatedEnemies;
            EnemyCountLabel.Text = $"敵: {remaining}";
        }
    }

    private void ShowGameOver()
    {
        if (GameOverScreen != null)
        {
            GameOverScreen.Visible = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        // ゲームオーバー時にリトライ
        if (GameOverScreen != null && GameOverScreen.Visible)
        {
            if (@event.IsActionPressed("attack") || @event.IsActionPressed("ui_accept"))
            {
                GetTree().ReloadCurrentScene();
            }
        }
    }
}
