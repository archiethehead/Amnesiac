using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            RawCommand = RawCommand.StripEdges();
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

        if (Tool is null) return;


        for (int i = 0; i < loop; i++) {

            Node Instance = Tool.Instantiate();
            GetTree().Root.AddChild(Instance);

            if (Instance is Node3D n) {

                n.GlobalPosition = GameManager.Player.GlobalPosition;

            }

        }

    }

    private static class GetOpt {

        private static int OptIndex = 1;
        private static char OptOpt;
        private static bool OptReset = true;
        private static string Arg = null;
        private const char BadChar = '?';
        private const char BadArg = ':';
        public static string OptArg { get; private set; }


        public static int Parse(string[] Args, string OptionString) {

            if (Args is  null || OptionString is null) return -1;

            if (OptReset || Arg is null) {

                OptReset = false;

                if (OptIndex >= Args.Length) {

                    Arg = null;
                    return -1;
                
                }

                Arg = Args[OptIndex];

                if (Arg is null || Arg[0] != '-') { 
                
                    Arg = null;
                    return -1;
                
                }

                if (Arg.Length > 1 && Arg[1] == '-') {

                    OptIndex++;
                    Arg = null;
                    return -1;
                
                }

                Arg = Arg.Substring(1);

            }

            OptOpt = Arg[1];
            Arg = Arg.Substring(1);

            int OptionIndex = OptionString.IndexOf(OptOpt);

            if (OptOpt == ':' || OptionIndex == -1) {

                if (Arg is null) OptIndex++;

                return BadChar;
            
            }

            if (OptionIndex++ >= OptionString.Length || OptionString[OptionIndex + 1] != ':') {

                OptArg = null;
                OptIndex++;

            }

            else {

                if (Arg is not null) {

                    OptArg = Arg;

                }

                else if (Args.Length <= OptIndex++) {

                    Args = null;
                    return BadChar;

                }

                else {

                    OptArg = Args[OptIndex];
                
                }

                Arg = null;
                OptIndex++;
            
            }

            return OptOpt;
        
        }

        public static void Reset() { 
        
            OptReset = true;
        
        }
    
    }

}
