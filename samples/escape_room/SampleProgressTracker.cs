using EscapeRoomBase;
using Godot;

namespace GenreBasesSamples.EscapeRoom;

/// <summary>
/// サンプル進行状態トラッカー
/// </summary>
public partial class SampleProgressTracker : BaseProgressTracker
{
    public override Godot.Collections.Dictionary<string, Variant> Serialize()
    {
        var data = new Godot.Collections.Dictionary<string, Variant>();

        // フラグをシリアライズ
        var flagsArray = new Godot.Collections.Array<string>();
        foreach (var kvp in Flags)
        {
            if (kvp.Value)
            {
                flagsArray.Add(kvp.Key);
            }
        }
        data["flags"] = flagsArray;

        // クリア済みパズルをシリアライズ
        var puzzlesArray = new Godot.Collections.Array<string>();
        foreach (var puzzleId in ClearedPuzzles)
        {
            puzzlesArray.Add(puzzleId);
        }
        data["cleared_puzzles"] = puzzlesArray;

        // 取得済みアイテムをシリアライズ
        var itemsArray = new Godot.Collections.Array<string>();
        foreach (var itemId in AcquiredItems)
        {
            itemsArray.Add(itemId);
        }
        data["acquired_items"] = itemsArray;

        return data;
    }

    public override void Deserialize(Godot.Collections.Dictionary<string, Variant> data)
    {
        Flags.Clear();
        ClearedPuzzles.Clear();
        AcquiredItems.Clear();

        if (data.TryGetValue("flags", out var flagsVariant))
        {
            var flagsArray = flagsVariant.AsGodotArray<string>();
            foreach (var flagId in flagsArray)
            {
                Flags[flagId] = true;
            }
        }

        if (data.TryGetValue("cleared_puzzles", out var puzzlesVariant))
        {
            var puzzlesArray = puzzlesVariant.AsGodotArray<string>();
            foreach (var puzzleId in puzzlesArray)
            {
                ClearedPuzzles.Add(puzzleId);
            }
        }

        if (data.TryGetValue("acquired_items", out var itemsVariant))
        {
            var itemsArray = itemsVariant.AsGodotArray<string>();
            foreach (var itemId in itemsArray)
            {
                AcquiredItems.Add(itemId);
            }
        }
    }
}
