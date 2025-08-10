using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace music_mod
{
    public class MusicPlayer : MonoBehaviour
    {
        public static MusicPlayer Instance { get; private set; }

        public string musicFolder = Path.Combine(Environment.CurrentDirectory, "Mods", "Music");
        internal Dictionary<string, AudioClip> musicCache = new Dictionary<string, AudioClip>();
        internal AudioSource audioSource;

        // Scenario → file mapping
        public Dictionary<string, string> musicAssignments = new Dictionary<string, string>();

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;

            if (!Directory.Exists(musicFolder))
            {
                Debug.LogError("[CustomMusic] Folder not found, so it'll be empty");
                Directory.CreateDirectory(musicFolder);
                return;
            }

            StartCoroutine(LoadAllMusic());
        }
        internal bool searchingformusic = false;
        internal void ReloadMusic()
        {
            if (searchingformusic) return;
            StartCoroutine(LoadAllMusic());
        }
        internal IEnumerator LoadAllMusic()
        {
            searchingformusic = true;
            musicCache.Clear();
            foreach (var file in Directory.GetFiles(musicFolder))
            {
                AudioType audioType = GetAudioType(file);
                string name = Path.GetFileNameWithoutExtension(file);
                if (audioType == AudioType.UNKNOWN)
                {
                    Debug.Log($"[CustomMusic] Non-playable file {name} skipped");
                    continue;
                }
                string url = "file:///" + file.Replace("\\", "/");
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, GetAudioType(file)))
                {
                    yield return www.SendWebRequest();
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                        musicCache[name] = clip;
                        Debug.Log($"[CustomMusic] Loaded '{name}'");
                    }
                    else
                    {
                        Debug.LogError($"[CustomMusic] Failed to load {file}: {www.error}");
                    }
                }
            }
            searchingformusic = false;
        }

        private static AudioType GetAudioType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            switch (ext)
            {
                case ".wav": return AudioType.WAV;
                case ".ogg": return AudioType.OGGVORBIS;
                case ".mp3": return AudioType.MPEG;
                default: return AudioType.UNKNOWN;
            }
        }

        public void PlayMusic(string clipName)
        {
            if (musicCache.TryGetValue(clipName, out AudioClip clip))
            {
                audioSource.clip = clip;
                audioSource.Play();
                Debug.Log($"[CustomMusic] Playing '{clipName}'");
            }
            else
            {
                Debug.LogWarning($"[CustomMusic] No loaded clip for '{clipName}'");
            }
        }

        public void PlayScenarioMusic(string scenarioName)
        {
            if (musicAssignments.TryGetValue(scenarioName, out string clipName))
            {
                if (musicCache.TryGetValue(clipName, out AudioClip clip))
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    Debug.Log($"[CustomMusic] Playing '{clipName}' for scenario '{scenarioName}'");
                }
                else
                {
                    Debug.LogWarning($"[CustomMusic] No loaded clip for '{clipName}'");
                }
            }
            else
            {
                Debug.LogWarning($"[CustomMusic] No assignment for '{scenarioName}'");
            }
        }

        public void StopMusic()
        {
            audioSource.Stop();
        }
    }
}
