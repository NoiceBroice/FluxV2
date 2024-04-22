using Gameplay;
using Gameplay.Mods;
using Godot;
using System;

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
        foreach(Mod mod in Game.Mods)
        {
            if(mod is ModNoFail) noFailButton.Pressed = true;
        }

        EmitSignal(nameof(UpdateSelectedModsLabel));
    }

    void NoFailToggled(bool toggled)
    {

        if(toggled) Game.Mods.Add(new ModNoFail());
        else Game.Mods.Remove(new ModNoFail());

        UpdateButtons();
    }

    void SpeedToggled(bool toggled)
    {
        configPanel.Visible = toggled;
        configPanel.GetNode<SpinBox>("VBoxContainer/Speed").Visible = toggled;
    }
}
