using Gameplay;
using Gameplay.Mods;
using Godot;
using System;
using System.Linq;

public class ModButtons : Node
{

    [Signal]
    delegate void UpdateSelectedModsLabel(); 

    Panel configPanel;

    Button noFailButton;
    Button speedButton;

    public override void _Ready()
    {
        configPanel = GetNode<Panel>("../ConfigPanel");
        noFailButton = GetNode<Button>("DiffDec/HBoxContainer/Nofail");
        speedButton = GetNode<Button>("Misc/HBoxContainer/Speed");
    }

    public void ConnectButtons()
    {
        noFailButton.Connect("toggled", this, nameof(NoFailToggled));
        speedButton.Connect("toggled", this, nameof(SpeedToggled));
    }

    public void UpdateButtons()
    {

        if(Game.Mods.OfType<ModNoFail>().Count() != 0) noFailButton.Pressed = true;
        else noFailButton.Pressed = false;

        if(Game.Mods.OfType<ModSpeed>().Count() != 0)
        {
            speedButton.Pressed = true;
            configPanel.Visible = true;
            configPanel.GetNode<SpinBox>("VBoxContainer/Speed").Visible = true;
        }
        else speedButton.Pressed = false;

        EmitSignal(nameof(UpdateSelectedModsLabel));
    }

    void NoFailToggled(bool toggled)
    {

        if(toggled) Game.Mods.Add(new ModNoFail());
        else Game.Mods.Remove(Game.Mods.OfType<ModNoFail>().FirstOrDefault());

        UpdateButtons();
    }

    void SpeedToggled(bool toggled)
    {
        if(toggled)
        {
            Game.Mods.Add(new ModSpeed());
            configPanel.GetNode<ConfigPanel>("VBoxContainer").UpdateConfigElements();
        }
        else Game.Mods.Remove(Game.Mods.OfType<ModSpeed>().FirstOrDefault());
        UpdateButtons();
        
        configPanel.Visible = speedButton.Pressed;
        configPanel.GetNode<SpinBox>("VBoxContainer/Speed").Visible = toggled;
    }
}
