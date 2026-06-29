using Godot;
using System;

public partial class Player : CharacterBody3D {

    private const float Speed = 5.0f;
    private const float JumpVelocity = 4.5f;
    private const float Sensitivity = 0.003f;
    private bool Interacting = false;
    private Interactable Interactable;

    [Export] GameManager GameManager = null;
    [Export] Camera3D Camera = null;
    [Export] RayCast3D RayCast = null;

    public override void _Ready() {

        Input.MouseMode = Input.MouseModeEnum.Captured;

    }

    public override void _Process(double delta) {

        RayCast.ForceRaycastUpdate();
        if (RayCast.IsColliding() && RayCast.GetCollider() is Interactable i) {

            if (RayCast.GetCollider() != Interactable) {

                Interactable = i;
                Interactable.ShowInteract();

            }

        }

        else if (Interactable is not null) {

            Interactable.HideInteract();
            Interactable.Uninteract();
            Interactable = null;


        }

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

    public override void _Input(InputEvent @event) {

        if (@event.IsActionPressed("E") && Interactable != null && !Interacting) {

            Interacting = true;
            Interactable.Interact();

        }

        else if (Interacting && (@event.IsActionReleased("E") && Interactable != null)) {

            Interacting = false;
            Interactable.Uninteract();

        }

        else if (@event.IsActionPressed("Escape")) {

            GameManager.Pause();

        }

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