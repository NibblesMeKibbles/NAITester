using Godot;

namespace NAITester;
public partial class Columns_HSlider : HSlider {
	public override void _ValueChanged(double newValue) {
		((Label)Game.Instance.UI["Columns_Label"]).Text = Value.ToString() + " Columns";
		((GridContainer)Game.Instance.UI["Images_GridContainer"]).Columns = (int)Value;
	}
}