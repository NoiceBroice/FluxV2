using Content.Beatmaps;
using Compatibility.SSP;

using Godot;
using System;
using System.Collections.Generic;

public class ToConvertList : VBoxContainer
{
    PackedScene Entry;
    public List<BeatmapToConvert> ToConvert = new List<BeatmapToConvert>();
    public override void _Ready()
    {
        Entry = GD.Load<PackedScene>("res://prefabs/convert/Entry.tscn");
        // Entry.Visible = false;
        PopulateList();
    }

    public override void _Input(InputEvent @event)
    {
        if(Input.IsActionJustPressed("reload_maps")) {
            BeatmapLoader.LoadMapsFromDirectory(Global.MapPath, true);
            PopulateList();
        }
    }

    public void PopulateList()
    {
        foreach(ToConvertEntry child in GetChildren()) {
            if(child.Visible) child.QueueFree();
        }

        foreach(var map in BeatmapLoader.MapsToConvert) {
            if(map.Version != 1) continue;
            var new_entry = (ToConvertEntry)Entry.Instance();
            new_entry.Visible = true;
            new_entry.Name = "not_template_lol";
            new_entry.GetNode<Label>("Name").Text = map.Name;
            new_entry.Info = map;
            AddChild(new_entry);
        }
    }

    public override void _Process(float delta)
    {
        foreach(ToConvertEntry entry in GetChildren())
        {
            if(!BeatmapLoader.MapsToConvert.Contains(entry.Info))
            {
                entry.Visible = false;
                entry.QueueFree();
            }
        }
    }
}
