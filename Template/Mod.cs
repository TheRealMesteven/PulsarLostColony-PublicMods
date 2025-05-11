using PulsarModLoader;

namespace [MOD_NAME]
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "[MOD_AUTHOR]";

        public override string ShortDescription => "[MOD_DESCRIPTION]";

        public override string Name => "[MOD_NAME]";

        public override string HarmonyIdentifier() => "[MOD_AUTHOR].[MOD_NAME]";
    }
}
