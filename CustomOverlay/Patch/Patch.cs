using ADOLib.SafeTools;
using CustomOverlay.Behaviour;
using HarmonyLib;
using Newgrounds;
using UnityEngine;
using UnityModManagerNet;

namespace CustomOverlay.Patch
{
    public class Patch
    {
        [SafePatch("CustomOverlay.HideTitlePatch", "scrController", "WaitForStartCo")]
        internal static class HideTitlePatch {
            private static void Postfix(scrController __instance) {
                if (CustomOverlay.settings.HideTitle && __instance.txtCaption != null) __instance.txtCaption.transform.localScale = Vector3.zero;
            }
        }
        
        [SafePatch("CustomOverlay.HideTitleAtStartPatch", "scrController", "Awake_Rewind")]
        internal static class HideTitleAtStartPatch {
            private static void Postfix(scrController __instance) {
                if (CustomOverlay.settings.HideTitle && __instance.txtCaption != null) __instance.txtCaption.transform.localScale = Vector3.zero;
            }
        }
        
        [SafePatch("CustomOverlay.PlayPatch", "CustomLevel", "Play")]
        internal static class PlayPatch
        {
            private static void Prefix()
            {
                CustomLevel.instance.checkpointsUsed = 0;
                OverlayBase.Score = 0;
                OverlayBase.Combo = 0;
            }
        }
        
        [SafePatch("CustomOverlay.ResetPatch", "scrController", "Awake")]
        internal static class ResetPatch
        {
            private static void Prefix()
            {
                OverlayBase.Score = 0;
                OverlayBase.Combo = 0;
            }
        }
        
        [SafePatch("CustomOverlay.MainMenuResetPatch", "scrController", "QuitToMainMenu")]
        internal static class MainMenuResetPatch
        {
            private static void Prefix()
            {
                OverlayBase.Score = 0;
                OverlayBase.Combo = 0;
                OverlayBase.Deaths = 0;
            }
        }
        
        [SafePatch("CustomOverlay.LoadLevelPatch", "CustomLevel", "LoadLevel")]
        internal static class LoadLevelPatch
        {
            private static void Prefix()
            {
                OverlayBase.Score = 0;
                OverlayBase.Combo = 0;
                OverlayBase.Deaths = 0;
            }
        }

        [SafePatch("CustomOverlay.StopPlayPatch", "scnEditor", "SwitchToEditMode")]
        internal static class StopPlayPatch
        {
            private static void Postfix()
            {
                OverlayBase.Score = 0;
                OverlayBase.Combo = 0;
            }
        }

        [SafePatch("CustomOverlay.DeathPatch", "scrController", "Fail2Action")]
        public static class DeathPatch
        {
            public static void Postfix()
            {
                OverlayBase.Deaths++;
            }
        }

        [SafePatch("CustomOverlay.AddhitPatch", "scrMistakesManager", "AddHit")]
        public static class AddhitPatch
        {
            public static void Postfix(HitMargin hit)
            {
                if (hit == HitMargin.Perfect)
                {
                    OverlayBase.Combo++;
                }
                else
                {
                    OverlayBase.Combo = 0;
                }

                switch (hit)
                {
                    case HitMargin.VeryEarly:
                    case HitMargin.VeryLate:
                        OverlayBase.Score += 91;
                        break;
                    case HitMargin.EarlyPerfect:
                    case HitMargin.LatePerfect:
                        OverlayBase.Score += 150;
                        break;
                    case HitMargin.Perfect:
                        OverlayBase.Score += 300;
                        break;
                }
            }
        }
    }
}