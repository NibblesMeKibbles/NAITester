using Godot;
using Godot.Collections;

namespace NAITester;
public partial class Game : Node2D {
	public static Game Instance { get; private set; }
	public Dictionary Config;
	public ImageGen ImageGen;
	public System.Collections.Generic.Dictionary<string, Node> UI = new();

	public const string resConfigPath = "res://config.json";
	public const string resReadmePath = "res://README.md";
	public const string userRootPath = "user://";
	public const string userConfigPath = userRootPath + "config.json";
	public const string userReadmePath = userRootPath + "README.md";
	public const string imagesRootPath = userRootPath + "SavedImages/";
	public const string ApiTest = "https://api.novelai.net/user/information";
	public const string ApiImage = "https://api.novelai.net/ai/generate-image";

	private bool firstConfigLoad = true;

	public override void _Ready() {
		Instance = this;

		// Build UI lookup Dictionary
		Array<Node> allNodes = FindChildren("*");
		foreach (Node node in allNodes) {
			UI[node.Name] = node;
		}

		ImageGen = new(this);
		LoadConfigJson();
	}

	public override void _Notification(int what) {
		if (what == NotificationWMCloseRequest || what == NotificationWMGoBackRequest) {
			ImageGen.CleanUp();
		}
	}

	public void SwitchMode(int mode) {
		if (ImageGen.IsRunning) {
			CreateAlert("Stop and restart prior to switching modes.");
		}
		else if (!ImageGen.IsFresh) {
			CreateAlert("Restart prior to switching modes.");
		}
		else {
			ImageGen.Mode = mode;
		}

		((Button)UI["RemoveMode_Button"]).SetPressedNoSignal(ImageGen.Mode == 0);
		((Button)UI["VoteMode_Button"]).SetPressedNoSignal(ImageGen.Mode == 1);
		((Button)UI["SequenceMode_Button"]).SetPressedNoSignal(ImageGen.Mode == 2);

		((HBoxContainer)UI["Columns_HBoxContainer"]).Visible = ImageGen.Mode == 0 || ImageGen.Mode == 1;
		((HBoxContainer)UI["ImagesCount_HBoxContainer"]).Visible = ImageGen.Mode == 1;
		((HBoxContainer)UI["Rows_HBoxContainer"]).Visible = ImageGen.Mode == 2;

		((Button)UI["ViewVotes_Button"]).Visible = ImageGen.Mode == 1;
	}

	public void LoadConfigJson(bool forceReset = false) {
		if (Engine.IsEditorHint() || OS.HasFeature("editor") || !FileAccess.FileExists(userConfigPath) && FileAccess.FileExists(resConfigPath) || forceReset) {
			// Copy config.json and README.md
			CopyResFileToUser(resConfigPath, userConfigPath);
			CopyResFileToUser(resReadmePath, userReadmePath);

			// Reset config.json to defaults
			FileAccess fileUser = FileAccess.Open(userConfigPath, FileAccess.ModeFlags.Write);
			FileAccess fileRes = FileAccess.Open(resConfigPath, FileAccess.ModeFlags.Read);
			string content = fileRes.GetAsText();

			fileUser.StoreString(content);
			fileUser.Close();
			fileRes.Close();
		}
		// Parse JSON
		if (FileAccess.FileExists(userConfigPath)) {
			FileAccess file = FileAccess.Open(userConfigPath, FileAccess.ModeFlags.Read);
			string contents = file.GetAsText();
			if (!string.IsNullOrWhiteSpace(contents)) {
				Dictionary json = Json.ParseString(contents).AsGodotDictionary();
				if (json.ContainsKey("ApiPayloadBody")) {
					Config = json;

					if (!firstConfigLoad)
						CreatePopup("Successfully reloaded JSON.");
					firstConfigLoad = false;
				}
			}
			file.Close();
		}
	}

	private void CopyResFileToUser(string pathRes, string pathUser) {
		FileAccess fileRes = FileAccess.Open(pathRes, FileAccess.ModeFlags.Read);
		if (fileRes == null) {
			CreateAlert("Failed to open " + pathRes);
			return;
		}
		FileAccess fileUser = FileAccess.Open(pathUser, FileAccess.ModeFlags.Write);
		if (fileRes == null) {
			CreateAlert("Failed to create " + pathUser +
				"\nFile Path: " + ProjectSettings.GlobalizePath(pathUser));
			fileRes.Close();
			return;
		}
		fileUser.StoreBuffer(fileRes.GetBuffer((long)fileRes.GetLength()));
		fileRes.Close();
		fileUser.Close();
	}

	public void OpenUserRoot() {
		Error error = OS.ShellOpen(ProjectSettings.GlobalizePath(userRootPath));
		if (error != Error.Ok) {
			CreateAlert("Open shell window failed" +
				"\nError Code: " + error.ToString());
		}
	}

	public void CreateAlert(string message) {
		AcceptDialog acceptDialog = new();
		((CanvasLayer)UI["CanvasLayer"]).AddChild(acceptDialog);
		acceptDialog.Title = "";
		acceptDialog.InitialPosition = Window.WindowInitialPosition.CenterPrimaryScreen;
		acceptDialog.DialogText = message;
		acceptDialog.Show();
	}
	public void CreatePopup(string message) {
		PopupMenu popup = new();
		((CanvasLayer)UI["CanvasLayer"]).AddChild(popup);
		popup.Title = "";
		popup.InitialPosition = Window.WindowInitialPosition.CenterPrimaryScreen;
		popup.AddItem(message);
		popup.Show();
	}
}
