using System;
using System.Collections.Generic;
using System.Reflection;
using ADOLib;
using ADOLib.Settings;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using ADOLib.Translation;
using CustomOverlay.Behaviour;
using UnityEngine.UI;
using static ADOLib.Settings.SettingsUI;
using Object = UnityEngine.Object;

namespace CustomOverlay {
	class CustomOverlay
	{
		public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }
		public static Settings settings;
		public static Translator Translator;
		public static string Path;
		public static UnityModManager.ModEntry ModEntry;

		internal static GUIStyle InputShorter = GUIExtended.TextInput;
		internal static List<Font> Fonts = new List<Font> {
			RDConstants.data.koreanFont,
			RDConstants.data.chineseFont,
			RDConstants.data.japaneseFont,
			RDConstants.data.legacyFont,
			RDConstants.data.arialFont,
			RDConstants.data.georgiaFont,
			RDConstants.data.impactFont,
			RDConstants.data.courierNewFont,
			RDConstants.data.timesNewRomanFont,
			RDConstants.data.comicSansMSFont
		};
		
		internal static List<string> FontNames = new List<string> ();
		internal static void Setup(UnityModManager.ModEntry modEntry)
		{
			ModEntry = modEntry;
			Harmony harmony = new Harmony(modEntry.Info.Id);
			Logger = modEntry.Logger;
			settings = Category.GetCategory<Settings>();

			foreach (var font in Fonts)
			{
				FontNames.Add(font.fontNames[0]);
			}
			
			Path = modEntry.Path;
			UnityModManager.Logger.Log(Path);
			Translator = new Translator(Path);
			var obj = new GameObject();
			obj.AddComponent<OverlayCanvas>();
			
			foreach (var overlay in settings.Overlays)
			{
				overlay.MakeOverlay();
			}

			InputShorter.font = new GUIStyle().font;
			InputShorter.fontSize = 18;
			InputShorter.richText = false;
			InputShorter.alignment = TextAnchor.UpperLeft;
		}
	}
}