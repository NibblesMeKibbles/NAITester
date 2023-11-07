using Godot;

namespace NAITester;
public partial class VoteMode_Button : Button {
	public override void _Pressed() {
		Game.Instance.SwitchMode(1);
	}
}