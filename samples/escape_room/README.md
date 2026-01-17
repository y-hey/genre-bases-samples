# Escape Room Sample

2Dポイント・アンド・クリック脱出ゲームのサンプル。

## 概要

シンプルな1部屋の脱出ゲームデモ。

### ゲームフロー
1. 部屋の中のオブジェクトをクリックして調査
2. 「鍵」アイテムを見つけて取得
3. 金庫の暗証番号パズル（ヒントは部屋のどこかに）
4. 金庫を開けて脱出！

## 使用しているコンポーネント

| クラス | 継承元 | 役割 |
|--------|--------|------|
| SampleEscapeRoomManager | BaseEscapeRoomManager | ゲーム全体の管理 |
| SampleInventory | BaseInventorySystem | アイテム管理・UI |
| SampleProgressTracker | BaseProgressTracker | フラグ・進行管理 |
| SampleCodePuzzle | PuzzleBase | 暗証番号パズル |
| SampleClickableObject | InteractableBase | クリック可能オブジェクト |

## 操作方法

- **左クリック**: オブジェクトを調べる/アイテムを取得
- **インベントリのアイテムをクリック**: アイテムを選択
- **選択中にオブジェクトをクリック**: アイテムを使用

## ファイル構成

```
samples/escape_room/
├── README.md
├── SampleEscapeRoomManager.cs
├── SampleInventory.cs
├── SampleProgressTracker.cs
├── SampleCodePuzzle.cs
├── SampleClickableObject.cs
└── SampleScene.tscn
```
