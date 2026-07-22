using Godot;
using Godot.Collections;
using System;

public partial class Reactor : Node3D {

    private const float TemperatureIncreaseRate = 100.0f;
    private const float WaterDecreaseRate = 1.0f;
    private float MaxInternalWaterLevel = 30.0f;
    private float MinInternalWaterLevel = 0.0f;
    private float InternalWaterLevel = 30.0f;
    private float WaterCoolRate {

        get {

            return 33.3f * (InternalWaterLevel / MaxInternalWaterLevel);

        }

    }

    private GameManager GameManager = null;
    private bool IsExploded;

    [Export] public float Temperature;
    [Export] public float MaxTemperature = 0.0f;
    [Export] private OmniLight3D Light = null;
    [Export] private Array<ControlRod> ControlRods = new();

    public override void _Ready() {

        GameManager = GameManager.Instance;
        GameManager.Reactor = this;

    }

    public override void _Process(double delta) {

        ApplyTemperatureChange((float)delta);

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

            if (GameManager != null) {

                GameManager.ReactorExplode();

            }

            else {

                ReactorExplode();

            }

        }

    }

    private void ApplyTemperatureChange(float delta) {

        float TemperaturePercentage = Temperature / MaxTemperature;
        if (TemperaturePercentage > 0.75f) {

            InternalWaterLevel -= (WaterDecreaseRate * delta) * TemperaturePercentage;

        }

        float ControlRodAffect = 1.0f;

        for (int i = 0; i < ControlRods.Count; i++) {

            ControlRodAffect -= ControlRods[i].TemperatureReduction;

        }

        Temperature += (TemperatureIncreaseRate * delta) * ControlRodAffect;
        Temperature -= (WaterCoolRate * delta);

    }

    public void ReactorExplode() {

        IsExploded = true;
        Light.LightColor = (new Color(0, 0, 0));
        this.SetProcess(false);
        GD.Print("BANG!");

    }

}