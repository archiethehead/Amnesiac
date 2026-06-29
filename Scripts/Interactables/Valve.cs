using Godot;
using Godot.Collections;
using System;

public interface ValveInterface {

    public void HandleValveValue(float ValveValue);

}

public partial class Valve : StaticBody3D, Interactable {

    private float RotationPercentage = 0.0f;
    private float DepthIncreaseRate = 0.5f;
    private float RotationRate = 3.0f;
    private int DepthModifier = 1;

    [Export] private Array<Node3D> LinkedValveObjects = new();
    [Export] private Label3D InteractLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {

        SetProcess(false);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {

        RotationPercentage += (DepthIncreaseRate * (float)delta) * DepthModifier;
        RotationPercentage = Mathf.Clamp(RotationPercentage, 0.0f, 1.0f);

        for (int i = 0; i < LinkedValveObjects.Count; i++) {

            if (LinkedValveObjects[i] is ValveInterface v) {

                v.HandleValveValue(RotationPercentage);

            }
        }

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