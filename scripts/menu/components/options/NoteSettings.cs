using Godot;
using System;

public class NoteSettings : Control
{
    public override void _Ready()
    {
        UpdateSettings();
		GetNode<CheckButton>("Pushback").Connect("pressed", this, nameof(OnButtonPressed));
    }
	public void UpdateSettings()
    {
		GetNode<CheckButton>("Pushback").Pressed = Settings.NotePushback;
    }

    public void OnButtonPressed()
	{
		Settings.NotePushback = GetNode<CheckButton>("Pushback").Pressed;
		Settings.UpdateSettings();
	}
}