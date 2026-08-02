using Godot;
using One.Woolly.VoronoiShatter.CSVoronoiAdapter;
using System;
using System.Runtime.CompilerServices;

public partial class DestructibleBase : Node3D, Hitable {

    [Export] Node3D ShatteredMesh = null;
    [Export] MeshInstance3D Mesh = null;
    [Export] CollisionShape3D Collider = null;
    [Export] MeshInstance3D ShatteredMeshPiece = null;
    Material MeshMaterial = null;
    private bool IsShattered = false;
    private float Timer = 0.0f;
    private const float TransparencyRate = 1.0f / 2.5f; // <---- The divisor of this is the number of seconds
                                                        // for the fadeout to finish (or be capped).

    public bool IsHittable { get; protected set; } = true;

    public override void _Process(double delta) {

        if (IsShattered) { 

            Timer += (float)delta;

        }

        if (Timer > 3.0f && MeshMaterial is BaseMaterial3D BaseMeshMaterial3D) {

            BaseMeshMaterial3D.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
            Color NewTransparency = BaseMeshMaterial3D.AlbedoColor;
            NewTransparency.A -= TransparencyRate * (float)delta;
            NewTransparency.A = Mathf.Clamp(NewTransparency.A, 0.0f, 1.0f);
            BaseMeshMaterial3D.AlbedoColor = NewTransparency;

            if (NewTransparency.A == 0.0f || Timer == 10.0f) {

                ShatteredMesh.QueueFree();
                this.QueueFree();
            
            }

        }

    }

    public override void _Ready() {

        // The shattered mesh/piece can't be set and by extension guarenteeed
        // as non-null in the base class, so a check for that at
        // instantiation is necessary to prevent errors.
        //
        // |
        // |
        // V
        
        if (ShatteredMesh is null || ShatteredMeshPiece is null) {

            this.QueueFree();
            GD.PrintErr("Destructible ShatteredMesh or ShatteredMeshPiece not set :(");
            return;
        
        }

        MeshMaterial = ShatteredMeshPiece.GetActiveMaterial(0);
        ShatteredMesh.ProcessMode = ProcessModeEnum.Disabled;

    }

    public void Shatter() {

        if (!IsShattered) {

            ShatteredMesh.ProcessMode = ProcessModeEnum.Pausable;
            ShatteredMesh.Reparent(GetTree().Root);
            ShatteredMesh.Visible = true;
            this.Visible = false;
            Collider.Disabled = true;
            IsShattered = true;

        }

    }

    void Hitable.Hit() {

        Shatter();
        IsHittable = false;

    }

}
