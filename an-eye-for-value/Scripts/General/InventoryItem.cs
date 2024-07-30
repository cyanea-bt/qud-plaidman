using ConsoleLib.Console;
using Plaidman.AnEyeForValue.Utils;
using XRL.World;

namespace Plaidman.AnEyeForValue.Menus {
	public enum SortType { Weight, Value };
	
	public class ToggledItem {
		public int Index { get; }
		public bool Value { get; }

		public ToggledItem(int index, bool value) {
			Index = index;
			Value = value;
		}
	}

	public class InventoryItem {
		public int Index { get; }
		public string DisplayName { get; }
		public IRenderable Icon { get; }
		public int Weight { get; }
		public double? Value { get; }
		public double? Ratio { get; }
		public bool Known { get; }

		public InventoryItem(int index, GameObject go, bool known) {
			Index = index;
			DisplayName = go.DisplayName;
			Icon = go.Render;
			Weight = go.Weight;
			Value = ValueUtils.GetValue(go);
			Ratio = ValueUtils.GetValueRatio(Value, Weight);
			Known = known;
		}
	}
}