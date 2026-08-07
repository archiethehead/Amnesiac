using Godot;
using System;

public partial class EnemyAI : CharacterBody3D
{
	private const float Speed = 3.0f;
    private const float Damage = 30.0f;
    private const float JumpVelocity = 6.0f;
    private const float IdleMaxTime = 1.5f;
    private float IdleTimer = 0.0f;

    private enum EnemyState {
    
        Idle,
        WaitingToMove,
        Moving
    
    }
    private EnemyState State = EnemyState.Idle;
    [Export] public Node3D Target;
    [Export] private NavigationAgent3D Navigator = null;


	public override void _PhysicsProcess(double delta)
	{

		if (!IsOnFloor())
		{
			Velocity += GetGravity() * (float)delta;
		}

        switch (State) { 
        
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.WaitingToMove:
                WaitingToMove((float)delta);
                break;

            case EnemyState.Moving:
                Moving();
                break;


        }

		MoveAndSlide();
	}

    private void Idle() {

        Velocity = Vector3.Zero;
        
        if (Target is not null) {
        
            IdleTimer = IdleMaxTime;
            State = EnemyState.WaitingToMove;
        
        }

    }

    private void WaitingToMove(float delta) {

        IdleTimer -= 1.0f * delta;

        if (IdleTimer <= 0.0f) {

            State = EnemyState.Moving;
        
        }
    
    }

    private void Moving() {

        if (IsOnWall() && IsOnFloor()) {

            Vector3 velocity = Velocity;
            velocity.Y = JumpVelocity;
            Velocity = velocity;
        
        }

        Navigator.TargetPosition = Target.GlobalTransform.Origin;
        Vector3 CurrentPosition = this.GlobalTransform.Origin;
        Vector3 NextPosition = Navigator.GetNextPathPosition();
        Vector3 Direction = (NextPosition - CurrentPosition).Normalized();
        Velocity = Direction * Speed;


    }

    public void TargetReached() {

        if (Target is Hitable h) {

            h.Hit(Damage);

            if (h.IsDestroyed) {

                Target = null;
            
            }

        }

        State = EnemyState.Idle;
    
    }

}
