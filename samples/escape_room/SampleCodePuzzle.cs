using EscapeRoomBase;
using Godot;

namespace GenreBasesSamples.EscapeRoom;

/// <summary>
/// サンプル暗証番号パズル（4桁）
/// </summary>
public partial class SampleCodePuzzle : PuzzleBase
{
    [Export] public string CorrectCode { get; set; } = "1234";
    [Export] public LineEdit? CodeInput { get; set; }
    [Export] public Button? SubmitButton { get; set; }
    [Export] public Button? CloseButton { get; set; }
    [Export] public Label? ResultLabel { get; set; }

    private SampleEscapeRoomManager? _manager;

    public override void _Ready()
    {
        Visible = false;

        if (SubmitButton != null)
        {
            SubmitButton.Pressed += OnSubmitPressed;
        }
        if (CloseButton != null)
        {
            CloseButton.Pressed += OnClosePressed;
        }

        // マネージャーを探す
        _manager = GetTree().CurrentScene.GetNodeOrNull<SampleEscapeRoomManager>("EscapeRoomManager");
    }

    protected override void OnOpen()
    {
        if (CodeInput != null)
        {
            CodeInput.Text = "";
            CodeInput.GrabFocus();
        }
        if (ResultLabel != null)
        {
            ResultLabel.Text = "";
        }

        if (_manager != null)
        {
            _manager.SetInputMode(InputMode.InPuzzle);
        }
    }

    protected override void OnClose()
    {
        if (_manager != null)
        {
            _manager.SetInputMode(InputMode.Normal);
        }
    }

    protected override void OnCleared()
    {
        if (ResultLabel != null)
        {
            ResultLabel.Text = "正解！";
        }
        if (_manager != null)
        {
            _manager.ClearPuzzle(PuzzleId);
        }
    }

    protected override bool CheckAnswer()
    {
        if (CodeInput == null) return false;
        return CodeInput.Text == CorrectCode;
    }

    public override void Reset()
    {
        IsCleared = false;
        if (CodeInput != null)
        {
            CodeInput.Text = "";
        }
        if (ResultLabel != null)
        {
            ResultLabel.Text = "";
        }
    }

    private void OnSubmitPressed()
    {
        if (IsCleared) return;

        if (CheckAnswer())
        {
            Clear();
        }
        else
        {
            if (ResultLabel != null)
            {
                ResultLabel.Text = "不正解...";
            }
        }
    }

    private void OnClosePressed()
    {
        Close();
    }
}
