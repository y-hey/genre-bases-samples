using Godot;
using Sidescroll2DBase;

namespace Sidescroll2DSample;

/// <summary>
/// サンプルプレイヤー - 基本移動のデモ
/// </summary>
public partial class SamplePlayer : BaseCharacter2D
{
    protected override void UpdateInput()
    {
        // 左右移動
        InputDirection = Input.GetAxis("ui_left", "ui_right");

        // ジャンプ
        JumpRequested = Input.IsActionJustPressed("ui_accept");
    }

    protected override void OnLanded()
    {
        GD.Print("着地しました");
    }

    protected override void OnFallStarted()
    {
        GD.Print("落下開始");
    }
}
