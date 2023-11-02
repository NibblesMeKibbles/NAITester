using Godot;

namespace NAITester;
public partial class Config_VBoxContainer : VBoxContainer {
	public override void _Ready() {
		GetViewport().SizeChanged += OnGameResized;
		((HSlider)GetNode("../Resize_HSlider")).ValueChanged += (double value) => { OnGameResized(); };

		SetDeferred("Size", new Vector2(GetTree().Root.Size.X * 0.34f, GetTree().Root.Size.Y));
	}

	private void OnGameResized() {
		Size = new Vector2(GetTree().Root.Size.X * (1f - (float)((HSlider)Game.Instance.UI["Resize_HSlider"]).Value), GetTree().Root.Size.Y - 4f);
		Position = new Vector2(GetTree().Root.Size.X - Size.X, 4f);
	}
}