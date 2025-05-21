using System;
using PulsarModLoader.Chat.Commands.CommandRouter;
using PulsarModLoader.Utilities;

namespace QuickChat
{
	internal class Restart : ChatCommand
	{
		public override string[] CommandAliases() => new string[] { "qcr" };
		public override string Description() => "reload binds from config";
		public override void Execute(string arguments)
		{
			Mod.InitCfg();
			Messaging.Notification("Quick Chat Commands Reloaded", PLNetworkManager.Instance.LocalPlayer, -1, 4000, false);
		}
	}
}
