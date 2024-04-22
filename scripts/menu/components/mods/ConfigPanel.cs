using Gameplay;
using Gameplay.Mods;
using Godot;
using System;
using System.Linq;

public class ConfigPanel : Control
{
    [Signal]
    delegate void UpdateSelectedModsLabel();

    public void ConnectConfigElements()
    {
        GetNode<SpinBox>("SpeedConfig/VBoxContainer/Speed").Connect("value_changed", this, nameof(UpdateSpeed));
    }

    public void UpdateConfigElements()
    {
        var speedMod = Game.Mods.OfType<ModSpeed>().FirstOrDefault();
        GetNode<SpinBox>("SpeedConfig/VBoxContainer/Speed").Value = speedMod?.SpeedMultiplier * 100f ?? 100f;
    }

    void UpdateSpeed(float speed)
    {
        Game.Mods.OfType<ModSpeed>().FirstOrDefault().SpeedMultiplier = speed / 100f;
        EmitSignal("UpdateSelectedModsLabel");
    }
}
