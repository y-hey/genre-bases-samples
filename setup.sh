#!/bin/bash
echo "Genre Bases Samples セットアップ"
echo

# submodule初期化
echo "[1/3] Submoduleを初期化中..."
git submodule update --init --recursive

# シンボリックリンク作成
echo "[2/3] addonsフォルダにリンクを作成中..."
mkdir -p addons

if [ -L addons/y2_core ] || [ -d addons/y2_core ]; then
    echo "  y2_core: 既に存在します"
else
    ln -s ../libs/y2-godot-core-lib/addons/y2_core addons/y2_core
fi

if [ -L addons/sidescroll_2d_base ] || [ -d addons/sidescroll_2d_base ]; then
    echo "  sidescroll_2d_base: 既に存在します"
else
    ln -s ../libs/sidescroll-2d-base/addons/sidescroll_2d_base addons/sidescroll_2d_base
fi

if [ -L addons/sidescroll_adv_base ] || [ -d addons/sidescroll_adv_base ]; then
    echo "  sidescroll_adv_base: 既に存在します"
else
    ln -s ../libs/sidescroll-adv-base/addons/sidescroll_adv_base addons/sidescroll_adv_base
fi

echo "[3/3] 完了!"
echo
echo "Godotでproject.godotを開いてサンプルを実行してください。"
