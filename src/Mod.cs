using PulsarModLoader;
using PulsarModLoader.CustomGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Nuaj.BoundingSlabs;

namespace BotCount
{
    internal class Mod : PulsarMod
    {
        public override string Version => "0.0.1";

        public override string Author => "Mest";

        public override string ShortDescription => "Adds a Bot Count to the Join Game GUI";

        public override string Name => "Bot Count";

        public override string HarmonyIdentifier() => "Mest.BotCount";
    }
    internal class Config : ModSettingsMenu
    {
        public override string Name() => "Bot Count Config";
        public override void Draw()
        {
            ColorPicker(new Rect(8, 30, 240, 160), "Box Colour", BoxColour);
            ColorPicker(new Rect(258, 30, 240, 160), "Text Colour", TextColour);
        }
        public static void ColorPicker(Rect rect, string Name, SaveValue<string> saveValue)
        {
            UnityEngine.Color color = UnityEngine.Color.yellow;
            ColorUtility.TryParseHtmlString(saveValue.Value, out color);
            GUILayout.BeginArea(rect, "", "Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{Name} Colour");
            GUILayout.Label(saveValue.Value);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"R", GUILayout.Width(10));
            color.r = GUILayout.HorizontalSlider(color.r, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"G", GUILayout.Width(10));
            color.g = GUILayout.HorizontalSlider(color.g, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"B", GUILayout.Width(10));
            color.b = GUILayout.HorizontalSlider(color.b, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"A", GUILayout.Width(10));
            color.a = GUILayout.HorizontalSlider(color.a, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.BeginVertical("Box", new GUILayoutOption[] { GUILayout.Width(44), GUILayout.Height(44) });
            GUI.color = color;
            saveValue.Value = $"#{ColorUtility.ToHtmlStringRGBA(color)}";
            GUILayout.Label(new Texture2D(60, 40));
            GUI.color = UnityEngine.Color.white;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Label($"{(int)(color.r * 255)},{(int)(color.g * 255)},{(int)(color.b * 255)},{(int)(color.a * 255)}");
            GUILayout.EndArea();
        }
        public static SaveValue<string> BoxColour = new SaveValue<string>("BoxColour", "#87000078");
        public static SaveValue<string> TextColour = new SaveValue<string>("TextColour", "#ffffffff");
    }
}
