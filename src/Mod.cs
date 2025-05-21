using PulsarModLoader;

namespace Cards
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "Mest";

        public override string ShortDescription => "Adds card to the game";

        public override string Name => "Cards";

        public override string HarmonyIdentifier() => "Mest.Cards";
    }
}
