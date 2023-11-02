using Godot;

namespace NAITester;
public partial class Reload_Button : Button {
	public override void _Pressed() {
		Game.Instance.LoadConfigJson();
	}
}