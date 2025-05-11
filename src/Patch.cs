using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using static PulsarModLoader.Patches.HarmonyHelpers;
using static HarmonyLib.AccessTools;
using static PLUIPlayMenu;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Reflection;
using System;

namespace BotCount
{
    public class ExtraData
    {
        public GameObject BotCount;
    }

    public static class UIJoinGameElementExtensions
    {
        private static readonly ConditionalWeakTable<UIJoinGameElement, ExtraData> _extraData = new ConditionalWeakTable<UIJoinGameElement, ExtraData>();

        public static ExtraData GetData(this PLUIPlayMenu.UIJoinGameElement instance)
        {
            return _extraData.GetOrCreateValue(instance);
        }
    }

    [HarmonyPatch(typeof(PLUIPlayMenu), "Update")]
    class BotCountGUIPatch
    {
        private static MethodInfo GetJoinGameElementFromRoomInfoInfo = AccessTools.Method(typeof(PLUIPlayMenu), "GetJoinGameElementFromRoomInfo");
        static void Postfix(PLUIPlayMenu __instance)
        {
            if (__instance.Visuals.activeSelf)
            {
                RoomInfo[] list = PhotonNetwork.GetRoomList();
                foreach (RoomInfo roomInfo2 in list)
                {
                    PLUIPlayMenu.UIJoinGameElement jge = (PLUIPlayMenu.UIJoinGameElement)GetJoinGameElementFromRoomInfoInfo.Invoke(__instance, new object[] { roomInfo2 }); // __instance.GetJoinGameElementFromRoomInfo(roomInfo2);
                    if (jge != null && jge.EnemyCount != null)
                    {
                        var data = jge.GetData();
                        int num = 0;
                        if (data.BotCount == null)
                        {
                            data.BotCount = UnityEngine.Object.Instantiate(jge.EnemyCount);
                            data.BotCount.name = "BotCount";
                            data.BotCount.transform.SetParent(jge.EnemyCount.transform.parent);
                            data.BotCount.transform.localScale = Vector3.one;
                            data.BotCount.transform.localPosition = jge.ShipType.transform.localPosition + new Vector3(5, 0);
                            jge.ShipType.transform.position -= new Vector3(15, 0);
                            data.BotCount.SetActive(false);
                        }
                        var text = data.BotCount.GetComponentInChildren<Text>();
                        if (roomInfo2.CustomProperties.ContainsKey("CurrentPlayersPlusBots"))
                        {
                            num = (int)roomInfo2.CustomProperties["CurrentPlayersPlusBots"];
                        }
                        if (text != null)
                        {
                            text.text = $"<color={Config.TextColour.Value}>{num - roomInfo2.PlayerCount}</color>";
                        }
                        if (ColorUtility.TryParseHtmlString(Config.BoxColour.Value, out Color color))
                        {
                            data.BotCount.GetComponent<Image>().color = color;
                        }
                        data.BotCount.SetActive(num > 0);
                    }
                }
            }
        }
    }
}
