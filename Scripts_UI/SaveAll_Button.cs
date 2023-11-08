using Godot;

namespace NAITester;
public partial class SaveAll_Button : Button {
	public override void _Pressed() {
		((Window)Game.Instance.UI["SaveAll_ConfirmationDialog"]).Show();
	}
}