using Godot;
using System;

public partial class Player : CharacterBody3D {

    private const float Speed = 5.0f;
    private const float JumpVelocity = 6.0f;
    private const float Sensitivity = 0.003f;
    private bool Interacting = false;
    private ToolBase EquippedTool = null;
    private Interactable Interactable;

    private GameManager GameManager = null;
    [Export] private Camera3D Camera = null;
    [Export] private RayCast3D RayCast = null;
    [Export] private Marker3D ToolPos = null;
    [Export] private CanvasLayer HUD = null;


    public override void _Ready() {

        GameManager = GameManager.Instance;
        GameManager.Player = this;
        Input.MouseMode = Input.MouseModeEnum.Captured;

    }

    public override void _Process(double delta) {

        RayCast.ForceRaycastUpdate();
        if (RayCast.IsColliding() && RayCast.GetCollider() is Interactable i && i.IsInteractable) {

            if (RayCast.GetCollider() != Interactable) {

                Interactable = i;
                Interactable.ShowInteract();

            }

        }

        else if (Interactable is not null || (Interactable is not null && !Interactable.IsInteractable)) {

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
        if (Input.IsActionJustPressed(InputMap.Jump) && IsOnFloor()) {

            velocity.Y = JumpVelocity;
        
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector(

                                            InputMap.Left,
                                            InputMap.Right,
                                            InputMap.Forward,
                                            InputMap.Backward

                                          );

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

        if (@event.IsActionPressed(InputMap.Interact) && Interactable != null && !Interacting) {

            Interacting = true;
            Interactable.Interact();

        }

        else if (Interacting && (@event.IsActionReleased(InputMap.Interact) && Interactable != null)) {

            Interacting = false;
            Interactable.Uninteract();

        }

        else if (@event.IsActionPressed(InputMap.Pause)) {

            HUD.Visible = false;
            GameManager.Pause();

        }

        else if (EquippedTool != null) {

            if (@event.IsActionPressed(InputMap.Primary)) {

                EquippedTool.PrimaryAction();

            }

            else if (@event.IsActionPressed(InputMap.Secondary)) {

                EquippedTool.SecondaryAction();

            }

            else if (@event.IsActionPressed(InputMap.Tertiary)) {

                EquippedTool.TertiaryAction();

            }

            else if (@event.IsActionPressed(InputMap.Drop)) {

                GameManager.Drop(EquippedTool);
                EquippedTool = null;

            }

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

    public void Unpause() {

        HUD.Visible = true;

    }

    public void Pickup(ToolBase Tool) {
        
        Interacting = false;
        Interactable.Uninteract();
        Tool.Reparent(ToolPos);
        EquippedTool = Tool;

    }

}