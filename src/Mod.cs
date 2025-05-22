using System;
using System.IO;
using PulsarModLoader;
using UnityEngine;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Converters;

namespace QuickChat
{
	public class Mod : PulsarMod
	{
		public override string HarmonyIdentifier() => "Badryuiner.QuickChatTweaked";
		public override string Author => "Badryuiner, Mest";
		public override string Name => "Quick Chat";
		public override string Version => "0.2.2";
		public Mod()
		{
			Mod.InitCfg();
		}
		public static void InitCfg()
		{
			string path = Directory.GetCurrentDirectory() + "\\QuickChat.json";
			if (!File.Exists(path))
			{
                Mod.binds = new Bind[]
				{
					new Bind(KeyCode.Equals, "This is a check to ensure the mod is working"),
				};
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
				jsonSerializerSettings.Converters.Add(new StringEnumConverter());
                File.WriteAllText(path, JsonConvert.SerializeObject(Mod.binds, Formatting.Indented, jsonSerializerSettings));
                return;
			}
            Mod.binds = JsonConvert.DeserializeObject<Bind[]>(File.ReadAllText(path));
		}
		public static Bind[] binds;
	}
}
