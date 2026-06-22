using Godot;
using System;

public partial class Reactor : StaticBody3D {

    const float TemperatureIncreaseRate = 50.0f;
    [Export] public float Temperature;

    [Export] GameManager GameManager;
    [Export] AreaLight3D Light;

    public override void _Process(double delta) {

        Temperature += TemperatureIncreaseRate * (float)delta;

        if (Temperature < 250.0f) {

            Light.LightColor = (new Color(0, 255, 0));

        }

        else if (Temperature < 500.0f) {

            Light.LightColor = (new Color(255, 255, 0));

        }


        else if (Temperature < 750.0f) {

            Light.LightColor = (new Color(255, 0, 0));

        }

        else {

            if (GameManager != null) {

                GameManager.ReactorExplode();

            }

            else {

                ReactorExplode();
            
            }
        
        }

    }

    public void ReactorExplode() {

        Light.LightColor = (new Color(0, 0, 0));
        this.SetProcess(false);
        GD.Print("BANG!");
    
    }

}