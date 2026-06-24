using Godot;
using System;

public partial class Reactor : StaticBody3D, Employee, Interactable {

    const float TemperatureIncreaseRate = 50.0f;
    [Export] public float Temperature;
    [Export] public float MaxTemperature = 0.0f;
    [Export] GameManager GameManager = null;
    [Export] AreaLight3D Light = null;

    public override void _Process(double delta) {

        Temperature += TemperatureIncreaseRate * (float)delta;

        if (Temperature < MaxTemperature * 0.25f) {

            Light.LightColor = (new Color(0, 255, 0));

        }

        else if (Temperature < MaxTemperature * 0.50f) {

            Light.LightColor = (new Color(255, 255, 0));

        }


        else if (Temperature < MaxTemperature * 0.75f) {

            Light.LightColor = (new Color(255, 0, 0));

        }

        else if (Temperature > MaxTemperature){

            Employee.CallManager(GameManager, this, "ReactorExplode");

        }

    }

    public void Interact() {

        GD.Print("You interacted with me!");

    }

    public void ReactorExplode() {

        Light.LightColor = (new Color(0, 0, 0));
        this.SetProcess(false);
        GD.Print("BANG!");

    }

}