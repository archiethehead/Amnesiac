using Godot;
using System;

// Manager will always refer to the mediator design pattern implemented in this class,
// and Employee can refer to any Node within the current scene that can call the Manager.

public interface Employee {

    public static void CallManager(GameManager Manager, Node Employee, string function) {

        if (Manager is not null) {

            Manager.Call(function);
            return;

        }

        Employee.Call(function);

    }

}

public partial class GameManager : Node3D {

    [Export] private Player Player = null;
    [Export] private Reactor Reactor = null;
    [Export] private DirectionalLight3D Sun = null;

    public override void _Process(double delta) {

        if (Sun != null) {

            Sun.RotateX(0.001f * (float)delta);

        }

    }

    private void CallEmployee(Node Employee, String Function) {

        if (Employee is not null && Employee.HasMethod(Function)) { 
        
            Employee.Call(Function);
        
        }
    
    }

    private void ReactorExplode() {

        CallEmployee(Reactor, "ReactorExplode");

    }

}