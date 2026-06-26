using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Reactor : Node3D, Employee {

    const float TemperatureIncreaseRate = 50.0f;
    public bool IsExploded;

    [Export] public float Temperature = 0.0f;
    [Export] public float MaxTemperature = 0.0f;
    [Export] GameManager GameManager = null;
    [Export] OmniLight3D Light = null;
    [Export] public Array<ControlRod> ControlRods = new();

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

        else if (Temperature > MaxTemperature) {

            Employee.CallManager(GameManager, this, "ReactorExplode");

        }

    }

    public void ApplyTemperatureChange() { 
    

    
    }

    public void ReactorExplode() {

        IsExploded = true;
        Light.LightColor = (new Color(0, 0, 0));
        this.SetProcess(false);
        GD.Print("BANG!");

    }

}