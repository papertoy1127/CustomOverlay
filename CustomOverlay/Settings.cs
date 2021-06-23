using System;
using System.Collections.Generic;
using ADOLib;
using ADOLib.Settings;
using ADOLib.Translation;
using CustomOverlay.Behaviour;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using Object = UnityEngine.Object;
using static ADOLib.Settings.GUIExtended;

namespace CustomOverlay {
	[Category(
		TabName = "Custom Overlay", 
		Name = "Custom Overlay",
		MinVersion = 71,
		PatchClass = typeof(Patch.Patch)
		)]
	public class Settings : Category
	{
		public List<OverlayData> Overlays = new List<OverlayData>();
		public List<(string, OverlayData)> Presets = new List<(string, OverlayData)>();
		public static bool PresetExpanded = false;
		public bool HideTitle = true;
		public override UnityModManager.ModEntry ModEntry => CustomOverlay.ModEntry;
		private static Translator Translator => CustomOverlay.Translator;

		private static Vector2 scrollPos;
		private static Vector2 scrollPos2;

		public override void OnSave() {
			Overlays = new List<OverlayData>();
			foreach (OverlayBase overlayBase in OverlayBase.Overlays)
			{
				Overlays.Add(overlayBase.Data);
			}
		}

		public override void OnGUI()
		{
			var link = GUIExtended.Text;
			link.normal.textColor = Color.blue * 0.8f;
			link.hover.textColor = Color.blue * 0.9f;
			var txt = GUIStyle.none;
			
			var temp = HideTitle;
			HideTitle = Toggle(HideTitle, CustomOverlay.Translator.Translate("UI.HideTitle"));
			if (temp != HideTitle) {
				if (HideTitle) {
					if (CustomOverlay.settings.HideTitle && scrController.instance.txtCaption != null)
						scrController.instance.txtCaption.transform.localScale = Vector3.zero;
				}
				else {
					UnityModManager.Logger.Log("on!");
					if (CustomOverlay.settings.HideTitle && scrController.instance.txtCaption != null) {
						scrController.instance.txtCaption.transform.localScale = Vector3.one;
					}
				}
			}

			//GUILayout.Label("<size=50>Custom Overlay</size>", SettingsUI.Text);
			GUILayout.BeginVertical(GUILayout.Width(400));
			scrollPos = GUILayout.BeginScrollView(scrollPos, txt, GUILayout.Height(300));
			GUILayout.Label(Translator.Translate("UI.Methods"), SettingsUI.Text, GUILayout.Width(350));
			GUILayout.Label(Translator.Translate("UI.HelpLink1"), SettingsUI.Text, GUILayout.Width(350));
			if (GUILayout.Button(Translator.Translate("UI.HelpLink"), link, GUILayout.Width(350)))
			{
				Application.OpenURL("https://www.notion.so/CustomOverlay-d4aca3bc86064859adb54a06d514613b");
			}
			GUILayout.Space(20);
			GUILayout.Label(Translator.Translate("UI.HelpCustomOverlay"), SettingsUI.Text, GUILayout.Width(350));

			GUILayout.Label(Translator.Translate("UI.HelpCustomOverlay"), SettingsUI.Text, GUILayout.Width(350));
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(Translator.Translate("UI.AddOverlay"), Selection, 
				GUILayout.Width(120), GUILayout.Height(50)))
			{
				GameObject textObject = new GameObject();
				textObject.transform.SetParent(OverlayCanvas.instance.transform);
				Object.DontDestroyOnLoad(textObject);
				//textObject.transform.localPosition = new Vector3(Screen.width / 2, Screen.height / 2);
				textObject.transform.position = new Vector3(200, 900);

				Text text = textObject.AddComponent<Text>();
				text.font = RDConstants.data.koreanFont;
				text.fontSize = 60;
				text.color = Color.white;

				RectTransform rectTransform = textObject.GetComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
				rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
				rectTransform.pivot = new Vector2(0.5f, 0.5f);
				rectTransform.sizeDelta = new Vector2(400f, 400f);


				text.alignment = TextAnchor.UpperLeft;
				OverlayBase.Overlays.Add(textObject.AddComponent<OverlayBase>());
				
				Presets.Add(("sans", textObject.GetComponent<OverlayBase>().Data));
			}

			GUILayout.Space(20);

			if (PresetExpanded) {
				PresetExpanded = GUILayout.Toggle(PresetExpanded, Translator.Translate("UI.Presets"), GUIExtended.SelectionActive, 
					GUILayout.Width(120), GUILayout.Height(50));
			}
			else {
				PresetExpanded = GUILayout.Toggle(PresetExpanded, Translator.Translate("UI.Presets"), GUIExtended.Selection, 
					GUILayout.Width(120), GUILayout.Height(50));
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(15);
			
			if (PresetExpanded) {
				var bg = GUIExtended.TextInput;
				var blackText = GUIExtended.Text;
				blackText.normal.textColor = Color.white;
				blackText.hover.textColor = Color.white;
				blackText.fontSize = 40;

				GUILayout.BeginVertical(bg, GUILayout.Width(220), GUILayout.MinHeight(200));
				GUILayout.Space(10);
				BeginIndent(10);
				if (Presets.Count == 0) GUILayout.Label("No preset found :(", blackText);
				else {
					foreach (var preset in Presets) {
						if (GUILayout.Button(preset.Item1, GUIExtended.Selection, GUILayout.Width(200))) {
							preset.Item2.MakeOverlay();
						}

						GUILayout.BeginHorizontal();
						GUILayout.Space(10);
						GUILayout.EndHorizontal();
						GUILayout.Space(10);
					}

					GUILayout.Space(10);
				}

				EndIndent();
				GUILayout.EndVertical();
			}
			var count = 1;
			foreach (var overlay in OverlayBase.Overlays.ToArray())
			{
				GUILayout.BeginVertical(GUILayout.Width(600));
				try
				{
					if (overlay.gameObject is null) {}
				} catch (NullReferenceException)
				{
					OverlayBase.Overlays.Remove(overlay);
				}
				GUILayout.BeginHorizontal(GUILayout.Width(230));
				GUILayout.Label($"<size=45>{Translator.Translate("UI.Overlay")} #{count}</size>", SettingsUI.Text);
				var Align = GUIExtended.Text;
				Align.alignment = TextAnchor.LowerRight;
				if (GUILayout.Button($"<color=#990000><size=#50>{Translator.Translate("UI.RemoveOverlay")} ✖️</size></color>", Align))
				{
					OverlayBase.Overlays.Remove(overlay);
					Object.Destroy(overlay.gameObject);
				}
				GUILayout.EndHorizontal();
				overlay.gameObject.SetActive(GUIExtended.Toggle(overlay.gameObject.activeInHierarchy, Translator.Translate("UI.OverlayEnabled")));
				
				GUILayout.Label(Translator.Translate("UI.OverlayContent"), SettingsUI.Text);
				GUILayout.BeginVertical(GUILayout.Width(600));
				scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Height(200));
				overlay.FormatText = GUILayout.TextArea(overlay.FormatText, CustomOverlay.InputShorter, GUILayout.MinHeight(200));
				GUILayout.EndScrollView();
				GUILayout.EndVertical();
				GUILayout.Label(Translator.Translate("UI.OverlayPosition"), SettingsUI.Text);
				Vector2 pos = overlay.transform.position;
				GUILayout.BeginHorizontal();
				GUILayout.Label("X: ", SettingsUI.Text, GUILayout.Width(50));
				var xString = GUILayout.TextField(((int)pos.x).ToString(), TextInput, GUILayout.Width(70), GUILayout.Height(40));
				GUILayout.Label("  Y: ", SettingsUI.Text, GUILayout.Width(50));
				var yString = GUILayout.TextField(((int)pos.y).ToString(), TextInput, GUILayout.Width(70), GUILayout.Height(40));
				if (int.TryParse(xString, out var xTmp)) pos.x = xTmp;
				if (int.TryParse(yString, out var yTmp)) pos.y = yTmp;
				overlay.transform.position = pos;
				GUILayout.EndHorizontal();
				
				GUILayout.Label(Translator.Translate("UI.OverlaySize"), SettingsUI.Text);
				Vector2 size = overlay.rectTransform.sizeDelta;
				GUILayout.BeginHorizontal();
				GUILayout.Label("X: ", SettingsUI.Text, GUILayout.Width(50));
				xString = GUILayout.TextField(((int)size.x).ToString(), TextInput, GUILayout.Width(70), GUILayout.Height(40));
				GUILayout.Label("  Y: ", SettingsUI.Text, GUILayout.Width(50));
				yString = GUILayout.TextField(((int)size.y).ToString(), TextInput, GUILayout.Width(70), GUILayout.Height(40));
				if (int.TryParse(xString, out var xTmp2)) size.x = xTmp2;
				if (int.TryParse(yString, out var yTmp2)) size.y = yTmp2;
				overlay.rectTransform.sizeDelta = size;
				GUILayout.EndHorizontal();
				
				GUILayout.Label(Translator.Translate("UI.OverlayLineSpace"), SettingsUI.Text);
				var lineSpace = GUILayout.TextField(overlay.text.lineSpacing.ToString(), TextInput, GUILayout.Width(70), GUILayout.Height(40));
				if (float.TryParse(lineSpace, out var lTmp)) overlay.text.lineSpacing = lTmp;
				
				GUILayout.Label(Translator.Translate("UI.OverlayPosition"), SettingsUI.Text);
				overlay.text.alignment = (TextAnchor)GUIExtended.SelectionGrid((int)overlay.text.alignment, new[]
				{
					"UpperLeft",
					"UpperCenter",
					"UpperRight",
					"MiddleLeft",
					"MiddleCenter",
					"MiddleRight",
					"LowerLeft",
					"LowerCenter",
					"LowerRight"
				}, 3, Selection, SelectionActive, GUILayout.Width(100), GUILayout.Height(40));
				count++;
				
				GUILayout.Label(Translator.Translate("UI.OverlayFont"), SettingsUI.Text);
				try
				{
					overlay.text.font =
						CustomOverlay.Fonts[SelectionGrid(CustomOverlay.Fonts.IndexOf(overlay.text.font), CustomOverlay.FontNames.ToArray(), 5)];
				}
				catch (ArgumentOutOfRangeException)
				{
					overlay.text.font =
						CustomOverlay.Fonts[0];
				}
				GUILayout.EndVertical();
			}
		}
	}
}
