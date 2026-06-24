using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Reactor : StaticBody3D, Employee, Interactable {

    const float TemperatureIncreaseRate = 50.0f;
    public bool IsExploded;

    [Export] public float Temperature;
    [Export] public float MaxTemperature = 0.0f;
    [Export] GameManager GameManager = null;
    [Export] AreaLight3D Light = null;
    [Export] Label3D InteractLabel = null;

    public override void _Process(double delta) {

        if (InteractLabel.Visible == true) {

            Employee.CallManagerWithSelf(GameManager, InteractLabel, "FacePlayer");

        }

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

    public void ShowInteract() {

        if (!IsExploded) {

            InteractLabel.Visible = true;

        }
    
    }

    public void HideInteract() {

        InteractLabel.Visible = false;

    }

    public void ReactorExplode() {

        IsExploded = true;
        Light.LightColor = (new Color(0, 0, 0));
        this.SetProcess(false);
        HideInteract();
        GD.Print("BANG!");

    }

}