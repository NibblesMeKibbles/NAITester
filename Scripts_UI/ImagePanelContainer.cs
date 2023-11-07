using Godot;

namespace NAITester;
public partial class ImagePanelContainer : PanelContainer {
	public int Index;
	public string Tag;
	public LineEdit LineEdit;
	public TextureRect TextureRect;

	public void Initialize(int index, string tag, int mode) {
		Index = index;
		Tag = tag;
		LineEdit = (LineEdit)GetNode("Image_LineEdit");
		LineEdit.Text = tag;
		TextureRect = (TextureRect)GetNode("Image_TextureRect");

		((Button)GetNode("Image_HBoxContainer/Reroll_Button")).Pressed += OnReroll;
		((Button)GetNode("Image_HBoxContainer/Save_Button")).Pressed += OnSave;
		((Button)GetNode("Image_HBoxContainer/Delete_Button")).Pressed += OnDelete;
		((Button)GetNode("Image_HBoxContainer/Reveal_Button")).Pressed += OnReveal;

		((Button)GetNode("Image_HBoxContainer/Delete_Button")).Visible = mode == 0;
		((Button)GetNode("Image_HBoxContainer/Reveal_Button")).Visible = mode == 1;

		((VBoxContainer)GetNode("Vote_VBoxContainer")).Visible = mode == 1;
		if (mode == 1) {
			OnReveal();
			((Button)GetNode("Vote_VBoxContainer/VoteGreat_Button")).Pressed += () => { OnVote(10); };
			((Button)GetNode("Vote_VBoxContainer/VoteGood_Button")).Pressed += () => { OnVote(7); };
			((Button)GetNode("Vote_VBoxContainer/VoteBad_Button")).Pressed += () => { OnVote(3); };
			((Button)GetNode("Vote_VBoxContainer/VoteAwful_Button")).Pressed += () => { OnVote(0); };
		}
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

	private void OnReveal() {
		LineEdit.Visible = !LineEdit.Visible;
	}

	private void OnVote(int vote) {
		Game.Instance.ImageGen.VoteTag(Index, Tag, vote);
	}
}