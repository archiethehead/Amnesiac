
using Godot;
using System.ComponentModel;

public interface Interactable {

    public bool IsInteractable { get; }
    public MeshInstance3D MeshInstance { get; }

    public void Interact();

    public void Uninteract() {

        return;
    
    }

    public void ShowInteract() {

        Material MeshMaterial = MeshInstance.GetActiveMaterial(0);

        if (MeshMaterial is BaseMaterial3D MeshBaseMaterial3D) {

            MeshBaseMaterial3D.StencilMode = BaseMaterial3D.StencilModeEnum.Outline;
            MeshBaseMaterial3D.StencilColor = new Color(0xff, 0xff, 0xff);
            MeshBaseMaterial3D.StencilOutlineThickness = 0.005f;

        
        }

    }

    public void HideInteract() {


        Material MeshMaterial = MeshInstance.GetActiveMaterial(0);

        if (MeshMaterial is BaseMaterial3D MeshBaseMaterial3D) {

            MeshBaseMaterial3D.StencilMode = BaseMaterial3D.StencilModeEnum.Disabled;

        }

    }

}