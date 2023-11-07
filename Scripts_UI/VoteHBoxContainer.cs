using Godot;
using System.Collections.Generic;
using System.Linq;

namespace NAITester;
public partial class VoteHBoxContainer : HBoxContainer {
	public string Tag;
	public List<int> Votes = new();
	public LineEdit LineEdit;
	public Button Button;

	public void Initialize(string tag, int vote) {
		Tag = tag;
		LineEdit = (LineEdit)GetNode("Vote_LineEdit");
		LineEdit.Text = tag;
		Button = (Button)GetNode("Vote_Button");
		Vote(vote);
	}

	public void Vote(int vote) {
		Votes.Add(vote);
		Button.Text = Votes.Count == 0 ? "0" : (Votes.Sum() / (float)Votes.Count).ToString("0.0");
	}
}