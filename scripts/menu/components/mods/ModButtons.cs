using Gameplay;
using Gameplay.Mods;
using Godot;
using System;

public class ModButtons : Node
{

    [Signal]
    delegate void UpdateSelectedModsLabel(); 

    Button noFailButton;

    public override void _Ready()
    {
        noFailButton = GetNode<Button>("DiffDec/HBoxContainer/Nofail");
    }

    public void ConnectButtons()
    {
        noFailButton.Connect("toggled", this, nameof(NoFailToggled));
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
}
