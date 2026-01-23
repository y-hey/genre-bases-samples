using Godot;

namespace TopDownRpgSample;

/// <summary>
/// 見下ろし型アクションRPGのプレイヤーキャラクター
/// 4方向移動と剣攻撃を実装
/// </summary>
public partial class TopDownPlayer : CharacterBody2D
{
    [Export] public float MoveSpeed { get; set; } = 200.0f;
    [Export] public int MaxHealth { get; set; } = 6;
    [Export] public float AttackDuration { get; set; } = 0.3f;
    [Export] public float InvincibleDuration { get; set; } = 1.0f;
    [Export] public int AttackDamage { get; set; } = 1;

    public int CurrentHealth { get; private set; }
    public Vector2 FacingDirection { get; private set; } = Vector2.Down;
    public bool IsAttacking { get; private set; }
    public bool IsInvincible { get; private set; }

    private Area2D? _swordHitbox;
    private AnimatedSprite2D? _sprite;
    private Timer? _attackTimer;
    private Timer? _invincibleTimer;
    private ColorRect? _visualBody;
    private ColorRect? _visualSword;

    [Signal]
    public delegate void HealthChangedEventHandler(int currentHealth, int maxHealth);

    [Signal]
    public delegate void PlayerDiedEventHandler();

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;

        _swordHitbox = GetNodeOrNull<Area2D>("SwordHitbox");
        _sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        _visualBody = GetNodeOrNull<ColorRect>("VisualBody");
        _visualSword = GetNodeOrNull<ColorRect>("VisualSword");

        // 攻撃タイマー
        _attackTimer = new Timer();
        _attackTimer.OneShot = true;
        _attackTimer.WaitTime = AttackDuration;
        _attackTimer.Timeout += OnAttackFinished;
        AddChild(_attackTimer);

        // 無敵タイマー
        _invincibleTimer = new Timer();
        _invincibleTimer.OneShot = true;
        _invincibleTimer.WaitTime = InvincibleDuration;
        _invincibleTimer.Timeout += OnInvincibleFinished;
        AddChild(_invincibleTimer);

        if (_swordHitbox != null)
        {
            _swordHitbox.Visible = false;
            _swordHitbox.AreaEntered += OnSwordHit;
        }

        if (_visualSword != null)
        {
            _visualSword.Visible = false;
        }

        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsAttacking)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        HandleMovement();
        HandleAttack();
        MoveAndSlide();
        UpdateVisuals();
    }

    private void HandleMovement()
    {
        var inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if (inputDir != Vector2.Zero)
        {
            inputDir = inputDir.Normalized();
            Velocity = inputDir * MoveSpeed;

            // 向きを更新（斜め移動時は水平方向を優先）
            if (Mathf.Abs(inputDir.X) > Mathf.Abs(inputDir.Y))
            {
                FacingDirection = inputDir.X > 0 ? Vector2.Right : Vector2.Left;
            }
            else
            {
                FacingDirection = inputDir.Y > 0 ? Vector2.Down : Vector2.Up;
            }
        }
        else
        {
            Velocity = Vector2.Zero;
        }
    }

    private void HandleAttack()
    {
        if (Input.IsActionJustPressed("attack") && !IsAttacking)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        IsAttacking = true;

        if (_swordHitbox != null)
        {
            // 向きに応じて剣の位置を設定
            _swordHitbox.Position = FacingDirection * 40;
            _swordHitbox.Visible = true;
            _swordHitbox.Monitoring = true;
        }

        if (_visualSword != null)
        {
            _visualSword.Visible = true;
            UpdateSwordVisual();
        }

        _attackTimer?.Start();
    }

    private void OnAttackFinished()
    {
        IsAttacking = false;

        if (_swordHitbox != null)
        {
            _swordHitbox.Visible = false;
            _swordHitbox.Monitoring = false;
        }

        if (_visualSword != null)
        {
            _visualSword.Visible = false;
        }
    }

    private void OnSwordHit(Area2D area)
    {
        // 敵へのダメージ処理
        if (area.GetParent() is TopDownEnemy enemy)
        {
            enemy.TakeDamage(AttackDamage, GlobalPosition);
        }

        // 壊せるオブジェクト
        if (area.GetParent() is BreakableObject breakable)
        {
            breakable.Break();
        }
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (IsInvincible || CurrentHealth <= 0) return;

        CurrentHealth -= damage;
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

        // ノックバック
        var knockbackDir = (GlobalPosition - sourcePosition).Normalized();
        Velocity = knockbackDir * 300;
        MoveAndSlide();

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartInvincibility();
        }
    }

    private void StartInvincibility()
    {
        IsInvincible = true;
        _invincibleTimer?.Start();
    }

    private void OnInvincibleFinished()
    {
        IsInvincible = false;
    }

    private void Die()
    {
        GD.Print("プレイヤー死亡");
        EmitSignal(SignalName.PlayerDied);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }

    private void UpdateVisuals()
    {
        // 無敵時の点滅
        if (_visualBody != null && IsInvincible)
        {
            _visualBody.Modulate = new Color(1, 1, 1, Mathf.Sin((float)Time.GetTicksMsec() / 50) > 0 ? 1.0f : 0.3f);
        }
        else if (_visualBody != null)
        {
            _visualBody.Modulate = Colors.White;
        }
    }

    private void UpdateSwordVisual()
    {
        if (_visualSword == null) return;

        // 向きに応じて剣のビジュアルを調整
        if (FacingDirection == Vector2.Right)
        {
            _visualSword.Position = new Vector2(20, -8);
            _visualSword.Size = new Vector2(40, 16);
        }
        else if (FacingDirection == Vector2.Left)
        {
            _visualSword.Position = new Vector2(-60, -8);
            _visualSword.Size = new Vector2(40, 16);
        }
        else if (FacingDirection == Vector2.Down)
        {
            _visualSword.Position = new Vector2(-8, 20);
            _visualSword.Size = new Vector2(16, 40);
        }
        else // Up
        {
            _visualSword.Position = new Vector2(-8, -60);
            _visualSword.Size = new Vector2(16, 40);
        }
    }
}
