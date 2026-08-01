using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Console : CanvasLayer
{

    private GameManager GameManager = null;
    private string[] Args = null;
    [Export] LineEdit LineEdit = null;
    private System.Collections.Generic.Dictionary<string, Action> CommandDict = new System.Collections.Generic.Dictionary<string, Action>();
    private bool FirstFrame = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

        GameManager = GameManager.Instance;
        GameManager.Console = this;
        LineEdit.ClearButtonEnabled = true;
        LineEdit.KeepEditingOnTextSubmit = true;
        this.ProcessMode = ProcessModeEnum.Disabled;

        CommandDict.Add("help", Help);
        CommandDict.Add("inst", Inst);

    }

    public override void _Process(double delta) {

        if (FirstFrame) LineEdit.Clear(); FirstFrame = false;

        if (Input.IsActionPressed(InputMap.Pause)) {

            LineEdit.Clear();
            GameManager.CloseConsole();

        }

        else if (Input.IsActionPressed(InputMap.Submit)) {

            string RawCommand = LineEdit.Text;
            string[] args = RawCommand.Split(' ');

            if (args.Length == 0) return;
            args[0] = args[0].ToLower();
            Args = args;

            if (!CommandDict.ContainsKey(args[0])) return;
            CommandDict[args[0]]();

            LineEdit.Clear();

        }

    }

    public void ConsoleOn() {

        GameManager.MouseMode = Input.MouseMode;
        Input.MouseMode = Input.MouseModeEnum.Confined;
        Show();
        this.ProcessMode = ProcessModeEnum.Always;
        LineEdit.GrabFocus();

    }

    public void ConsoleOff() {

        Input.MouseMode = GameManager.MouseMode;
        Hide();
        this.ProcessMode = ProcessModeEnum.Disabled;
        FirstFrame = true;

    }

    // Commands

    public void Help() {

        GD.Print("test");
    
    }

    public void Inst() {

        string FilePath = "res://Objects/Tools/" + Args[1] + ".tscn";
        int loop = 1;

        if (Args.Length > 2) {

            loop = Args[2].ToInt();
        
        }

        PackedScene Tool = GD.Load<PackedScene>(FilePath);

        if (Tool == null) return;


        for (int i = 0; i < loop; i++) {

            Node Instance = Tool.Instantiate();
            GetTree().Root.AddChild(Instance);

            if (Instance is Node3D n) {

                n.GlobalPosition = GameManager.Player.GlobalPosition;

            }

        }

    }
	
}
