using Content.Beatmaps;
using Godot;
using System;
using System.Linq;

public class ConvertOptions : Panel
{
    public override void _Ready()
    {
        GetNode<Button>("VBoxContainer/ConvertSelected").Connect("pressed", this, nameof(ConvertSelected));
        GetNode<Button>("VBoxContainer/SelectAll").Connect("pressed", this, nameof(SelectAll));
    }

    void SelectAll()
    {
        foreach(ToConvertEntry entry in GetNode<ToConvertList>("../ToConvert/ScrollContainer/VBoxContainer").GetChildren()) 
        {
            if(!entry.GetNode<CheckBox>("ConvertCheckbox").Pressed) {
                entry.GetNode<CheckBox>("ConvertCheckbox").Pressed = true;
            }
            // entry.GetNode<CheckBox>("ConvertCheckbox").EmitSignal("toggled", true);
        }
    }

    void ConvertSelected()
    {
        foreach(BeatmapToConvert map in GetNode<ToConvertList>("../ToConvert/ScrollContainer/VBoxContainer").ToConvert.ToList()) 
        {
            if(!Compatibility.SSP.Map.Import(map))
            {
                GD.Print("Failed to convert map " + map.Name);
                continue;
            }
            GetNode<ToConvertList>("../ToConvert/ScrollContainer/VBoxContainer").ToConvert.Remove(map);
            BeatmapLoader.MapsToConvert.Remove(map);
            var dir = new Directory();
            GD.Print("Removing " + map.Path);
            dir.Remove(map.Path);
        }

        BeatmapLoader.LoadMapsFromDirectory(Global.MapPath, true);
    }
}
