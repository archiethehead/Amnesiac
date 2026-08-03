using Godot;
using System;

public partial class PauseMenu : CanvasLayer {

    private float PauseCooldown = 0.5f;
    private bool IsPaused = false;
    private GameManager GameManager = null;

    public override void _Ready() {

        GameManager = GameManager.Instance;
        GameManager.PauseMenu = this;
        this.ProcessMode = ProcessModeEnum.Disabled;

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {

        if (PauseCooldown > 0.0f) {

            PauseCooldown -= 1.0f * (float)delta;

        }

        else if (Input.IsActionJustPressed(InputMap.Pause)) {

            Unpause();

        }

    }

    public void Pause() {

        GameManager.MouseMode = Input.MouseMode;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        this.ProcessMode = ProcessModeEnum.Always;
        this.Visible = true;
        IsPaused = true;

    }

    private void Unpause() {

        Input.MouseMode = GameManager.MouseMode;
        PauseCooldown = 0.5f;
        this.Visible = false;
        this.ProcessMode = ProcessModeEnum.Disabled;
        IsPaused = false;
        GameManager.Unpause();

    }

    private void _on_resume_pressed() {

        Unpause();

    }

    private void _on_exit_pressed() {

        GameManager.Exit();

    }

}