using Godot;
using System;

public partial class LeadPipe : ToolBase {

    public override ToolBitMask ToolID { get; protected set; } = ToolBitMask.LeadPipe;
    public override float Range { get; protected set; } = 2.0f;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public override void PrimaryAction() {

        Vector3 ScaleBuffer = HitCast.Scale;
        HitCast.Scale = new Vector3(ScaleBuffer.X, Range, ScaleBuffer.Z);

        HitCast.ForceRaycastUpdate();
        if (HitCast.IsColliding() && HitCast.GetCollider() is Hitable h && h.IsHittable) {

            h.Hit();
        
        }

        HitCast.Scale = ScaleBuffer;

    }

    public override void SecondaryAction() {
        GD.Print("BLOCK");
    }

    public override void TertiaryAction() {
        GD.Print("DUNNO");
    }
}