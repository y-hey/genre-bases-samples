using Godot;

namespace TopDownRpgSample;

/// <summary>
/// 見下ろし型アクションRPGの敵キャラクター
/// プレイヤーを追跡し、接触でダメージを与える
/// </summary>
public partial class TopDownEnemy : CharacterBody2D
{
    public enum EnemyState
    {
        Idle,
        Wander,
        Chase,
        Hurt,
        Dead
    }

    [Export] public float MoveSpeed { get; set; } = 80.0f;
    [Export] public float ChaseSpeed { get; set; } = 120.0f;
    [Export] public float DetectionRange { get; set; } = 200.0f;
    [Export] public float AttackRange { get; set; } = 30.0f;
    [Export] public int MaxHealth { get; set; } = 2;
    [Export] public int ContactDamage { get; set; } = 1;
    [Export] public float WanderInterval { get; set; } = 2.0f;
    [Export] public float HurtDuration { get; set; } = 0.3f;

    public int CurrentHealth { get; private set; }
    public EnemyState State { get; private set; } = EnemyState.Idle;

    private TopDownPlayer? _player;
    private Timer? _wanderTimer;
    private Timer? _hurtTimer;
    private Vector2 _wanderDirection;
    private ColorRect? _visual;
    private Area2D? _hitbox;

    [Signal]
    public delegate void EnemyDiedEventHandler(TopDownEnemy enemy);

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;

        _visual = GetNodeOrNull<ColorRect>("Visual");
        _hitbox = GetNodeOrNull<Area2D>("Hitbox");

        // 徘徊タイマー
        _wanderTimer = new Timer();
        _wanderTimer.WaitTime = WanderInterval;
        _wanderTimer.Timeout += OnWanderTimerTimeout;
        AddChild(_wanderTimer);
        _wanderTimer.Start();

        // 被ダメージタイマー
        _hurtTimer = new Timer();
        _hurtTimer.OneShot = true;
        _hurtTimer.WaitTime = HurtDuration;
        _hurtTimer.Timeout += OnHurtFinished;
        AddChild(_hurtTimer);

        if (_hitbox != null)
        {
            _hitbox.BodyEntered += OnBodyEntered;
        }

        // プレイヤーを取得
        CallDeferred(nameof(FindPlayer));
    }

    private void FindPlayer()
    {
        _player = GetTree().GetFirstNodeInGroup("player") as TopDownPlayer;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (State == EnemyState.Dead) return;

        if (State == EnemyState.Hurt)
        {
            MoveAndSlide();
            return;
        }

        UpdateState();
        UpdateMovement();
        MoveAndSlide();
    }

    private void UpdateState()
    {
        if (_player == null || State == EnemyState.Hurt) return;

        var distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);

        if (distanceToPlayer <= DetectionRange)
        {
            State = EnemyState.Chase;
        }
        else
        {
            State = EnemyState.Wander;
        }
    }

    private void UpdateMovement()
    {
        switch (State)
        {
            case EnemyState.Idle:
                Velocity = Vector2.Zero;
                break;

            case EnemyState.Wander:
                Velocity = _wanderDirection * MoveSpeed;
                break;

            case EnemyState.Chase:
                if (_player != null)
                {
                    var direction = (_player.GlobalPosition - GlobalPosition).Normalized();
                    Velocity = direction * ChaseSpeed;
                }
                break;
        }
    }

    private void OnWanderTimerTimeout()
    {
        // ランダムな方向に徘徊
        var angle = GD.Randf() * Mathf.Tau;
        _wanderDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // たまに止まる
        if (GD.Randf() < 0.3f)
        {
            _wanderDirection = Vector2.Zero;
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is TopDownPlayer player && State != EnemyState.Dead)
        {
            player.TakeDamage(ContactDamage, GlobalPosition);
        }
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (State == EnemyState.Dead) return;

        CurrentHealth -= damage;

        // ノックバック
        State = EnemyState.Hurt;
        var knockbackDir = (GlobalPosition - sourcePosition).Normalized();
        Velocity = knockbackDir * 200;
        _hurtTimer?.Start();

        // ダメージエフェクト
        if (_visual != null)
        {
            _visual.Modulate = Colors.Red;
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void OnHurtFinished()
    {
        if (State == EnemyState.Dead) return;

        State = EnemyState.Idle;
        if (_visual != null)
        {
            _visual.Modulate = Colors.White;
        }
    }

    private void Die()
    {
        State = EnemyState.Dead;
        EmitSignal(SignalName.EnemyDied, this);

        // アイテムドロップのチャンス
        if (GD.Randf() < 0.3f)
        {
            SpawnHeart();
        }

        // 死亡エフェクト後に削除
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0.0f, 0.3f);
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void SpawnHeart()
    {
        var heartScene = GD.Load<PackedScene>("res://samples/topdown_rpg/HeartPickup.tscn");
        if (heartScene != null)
        {
            var heart = heartScene.Instantiate<Node2D>();
            heart.GlobalPosition = GlobalPosition;
            GetParent().AddChild(heart);
        }
    }
}
