using Gameplay;
using Godot;
using System;

public class ConfigPanel : Control
{
    [Signal]
    delegate void UpdateSelectedModsLabel(); 
    
    public void ConnectConfigElements()
    {
        GetNode<SpinBox>("Speed").Connect("value_changed", this, nameof(UpdateSpeed));
    }

    public void UpdateConfigElements()
    {
        GetNode<SpinBox>("Speed").Value = Game.Speed * 100;
    }

    void UpdateSpeed(float speed)
    {
        Game.Speed = speed / 100f;
        EmitSignal("UpdateSelectedModsLabel");
    }
}
