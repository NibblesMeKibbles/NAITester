using Godot;

namespace NAITester;
public partial class Restart_Button : Button {
	public override void _Pressed() {
		Game.Instance.ImageGen.StartGeneration(true);
	}
}