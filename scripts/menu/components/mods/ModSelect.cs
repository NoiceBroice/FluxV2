using Gameplay;
using Godot;
using System;

public class ModSelect : View
{

    Label selectedMods;

    public override void _Ready()
    {
        this.ViewTween = GetNode<Tween>("Tween");
        selectedMods = GetNode<Label>("ModPanel/VBoxContainer/SelectedMods");
        
        GetNode<Button>("ModPanel/Close").Connect("pressed", this, nameof(OnHide));
        
        GetNode<ModButtons>("ModPanel/VBoxContainer").Connect("UpdateSelectedModsLabel", this, nameof(UpdateSelectedModsLabel));
        GetNode<ConfigPanel>("ModPanel/ConfigPanel/VBoxContainer").Connect("UpdateSelectedModsLabel", this, nameof(UpdateSelectedModsLabel));

        GetNode<ModButtons>("ModPanel/VBoxContainer").UpdateButtons();
        GetNode<ModButtons>("ModPanel/VBoxContainer").ConnectButtons();
        UpdateSelectedModsLabel();
    }

    void UpdateSelectedModsLabel()
    {
        selectedMods.Text = "Selected:" + Game.Mods.ToString();
        if(Game.Speed != 1f)
        {
            selectedMods.Text += " S" + (int)(Game.Speed * 100f);
        }
    }

    public override async void OnShow()
	{
		this.Visible = true;
		ViewTween.RemoveAll();
		ViewTween.InterpolateProperty(this, "modulate:a", 0, 1, 0.3f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
		ViewTween.Start();
		await ToSignal(ViewTween, "tween_all_completed");
		this.IsActive = true;
	}
	public override async void OnHide()
	{
		this.IsActive = false;
		ViewTween.RemoveAll();
		ViewTween.InterpolateProperty(this, "modulate:a", 1, 0, 0.3f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
		ViewTween.Start();
		await ToSignal(ViewTween, "tween_all_completed");
		this.Visible = false;
	}
}
