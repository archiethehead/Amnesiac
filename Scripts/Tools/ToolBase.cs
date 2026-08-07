using Godot;

public enum ToolBitMask {

    LeadPipe = 1 << 0

}

public interface Hitable {

    public bool IsHittable { get; }
    public bool IsDestroyed { get; }
    public float BreakSpeed { get; }
    public void Hit(float damage);

}

public partial class ToolBase : RigidBody3D, Interactable {

    public virtual ToolBitMask ToolID { get; protected set; }
    protected virtual float Range { get; set; }

    private bool IsDropped = false;
    private const float DropCooldownRate = 1.0f;
    private float DropCooldown = 2.0f;
    public bool IsInteractable { get; protected set; } = true;

    [Export] public MeshInstance3D MeshInstance { get; protected set; } = null;
    [Export] private CollisionShape3D Collider = null;
    public RayCast3D HitCast = null;

    private GameManager GameManager = null;

    public sealed override void _Ready() {

        GameManager = GameManager.Instance;

    }

    public override void _PhysicsProcess(double delta) {

        if (IsDropped) {

            DropCooldown -= DropCooldownRate * (float)delta;

            if (DropCooldown <= 0.0f) {

                IsDropped = false;
                DropCooldown = DropCooldownRate;

            }

        }

    }

    public void Pickup() {

        IsInteractable = false;
        Collider.Disabled = true;
        this.Position = Vector3.Zero;
        this.Rotation = Vector3.Zero;
        this.Freeze = true;
        GD.Print("you picked me up");

    }


    public void Drop() {

        IsDropped = true;
        IsInteractable = true;
        Collider.Disabled = false;
        this.Freeze = false;
        GD.Print("you dropped me up");
    }

    public void OnBodyEntered(Node Body) {

        if (Body is Player p && !IsDropped) {

            GameManager.CallDeferred(GameManager.MethodName.PickUp, this);

        }

        else if (Body is Hitable h) {

            float Speed = LinearVelocity.Length();

            if (Speed >= h.BreakSpeed) {

                h.Hit(0.0f);

            }

        }

    }

    public void Equip() { }

    public void Unequip() { }

    public virtual void PrimaryAction() { }
    public virtual void SecondaryAction() { }
    public virtual void TertiaryAction() { }

    public void Interact() {

        GameManager.PickUp(this);

    }


}