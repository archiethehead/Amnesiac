using Godot;
using One.Woolly.VoronoiShatter.CSVoronoiAdapter;
using System;
using System.Runtime.CompilerServices;

public partial class DestructibleBase : Node3D
{

    [Export] Node3D ShatteredMesh = null;
    [Export] RigidBody3D PhysicsBody = null;
    [Export] CollisionShape3D Collider = null;
    private bool IsShattered = false;

    public override void _Ready() {

        // The shattered mesh can't be set, and by extension guarenteeed
        // as non-null, in the base class, so a check for that at
        // instantiation is necessary to prevent errors.
        //
        // |
        // |
        // V
        
        if (ShatteredMesh == null) {

            this.QueueFree();
            return;
        
        }

        ShatteredMesh.ProcessMode = ProcessModeEnum.Disabled;

    }

    public void Shatter() {

        if (!IsShattered) {

            ShatteredMesh.ProcessMode = ProcessModeEnum.Pausable;
            PhysicsBody.Visible = false;
            PhysicsBody.Freeze = true;
            Collider.Disabled = true;
            IsShattered = true;

        }

    }

}
