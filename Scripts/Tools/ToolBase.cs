using Godot;
using System;

public enum ToolBitMask { 

    LeadPipe = 1 << 0

}

public partial class ToolBase : RigidBody3D, Interactable
{

    public virtual ToolBitMask ToolID { get; protected set; }

    public bool IsInteractable { get; protected set; } = true;
    [Export] public MeshInstance3D MeshInstance { get; protected set; } = null;
    [Export] private CollisionShape3D Collider = null;

    private GameManager GameManager = null;

    public sealed override void _Ready() {

        GameManager = GameManager.Instance;

    }

    public void Equip() {

        IsInteractable = false;
        Collider.Disabled = true;
        GD.Print("you picked me up");

    }

    public void Unequip() { }

    public virtual void PrimaryAction() { }
    public virtual void SecondaryAction() { }
    public virtual void TertiaryAction() { }

    public void Interact() {

        GameManager.PickUp(this);

    }

}