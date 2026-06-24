using Godot;
using System;

public partial class Player : CharacterBody3D, Employee {

    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;
    public const float Sensitivity = 0.003f;

    [Export] GameManager GameManager;
    [Export] Camera3D Camera = null;

    public override void _Ready() {

        Input.MouseMode = Input.MouseModeEnum.Captured;

    }

    public override void _PhysicsProcess(double delta) {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor()) {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("Spacebar") && IsOnFloor()) {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("A", "D", "W", "S");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero) {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public override void _UnhandledInput(InputEvent @Event) {

        if (@Event is InputEventMouseMotion) {

            InputEventMouseMotion MouseMotionEvent = @Event as InputEventMouseMotion;

            Vector2 RelativePosition = MouseMotionEvent.GetRelative();
            float RelativeX = RelativePosition.X * -1;
            float RelativeY = RelativePosition.Y * -1;

            RotateY(RelativeX * Sensitivity);
            Camera.RotateX(RelativeY * Sensitivity);

            Vector3 newRotation = Camera.GetRotation();
            newRotation.X = Math.Clamp(Camera.Rotation.X, Mathf.DegToRad(-60), Mathf.DegToRad(70));
            Camera.SetRotation(newRotation);

        }

    }

}