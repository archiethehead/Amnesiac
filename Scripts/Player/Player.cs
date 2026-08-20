using System;
using Godot;

public partial class Player : CharacterBody3D, Hitable {


    // Gameplay Exports

    private const float Sensitivity = 0.003f;

    [ExportGroup("Movement")]

    [Export(PropertyHint.None, "suffix:m/s")] private float JumpVelocity = 5.5f;
    [Export(PropertyHint.None, "suffix:m/s")] private float GroundAcceleration = 10.0f;
    [Export(PropertyHint.None, "suffix:m/s")] private float GroundFriction = 4.0f;
    [Export(PropertyHint.None, "suffix:m/s")] private float WalkSpeed = 5.0f;
    [Export(PropertyHint.None, "suffix:m/s")] private float SprintSpeed = 7.5f;
    [Export(PropertyHint.None, "suffix:%/s")] private float StaminaLossRate = 20.0f;
    [Export(PropertyHint.None, "suffix:%/s")] private float StaminaGainRate = 10.0f;

    private float FrictionBuffer = 0.0f;
    private float AccelerationBuffer = 0.0f;

    private float Acceleration {

        get {

            if (CurrentState == PlayerState.Falling || CurrentState == PlayerState.Jumping) {

                AccelerationBuffer = Mathf.Lerp(AccelerationBuffer, AirAcceleration, 0.5f * (float)GetProcessDeltaTime());
                return AccelerationBuffer;
            

            }

            AccelerationBuffer = GroundAcceleration;
            return GroundAcceleration;
        
        }
    
    }

    private float Friction {

        get {

            if (CurrentState == PlayerState.Falling || CurrentState == PlayerState.Jumping) {

                FrictionBuffer = Mathf.Lerp(FrictionBuffer, AirFriction, 0.5f * (float)GetProcessDeltaTime());
                return FrictionBuffer;

            }

            FrictionBuffer = GroundFriction;
            return GroundFriction;
        }
            
    }

    [ExportGroup("Falling")]
    [Export(PropertyHint.Range, "0.0,100.0,or_greater,suffix:%")]
    private float FallSpeedIncreasePercentage {

        get;

        set => FallSpeed = 1 + (value / 100);

    } = 50.0f;

    private float FallSpeed = 1.5f;

    [Export(PropertyHint.None, "suffix:m/s")] private float AirAcceleration = 2.5f;
    [Export(PropertyHint.None, "suffix:m/s")] private float AirFriction = 1.0f;
    [Export(PropertyHint.None, "suffix:m/s")] private float MaxSafeFallSpeed = 15.0f;
    [Export(PropertyHint.None, "suffix:m/s")] private float FatalFallSpeed = 25.0f;


    // Gameplay Variables

    private enum PlayerState {

        Dead,
        Idle,
        Walking,
        Running,
        Jumping,
        Falling


    }

    private PlayerState CurrentState {

        get => StateBuffer;

        set {

            LastState = StateBuffer;
            StateBuffer = value;

        }

    }
    private PlayerState StateBuffer = PlayerState.Idle;
    private PlayerState LastState = PlayerState.Idle;

    private float Health = 100.0f;
    private float Stamina = 100.0f;
    private float StaminaCooldown = 1.0f;

    private float Speed {

        get {

            switch (CurrentState) {

                case PlayerState.Running:
                    return SprintSpeed;

                case PlayerState.Jumping:
                    if (LastState == PlayerState.Walking) return WalkSpeed * 1.5f;
                    return SprintSpeed * 1.1f;

                case PlayerState.Falling:
                    float HorizontalVelocity = new Vector2(Velocity.X, Velocity.Z).Length();
                    if (HorizontalVelocity > WalkSpeed) return HorizontalVelocity;
                    return WalkSpeed;

                default:
                    return WalkSpeed;

            }

        }

    }

    private float FallDamage = 0.0f;
    private bool Dead = false;
    private Vector3 PreviousVelocity;

    // Bools
    private bool Interacting = false;
    private bool NoClip = false;
    private bool IsExhausted {

        get => Stamina == 0.0f;
    
    }

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

    // Child Node Exports
    [ExportGroup("Child Nodes")]
    private GameManager GameManager = null;
    [Export] private Camera3D Camera = null;
    [Export] private RayCast3D RayCast = null;
    [Export] private Marker3D ToolPos = null;
    [Export] private CanvasLayer HUD = null;
    [Export] private CollisionShape3D Collider = null;
    [Export] private Label HealthLabel = null;
    [Export] private Label StaminaLabel = null;
    [Export] private RigidBody3D CameraPhysics = null;
    [Export] private CollisionShape3D CameraCollider = null;
    private Transform3D CameraPos;

    // Interfaces
    public bool IsHittable { get; protected set; } = true;
    public bool IsDestroyed { get; protected set; } = false;
    public float BreakSpeed { get; protected set; } = 5.0f;

    public override void _Ready() {

        HealthLabel.Text = string.Format("Health: {0}", Health.ToString());
        StaminaLabel.Text = string.Format("Stamina: {0}", (Mathf.Floor(Stamina)).ToString());
        GameManager = GameManager.Instance;
        GameManager.Player = this;
        RayCast.AddException(this);
        Input.MouseMode = Input.MouseModeEnum.Captured;

    }

    public override void _Process(double delta) {

        GameManager.DebugOut("Horizontal Velocity: {0}", new Vector2(Velocity.X, Velocity.Z).Length());

        if (!IsExhausted && !NoClip) {

            switch (CurrentState == PlayerState.Running) {

                case true:
                    Stamina -= StaminaLossRate * (float)delta;
                    break;

                case false:
                    Stamina += StaminaGainRate * (float)delta;
                    break;

            }

            Stamina = Mathf.Clamp(Stamina, 0.0f, 100.0f);
            StaminaLabel.Text = string.Format("Stamina: {0}", Mathf.Floor(Stamina).ToString());

            if (IsExhausted) CurrentState = PlayerState.Walking;

        }

        else {

            StaminaCooldown -= 1.0f * (float)delta;

            if (StaminaCooldown <= 0.0f) {

                Stamina += StaminaGainRate * (float)delta;
                StaminaCooldown = 1.0f;

            }

        }

        if (Input.IsActionPressed(InputMap.SpeedUp) && !IsExhausted && Speed < SprintSpeed) {

            CurrentState = PlayerState.Running;

        }

        else if (Input.IsActionJustReleased(InputMap.SpeedUp) || (IsExhausted && IsOnFloor())) {

            CurrentState = PlayerState.Walking;

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

        if (Dead) return;

        Vector2 inputDir = Input.GetVector(

                                    InputMap.Left,
                                    InputMap.Right,
                                    InputMap.Forward,
                                    InputMap.Backward

                                    );

        if (NoClip) {

            float NoClipSpeed = WalkSpeed;

            if (Input.IsActionPressed(InputMap.SpeedUp)) NoClipSpeed = WalkSpeed * 1.5f;

            Vector3 Forward = Camera.GlobalTransform.Basis.Z;
            Vector3 Right = Camera.GlobalTransform.Basis.X;
            Vector3 Direction = (Right * inputDir.X + Forward * inputDir.Y).Normalized();

            this.GlobalPosition += (Direction * (NoClipSpeed * 2.0f)) * (float)delta;
            return;

        }

        Vector3 velocity = Velocity;

        if (!IsOnFloor()) {

            if (velocity.Y > 0.0f) {

                if (CurrentState != PlayerState.Jumping) CurrentState = PlayerState.Falling;
                velocity += GetGravity() * (float)delta;

            }

            else {

                CurrentState = PlayerState.Falling;
                velocity += (GetGravity() * FallSpeed) * (float)delta;

            }

        }

        else if (CurrentState == PlayerState.Falling) {

            CurrentState = PlayerState.Walking;
            float AbsoluteY = PreviousVelocity.Y * -1;

            if (AbsoluteY > MaxSafeFallSpeed) {

                float Damage = (AbsoluteY - MaxSafeFallSpeed) * (100 / (FatalFallSpeed - MaxSafeFallSpeed));
                Hit(Damage);

            }

            FallDamage = 0.0f;

        }

        PreviousVelocity = velocity;

        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        Vector3 HorizontalVelocity = new Vector3(velocity.X, 0.0f, velocity.Z);

        if (direction != Vector3.Zero) {

            HorizontalVelocity = HorizontalVelocity.MoveToward(direction * Speed, Acceleration * (float)delta * 10);

        }

        else {

            HorizontalVelocity = HorizontalVelocity.MoveToward(Vector3.Zero, Friction * (float)delta * 10);

        }

        velocity.X = HorizontalVelocity.X;
        velocity.Z = HorizontalVelocity.Z;

        // Handle Jump.
        if (Input.IsActionJustPressed(InputMap.Jump) && IsOnFloor()) {

            CurrentState = PlayerState.Jumping;
            velocity.Y = JumpVelocity;

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

        if (CurrentState == PlayerState.Dead) return;

        CurrentState = PlayerState.Dead;

        if (EquippedTool is not null) {

            GameManager.Drop(EquippedTool);
            EquippedTool.LinearVelocity = PreviousVelocity;
            EquippedTool = null;

        }

        CameraPos = CameraPhysics.Transform;
        CameraPhysics.Freeze = false;
        CameraCollider.Disabled = false;
        CameraPhysics.Reparent(GetTree().Root);
        CameraPhysics.LinearVelocity = PreviousVelocity;
        Collider.Disabled = true;
        this.Visible = false;
        this.ProcessMode = ProcessModeEnum.Disabled;
        this.Velocity = Vector3.Zero;

    }

    public void Undie() {

        if (!(CurrentState == PlayerState.Dead)) return;
        CurrentState = PlayerState.Idle;

        this.GlobalPosition = CameraPhysics.GlobalPosition;
        CameraPhysics.Freeze = true;
        CameraCollider.Disabled = true;
        CameraPhysics.Reparent(this);
        CameraPhysics.Transform = CameraPos;
        Collider.Disabled = false;
        this.Visible = true;
        this.ProcessMode = ProcessModeEnum.Pausable;
        Dead = false;

    }

    public void Hit(float damage) {


        Health -= damage;
        Health = Mathf.Clamp(Health, 0.0f, 100.0f);
        Health = Mathf.Floor(Health);

        HealthLabel.Text = string.Format("Health: {0}", Health.ToString());

        if (Health == 0.0f) {

            IsDestroyed = true;
            Die();

        }

    }
}