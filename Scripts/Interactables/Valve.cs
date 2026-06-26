using Godot;
using System;

public partial class Valve : StaticBody3D, Interactable {

    private float RotationPercentage = 0.0f;
    private float DepthIncreaseRate = 0.5f;
    private float RotationRate = 3.0f;
    private int DepthModifier = 1;

    [Export] private ControlRod ControlRod;
    [Export] private Label3D InteractLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {

        SetProcess(false);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {

        RotationPercentage += (DepthIncreaseRate * (float)delta) * DepthModifier;
        RotationPercentage = Mathf.Clamp(RotationPercentage, 0.0f, 1.0f);
        ControlRod.NewRodDepth = RotationPercentage;
        ControlRod.SetProcess(true);

        if (RotationPercentage > 0.0f && RotationPercentage < 1.0f) {

            RotateY((RotationRate * (float)delta) * DepthModifier);

        }

    }

    public void Interact() {

        SetProcess(true);

    }

    public void Uninteract() {

        DepthModifier *= -1;
        SetProcess(false);

    }

    public void ShowInteract() {

        InteractLabel.Visible = true;

    }

    public void HideInteract() {

        InteractLabel.Visible = false;

    }

}