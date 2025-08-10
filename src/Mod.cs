using PulsarModLoader;

namespace music_mod
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "Mest";

        public override string ShortDescription => "Allows user to change ingame music with custom music";

        public override string Name => "music_mod";

        public override string HarmonyIdentifier() => "Mest.music_mod";
    }
}
