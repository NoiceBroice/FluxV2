using Content.Beatmaps;
using Godot;
using System;

public class ToConvertEntry : Panel
{
    public BeatmapToConvert Info;
    public override void _Ready()
    {
        GetNode<CheckBox>("ConvertCheckbox").Connect("toggled", this, nameof(Toggled));
    }

    void Toggled(bool toggled) 
    {
        if(toggled) ((ToConvertList)GetParent()).ToConvert.Add(Info);
        else ((ToConvertList)GetParent()).ToConvert.Remove(Info);
    }
}
