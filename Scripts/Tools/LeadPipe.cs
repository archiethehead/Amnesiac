using Godot;
using System;

public partial class LeadPipe : ToolBase {

    public override ToolBitMask ToolID { get; protected set; } = ToolBitMask.LeadPipe;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
    }

    public override void PrimaryAction() {
        GD.Print("SWING");
    }

    public override void SecondaryAction() {
        GD.Print("BLOCK");
    }

    public override void TertiaryAction() {
        GD.Print("DUNNO");
    }
}