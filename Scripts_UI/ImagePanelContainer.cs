using Godot;

namespace NAITester;
public partial class ImagePanelContainer : PanelContainer {
	public int Index;
	public LineEdit LineEdit;
	public TextureRect TextureRect;

	public void Initialize(int index, string tag) {
		Index = index;
		LineEdit = (LineEdit)GetNode("Image_LineEdit");
		LineEdit.Text = tag;
		TextureRect = (TextureRect)GetNode("Image_TextureRect");

		((Button)GetNode("Image_HBoxContaine/Reroll_Button")).Pressed += OnReroll;
		((Button)GetNode("Image_HBoxContaine/Save_Button")).Pressed += OnSave;
		((Button)GetNode("Image_HBoxContaine/Delete_Button")).Pressed += OnDelete;
	}

	private void OnReroll() {
		Game.Instance.ImageGen.RerollImage(Index);
	}

	private void OnSave() {
		Game.Instance.ImageGen.SaveImage(Index);
	}

	private void OnDelete() {
		Game.Instance.ImageGen.DeleteIndex(Index);
	}
}