using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace music_mod
{
    internal class Patches
    {
        internal static bool loopingMusic = false;
        private static FieldInfo m_LoopingMusicPlayinginfo = AccessTools.Field(typeof(PLMusic), "m_LoopingMusicPlaying");
        private static FieldInfo LastPlayMusicTimeinfo = AccessTools.Field(typeof(PLMusic), "LastPlayMusicTime");
        [HarmonyPatch(typeof(PLMusic), "PlayMusic")]
        internal class PlayMusicPatch
        {
            public static bool Prefix(PLMusic __instance, string inMusicString, bool isCombatTrack, bool isPlanetTrack, bool isSpecialTrack = false, bool isLoopingTrack = false)
            {
                if (!Config.OverrideDefaultMusic.Value) return true;
                Debug.Log($"Overriding music request: {inMusicString}, isCombatTrack:{isCombatTrack}, isPlanetTrack:{isPlanetTrack}, isSpecialTrack:{isSpecialTrack}, isLoopingTrack{isLoopingTrack}");
                LastPlayMusicTimeinfo.SetValue(__instance, Time.time);
                __instance.StopCurrentMusic();

                MusicPlayer.Instance.PlayRandomMusic();

                __instance.CombatMusicPlaying = isCombatTrack;
                __instance.PlanetMusicPlaying = isPlanetTrack;
                m_LoopingMusicPlayinginfo.SetValue(__instance, isLoopingTrack);
                loopingMusic = isLoopingTrack;
                __instance.SpecialMusicPlaying = isSpecialTrack;
                return false;
            }
        }
        [HarmonyPatch(typeof(PLMusic), "StopCurrentMusic")]
        internal class StopMusicPatch
        {
            public static bool Prefix(PLMusic __instance)
            {
                if (__instance.CurrentPlayingMusicEventString == "") return false;
                loopingMusic = false;
                if (__instance.CurrentPlayingMusicEventString.Contains("[MODDED]"))
                {
                    MusicPlayer.Instance.StopMusic();
                    __instance.CurrentPlayingMusicEventString = "";
                    __instance.CombatMusicPlaying = false;
                    __instance.SpecialMusicPlaying = false;
                    m_LoopingMusicPlayinginfo.SetValue(__instance, false);
                    return false;
                }
                return true;
            }
        }
    }
}
