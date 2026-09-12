using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class PathfindingAI : CharacterBody3D {

    [Export(PropertyHint.None, "suffix:m")] private float RandomPathDistance = 5.0f;

    protected virtual float Speed { get; } = 3.0f;
    protected virtual float ActionCooldownTimer { get; set; } = 1.5f;
    protected virtual float NavigatorCooldown { get; } = 1.0f;
    private float NavigatorCooldownTimer = 0.0f;

    [Export] protected Node3D TempTarget = null;
    [Export] protected Node3D Target {

        get => TargetBuffer;

        set {

            TargetBuffer = value;

            if (value is not null)
                Navigator.TargetPosition = value.GlobalPosition;

        }

    }
    private Node3D TargetBuffer = null;
    [Export] protected NavigationAgent3D Navigator = null;


    public override void _PhysicsProcess(double delta) {

        if (!IsOnFloor()) {
            Velocity += GetGravity() * (float)delta;
        }

        MoveAndSlide();
    }

    protected void Moving(double delta) {

        if (Navigator.TargetPosition != Target.GlobalPosition)
            Navigator.TargetPosition = Target.GlobalPosition;
            
        Vector3 Direction = (Navigator.GetNextPathPosition() - GlobalPosition);
        Navigator.Velocity = Direction.Normalized() * Speed;
        Direction.Y = 0.0f;
        GlobalRotation = RotateTowards(GlobalRotation, Direction, 4.0f * (float)delta);

    }


    // u/DaWeedM (n.d.).
    // How to make an object always “look at” the camera without using lookat().
    // Reddit.
    // Available at: https://www.reddit.com/r/godot/comments/1gdire7/how_to_make_an_object_always_look_at_the_camera
    // [Accessed 11 Sept. 2026].
    
    protected Vector3 RotateTowards(Vector3 _currentRotation, Vector3 _direction, float _lerpValue) {
    
        if (_direction.LengthSquared() == 0) 
            return _currentRotation;

        float yRotation = Mathf.LerpAngle(_currentRotation.Y, Mathf.Atan2(_direction.X, _direction.Z), _lerpValue);
        Vector3 rotationSmoothed = new Vector3(_currentRotation.X, yRotation, _currentRotation.Z);
        return rotationSmoothed;
    
    }

    protected void SetRandomTargetLocation() {

        TempTarget.Reparent(this);
        Vector3 RandomPoint = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, true);
        TempTarget.GlobalPosition = RandomPoint; 
        Target = TempTarget;
        TempTarget.Reparent(GameManager.Instance);

    }

    public virtual void NavigationFinished() {}

    public void NavigatorVelocitySet(Vector3 SafeVelocity) {

        if (IsOnFloor())
            Velocity = Navigator.Velocity.MoveToward(SafeVelocity, 0.25f);
    
    }

}