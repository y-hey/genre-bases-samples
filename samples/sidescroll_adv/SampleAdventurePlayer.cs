using Godot;
using Sidescroll2DBase;
using SidescrollAdvBase;

namespace SidescrollAdvSample;

/// <summary>
/// サンプルアドベンチャープレイヤー - インタラクト機能のデモ
/// </summary>
public partial class SampleAdventurePlayer : BaseAdventurePlayer, IPlayerCallbacks
{
    /// <summary>インタラクトプロンプト表示用ラベル</summary>
    [Export] public Label PromptLabel { get; set; }

    protected override void OnReady()
    {
        base.OnReady();

        if (PromptLabel != null)
        {
            PromptLabel.Visible = false;
        }
    }

    protected override void UpdateInput()
    {
        if (IsMovementLocked) return;

        // 左右移動
        InputDirection = Input.GetAxis("ui_left", "ui_right");

        // ジャンプ
        JumpRequested = Input.IsActionJustPressed("ui_accept");
    }

    // IPlayerCallbacks実装
    public void OnInteracted(InteractableBase interactable)
    {
        GD.Print($"インタラクト実行: {interactable.Name}");
    }

    public void OnInteractableNearby(InteractableBase interactable)
    {
        GD.Print($"インタラクト可能: {interactable.InteractPrompt}");
        if (PromptLabel != null)
        {
            PromptLabel.Text = $"[E] {interactable.InteractPrompt}";
            PromptLabel.Visible = true;
        }
    }

    public void OnInteractableLeft()
    {
        if (PromptLabel != null)
        {
            PromptLabel.Visible = false;
        }
    }

    public void OnMovementLocked()
    {
        GD.Print("移動ロック");
    }

    public void OnMovementUnlocked()
    {
        GD.Print("移動アンロック");
    }
}
