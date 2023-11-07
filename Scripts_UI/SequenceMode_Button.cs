using Godot;

namespace NAITester;
public partial class SequenceMode_Button : Button {
	public override void _Pressed() {
		Game.Instance.SwitchMode(2);
	}
}