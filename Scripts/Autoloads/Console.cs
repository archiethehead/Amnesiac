using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

public partial class Console : CanvasLayer {

    private struct Command {

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
    public override void _Ready() {

        System.Console.SetOut(ConOut);
        GameManager = GameManager.Instance;
        GameManager.Console = this;
        LineEdit.ClearButtonEnabled = true;
        LineEdit.KeepEditingOnTextSubmit = true;
        this.ProcessMode = ProcessModeEnum.Disabled;

        CommandDict.Add("help", new Command("Outputs each command and it's respective arguments.", Help));
        CommandDict.Add("clear", new Command("Clears the output window.", Clear));
        CommandDict.Add("exit", new Command("Kills the master process, exiting the game", GameManager.Exit));
        CommandDict.Add("inst", new Command("<-c Category, -n Name> [-q Quantity] Instansiates an object into the scene.", Inst));
        CommandDict.Add("noclip", new Command("Toggles no-clip.", NoClip));
        CommandDict.Add("setweather", new Command("<-t Type> Changes the weather to the specified type", ChangeWeather));
        CommandDict.Add("setwindow", new Command("<-f/-w> Sets the window mode between fullscreen and windows", Screen));

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

            if (RawCommand == "") return;

            string[] args = RawCommand.Split(' ');

            CommandHistory.Record(RawCommand);

            args[0] = args[0].ToLower();
            Args = args;

            GetOpt.Reset();
            LineEdit.Clear();

            if (!CommandDict.ContainsKey(args[0])) {

                Error = true;
                ConsoleOut("{0} is not a recognised command, try 'help'", args[0]);
                return;

            }

            ConsoleOut("[i] > {0}[/i]", RawCommand);
            CommandDict[args[0]].Function();

        }

        else if (Input.IsActionJustPressed(InputMap.Up)) {

            LineEdit.Clear();
            LineEdit.InsertTextAtCaret(CommandHistory.Get(true));

        }

        else if (Input.IsActionJustPressed(InputMap.Down)) {

            LineEdit.Clear();
            LineEdit.InsertTextAtCaret(CommandHistory.Get(false));

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

    private void OutputWindowOut() {

        Color BaseColour = OutputWindow.GetThemeColor("font_color");

        if (Error) {

            OutputWindow.AppendText("[color=red]Error: [/color]");
            Error = false;

        }

        OutputWindow.AppendText(ConOut.ToString());
        ConOut.GetStringBuilder().Clear();

    }

    private void ConsoleOut(string Text) {

        System.Console.WriteLine(Text);
        OutputWindowOut();

    }

    private void ConsoleOut(string Text, params object[] Objects) {

        System.Console.WriteLine(string.Format(Text, Objects));
        OutputWindowOut();

    }

    // Commands

    private void Help() {

        ConsoleOut("COMMANDS\n\n<> = Mandatory Argument(s)\n[] = Optional Argument(s)\nFilepath arguments are case-sentivie on *nix systems\n" +
                    "Arguments are not order-sensitive\nView verbose output with the '-v' flag on any command (if applicable).\n" +
                    "The Up and Down arrow can be used to navigate the command history.\n");

        string[] KeyList = CommandDict.Keys.ToList().ToArray();
        Command[] CommandList = CommandDict.Values.ToList().ToArray();

        for (int i = 0; i < KeyList.Length; i++) {

            ConsoleOut("{0,-15} {1}", (KeyList[i] + ": "), CommandList[i].Description);

        }

        ConsoleOut("\nExample Command: inst -c Tools -n LeadPipe -q 100 -v");

    }

    private void Clear() {

        OutputWindow.Text = "";

    }

    private void NoClip() {

        GameManager.Player.ToggleNoClip();

    }

    private void Inst() {

        int Opt;
        string Type = null;
        string Name = null;
        int Quantity = 1;

        while ((Opt = GetOpt.Parse(Args, "c:n:q:v")) != -1) {

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
                    ConsoleOut("{0} is an unrecognised argument", (char)GetOpt.OptOpt);
                    goto done;

            }

        done:;

        }

        if (Type is null) {

            Error = true;
            ConsoleOut("Object type unspecified");
            return;

        }

        else if (Name is null) {

            Error = true;
            ConsoleOut("Object name unspecified");
            return;

        }

        string FilePath = ("res://Objects/" + Type + "/" + Name + ".tscn");

        PackedScene Object = GD.Load<PackedScene>(FilePath);

        if (Object is null) {

            Error = true;
            ConsoleOut("{0} is not a valid filepath", FilePath);
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

    private void ChangeWeather() {

        int Opt;
        Weather.WeatherTypeEnum Type = 0;
        string StringType = null;

        while ((Opt = GetOpt.Parse(Args, "t:v")) != -1) {

            switch ((char)Opt) {

                case 't':
                    StringType = GetOpt.OptArg;

                    switch (StringType.ToLower()) {

                        case "rain":
                            Type = Weather.WeatherTypeEnum.Raining;
                            goto weather_selected;

                        case "sunny":
                            Type = Weather.WeatherTypeEnum.Sunny;
                            goto weather_selected;

                        default:
                            Error = true;
                            ConsoleOut("{0} is not a valid weather argument", StringType);
                            return;

                    }

                weather_selected:;

                    goto done;

                case 'v':
                    Verbose = true;
                    goto done;

                case '?':

                    Error = true;
                    ConsoleOut("{0} is an unrecognised argument", (char)GetOpt.OptOpt);
                    goto done;

            }

        done:;

        }


        if (Type == 0) {

            Error = true;
            ConsoleOut("Weather type not specified");
            return;

        }

        GameManager.ChangeWeather(Type);

        if (Verbose) ConsoleOut("Weather set to {0}", StringType);

    }

    public void Screen() {

        int Opt;
        bool Fullscreen = false;
        bool Set = false;

        while ((Opt = GetOpt.Parse(Args, "fwv")) != -1) {

            switch ((char)Opt) {

                case 'f':
                    Fullscreen = true;
                    Set = true;
                    goto done;

                case 'w':
                    Fullscreen = false;
                    Set = true;
                    goto done;

                case 'v':
                    Verbose = true;
                    goto done;

                case '?':
                    Error = true;
                    ConsoleOut("{0} is an unrecognised argument", (char)GetOpt.OptOpt);
                    goto done;

            }

        done:;

        }

        if (!Set) {

            Error = true;
            ConsoleOut("Argument must be specified (-f/-w)");
            return;
        
        }

        switch (Fullscreen) {

            case true:
                if (Verbose) ConsoleOut("Window mode set to fullscreen");
                GameManager.SetFullscreen(true);
                break;

            case false:
                if (Verbose) ConsoleOut("Window mode set to windowed");
                GameManager.SetFullscreen(false);
                break;
        
        }

    }

    private static class CommandHistory {

        private static List<string> CommandList = [""];
        private static int Index = 0;

        // Get the current index instead of shifting the index after
        // the console adds to the history.
        //
        // |
        // |
        // V

        private static bool FirstInput = true;
        private static bool VeryFirstInput = true;

        public static void Record(string RecordCommand) {

            CommandList[0] = RecordCommand;
            CommandList.Insert(0, "");

            if (!VeryFirstInput) Index++;

            FirstInput = true;

        }

        public static string Get(bool IsUp) {

            if (FirstInput) {

                VeryFirstInput = false;
                FirstInput = false;
                return CommandList[Index];

            }

            int Direction = -1;

            if (IsUp) {

                Direction = 1;

            }

            int NewIndex = Index + Direction;

            if (NewIndex >= CommandList.Count || NewIndex < 0) {

                return CommandList[Index];

            }

            Index = NewIndex;
            return CommandList[Index];


        }

    }

    private static class GetOpt {

        private static int OptIndex = 1;
        public static char OptOpt;
        private static bool OptReset = true;
        private static string Arg = null;
        private const char BadChar = '?';
        public static string OptArg { get; private set; }

        // This implementation differs slightly from the classic GNU/Linux
        // version of GetOpt. Optional arugments must be handled by the caller,
        // and you there's error logging and, therefore, no handling of error
        // logging args in ostring (OptionString).
        //
        // |
        // |
        // V

        public static int Parse(string[] Args, string OptionString) {

            if (Args is null || OptionString is null) return -1;

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

                else if (OptIndex + 1 < Args.Length) {

                    OptIndex++;
                    OptArg = Args[OptIndex];

                }

                else {

                    OptArg = null;

                }

                Arg = null;
                OptIndex++;

            }

            return OptOpt;

        }

        public static void Reset() {

            Console.Verbose = false;
            Arg = null;
            OptArg = null;
            OptReset = true;
            OptIndex = 1;

        }

    }

}