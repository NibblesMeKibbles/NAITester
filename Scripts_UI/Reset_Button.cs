using Godot;

namespace NAITester;
public partial class Reset_Button : Button {
	public override void _Pressed() {
		((Window)Game.Instance.UI["Reset_ConfirmationDialog"]).Show();
	}
}