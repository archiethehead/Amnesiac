using Godot;

public struct Countdown {

    private float Timer = 0.0f;
    public float Threshold = 1.0f;

    public Countdown(float T) : this() => this.Threshold = T;

    public bool LogTime(double delta) {

        Timer += 1.0f * (float)delta;

        if (Timer >= Threshold) {

            Timer = 0.0f;
            return true;

        }

        return false;

    }

}