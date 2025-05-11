using PulsarModLoader;

namespace Talents
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "Mest";

        public override string ShortDescription => "Overhauls Talent System";

        public override string Name => "Talents";

        public override string HarmonyIdentifier() => "Mest.Talents";
    }
}
