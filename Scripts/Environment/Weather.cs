using Godot;
using System;

public partial class Weather : WorldEnvironment {

    [Export] GpuParticles3D Rain = null;
    private GameManager GameManager = null;

    public enum WeatherTypeEnum {

        Raining,
        Sunny

    }

    private WeatherTypeEnum WeatherType = WeatherTypeEnum.Sunny;

    public override void _Ready() {

        GameManager = GameManager.Instance;
        GameManager.Weather = this;

    }

    public void UpdateWeather(WeatherTypeEnum Type) {

        WeatherType = Type;

        switch (WeatherType) {

            case WeatherTypeEnum.Raining:
                Rain.Visible = true;
                this.Environment.VolumetricFogEnabled = true;
                break;

            case WeatherTypeEnum.Sunny:
                Rain.Visible = false;
                this.Environment.VolumetricFogEnabled = false;
                break;

        }


    }



}