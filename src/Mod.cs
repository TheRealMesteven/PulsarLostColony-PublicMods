using PulsarModLoader;

namespace Bot-Count
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "Mest";

        public override string ShortDescription => "Adds a Bot Count to the Join Game Menu";

        public override string Name => "Bot-Count";

        public override string HarmonyIdentifier() => "Mest.Bot-Count";
    }
}
