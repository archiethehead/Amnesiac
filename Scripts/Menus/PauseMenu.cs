using Godot;
using System;

public partial class PauseMenu : CanvasLayer {

    private float PauseCooldown = 0.5f;
    private bool IsPaused = false;
    [Export] private GameManager GameManager = null;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {

        if (!IsPaused) {

            Input.MouseMode = Input.MouseModeEnum.Confined;
            Visible = true;
            IsPaused = true;

        }

        if (PauseCooldown > 0.0f) {

            PauseCooldown -= 1.0f * (float)delta;

        }

        else if (Input.IsActionJustPressed("Escape")) {

            Unpause();

        }

    }

    private void Unpause() {

        Input.MouseMode = Input.MouseModeEnum.Captured;
        PauseCooldown = 0.5f;
        Visible = false;
        IsPaused = false;
        GameManager.Unpause();

    }

    public void _on_resume_pressed() {

        Unpause();

    }

    private void _on_exit_pressed() {

        GameManager.Exit();

    }

}