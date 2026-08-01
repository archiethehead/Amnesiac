using Godot;
using System;

public partial class Console : CanvasLayer
{

    private GameManager GameManager = null;
    [Export] LineEdit LineEdit = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

        GameManager = GameManager.Instance;
        GameManager.Console = this;
        LineEdit.ClearButtonEnabled = true;
        LineEdit.KeepEditingOnTextSubmit = true;
        this.ProcessMode = ProcessModeEnum.Disabled;

    }

    public override void _Process(double delta) {

        if (Input.IsActionPressed(InputMap.Pause)) {

            LineEdit.Clear();
            GameManager.CloseConsole();

        }

        else if (Input.IsActionPressed(InputMap.Submit)) {

            LineEdit.Clear();

        }

    }

    public void ConsoleOn() {

        GameManager.MouseMode = Input.MouseMode;
        Input.MouseMode = Input.MouseModeEnum.Confined;
        Show();
        this.ProcessMode = ProcessModeEnum.Always;
        LineEdit.GrabFocus();

    }

    public void ConsoleOff() {

        Input.MouseMode = GameManager.MouseMode;
        Hide();
        this.ProcessMode = ProcessModeEnum.Disabled;

    }
	
}
