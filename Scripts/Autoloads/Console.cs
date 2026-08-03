using Godot;
using System;
using System.Globalization;
using System.Linq;

public partial class Console : CanvasLayer
{

    public struct Command {

        public string Description;
        public Action Function;

        public Command(string D, Action F) : this() {

            this.Description = D;
            this.Function = F;

        }
    }

    [Export] LineEdit LineEdit = null;
    [Export] RichTextLabel OutputWindow = null;
    private GameManager GameManager = null;
    private string[] Args = null;
    private System.Collections.Generic.Dictionary<string, Command> CommandDict = new System.Collections.Generic.Dictionary<string, Command>();
    private bool FirstFrame = true;
    private static bool Verbose = false;
    private static bool Error = false;
    private System.IO.StringWriter ConOut = new System.IO.StringWriter();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

        System.Console.SetOut(ConOut);
        GameManager = GameManager.Instance;
        GameManager.Console = this;
        LineEdit.ClearButtonEnabled = true;
        LineEdit.KeepEditingOnTextSubmit = true;
        this.ProcessMode = ProcessModeEnum.Disabled;

        CommandDict.Add("help", new Command("Outputs each command and it's respective arguments.", Help));
        CommandDict.Add("clear", new Command("Clears the output window.", Clear));
        CommandDict.Add("inst", new Command("<-c Category, -n Name> [-q Quantity] Instansiates an object into the scene.", Inst));

    }

    public override void _Process(double delta) {

        if (FirstFrame) LineEdit.Clear(); FirstFrame = false;

        if (Input.IsActionPressed(InputMap.Pause)) {

            LineEdit.Clear();
            GameManager.CloseConsole();

        }

        else if (Input.IsActionJustPressed(InputMap.Submit)) {

            string RawCommand = LineEdit.Text;
            RawCommand = RawCommand.StripEdges();
            string[] args = RawCommand.Split(' ');

            if (args.Length == 0) return;
            args[0] = args[0].ToLower();
            Args = args;

            GetOpt.Reset();
            LineEdit.Clear();

            if (!CommandDict.ContainsKey(args[0])) {

                Error = true;
                ConsoleOut("{0} is not a recognised command", args[0]);
                return;
            
            }

            ConsoleOut(" > {0}", RawCommand);
            CommandDict[args[0]].Function();

        }

    }

    public void ConsoleOn() {

        GameManager.MouseMode = Input.MouseMode;
        Input.MouseMode = Input.MouseModeEnum.Confined;
        Show();
        this.ProcessMode = ProcessModeEnum.Always;
        LineEdit.GrabFocus();
        LineEdit.Flat = true;
        LineEdit.Clear();

    }

    public void ConsoleOff() {

        Input.MouseMode = GameManager.MouseMode;
        Hide();
        LineEdit.Clear();
        this.ProcessMode = ProcessModeEnum.Disabled;
        FirstFrame = true;

    }

    public void OutputWindowOut() {

        Color BaseColour = OutputWindow.GetThemeColor("font_color");

        if (Error) {

            OutputWindow.AppendText("[color=red]Error:   [/color]");
            Error = false;

        }

        OutputWindow.AppendText(ConOut.ToString());
        ConOut.GetStringBuilder().Clear();

    }

    public void ConsoleOut(string Text) {

        System.Console.WriteLine(Text);
        OutputWindowOut();

    }

    public void ConsoleOut(string Text, params object[] Objects) {

        System.Console.WriteLine(string.Format(Text, Objects));
        OutputWindowOut();

    }

    // Commands

    public void Help() {

        ConsoleOut( "COMMANDS\n\n<> = Mandatory Argument(s)\n[] = Optional Argument(s)\nFilepath arguments are case-sentivie on *nix systems\n" +
                    "Arguments are not order-sensitive\nView verbose output with the '-v' flag on any command (if applicable).\n");

        string[] KeyList = CommandDict.Keys.ToList().ToArray();
        Command[] CommandList = CommandDict.Values.ToList().ToArray();

        for (int i = 0; i < KeyList.Length; i++) {

            ConsoleOut("{0,-10} {1}", (KeyList[i] + ": "), CommandList[i].Description);
        
        }

    }

    public void Clear() {

        OutputWindow.Text = "";
    
    }

    public void Inst() {

        int Opt;
        string Type = null;
        string Name = null;
        int Quantity = 1;

        while ((Opt = GetOpt.Parse(Args, "c:n:q::v")) != -1) {

            switch ((char)Opt) {

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

                        Error = true;
                        ConsoleOut("Quantity must be a valid positive integer, defaulting to 1");
                    
                    }

                    goto done;

                case 'v':
                    Verbose = true;
                    goto done;

                case '?':

                    Error = true;
                    ConsoleOut("{0} is an unrecognised argument", (char)Opt);
                    goto done;
            
            }

        done:;
        
        }

        string FilePath = ("res://Objects/" + Type + "s/" + Name + ".tscn");

        PackedScene Object = GD.Load<PackedScene>(FilePath);

        if (Object is null) {

            Error = true;
            ConsoleOut("{0} is not a valid filepath",FilePath);
            return;
        
        }

        if (Verbose) ConsoleOut("Object at {0} loaded", FilePath);

        for (int i = 0; i < Quantity; i++) {

            Node Instance = Object.Instantiate();
            GetTree().Root.AddChild(Instance);

            if (Verbose) ConsoleOut("{0} instantiated into scene", Instance.ToString());

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
