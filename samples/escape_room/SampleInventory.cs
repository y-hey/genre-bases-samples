using EscapeRoomBase;
using Godot;

namespace GenreBasesSamples.EscapeRoom;

/// <summary>
/// サンプルインベントリシステム
/// </summary>
public partial class SampleInventory : BaseInventorySystem
{
    [Export] public HBoxContainer? ItemContainer { get; set; }
    [Export] public PackedScene? ItemButtonScene { get; set; }

    public override void _Ready()
    {
        ItemAdded += OnItemAdded;
        ItemRemoved += OnItemRemoved;
        ItemSelected += OnItemSelected;
    }

    public override string TryCombineItems(string itemId1, string itemId2)
    {
        // このサンプルでは合成なし
        return "";
    }

    private void OnItemAdded(string itemId)
    {
        UpdateUI();
    }

    private void OnItemRemoved(string itemId)
    {
        UpdateUI();
    }

    private void OnItemSelected(string itemId)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ItemContainer == null) return;

        // 既存のボタンをクリア
        foreach (var child in ItemContainer.GetChildren())
        {
            child.QueueFree();
        }

        // アイテムごとにボタン作成
        foreach (var itemId in GetItems())
        {
            var button = new Button();
            button.Text = itemId;
            button.CustomMinimumSize = new Vector2(80, 40);

            if (itemId == SelectedItemId)
            {
                button.Modulate = new Color(1, 1, 0); // 選択中は黄色
            }

            var capturedItemId = itemId;
            button.Pressed += () => OnItemButtonPressed(capturedItemId);
            ItemContainer.AddChild(button);
        }
    }

    private void OnItemButtonPressed(string itemId)
    {
        if (SelectedItemId == itemId)
        {
            DeselectItem();
        }
        else
        {
            SelectItem(itemId);
        }
    }
}
