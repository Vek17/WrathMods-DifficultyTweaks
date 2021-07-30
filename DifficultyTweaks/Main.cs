using HarmonyLib;
using Kingmaker.Settings;
using Kingmaker.Settings.Difficulty;
using System;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace DifficultyTweaks {
    static class Main {

        public static Settings Settings;
        public static bool Enabled;
        public static ModEntry Mod;
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Log(string msg) {
            Mod.Logger.Log(msg);
        }

        static bool Load(UnityModManager.ModEntry modEntry) {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();
            Mod = modEntry;
            Settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) {
            Enabled = value;
            Settings.enableOverride = !Settings.enableOverride;
            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry) {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Disable", GUILayout.ExpandWidth(false))) {
                Settings.enableOverride = false;
            }
            GUILayout.Label(
                string.Format("Difficulty Override: {0}", Settings.enableOverride ? Settings.difficultySetting.ToString() : "Disabled"), GUILayout.ExpandWidth(false)
            );
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            foreach (GameDifficultyOption difficulty in Enum.GetValues(typeof(GameDifficultyOption))) {
                if (GUILayout.Button(difficulty.ToString(), GUILayout.ExpandWidth(false))) {
                    Settings.difficultySetting = difficulty;
                    Settings.enableOverride = true;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry) {
            Settings.Save(modEntry);
        }

        [HarmonyPatch(typeof(DifficultyPresetsController), "CurrentDifficultyCompareTo", new Type[] { typeof(GameDifficultyOption) })]
        static class Difficulty_Override_Patch {
            static void Postfix(GameDifficultyOption gameDifficultyOption, ref int __result) {
                if (!Settings.enableOverride) { return; }
                if (gameDifficultyOption < Settings.difficultySetting) {
                    __result = 1; 
                } else if(gameDifficultyOption == Settings.difficultySetting) {
                    __result = 0;
                } else if (gameDifficultyOption > Settings.difficultySetting) {
                    __result = -1;
                }
            }
        }
    }
}