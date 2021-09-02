using Kingmaker.Settings;
using UnityModManagerNet;

namespace DifficultyTweaks {
    public class Settings : UnityModManager.ModSettings {
        public GameDifficultyOption difficultySetting = GameDifficultyOption.Custom;
        public bool enableOverride = false;

        public override void Save(UnityModManager.ModEntry modEntry) {
            Save(this, modEntry);
        }
    }
}
