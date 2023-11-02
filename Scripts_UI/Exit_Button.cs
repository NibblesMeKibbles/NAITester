using Godot;

namespace NAITester;
public partial class Exit_Button : Button {
	public override void _Pressed() {
		GetTree().Quit();
	}
}