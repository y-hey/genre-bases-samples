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
    private Sprite2D? _sprite;
    private bool _itemTaken;

    public override void _Ready()
    {
        base._Ready();

        _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");

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
            if (_sprite != null)
            {
                _sprite.Modulate = new Color(1, 1, 1, 0.3f);
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
            if (_sprite != null)
            {
                _sprite.Modulate = new Color(0.5f, 1, 0.5f);
            }
        }
        else
        {
            _manager.ShowMessage("このアイテムは使えない");
        }
    }

    protected override void OnMouseEntered()
    {
        if (_sprite != null && IsInteractable)
        {
            _sprite.Modulate = new Color(1.2f, 1.2f, 1.2f);
        }
    }

    protected override void OnMouseExited()
    {
        if (_sprite != null && IsInteractable && !_itemTaken)
        {
            _sprite.Modulate = new Color(1, 1, 1);
        }
    }
}
