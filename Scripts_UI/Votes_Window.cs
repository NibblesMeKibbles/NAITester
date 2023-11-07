using Godot;

namespace NAITester;
public partial class Votes_Window : Window {
	public override void _Ready() {
		CloseRequested += OnCloseRequested;
	}

	private void OnCloseRequested() {
		Hide();
	}
}