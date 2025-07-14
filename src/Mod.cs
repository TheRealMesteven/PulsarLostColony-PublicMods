using PulsarModLoader;

namespace OpenTrack-Tracker
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "Mest";

        public override string ShortDescription => "Reads OpenTracks information";

        public override string Name => "OpenTrack-Tracker";

        public override string HarmonyIdentifier() => "Mest.OpenTrack-Tracker";
    }
}
