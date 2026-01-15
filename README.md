# Genre Bases Samples

genre-bases ライブラリの動作確認用サンプルプロジェクト。

## セットアップ

### Windows

```bash
git clone --recursive https://github.com/y-hey/genre-bases-samples.git
cd genre-bases-samples
setup.bat
```

### macOS / Linux

```bash
git clone --recursive https://github.com/y-hey/genre-bases-samples.git
cd genre-bases-samples
chmod +x setup.sh
./setup.sh
```

## サンプル一覧

| サンプル | 場所 | 確認できる機能 |
|----------|------|----------------|
| sidescroll_2d | `samples/sidescroll_2d/SampleScene.tscn` | 基本移動、ジャンプ、追従カメラ |
| sidescroll_adv | `samples/sidescroll_adv/SampleScene.tscn` | インタラクト、会話、アイテム取得 |

## 操作方法

| 入力 | 動作 |
|------|------|
| ← → | 移動 |
| Space/Enter | ジャンプ / 会話進行 |
| E | インタラクト |

## 構成

```
genre-bases-samples/
├── project.godot
├── addons/                      ← ジャンクション/シンボリックリンク
│   ├── sidescroll_2d_base/     → libs/sidescroll-2d-base/addons/...
│   └── sidescroll_adv_base/    → libs/sidescroll-adv-base/addons/...
├── libs/                        ← submodule
│   ├── sidescroll-2d-base/
│   └── sidescroll-adv-base/
└── samples/
    ├── sidescroll_2d/
    └── sidescroll_adv/
```

## 依存ライブラリ

- [sidescroll-2d-base](https://github.com/y-hey/sidescroll-2d-base)
- [sidescroll-adv-base](https://github.com/y-hey/sidescroll-adv-base)
