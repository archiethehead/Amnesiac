using Godot;
using System;

public partial class ToolBase : RigidBody3D, Interactable
{
    public bool IsInteractable { get; protected set; } = true;
    [Export] public MeshInstance3D MeshInstance { get; protected set; } = null;

    private GameManager GameManager = null;

    public void PickUp() { }

    public void Equip() { }

    public void Unequip() { }

    public virtual void PrimaryAction() { }
    public virtual void SecondaryAction() { }
    public virtual void TertiaryAction() { }

    public void Interact() {

        IsInteractable = false;
        PickUp();
        Equip();
        GD.Print("you picked me up");
    }

}