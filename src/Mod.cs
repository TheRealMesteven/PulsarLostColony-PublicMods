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
            if (!MusicPlayer.Instance.searchingformusic)
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

    }
}
