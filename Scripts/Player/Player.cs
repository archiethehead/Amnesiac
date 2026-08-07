using Godot;
using System;

public partial class Player : CharacterBody3D, Hitable {


    // Constants
    private const float JumpVelocity = 6.0f;
    private const float Sensitivity = 0.003f;
    private const float ConstSpeed = 5.0f;
    private const float StaminaLossRate = 0.2f;
    private const float StaminaGainRate = StaminaLossRate / 2.0f;
    private const float FallDamageMultiplier = 20.0f;
    private const float FallDamageThreshold = FallDamageMultiplier * 1.0f; // <--- The number of seconds
                                                                           // until fall damage applies.

    // Gameplay Stats
    private float Health = 100.0f;
    private float Stamina = 1.0f;
    private float StaminaCooldown = 1.0f;
    private float Speed = ConstSpeed;
    private float FallDamage = 0.0f;

    // Bools
    private bool Interacting = false;
    private bool NoClip = false;
    private bool Falling = false;
    private bool IsRunning = false;
    private bool IsExhausted = false;

    // Tools
    private ToolBase EquippedTool = null;
    private Interactable Interactable;

    // Item throw variables
    private const float MaxThrowForce = 60.0f;
    private const float MaxArc = 30.0f;
    private const float MaxTorque = 20.0f;
    private const float TimeToMax = 2.0f;
    private float ThrowForce = 0.0f;
    private float Arc = 0.0f;
    private float Torque = 0.0f;
    private bool Throwing = false;

    // Exports
    private GameManager GameManager = null;
    [Export] private Camera3D Camera = null;
    [Export] private RayCast3D RayCast = null;
    [Export] private Marker3D ToolPos = null;   
    [Export] private CanvasLayer HUD = null;
    [Export] private CollisionShape3D Collider = null;

    // Camera Physics
    [Export] private RigidBody3D CameraPhysics = null;
    [Export] private CollisionShape3D CameraCollider = null;

    // Interfaces
    public bool IsHittable { get; protected set; } = true;
    public bool IsDestroyed { get; protected set; } = false;
    public float BreakSpeed { get; protected set; } = 5.0f;

    public override void _Ready() {

        GameManager = GameManager.Instance;
        GameManager.Player = this;
        RayCast.AddException(this);
        Input.MouseMode = Input.MouseModeEnum.Captured;

    }

    public override void _Process(double delta) {

        GD.Print("Health: ", Health);

        if (!IsExhausted && !NoClip) {

            switch (IsRunning) {

                case true:
                    Stamina -= StaminaLossRate * (float)delta;
                    break;

                case false:
                    Stamina += StaminaGainRate * (float)delta;
                    break;

            }

            Stamina = Mathf.Clamp(Stamina, 0.0f, 1.0f);

        }

        else {

            StaminaCooldown -= 1.0f * (float)delta;

            if (StaminaCooldown <= 0.0f) {

                StaminaCooldown = 1.0f;
                IsExhausted = false;
            
            }

        }

        if (Input.IsActionPressed(InputMap.SpeedUp) && !IsExhausted) {

            Speed = ConstSpeed * 2;
            IsRunning = true;

            if (Stamina <= 0.0f) {

                IsExhausted = true;

            }

        }

        else {

            Speed = ConstSpeed;
            IsRunning = false;

        }

        RayCast.ForceRaycastUpdate();
        if (RayCast.IsColliding() && RayCast.GetCollider() is Interactable i && i.IsInteractable) {

            if (i != Interactable) {

                if (Interactable is not null) {

                    Interactable.HideInteract();
                    Interactable.Uninteract();
                
                }

                Interactable = i;
                Interactable.ShowInteract();

            }

        }

        else if (Interactable is not null || (Interactable is not null && !Interactable.IsInteractable)) {

            Interactable.HideInteract();
            Interactable.Uninteract();
            Interactable = null;


        }

        if (Throwing) {

            if (ThrowForce < 60.0f) {

                ThrowForce += (float)((MaxThrowForce / TimeToMax) * delta);
                Arc += (float)((MaxArc / TimeToMax) * delta);
                Torque += (float)((MaxTorque / TimeToMax) * delta);

            }

            else if (ThrowForce > 60.0f) {

                Mathf.Clamp(ThrowForce, 0.0f, MaxThrowForce);
                Mathf.Clamp(Arc, 0.0f, MaxArc);
                Mathf.Clamp(Torque, 0.0f, MaxTorque);

            }

        }

    }

    public override void _PhysicsProcess(double delta) {

        Vector2 inputDir = Input.GetVector(

                                    InputMap.Left,
                                    InputMap.Right,
                                    InputMap.Forward,
                                    InputMap.Backward

                                    );

        if (NoClip) {


            Vector3 Forward = Camera.GlobalTransform.Basis.Z;
            Vector3 Right = Camera.GlobalTransform.Basis.X;
            Vector3 Direction = (Right * inputDir.X + Forward * inputDir.Y).Normalized();

            this.GlobalPosition += (Direction * (Speed * 2.0f)) * (float)delta;
            return;

        }

        Vector3 velocity = Velocity;

        if (!IsOnFloor()) {

            velocity += GetGravity() * (float)delta;
            Falling = true;
            FallDamage += FallDamageMultiplier * (float)delta;

        }

        else if (Falling) {

            Falling = false;

            if (FallDamage >= FallDamageThreshold) {

                Hit(FallDamage);
            
            }

            FallDamage = 0.0f;
        
        }

        // Handle Jump.
        if (Input.IsActionJustPressed(InputMap.Jump) && IsOnFloor()) {
            velocity.Y = JumpVelocity;
        }
                
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

        else if (@event.IsActionReleased(InputMap.Interact) && Interacting) {

            Interacting = false;

            if (Interactable != null) {

                Interactable.Uninteract();

            }

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

                Throwing = true;

            }

            else if (@event.IsActionReleased(InputMap.Drop)) {

                GameManager.Drop(EquippedTool);

                Vector3 Forward = -Camera.GlobalTransform.Basis.Z.Normalized();
                Vector3 ThrowDirection = (Forward * ThrowForce) + (Vector3.Up * Arc);
                Vector3 LocalXAxis = EquippedTool.GlobalTransform.Basis.X;

                EquippedTool.ApplyCentralImpulse(ThrowDirection);
                EquippedTool.AngularVelocity = LocalXAxis * (-Torque);

                EquippedTool = null;
                Throwing = false;
                ThrowForce = 0.0f;
                Arc = 0.0f;
                Torque = 0.0f;

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
            newRotation.X = Math.Clamp(Camera.Rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
            Camera.SetRotation(newRotation);

        }

    }


    public void Pause() {

        HUD.Visible = false;
    
    }

    public void Unpause() {

        HUD.Visible = true;

    }

    public void ToggleNoClip() {

        this.Velocity = Vector3.Zero;
        NoClip = !NoClip;
        Collider.Disabled = NoClip;

    }

    public void Pickup(ToolBase Tool) {

        Tool.HitCast = RayCast;
        Tool.Reparent(ToolPos);
        EquippedTool = Tool;

    }

    public void Die() {

        if (EquippedTool is not null) {

            GameManager.Drop(EquippedTool);
            EquippedTool = null;
        
        }

        CameraPhysics.Freeze = false;
        CameraCollider.Disabled = false;
        CameraPhysics.Reparent(GetTree().Root);
        Collider.Disabled = true;
        this.Visible = false;
        this.ProcessMode = ProcessModeEnum.Disabled;
        this.SetPhysicsProcess(false);

    }

    public void Hit(float damage) {
        
        Health -= damage;
        Health = Mathf.Clamp(Health, 0.0f, 100.0f);

        if (Health == 0.0f) {

            IsDestroyed = true;
            Die();
        
        }

    }
}