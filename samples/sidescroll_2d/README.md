# sidescroll-2d-base サンプル

## 実行方法

1. このフォルダをGodot 4.4+プロジェクトにコピー
2. `SampleScene.tscn`を開いて実行

## 操作方法

| 入力 | 動作 |
|------|------|
| ← → | 左右移動 |
| Space/Enter | ジャンプ |

## 確認できる機能

- BaseCharacter2D による移動・ジャンプ
- FollowCamera2D による追従カメラ
- 着地/落下検知コールバック
- CameraLimit によるカメラ境界
- AreaGate（遷移先未設定）
- SpawnPoint

## 注意

- CharacterConfigは`sample_config.tres`で調整可能
- AreaGateの遷移先は未設定（シグナル発火のみ確認可能）
