using EscapeRoomBase;
using Godot;

namespace GenreBasesSamples.EscapeRoom;

/// <summary>
/// サンプルクリック可能オブジェクト
/// </summary>
public partial class SampleClickableObject : InteractableBase
{
    public enum ObjectType
    {
        /// <summary>調べるだけ</summary>
        Examinable,
        /// <summary>アイテムを取得できる</summary>
        ItemSource,
        /// <summary>パズルを開く</summary>
        PuzzleTrigger,
        /// <summary>アイテムを使用する場所</summary>
        ItemTarget
    }

    [Export] public ObjectType Type { get; set; } = ObjectType.Examinable;
    [Export] public string ItemId { get; set; } = "";
    [Export] public string RequiredItemId { get; set; } = "";
    [Export] public NodePath PuzzlePath { get; set; } = "";

    private SampleEscapeRoomManager? _manager;
    private SampleInventory? _inventory;
    private CanvasItem? _visual;
    private Color _originalColor;
    private bool _itemTaken;

    public override void _Ready()
    {
        base._Ready();

        // Visual（ColorRectまたはSprite2D）を取得
        _visual = GetNodeOrNull<CanvasItem>("Visual");
        if (_visual != null)
        {
            _originalColor = _visual.Modulate;
        }

        // マネージャーとインベントリを探す
        var scene = GetTree().CurrentScene;
        _manager = scene.GetNodeOrNull<SampleEscapeRoomManager>("EscapeRoomManager");
        _inventory = scene.GetNodeOrNull<SampleInventory>("EscapeRoomManager/Inventory");
    }

    protected override void OnClick()
    {
        if (_manager == null) return;

        switch (Type)
        {
            case ObjectType.Examinable:
                HandleExamine();
                break;
            case ObjectType.ItemSource:
                HandleItemSource();
                break;
            case ObjectType.PuzzleTrigger:
                HandlePuzzleTrigger();
                break;
            case ObjectType.ItemTarget:
                HandleItemTarget();
                break;
        }

        base.OnClick();
    }

    private void HandleExamine()
    {
        Examine();
        _manager?.ShowMessage(Description);
    }

    private void HandleItemSource()
    {
        if (_itemTaken)
        {
            _manager?.ShowMessage("既に取得済み");
            return;
        }

        if (!string.IsNullOrEmpty(ItemId))
        {
            _manager?.AcquireItem(ItemId);
            _itemTaken = true;

            // 見た目を変える（薄くする）
            if (_visual != null)
            {
                _visual.Modulate = new Color(_originalColor.R, _originalColor.G, _originalColor.B, 0.3f);
            }
        }
    }

    private void HandlePuzzleTrigger()
    {
        if (string.IsNullOrEmpty(PuzzlePath)) return;

        var puzzle = GetNodeOrNull<PuzzleBase>(PuzzlePath);
        if (puzzle != null)
        {
            puzzle.Open();
        }
    }

    private void HandleItemTarget()
    {
        if (_inventory == null || _manager == null) return;

        if (string.IsNullOrEmpty(_inventory.SelectedItemId))
        {
            _manager.ShowMessage("アイテムを選択してください");
            return;
        }

        if (_inventory.SelectedItemId == RequiredItemId)
        {
            _manager.UseItem(RequiredItemId, ObjectId);
            _inventory.RemoveItem(RequiredItemId);
            _manager.ShowMessage($"{RequiredItemId}を使用した！");

            // 使用後の処理（例：ドアが開くなど）
            IsInteractable = false;
            if (_visual != null)
            {
                _visual.Modulate = new Color(0.5f, 1, 0.5f);
            }
        }
        else
        {
            _manager.ShowMessage("このアイテムは使えない");
        }
    }

    protected override void OnMouseEntered()
    {
        if (_visual != null && IsInteractable)
        {
            _visual.Modulate = new Color(
                _originalColor.R * 1.3f,
                _originalColor.G * 1.3f,
                _originalColor.B * 1.3f,
                _originalColor.A);
        }
    }

    protected override void OnMouseExited()
    {
        if (_visual != null && IsInteractable && !_itemTaken)
        {
            _visual.Modulate = _originalColor;
        }
    }
}
