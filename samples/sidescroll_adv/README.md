# sidescroll-adv-base サンプル

## 前提条件

- Godot 4.4+
- sidescroll-2d-base が同プロジェクト内に存在すること

## 実行方法

1. sidescroll-2d-base を `addons/sidescroll_2d_base/` に配置
2. sidescroll-adv-base を `addons/sidescroll_adv_base/` に配置
3. このsamplesフォルダをプロジェクトルートに配置
4. プロジェクト設定でインプットマップに `interact` アクションを追加（推奨: Eキー）
5. `SampleScene.tscn`を開いて実行

## 操作方法

| 入力 | 動作 |
|------|------|
| ← → | 左右移動 |
| Space/Enter | ジャンプ / 会話進行 |
| E（要設定）| インタラクト |

※ `interact`アクション未設定の場合、Space/Enterでインタラクトも可能

## 確認できる機能

### プレイヤー
- BaseAdventurePlayer による移動
- インタラクト検出・実行
- 移動ロック/アンロック

### インタラクト
- InteractableDialogue（NPC会話）
- InteractableItem（アイテム取得）

### 会話システム
- DialogueManager による会話進行
- SampleDialogueUI による表示

### インベントリ
- InventoryManager によるアイテム管理
- アイテム取得ログ（コンソール出力）

## シングルトン構成

サンプルではAutoload登録せず、シーン内ノードとして配置しています。
本番環境ではAutoloadへの登録を推奨：

```
GameFlagManager  -> res://addons/sidescroll_adv_base/flags/GameFlagManager.cs
InventoryManager -> res://addons/sidescroll_adv_base/inventory/InventoryManager.cs
DialogueManager  -> res://addons/sidescroll_adv_base/dialogue/DialogueManager.cs
```

## カスタマイズ

- `sample_config.tres` - 移動パラメータ調整
- `sample_dialogue.tres` - 会話内容編集
- `sample_item.tres` - アイテムデータ編集
