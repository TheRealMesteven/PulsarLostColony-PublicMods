using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.Chat.Commands.CommandRouter;
using PulsarModLoader.Utilities;
using UnityEngine;

namespace QuickChat
{
	[HarmonyPatch(typeof(PLServer), "Update")]
	internal static class Patch
	{
		private static void Postfix()
		{
			if (!PLNetworkManager.Instance.IsTyping)
			{
				Bind[] binds = Mod.binds;
				for (int i = 0; i < binds.Length; i++)
				{
					if (Input.GetKeyDown(binds[i].key))
					{
						if (binds[i].msg.Substring(0, 1) == "/")
						{
							Patch.Execute(binds[i].msg);
						}
						else
						{
							Messaging.ChatMessage(PhotonTargets.All, binds[i].msg, -1);
						}
					}
				}
			}
		}
		private static void Execute(string Command)
		{
			bool flag = false;
			foreach (PulsarMod pulsarMod in ModManager.Instance.GetAllMods())
			{
				if (!flag)
				{
					foreach (Type type in pulsarMod.GetType().Assembly.GetTypes())
					{
						if (!flag && typeof(ChatCommand).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
						{
							ChatCommand chatCommand = (ChatCommand)Activator.CreateInstance(type);
							foreach (string b in chatCommand.CommandAliases())
							{
								if (Command.Split(new char[]
								{
									' '
								})[0].Remove(0, 1) == b)
								{
									chatCommand.Execute(Regex.Replace(Command, Command.Split(new char[]
									{
										' '
									})[0], "").TrimStart(new char[]
									{
										' '
									}));
									flag = true;
									break;
								}
							}
						}
					}
				}
			}
			if (!flag)
			{
				Messaging.Notification("Command not found: " + Command.Split(new char[]
				{
					' '
				})[0], PLNetworkManager.Instance.LocalPlayer, -1, 4000, false);
			}
		}
	}
}
