using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

public class LocalePackString
{
	public Dictionary<string, string> Armies { get; set; }
	public Dictionary<string, string> Types { get; set; }
	public Dictionary<string, string> Orders { get; set; }
	public Dictionary<string, string> Squads { get; set; }

	public static LocalePackString FromName(string packName, string localeName)
	{
		string path = $"resources/datapacks/{packName}/{localeName}.yml";

		string yml = File.ReadAllText(path);
		Deserializer deser = new();

		return deser.Deserialize<LocalePackString>(yml);
	}

	public void LoadFromName(string packName, string localeName)
	{
		var locale = FromName(packName, localeName);
		Armies = locale.Armies;
		Types = locale.Types;
		Orders = locale.Orders;
		Squads = locale.Squads;
	}
}

/*
public class LocalePack<TArmy, TType, TOrder>
where TArmy : Enum
where TType : Enum
where TOrder : Enum
{
	public Dictionary<TArmy, string> Armies { get; set; }
	public Dictionary<TType, string> Types { get; set; }
	public Dictionary<TOrder, string> Orders { get; set; }
	public string[] Squads { get; set; }

	public static LocalePack<TArmy, TType, TOrder> FromName(string packName, string localeName)
	{
		string path = $"resources/datapacks/{packName}/{localeName}.yml";

		string yml = File.ReadAllText(path);
		Deserializer deser = new();

		return deser.Deserialize<LocalePack<TArmy, TType, TOrder>>(yml);
	}

	public void LoadFromName(string packName, string localeName)
	{
		var locale = FromName(packName, localeName);
		Armies = locale.Armies;
		Types = locale.Types;
		Orders = locale.Orders;
		Squads = locale.Squads;
	}
}
*/
