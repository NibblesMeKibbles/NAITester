using Godot;

namespace NAITester;
public partial class RemoveMode_Button : Button {
	public override void _Pressed() {
		Game.Instance.SwitchMode(0);
	}
}