using Godot;
using System;

public static class InputMap {

    // Movement
    public static StringName Forward = new StringName("W");
    public static StringName Left = new StringName("A");
    public static StringName Backward = new StringName("S");
    public static StringName Right = new StringName("D");
    public static StringName Jump = new StringName("Spacebar");

    // Action
    public static StringName Interact = new StringName("E");
    public static StringName Primary = new StringName("Left Click");
    public static StringName Secondary = new StringName("Right Click");
    public static StringName Tertiary = new StringName("R");
    public static StringName Drop = new StringName("Q");

    // Executive
    public static StringName Pause = new StringName("Escape");
    public static StringName Submit = new StringName("Enter");
    public static StringName Command = new StringName("~");

}