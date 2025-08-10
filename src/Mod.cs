using PulsarModLoader;
using PulsarModLoader.CustomGUI;
using UnityEngine;

namespace music_mod
{
    public class Mod : PulsarMod
    {
        public override string Version => "0.0.0";

        public override string Author => "Mest";

        public override string ShortDescription => "Allows user to change ingame music with custom music";

        public override string Name => "music_mod";

        public override string HarmonyIdentifier() => "Mest.music_mod";
        public Mod()
        {
            GameObject go = new UnityEngine.GameObject("CustomMusicPlayer");
            go.AddComponent<MusicPlayer>();
            GameObject.DontDestroyOnLoad(go);
        }
    }
    public class Config : ModSettingsMenu
    {
        public override string Name() => "music mod config";
        public override void Draw()
        {
            OverrideDefaultMusic.Value = GUILayout.Toggle(OverrideDefaultMusic.Value, "Override ingame music");
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Volume: {Volume.Value*100}%", GUILayout.MaxWidth(100));
            GUILayout.Space(10);
            Volume.Value = GUILayout.HorizontalSlider(Volume.Value, 0f, 1f);
            GUILayout.EndHorizontal();
            if (MusicPlayer.Instance == null)
            {
                GUILayout.Label("Music player class is null!");
                return;
            }
            if (MusicPlayer.Instance.musicCache == null)
            {
                GUILayout.Label("Music cache is null!");
                return;
            }
            if (MusicPlayer.Instance.audioSource == null)
            {
                GUILayout.Label("Audio source is null!");
            }
            GUILayout.Label($"Found {MusicPlayer.Instance.musicCache.Count} Custom Music Tracks");
            if (GUILayout.Button("STOP MUSIC"))
            {
                MusicPlayer.Instance.StopMusic();
            }
            if (!MusicPlayer.Instance.searchingForMusic)
            {
                if (GUILayout.Button("Reload"))
                {
                    MusicPlayer.Instance.ReloadMusic();
                }
            }
            else
            {
                GUILayout.Label("Searching for music . . .");
            }
            foreach (var music in MusicPlayer.Instance.musicCache)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(music.Key))
                {
                    MusicPlayer.Instance.PlayMusic(music.Key);
                }
                GUILayout.EndHorizontal();
            }
        }
        internal static SaveValue<bool> OverrideDefaultMusic = new SaveValue<bool>("OverrideDefaultMusic", true);
        internal static SaveValue<float> Volume = new SaveValue<float>("Volume", 0.5f);
    }
}
