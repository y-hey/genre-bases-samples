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
    [Export] public float KnockbackDuration { get; set; } = 0.15f;
    [Export] public float KnockbackSpeed { get; set; } = 300.0f;
    [Export] public float KnockbackDeceleration { get; set; } = 1500.0f;

    public int CurrentHealth { get; private set; }
    public Vector2 FacingDirection { get; private set; } = Vector2.Down;
    public bool IsAttacking { get; private set; }
    public bool IsInvincible { get; private set; }
    public bool IsDead { get; private set; }

    private bool _isKnockback;
    private Vector2 _knockbackVelocity;

    private Area2D? _swordHitbox;
    private Timer? _attackTimer;
    private Timer? _invincibleTimer;
    private Timer? _knockbackTimer;
    private ColorRect? _visualBody;
    private ColorRect? _visualSword;

    // 色定数（GC対策）
    private static readonly Color NormalModulate = Colors.White;
    private static readonly Color FlashModulateVisible = new(1, 1, 1, 1.0f);
    private static readonly Color FlashModulateHidden = new(1, 1, 1, 0.3f);

    [Signal]
    public delegate void HealthChangedEventHandler(int currentHealth, int maxHealth);

    [Signal]
    public delegate void PlayerDiedEventHandler();

    public override void _Ready()
    {
        CurrentHealth = MaxHealth;

        _swordHitbox = GetNodeOrNull<Area2D>("SwordHitbox");
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

        // ノックバックタイマー
        _knockbackTimer = new Timer();
        _knockbackTimer.OneShot = true;
        _knockbackTimer.WaitTime = KnockbackDuration;
        _knockbackTimer.Timeout += OnKnockbackFinished;
        AddChild(_knockbackTimer);

        if (_swordHitbox != null)
        {
            _swordHitbox.Visible = false;
            _swordHitbox.Monitoring = false;
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
        // 死亡時は何もしない
        if (IsDead)
        {
            Velocity = Vector2.Zero;
            return;
        }

        // 無敵時の点滅は常に更新
        UpdateInvincibleFlash();

        // ノックバック中
        if (_isKnockback)
        {
            // 減速処理
            var decel = KnockbackDeceleration * (float)delta;
            _knockbackVelocity = _knockbackVelocity.MoveToward(Vector2.Zero, decel);
            Velocity = _knockbackVelocity;
            MoveAndSlide();
            return;
        }

        // 攻撃中は移動不可だが、UpdateVisualsは呼ばない（点滅は上で処理済み）
        if (IsAttacking)
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
            return;
        }

        HandleMovement();
        HandleAttack();
        MoveAndSlide();
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
        if (IsInvincible || IsDead || CurrentHealth <= 0) return;

        CurrentHealth -= damage;
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);

        // ノックバック開始（_PhysicsProcess内で処理）
        var knockbackDir = GlobalPosition - sourcePosition;
        if (knockbackDir.LengthSquared() > 0.0001f)
        {
            knockbackDir = knockbackDir.Normalized();
        }
        else
        {
            // 同一位置の場合はランダム方向
            knockbackDir = Vector2.Right.Rotated(GD.Randf() * Mathf.Tau);
        }
        _knockbackVelocity = knockbackDir * KnockbackSpeed;
        _isKnockback = true;
        _knockbackTimer?.Start();

        if (CurrentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartInvincibility();
        }
    }

    private void OnKnockbackFinished()
    {
        _isKnockback = false;
        _knockbackVelocity = Vector2.Zero;
    }

    private void StartInvincibility()
    {
        IsInvincible = true;
        _invincibleTimer?.Start();
    }

    private void OnInvincibleFinished()
    {
        IsInvincible = false;
        // 点滅終了時に確実に通常色に戻す
        if (_visualBody != null)
        {
            _visualBody.Modulate = NormalModulate;
        }
    }

    private void Die()
    {
        IsDead = true;
        GD.Print("プレイヤー死亡");
        EmitSignal(SignalName.PlayerDied);
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        EmitSignal(SignalName.HealthChanged, CurrentHealth, MaxHealth);
    }

    private void UpdateInvincibleFlash()
    {
        if (_visualBody == null) return;

        if (IsInvincible)
        {
            // 点滅（Sinを使わず、タイムベースで交互に）
            var flash = ((int)(Time.GetTicksMsec() / 100)) % 2 == 0;
            _visualBody.Modulate = flash ? FlashModulateVisible : FlashModulateHidden;
        }
        else
        {
            _visualBody.Modulate = NormalModulate;
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
