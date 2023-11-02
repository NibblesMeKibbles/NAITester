using Godot;

namespace NAITester;
public partial class Images_ScrollContainer : ScrollContainer {
	public override void _Ready() {
		GetViewport().SizeChanged += OnGameResized;
		((HSlider)GetNode("../Resize_HSlider")).ValueChanged += (double value) => { OnGameResized(); };

		SetDeferred("Size", new Vector2(GetTree().Root.Size.X * 0.655f, GetTree().Root.Size.Y));
		((GridContainer)GetNode("Images_GridContainer")).SetDeferred("Size", new Vector2(GetTree().Root.Size.X * 0.655f, GetTree().Root.Size.Y));
	}

	private void OnGameResized() {
		Size = new Vector2(GetTree().Root.Size.X * ((float)((HSlider)Game.Instance.UI["Resize_HSlider"]).Value - 0.005f), GetTree().Root.Size.Y - 4f);;
		((GridContainer)GetNode("Images_GridContainer")).Size = new Vector2(GetTree().Root.Size.X * ((float)((HSlider)Game.Instance.UI["Resize_HSlider"]).Value - 0.005f), GetTree().Root.Size.Y - 4f);
	}
}