using Gameplay.Mods;
using Godot;
using System;
using System.Collections.Generic;

public class Global : Node
{
	public static string MapPath = OS.GetUserDataDir().PlusFile("maps");

	public static Global Instance;
	public static Discord.DiscordW Discord;
	// public static ModList Mods;
	public static Texture PlaceholderTexture;
	public static Texture Matt;
	public Node CurrentScene { get; private set; }
	public Control Overlay { get; private set; }
	public Dictionary<string, Control> Overlays { get; private set; }
	public Global() : base()
	{
		if (!System.IO.Directory.Exists(MapPath))
			System.IO.Directory.CreateDirectory(MapPath);
		Instance = this;
		Discord = new Discord.DiscordW();
		Discord.SetActivity(new Discord.ActivityW());
	}
	public override void _Ready()
	{
		Input.UseAccumulatedInput = false;
		Matt = (StreamTexture)GD.Load("res://assets/images/matt.jpg");
		PlaceholderTexture = (StreamTexture)GD.Load("res://assets/images/placeholder.png");
		Viewport root = GetTree().Root;
		CurrentScene = root.GetChild(root.GetChildCount() - 1);
	}
	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionJustPressed("fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
		Discord.RunCallbacks();
	}
	public void AddOverlay()
	{
		CallDeferred(nameof(DeferredAddOverlay));
	}
	private void DeferredAddOverlay()
	{
		var overlayScene = (PackedScene)GD.Load("res://scenes/Overlay.tscn");
		Overlay = (Control)overlayScene.Instance();
		Overlays = new Dictionary<string, Control>();
		GetTree().Root.AddChild(Overlay);
		GetTree().Root.MoveChild(Overlay, 2);
		foreach (Control overlay in Overlay.GetChildren())
		{
			Overlays.Add(overlay.Name, overlay);
		}
	}
	public void GotoScene(string path, Action<Node> callback = null)
	{
		CallDeferred(nameof(DeferredGotoScene), path, callback);
	}
	private void DeferredGotoScene(string path, Action<Node> callback = null)
	{
		CurrentScene.Free();
		var nextScene = (PackedScene)GD.Load(path);
		CurrentScene = nextScene.Instance();
		GetTree().Root.AddChild(CurrentScene);
		GetTree().Root.MoveChild(CurrentScene, 1);
		callback?.Invoke(CurrentScene);
	}
	public void FinishedLoading()
	{
		ViewportChanged();
		GetViewport().Connect("size_changed", this, nameof(ViewportChanged));
	}
	public void ViewportChanged()
	{
		GetViewport().Size = OS.WindowSize * Settings.RenderScale;
		GetViewport().Debanding = Settings.Debanding;
	}
}
