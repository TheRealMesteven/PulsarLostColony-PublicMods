using PulsarModLoader;

namespace QuickChat
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "Badruiner, Mest";

        public override string ShortDescription => "Detects keypresses when not in chat and runs a bindable command / phrase.";

        public override string Name => "QuickChat";

        public override string HarmonyIdentifier() => "Badruiner, Mest.QuickChat";
    }
}
