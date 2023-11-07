using Godot;

namespace NAITester;
public partial class Start_Button : Button {
	public override void _Pressed() {
		Game.Instance.ImageGen.StartGeneration();
	}
}