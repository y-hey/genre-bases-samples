@echo off
chcp 65001 >nul
echo Genre Bases Samples セットアップ
echo.

REM submodule初期化
echo [1/3] Submoduleを初期化中...
git submodule update --init --recursive

REM ジャンクション作成
echo [2/3] addonsフォルダにリンクを作成中...
if not exist addons mkdir addons

if exist addons\sidescroll_2d_base (
    echo   sidescroll_2d_base: 既に存在します
) else (
    mklink /J addons\sidescroll_2d_base libs\sidescroll-2d-base\addons\sidescroll_2d_base
)

if exist addons\sidescroll_adv_base (
    echo   sidescroll_adv_base: 既に存在します
) else (
    mklink /J addons\sidescroll_adv_base libs\sidescroll-adv-base\addons\sidescroll_adv_base
)

echo [3/3] 完了!
echo.
echo Godotでproject.godotを開いてサンプルを実行してください。
pause
