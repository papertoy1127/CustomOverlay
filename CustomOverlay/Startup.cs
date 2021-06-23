using UnityModManagerNet;
using System.Reflection;
using ADOLib.Settings;
using HarmonyLib;

namespace CustomOverlay {
    public class Startup {
        public static void Load(UnityModManager.ModEntry modEntry) {
            CustomOverlay.Setup(modEntry);
        }
    }
}
