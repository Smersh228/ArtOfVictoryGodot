using Godot;
using Godot.Collections;
using System;
using System.IO;

public partial class UnitData : Node
{
    public static UnitData Instance { get; private set; }
    public static Dictionary<string, Dictionary<string, Variant>> GlobalUnitData;

    public override void _Ready()
    {
        Instance = this;

        string json_string = File.ReadAllText("units.json");
		Json json = new Json();
		Error error = json.Parse(json_string);
		if (error == Error.Ok)
		{
			GlobalUnitData = json.Data.AsGodotDictionary<string, Dictionary<string, Variant>>();
		}
    }
}
