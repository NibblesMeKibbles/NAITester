using Godot;

namespace NAITester;
public partial class SaveAll_ConfirmationDialog : ConfirmationDialog {
	public override void _Ready() {
		GetOkButton().Pressed += CallSaveAll;
	}
	private void CallSaveAll() {
		Game.Instance.ImageGen.SaveAllImages();
	}
}