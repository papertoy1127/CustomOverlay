using System;
using JetBrains.Annotations;

namespace CustomOverlay.Behaviour {
    public class OverlayItem {
        public readonly string PlaceHolder;
        public readonly string Name;
        public readonly Func<bool> ActiveCondition;
        public readonly Func<string, string> ValueGetter;
        
        public OverlayItem(string name, Func<bool> activeCondition, Func<string, string> valueGetter, string placeHolder = "Not Playing") {
            Name = name;
            ActiveCondition = activeCondition;
            ValueGetter = valueGetter;
            PlaceHolder = placeHolder;
        }
        
        public OverlayItem(string name, Func<string, string> valueGetter, string placeHolder = "Not Playing") {
            Name = name;
            ActiveCondition = null;
            ValueGetter = valueGetter;
            PlaceHolder = placeHolder;
        }
    }
}