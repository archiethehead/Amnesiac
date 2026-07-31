using Godot;
using System;

public enum ToolBitMask {

    LeadPipe = 1 << 0

}

public interface Hitable {

    public bool IsHittable { get; }
    public void Hit();

}

public partial class ToolBase : RigidBody3D, Interactable {

    public virtual ToolBitMask ToolID { get; protected set; }
    public virtual float Range { get; protected set; }

    public bool IsInteractable { get; protected set; } = true;
    [Export] public MeshInstance3D MeshInstance { get; protected set; } = null;
    [Export] private CollisionShape3D Collider = null;
    [Export] protected float SpeedHitThreshold = 0.0f;
    public RayCast3D HitCast = null;

    private GameManager GameManager = null;

    public sealed override void _Ready() {

        GameManager = GameManager.Instance;

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

        IsInteractable = true;
        Collider.Disabled = false;
        this.Freeze = false;
        GD.Print("you dropped me up");
    }

    public void OnBodyEntered(Node Body) {

        float Speed = LinearVelocity.Length();

        if (Speed >= SpeedHitThreshold && Body is Hitable h) {

            h.Hit();
        
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