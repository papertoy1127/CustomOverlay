using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CustomOverlay.Behaviour
{
    [Serializable]
    public struct OverlayData
    {
        public string FormatText;
        public bool isEnabled;
        public Vector2 Position;
        public Vector2 Size;
        public float LineSpace;
        public TextAnchor TextPosition;
        public int Font;

        public OverlayData(string formatText, bool isEnabled, Vector2 position, Vector2 size, int lineSpace, TextAnchor textPosition, int font)
        {
            FormatText = formatText;
            this.isEnabled = isEnabled;
            Position = position;
            Size = size;
            LineSpace = lineSpace;
            TextPosition = textPosition;
            Font = font;
        }

        public OverlayBase MakeOverlay()
        {
            GameObject textObject = new GameObject();
            textObject.transform.SetParent(OverlayCanvas.instance.transform);
            Object.DontDestroyOnLoad(textObject);
            textObject.transform.position = Position;

            Text text = textObject.AddComponent<Text>();
            text.font = RDString.GetFontDataForLanguage(RDString.language).font;
            text.fontSize = 60;
            text.color = Color.white;

            RectTransform rectTransform = textObject.GetOrAddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = Size;

            text.alignment = TextPosition;
            text.lineSpacing = LineSpace;
            text.font = CustomOverlay.Fonts[Font];

            var result = textObject.AddComponent<OverlayBase>();
            result.FormatText = FormatText;
            OverlayBase.Overlays.Add(result);
            textObject.SetActive(isEnabled);
            return result;
        }
    }
}