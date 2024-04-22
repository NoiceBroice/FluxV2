using Gameplay;
using Godot;
using System;

public class ConfigPanel : Control
{
    [Signal]
    delegate void UpdateSelectedModsLabel(); 

    public override void _Ready()
    {
        GetNode<SpinBox>("Speed").Connect("value_changed", this, nameof(UpdateSpeed));
    }

    void UpdateSpeed(float speed)
    {
        Game.Speed = speed / 100f;
        EmitSignal("UpdateSelectedModsLabel");
    }
}
