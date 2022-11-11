using Godot;
using System;
using System.Collections.Generic;

public class MapList : Control
{
	public List<BeatmapSet> RootMaps = BeatmapLoader.LoadedMaps;
	public List<BeatmapSet> DisplayedMaps { get; private set; }
	public BeatmapSet SelectedMapset { get; private set; }
	public Beatmap SelectedMap { get; private set; }
	private MapsetButton origin;
	private MapsetButton[] mapButtons = new MapsetButton[0];
	private Control content;
	private Control anchor;
	private Control filters;
	private float scroll = 0;
	private float scrollf = 0;
	private int offset = 0;
	private int visible = 0;
	private int buttonOffset = 0;
	private int buttonOffsetAt = 0;
	public Action<Beatmap> MapSelected;
	public Action<BeatmapSet> MapsetSelected;
	public override void _Ready()
	{
		content = GetNode<Control>("Content");
		anchor = content.GetNode<Control>("Anchor");
		origin = anchor.GetNode<MapsetButton>("Mapset");
		origin.Visible = false;
		filters = GetNode<Control>("Filters");
		filters.GetNode<LineEdit>("Search").Connect("text_changed", this, nameof(SearchChanged));
		RootMaps = BeatmapLoader.LoadedMaps;
	}
	public override void _Process(float delta)
	{
		anchor.RectPosition = new Vector2(anchor.RectPosition.x, -scrollf * 76);
		base._Process(delta);
	}
	public override void _GuiInput(InputEvent @event)
	{
		if (!(@event is InputEventMouseButton))
			return;
		var ev = (InputEventMouseButton)@event;
		if (ev.ButtonIndex == (int)ButtonList.WheelUp)
			Scroll(-1);
		else if (ev.ButtonIndex == (int)ButtonList.WheelDown)
			Scroll(1);
	}
	public void Scroll(int amount)
	{
		if (scroll + amount < 0)
			return;
		if (scroll + amount > visible)
			return;
		scroll += amount;
		offset += amount;
		buttonOffsetAt -= amount;
		RenderButtons();
		var tween = anchor.GetNode<Tween>("Tween");
		tween.StopAll();
		tween.InterpolateProperty(this, nameof(scrollf), scroll - amount, scroll, 0.1f);
		tween.Start();
	}
	private MapsetButton newButton()
	{
		MapsetButton newBtn = origin.Duplicate() as MapsetButton;
		newBtn.MapSelected += mapSelected;
		newBtn.Connect("pressed", this, nameof(btnPressed), new Godot.Collections.Array(newBtn));
		newBtn.Visible = true;
		anchor.AddChild(newBtn);
		return newBtn;
	}
	private void mapSelected(Beatmap map)
	{
		GD.Print(map.Name);
		SelectedMap = map;
		MapSelected(SelectedMap);
	}
	private void btnPressed(MapsetButton btn)
	{
		GD.Print(btn.Mapset.Name);
		SelectedMapset = btn.Mapset;
		MapsetSelected(SelectedMapset);
		int buttonIndex = Array.IndexOf(mapButtons, btn);
		buttonOffsetAt = buttonIndex;
		RenderButtons();
	}
	public void SearchChanged(string search)
	{
		UpdateDisplayed(true);
	}
	private bool IsSimilar(BeatmapSet set, string search)
	{
		var name = set.Name.ToLower().Trim();
		search = search.ToLower().Trim();
		return name.Contains(search)
			|| name.Similarity(search) >= 0.5;
	}
	public void UpdateDisplayed(bool render = false)
	{
		var search = filters.GetNode<LineEdit>("Search").Text.Trim();
		if (search != "")
			DisplayedMaps = RootMaps.FindAll((BeatmapSet set) => IsSimilar(set, search));
		else
			DisplayedMaps = RootMaps;
		if (!render)
			return;
		RenderButtons();
	}
	public void UpdateButton(MapsetButton button, int buttonIndex, bool animate = true)
	{
		if (button.Mapset == SelectedMapset)
		{
			button.ManualUpdate(true);
			button.Expand(animate);
			buttonOffset = (int)button.GetNode<VBoxContainer>("Maps").RectSize.y;
			return;
		}
		button.ManualUpdate();
		button.Collapse(animate);
	}
	public void RenderButtons()
	{
		visible = Math.Min(DisplayedMaps.Count - offset, (int)Math.Ceiling(content.RectSize.y / 72));
		if (scroll > visible)
		{
			offset += (visible - offset);
			scroll = visible;
		}
		for (int i = 0; i < mapButtons.Length; i++)
		{
			if (i >= visible)
				mapButtons[i].QueueFree();
		}
		Array.Resize(ref mapButtons, visible);
		for (int i = 0; i < visible; i++)
		{
			if (mapButtons[i] == null)
				mapButtons[i] = newButton();
			var btn = mapButtons[i];
			btn.Mapset = DisplayedMaps[offset + i];
			UpdateButton(btn, i, false);
			int thisOffset = buttonOffsetAt < i ? buttonOffset : 0;
			btn.RectPosition = new Vector2(btn.RectPosition.x, thisOffset + (offset + i) * 76);
		}
	}
}
