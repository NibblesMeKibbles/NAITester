using Godot;

namespace NAITester;
public partial class Open_Button : Button {
	public override void _Pressed() {
		Game.Instance.OpenUserRoot();
	}
}