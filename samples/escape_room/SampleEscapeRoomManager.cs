using EscapeRoomBase;
using Godot;

namespace GenreBasesSamples.EscapeRoom;

/// <summary>
/// サンプル脱出ゲームマネージャー
/// </summary>
public partial class SampleEscapeRoomManager : BaseEscapeRoomManager, IEscapeRoomCallbacks
{
    [Export] public SampleInventory? Inventory { get; set; }
    [Export] public SampleProgressTracker? Progress { get; set; }
    [Export] public Label? MessageLabel { get; set; }
    [Export] public Control? ClearScreen { get; set; }

    public override void _Ready()
    {
        Callbacks = this;
        if (ClearScreen != null)
        {
            ClearScreen.Visible = false;
        }
    }

    protected override void OnRoomChange(string roomId)
    {
        ShowMessage($"視点を変更: {roomId}");
    }

    protected override void OnItemAcquire(string itemId)
    {
        if (Inventory != null)
        {
            Inventory.AddItem(itemId);
        }
        if (Progress != null)
        {
            Progress.MarkItemAcquired(itemId);
        }
        ShowMessage($"アイテムを取得: {itemId}");
    }

    protected override void OnPuzzleClear(string puzzleId)
    {
        if (Progress != null)
        {
            Progress.MarkPuzzleCleared(puzzleId);
        }
        ShowMessage($"パズルをクリア: {puzzleId}");
    }

    protected override bool CheckGameClear()
    {
        // 出口パズル（door_lock）がクリア済みならゲームクリア
        if (Progress != null)
        {
            return Progress.IsPuzzleCleared("door_lock");
        }
        return false;
    }

    protected override void OnGameStateChanged(GameState previous, GameState current)
    {
        if (current == GameState.Cleared)
        {
            ShowClearScreen();
        }
    }

    private void ShowClearScreen()
    {
        if (ClearScreen != null)
        {
            ClearScreen.Visible = true;
        }
        ShowMessage("脱出成功！");
    }

    public void ShowMessage(string message)
    {
        if (MessageLabel != null)
        {
            MessageLabel.Text = message;
        }
        GD.Print($"[EscapeRoom] {message}");
    }

    // IEscapeRoomCallbacks 実装
    public void OnItemAcquired(string itemId)
    {
        GD.Print($"[Callback] Item acquired: {itemId}");
    }

    public void OnItemUsed(string itemId, string targetId)
    {
        GD.Print($"[Callback] Item used: {itemId} on {targetId}");
    }

    public void OnPuzzleCleared(string puzzleId)
    {
        GD.Print($"[Callback] Puzzle cleared: {puzzleId}");
    }

    public void OnRoomChanged(string roomId)
    {
        GD.Print($"[Callback] Room changed: {roomId}");
    }

    public void OnGameCleared()
    {
        GD.Print("[Callback] Game cleared!");
    }
}
