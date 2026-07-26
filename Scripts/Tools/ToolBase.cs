using Godot;
using System;

public partial class ToolBase : RigidBody3D, Interactable
{
    public bool IsInteractable { get; protected set; } = true;
    [Export] public MeshInstance3D MeshInstance { get; protected set; } = null;

    [Export] CollisionShape3D Collider = null;

    private GameManager GameManager = null;

    public override void _Ready() {

        GameManager = GameManager.Instance;

    }

    public void Equip() {

        Collider.Disabled = true;
    
    }

    public void Unequip() { }

    public virtual void PrimaryAction() { }
    public virtual void SecondaryAction() { }
    public virtual void TertiaryAction() { }

    public void Interact() {

        IsInteractable = false;
        GameManager.PickUp(this);
        Equip();
        GD.Print("you picked me up");

    }

}