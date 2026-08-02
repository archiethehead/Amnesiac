using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

public partial class Console : CanvasLayer
{

    private GameManager GameManager = null;
    private string[] Args = null;
    [Export] LineEdit LineEdit = null;
    private System.Collections.Generic.Dictionary<string, Action> CommandDict = new System.Collections.Generic.Dictionary<string, Action>();
    private bool FirstFrame = true;
    private static bool Verbose = false;

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

            GetOpt.Reset();

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

        int opt;
        string Type = null;
        string Name = null;
        int Quantity = 1;

        while ((opt = GetOpt.Parse(Args, "c:n:q::v")) != -1) {

            switch ((char)opt) {

                case 'c':
                    Type = GetOpt.OptArg;
                    goto done;

                case 'n':
                    Name = GetOpt.OptArg;
                    goto done;

                case 'q':
                    if (GetOpt.OptArg.IsValidInt()) {

                        Quantity = GetOpt.OptArg.ToInt();

                    }

                    else {

                        throw new NotImplementedException();
                    
                    }

                    goto done;

                case 'v':
                    Verbose = true;
                    goto done;

                case '?':

                    throw new NotImplementedException();
            
            }

        done:;
        
        }

        string FilePath = ("res://Objects/" + Type + "s/" + Name + ".tscn");

        PackedScene Object = GD.Load<PackedScene>(FilePath);

        if (Object is null) throw new NotImplementedException();

        if (Verbose) GD.Print("yay");

        for (int i = 0; i < Quantity; i++) {

            Node Instance = Object.Instantiate();
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
        public static string OptArg { get; private set; }


        public static int Parse(string[] Args, string OptionString) {

            if (Args is  null || OptionString is null) return -1;

            if (OptReset || string.IsNullOrEmpty(Arg)) {

                OptReset = false;

                if (OptIndex >= Args.Length) {

                    Arg = null;
                    return -1;
                
                }

                Arg = Args[OptIndex];

                if (string.IsNullOrEmpty(Arg) || Arg[0] != '-') { 
                
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

            OptOpt = Arg[0];
            Arg = Arg.Substring(1);

            int ArgPos = OptionString.IndexOf(OptOpt);

            if (ArgPos == -1) {

                OptIndex++;

                return BadChar;
            
            }

            ArgPos++;
            if (ArgPos >= OptionString.Length || OptionString[ArgPos] != ':') {

                OptArg = null;
                OptIndex++;

            }

            else {

                if (!string.IsNullOrEmpty(Arg)) {

                    OptArg = Arg;

                }

                else if (OptIndex + 1 >= Args.Length) {

                    Args = null;
                    return BadChar;

                }

                else {

                    OptIndex++;
                    OptArg = Args[OptIndex];
                
                }

                Arg = null;
                OptIndex++;
            
            }

            return OptOpt;
        
        }

        public static void Reset() {

            Console.Verbose = false;
            OptReset = true;
            OptIndex = 1;
        
        }
    
    }

}
