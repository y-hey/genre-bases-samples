using Godot;
using SidescrollAdvBase;

namespace SidescrollAdvSample;

/// <summary>
/// サンプル用シンプル会話UI
/// </summary>
public partial class SampleDialogueUI : CanvasLayer, IDialogueCallbacks
{
    [Export] public Panel DialoguePanel { get; set; }
    [Export] public Label SpeakerLabel { get; set; }
    [Export] public Label TextLabel { get; set; }
    [Export] public Label ContinueHint { get; set; }

    private bool _isDialogueActive;
    private bool _isDialogueManagerConnected;

    public override void _Ready()
    {
        if (DialoguePanel != null)
        {
            DialoguePanel.Visible = false;
        }

        // DialogueManagerにコールバック登録
        if (DialogueManager.HasInstance)
        {
            DialogueManager.Instance.SetCallbacks(this);
            DialogueManager.Instance.DialogueStarted += OnDialogueStartedSignal;
            DialogueManager.Instance.DialogueEnded += OnDialogueEndedSignal;
            _isDialogueManagerConnected = true;
        }
    }

    public override void _ExitTree()
    {
        if (_isDialogueManagerConnected && DialogueManager.HasInstance)
        {
            DialogueManager.Instance.DialogueStarted -= OnDialogueStartedSignal;
            DialogueManager.Instance.DialogueEnded -= OnDialogueEndedSignal;
            _isDialogueManagerConnected = false;
        }
    }

    public override void _Process(double delta)
    {
        if (!_isDialogueActive) return;

        // 進行入力
        if (Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("interact"))
        {
            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.Advance();
            }
        }
    }

    private void OnDialogueStartedSignal()
    {
        _isDialogueActive = true;
        if (DialoguePanel != null)
        {
            DialoguePanel.Visible = true;
        }
    }

    private void OnDialogueEndedSignal()
    {
        _isDialogueActive = false;
        if (DialoguePanel != null)
        {
            DialoguePanel.Visible = false;
        }
    }

    // IDialogueCallbacks実装
    public void OnLineDisplayed(string speaker, string text)
    {
        if (SpeakerLabel != null)
        {
            SpeakerLabel.Text = speaker;
        }
        if (TextLabel != null)
        {
            TextLabel.Text = text;
        }
        GD.Print($"[{speaker}] {text}");
    }

    public void OnChoicesDisplayed(Godot.Collections.Array<DialogueChoice> choices)
    {
        // サンプルでは選択肢未対応
        GD.Print("選択肢表示（サンプルでは未対応）");
    }

    public void OnDialogueStarted(DialogueData data)
    {
        GD.Print($"会話開始: {data.DialogueId}");
    }

    public void OnDialogueEnded()
    {
        GD.Print("会話終了");
    }
}
