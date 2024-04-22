using Content.Beatmaps;
using Compatibility.SSP;

using Godot;
using System;
using System.Collections.Generic;

public class ToConvertList : VBoxContainer
{
    ToConvertEntry Entry;
    public List<BeatmapToConvert> ToConvert = new List<BeatmapToConvert>();
    public override void _Ready()
    {
        Entry = GetNode<ToConvertEntry>("Entry");

        foreach(BeatmapToConvert map in BeatmapLoader.MapsToConvert) {
            if(map.Version != 1) continue;
            var new_entry = (ToConvertEntry)Entry.Duplicate();
            new_entry.Visible = true;
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
