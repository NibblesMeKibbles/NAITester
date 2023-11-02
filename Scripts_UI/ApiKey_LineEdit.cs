using Godot;

namespace NAITester;
public partial class ApiKey_LineEdit : LineEdit {
	public override void _Ready() {
		GrabFocus();
	}
}