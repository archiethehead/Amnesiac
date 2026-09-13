using Godot;

public struct Countdown {

    public float Timer { get; private set; } = 0.0f;
    public float Threshold = 1.0f;

    public bool LogTime(double delta) {

        Timer += 1.0f * (float)delta;

        if (Timer >= Threshold) {

            Timer = 0.0f;
            return true;

        }

        return false;

    }

    public void Reset() => Timer = 0.0f;

    public Countdown(float T) : this() => this.Threshold = T;

}