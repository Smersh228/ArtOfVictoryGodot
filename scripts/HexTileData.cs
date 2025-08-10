using Godot;
using Godot.Collections;
using System;
using System.IO;

public partial class HexTileData : Node
{
    public static HexTileData Instance { get; private set; }
    public static Dictionary<string, Dictionary<string, Variant>> GlobalTileData;
    public override void _Ready()
    {
        Instance = this;

        string json_string = File.ReadAllText("tiles.json");
		Json json = new();
		Error error = json.Parse(json_string);
        if (error == Error.Ok)
        {
            GlobalTileData = json.Data.AsGodotDictionary<string, Dictionary<string, Variant>>();
		}
    }
}
