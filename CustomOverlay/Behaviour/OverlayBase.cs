using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityModManagerNet;

namespace CustomOverlay.Behaviour {
    public class OverlayBase : MonoBehaviour {
        public static List<OverlayBase> Overlays = new List<OverlayBase>();
        public static int Combo;
        public static int Score;
        public static int Deaths;
        public Text text;
        public RectTransform rectTransform;
        private OverlayData _Data;

        public static List<OverlayItem> Items = new List<OverlayItem>();

        static OverlayBase() {
                        Items.Add(new OverlayItem(
                "BPM",
                IfPlaying,
                meta => {
                    meta ??= "0.00";
                    var pitch = CustomLevel.instance is null
                        ? scrConductor.instance.song.pitch
                        : CustomLevel.instance.levelData.pitch / 100.0f;
                    var speed = scrController.instance.currFloor.speed * scrConductor.instance.bpm * pitch;
                    if (int.TryParse(meta, out int mint))
                        return $"{Math.Round(speed, mint)}";
                    return string.Format($"{{0:{meta}}}",
                        speed);
                }));

            Items.Add(new OverlayItem(
                "TileCount",
                IfPlaying,
                meta => (scrLevelMaker.instance.listFloors.Count - 1).ToString()));

            Items.Add(new OverlayItem(
                "CurrentTile",
                IfPlaying,
                meta => scrController.instance.currFloor.seqID.ToString()));

            Items.Add(new OverlayItem(
                "Accurancy",
                IfPlaying,
                meta => {
                    meta ??= "0";
                    var acc = scrController.instance.mistakesManager.percentAcc * 100;
                    if (int.TryParse(meta, out int mint))
                        return $"{Math.Round(acc, mint)}";
                    return string.Format($"{{0:{meta}}}",
                        acc);
                }));

            Items.Add(new OverlayItem(
                "Progress",
                IfPlaying,
                meta => {
                    meta ??= "0";
                    var seqID = scrController.instance.currFloor.seqID;
                    var progress = seqID * 100.0 / (scrLevelMaker.instance.listFloors.Count - 1);
                    if (int.TryParse(meta, out int mint))
                        return $"{Math.Round(progress, mint)}";
                    return string.Format($"{{0:{meta}}}",
                        progress);
                }));

            Items.Add(new OverlayItem(
                "LevelName",
                IfPlaying,
                meta => {
                    return scrUIController.instance.txtLevelName.text; //CustomLevel.instance is null
                    //? SceneManager.GetActiveScene().name
                    //: $"{CustomLevel.instance.levelData.artist} - {CustomLevel.instance.levelData.song}";
                }));

            Items.Add(new OverlayItem(
                "SongProgress",
                IfPlaying,
                meta => {
                    meta ??= "0";
                    if (scrConductor.instance.song.clip is null) return string.Format($"{{0:{meta}}}", 0);
                    var progress = scrConductor.instance.song.time / scrConductor.instance.song.clip.length * 100;
                    if (int.TryParse(meta, out int mint))
                        return $"{Math.Round(progress, mint)}";
                    return string.Format($"{{0:{meta}}}",
                        progress);
                }));

            Items.Add(new OverlayItem(
                "CurrentSongTime",
                IfPlaying,
                meta => {
                    if (scrConductor.instance.song.clip is null) return "00:00";
                    var nowt = TimeSpan.FromSeconds(scrConductor.instance.song.time);
                    return $"{nowt.Minutes}:{nowt.Seconds:00}";
                }));

            Items.Add(new OverlayItem(
                "SongLength",
                IfPlaying,
                meta => {
                    if (scrConductor.instance.song.clip is null) return "00:00";
                    var nowt = TimeSpan.FromSeconds(scrConductor.instance.song.clip.length);
                    return $"{nowt.Minutes}:{nowt.Seconds:00}";
                }));

            Items.Add(new OverlayItem(
                "Score",
                meta => {
                    if (IfPlaying()) {
                        return Score.ToString();
                    }

                    Score = 0;
                    return "Not Playing";
                }));

            Items.Add(new OverlayItem(
                "Combo",
                meta => {
                    if (IfPlaying()) {
                        return Combo.ToString();
                    }

                    Combo = 0;
                    return "Not Playing";
                }));

            Items.Add(new OverlayItem(
                "Deaths",
                IfPlaying,
                meta => Deaths.ToString()));

            Items.Add(new OverlayItem(
                "Uptime",
                meta => $"{UptimeHour}:{UptimeMin:00}:{UptimeSec:00}"));

            Items.Add(new OverlayItem(
                "UptimeDay",
                meta => $"{UptimeDay}"));

            Items.Add(new OverlayItem(
                "UptimeHour",
                meta => $"{UptimeHour}"));

            Items.Add(new OverlayItem(
                "UptimeMin",
                meta => $"{UptimeMin}"));

            Items.Add(new OverlayItem(
                "UptimeSec",
                meta => $"{UptimeSec}"));

            Items.Add(new OverlayItem(
                "Meta",
                meta => meta));

            Items.Add(new OverlayItem(
                "Custom",
                meta => meta is null ? "{Custom}" : $"{Eval(meta)}"));

            Items.Add(new OverlayItem(
                "If",
                (meta) => {
                    meta ??= "";
                    var metas = meta.Split(new[] {':'}, 2);
                    if (metas.Length == 1) return meta;
                    if (metas[1].StartsWith("\n")) metas[1] = metas[1].Remove(0, 1);
                    return Eval(metas[0]) as bool? == true ? metas[1] : "";
                }));
        }

        public OverlayData Data {
            get {
                _Data.isEnabled = gameObject.activeInHierarchy;
                _Data.Position = transform.position;
                _Data.FormatText = FormatText;
                _Data.Size = rectTransform.sizeDelta;
                _Data.LineSpace = text.lineSpacing;
                _Data.TextPosition = text.alignment;
                _Data.Font = CustomOverlay.Fonts.IndexOf(text.font);
                return _Data;
            }
        }

        public string FormatText;

        private void Awake() {
            text = gameObject.GetOrAddComponent<Text>();
            rectTransform = gameObject.GetOrAddComponent<RectTransform>();
            text.font = RDString.GetFontDataForLanguage(RDString.language).font;
            text.fontSize = 60;
            text.color = Color.white;
            var shadow = gameObject.AddComponent<Shadow>();
            shadow.enabled = true;
            shadow.effectDistance = new Vector2(3, -3);
        }

        private void Update() {
            text.text = Format(FormatText);
        }

        private static TimeSpan UptimeSpan;
        private static int UptimeSec;
        private static int UptimeMin;
        private static int UptimeHour;
        private static int UptimeDay;

        public static string Format(string str) {

            var startTime = Process.GetCurrentProcess().StartTime;
            UptimeSpan = DateTime.Now - startTime;
            UptimeSec = UptimeSpan.Seconds;
            UptimeMin = UptimeSpan.Minutes;
            UptimeHour = UptimeSpan.Hours;
            UptimeDay = UptimeSpan.Days;

            var input = str ?? "";

            foreach (var item in Items) {
                var sig = item.Name;
                if (!input.Contains($"{{{sig}")) continue;
                string replaceHolder = null;
                if (!(item.ActiveCondition is null || item.ActiveCondition())) replaceHolder = item.PlaceHolder;
                var pattern = new Regex($"{{{sig}}}");
                input = pattern.Replace(input, replaceHolder ?? (item.ValueGetter(null) ?? "null"));

                var pattern2 = new Regex($"{{{sig}:.*?}}", RegexOptions.Singleline);

                var match = pattern2.Match(input);
                while (match.Value != String.Empty) {
                    var index = sig.Length + 2;
                    var metaValue = match.Value.Substring(index, match.Value.Length - 1 - index);
                    input = pattern2.Replace(input, replaceHolder ?? item.ValueGetter(metaValue) ?? "null", 1);

                    match = pattern2.Match(input);
                }
            }

            return input;
        }

        internal static bool IfPlaying() => !(scnEditor.instance is null) &&
                                            !(scrController.instance.paused && !GCS.standaloneLevelMode) ||
                                            scnEditor.instance is null && scrController.isGameWorld;
            //!(scrController.instance is null || scrConductor.instance is null) &&
              //                              (!scrController.instance.levelEditorMode || Playing && !(scnEditor.instance is null) && !(scnCLS.instance is null));

        public static object Eval(string path) {
            if (path == null) return "null";

            switch (path) {
                case "Playing":
                    return IfPlaying();
            }

            var Fields = path.Split('.');
            var toGet = Assembly.GetAssembly(typeof(scrController)).GetType(Fields[0]);
            if (toGet is null) {
                return $"Type {Fields[0]} not found";
            }

            object value = null;
            var forCount = 0;
            foreach (var f in Fields) {
                var name = f.Split('(')[0];
                var args = f.Replace(name, "");
                if (forCount == 0) {
                    forCount++;
                    continue;
                }

                MemberInfo[] members = toGet.GetMember(name, AccessTools.all);
                if (members.Length == 0) {
                    return $"Field {path} not found";
                }

                MemberInfo member = members[0];

                if (member is FieldInfo field) {
                    value = field.GetValue(value);
                    if (value is null) {
                        return "null";
                    }

                    toGet = value.GetType();
                    continue;
                }

                if (member is PropertyInfo property) {
                    value = property.GetValue(value);
                    if (value is null) {
                        return "null";
                    }

                    toGet = value.GetType();
                }

                forCount++;
            }

            if (forCount == 1) return toGet;
            value ??= "null";
            return value;

        }
    }
}
