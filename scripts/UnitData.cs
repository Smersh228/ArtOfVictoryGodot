using Godot;
using Godot.Collections;
using System;
using System.IO;

public partial class UnitData : Node
{
    public static UnitData Instance { get; private set; }
    private Dictionary<string, Dictionary<string, Variant>> globalUnitData;

    public override void _Ready()
    {
        Instance = this;

        string json_string = File.ReadAllText("units.json");
        Json json = new Json();
        Error error = json.Parse(json_string);
        if (error == Error.Ok)
        {
            globalUnitData = json.Data.AsGodotDictionary<string, Dictionary<string, Variant>>();
        }
    }
    public Dictionary<string, Variant> this[string id]
    {
        get
        {
            return globalUnitData[id];
        }
    }
}
