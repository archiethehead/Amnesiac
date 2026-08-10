using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public partial class DestructibleBase : Node3D, Hitable {

    [Export] Node3D ShatteredMesh = null;
    [Export] CollisionShape3D Collider = null;
    List<MeshInstance3D> ShatteredMeshArray = [];
    Material MeshMaterial = null;
    private bool IsShattered = false;
    private float Timer = 0.0f;
    private const float TransparencyRate = 1.0f / 2.5f; // <---- The divisor of this is the number of seconds
                                                        // for the fadeout to finish (or be capped).

    public bool IsHittable { get; protected set; } = true;
    public bool IsDestroyed { get; protected set; } = false;
    [Export] public float BreakSpeed { get; protected set; } = 0.0f;

    public override void _Process(double delta) {

        if (IsShattered) {

            Timer += (float)delta;

        }

        if (Timer > 3.0f) {

            foreach (GeometryInstance3D Node in ShatteredMeshArray) {

                Node.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
                Node.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
                Node.Transparency += TransparencyRate * (float)delta;

            }

            if (ShatteredMeshArray[0].Transparency >= 1.0f) {

                this.QueueFree();
                ShatteredMesh.QueueFree();

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

        if (ShatteredMesh is null) {

            this.QueueFree();
            GD.PrintErr("Destructible ShatteredMesh not set :(");
            return;

        }

        Godot.Collections.Array<Node> NodeArray = ShatteredMesh.GetChildren();

        foreach (Node node in NodeArray) {

            ShatteredMeshArray.Add((MeshInstance3D)node.GetChild(1));

        }

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

    void Hitable.Hit(float damage) {

        Shatter();
        IsDestroyed = true;
        IsHittable = false;

    }

}