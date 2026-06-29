using Godot;
using System;

public partial class ControlRod : Node3D, ValveInterface {

    [Export] private float Modifier = 0.25f;
    [Export] private float RodDepth = 0.0f;
    [Export] private float NewRodDepth = 0.0f;

    private const float MaxRodDepth = -4.0f;
    private const float MinRodDepth = 0.0f;
    private const float RodSpeed = 2.0f;
    private bool IsMoving = false;
    private Vector3 CurrentPosition;

    public float TemperatureReduction {

        get {

            return Modifier * RodDepth;

        }

    }

    public override void _Ready() {

        CurrentPosition = Position;
        SetProcess(false);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {

        if (RodDepth != NewRodDepth) {

            float DepthTarget = MaxRodDepth * NewRodDepth;

            float weight = 1.0f - Mathf.Exp(-RodSpeed * (float)delta);
            Position = Position.Lerp(new Vector3(CurrentPosition.X, DepthTarget, CurrentPosition.Z), weight);
            RodDepth = Mathf.Clamp(Position.Y / MaxRodDepth, 0.0f, 1.0f);

            // Snap to the desired position when the distance between them is negligable,
            // to avoid needless interpolation calculations.

            if ((RodDepth * NewRodDepth) > 0.999) {

                Position = new Vector3(CurrentPosition.X, DepthTarget, CurrentPosition.Z);
                RodDepth = NewRodDepth;
                IsMoving = false;
                SetProcess(false);

            }

        }

    }

    public void HandleValveValue(float ValveValue) {

        NewRodDepth = ValveValue;

        if (!IsMoving) {

            SetProcess(true);
            IsMoving = true;
        
        }
    
    }


}