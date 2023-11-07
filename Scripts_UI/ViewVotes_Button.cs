using Godot;

namespace NAITester;
public partial class ViewVotes_Button : Button {
	public override void _Pressed() {
		((Window)Game.Instance.UI["Votes_Window"]).Visible = true;
	}
}