using Godot;

namespace NAITester;
public partial class Loading_Label : Label {
	public override void _Process(double delta) {
		Rotation = (Rotation + (float)delta * 5f) % 360f;
	}
}