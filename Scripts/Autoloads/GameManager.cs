using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node3D {

    public static GameManager Instance { get; private set; }
    public Inventory Inventory { get; set; } = null;
    public Player Player { get; set; } = null;
    public Reactor Reactor { get; set; } = null;
    public PauseMenu PauseMenu { get; set; } = null;
    public Console Console { get; set; } = null;
    public Weather Weather { get; set; } = null;
    public Input.MouseModeEnum MouseMode { get; set; }

    public override void _Ready() {

        SetFullscreen(false);
        Instance = this;

    }

    public override void _Input(InputEvent @event) {



        if (@event.IsActionPressed(InputMap.Pause)) {

            Player.Pause();
            Pause();

        }

        else if (@event.IsActionPressed(InputMap.Command)) {

            OpenConsole();

        }

    }

    public void SetFullscreen(bool IsFullscreen) { 

        switch (IsFullscreen) { 
        
            case true:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                break;

            case false:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                break;


        }
    
    }

    public void PickUp(ToolBase Tool) {

        if (!Inventory.IsItemInInventory((int)Tool.ToolID)) {

            Inventory.AddItemToInventory((int)Tool.ToolID);
            Player.Pickup(Tool);
            Tool.Pickup();

        }

    }

    public void Drop(ToolBase Tool) {

        Tool.Reparent(GetTree().Root);
        Tool.Drop();
        Inventory.RemoveItemFromInventory();

    }

    public void ReactorExplode() {

        if (Reactor != null) {

            Reactor.ReactorExplode();

        }

    }

    public void FacePlayer(Node3D Object) {

        Object.LookAt(Player.GlobalTransform.Origin, Vector3.Up, true);
        Object.GlobalRotation = new Vector3(0.0f, Object.GlobalRotation.Y, Object.GlobalRotation.Z);

    }

    public void ChangeWeather(Weather.WeatherTypeEnum NewWeatherType) {

        Weather.UpdateWeather(NewWeatherType);
    
    }

    public void OpenConsole() {

        GameSuspended(true);
        Console.ConsoleOn();

    }

    public void CloseConsole() {

        GameSuspended(false);
        Console.ConsoleOff();

    }

    public void GameSuspended(bool state) {

        GetTree().Paused = state;

        if (state) {

            Player.Pause();
            return;
        
        }

        Player.Unpause();

    }

    public void Pause() {

        GameSuspended(true);
        PauseMenu.Pause();

    }

    public void Unpause() {

        GameSuspended(false);
        Player.Unpause();

    }

    public void Exit() {

        GetTree().Quit();

    }

}