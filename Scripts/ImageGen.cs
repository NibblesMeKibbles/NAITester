using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAITester;
public partial class ImageGen {

	public class ImageContainer {
		public int Index;
		public string Tag;
		public byte[] Bytes;
		public ImagePanelContainer Container;
	}

	private readonly Game Game;

	public bool IsRunning;
	public int ImageIndex = 0;
	public int RerollIndex = -1;
	public int RetryCounter = 3;
	public System.Collections.Generic.Dictionary<int, ImageContainer> Images = new();

	public string Prompt;
	public string UndesiredContent;
	public uint Seed;
	public List<string> TagList = new();

	public RandomNumberGenerator Random = new();

	private readonly HttpRequest httpRequestImage;
	private bool httpBusy = false;
	private readonly string zipPath = Game.userRootPath + "images.zip";

	public ImageGen(Game game) {
		Game = game;
		zipPath = Game.userRootPath + "images-" + Random.Randi() + ".zip";

		httpRequestImage = (HttpRequest)Game.UI["HTTPRequest_Image"];
		httpRequestImage.DownloadFile = zipPath;
		httpRequestImage.RequestCompleted += OnImageRequestCompleted;
		httpRequestImage.Timeout = 25.0;
	}

	public void CleanUp() {
		httpRequestImage.CancelRequest();
		if (FileAccess.FileExists(zipPath)) {
			DirAccess dir = DirAccess.Open(Game.userRootPath);
			dir.Remove(zipPath);
		}
	}

	public void StartGeneration() {
		if (!((LineEdit)Game.Instance.UI["ApiKey_LineEdit"]).Text.Contains("pst")) {
			Game.CreateAlert("No valid API Key was provided.");
			return;
		}
		if (IsRunning) {
			SetStatus(false);
			return;
		}
		if (httpBusy) {
			Game.CreateAlert("An API call is currently in progress.");
			return;
		}

		Random.Randomize();
		Prompt = ((TextEdit)Game.Instance.UI["Prompt_TextEdit"]).Text;
		UndesiredContent = ((TextEdit)Game.Instance.UI["Undesired_TextEdit"]).Text;
		Seed = string.IsNullOrWhiteSpace(((LineEdit)Game.Instance.UI["Seed_LineEdit"]).Text) ? Random.Randi(): Convert.ToUInt32(((LineEdit)Game.Instance.UI["Seed_LineEdit"]).Text);
		((LineEdit)Game.Instance.UI["Seed_LineEdit"]).Text = Seed.ToString();
		TagList = ((TextEdit)Game.Instance.UI["TagList_TextEdit"]).Text.Split("\n").ToList();

		if (ImageIndex < TagList.Count) {
			GenerateImage(ImageIndex);
		}
	}

	public void ResetImages() {
		ImageIndex = 0;
		foreach (int index in Images.Keys) {
			if (GodotObject.IsInstanceValid(Images[index].Container)) {
				Images[index].Container?.QueueFree();
			}
		}
		Images.Clear();
	}

	public void GenerateImage(int index, bool forceRandomSeed = false) {
		if (!forceRandomSeed)
			ImageIndex = index;
		((Label)Game.UI["Index_Label"]).Text = index.ToString() + " / " + TagList.Count.ToString();

		Random.Randomize();
		SetStatus(true);
		RetryCounter--;

		string[] headers = new string[] {
			"Authorization: Bearer " + ((LineEdit)Game.Instance.UI["ApiKey_LineEdit"]).Text,
			"Content-Type: application/json"
		};

		string thisTag = TagList[index];
		string thisPrompt = Prompt.Replace("%TAG%", thisTag);
		string thisUndesired = UndesiredContent.Replace("%TAG%", thisTag);
		uint thisSeed = forceRandomSeed ? Random.Randi() : Seed;

		if ((bool)((Dictionary)((Dictionary)Game.Config["ApiPayloadBody"])["parameters"])["qualityToggle"]) {
			thisPrompt = ((string)Game.Config["QualityToggle"]) + thisPrompt;
		}
		thisUndesired = ((string[])Game.Config["UCPresets"])[(int)((Dictionary)((Dictionary)Game.Config["ApiPayloadBody"])["parameters"])["ucPreset"]] + thisUndesired;

		((Dictionary)Game.Config["ApiPayloadBody"])["input"] = thisPrompt;
		((Dictionary)((Dictionary)Game.Config["ApiPayloadBody"])["parameters"])["negative_prompt"] = thisUndesired;
		((Dictionary)((Dictionary)Game.Config["ApiPayloadBody"])["parameters"])["seed"] = thisSeed;
		string body = Json.Stringify((Dictionary)Game.Config["ApiPayloadBody"]);

		httpBusy = true;
		Error error = httpRequestImage.Request(Game.ApiImage, headers, HttpClient.Method.Post, body);
		if (error != Error.Ok) {
			if (RetryCounter < 0) {
				Game.CreateAlert("Failed to send HttpRequest" +
					"\nURI: /api/generate-iamge" +
					"\nError Code: " + error.ToString());
				SetStatus(false, true);
				httpBusy = false;
			}
			else {
				GenerateImage(index, forceRandomSeed);
			}
		}
	}

	// HttpRequest byte stream is in x-zip-compress format, save to .zip, then unzip the .png
	private void OnImageRequestCompleted(long result, long responseCode, string[] headers, byte[] body) {
		httpBusy = false;
		if (result != (long)HttpRequest.Result.Success || responseCode != 200) {
			if (RetryCounter < 0) {
				string responseBody = Encoding.UTF8.GetString(body);
				if (FileAccess.FileExists(zipPath)) {
					FileAccess fileAccess = FileAccess.Open(zipPath, FileAccess.ModeFlags.Read);
					if (fileAccess.GetLength() < 1000) {
						responseBody = fileAccess.GetAsText();
					}
					fileAccess.Close();
				}
				Game.CreateAlert("/api/generate-image" +
					"\nHttpRequest Status: " + ((HttpRequest.Result)result).ToString() +
					"\nResponse Code: " + responseCode.ToString() +
					"\nResponse Body: " + responseBody);
				SetStatus(false, true);
			}
			else {
				GenerateImage(RerollIndex >= 0 ? RerollIndex : ImageIndex, RerollIndex >= 0);
			}
		}
		else {
			ZipReader reader = new();
			Error error = reader.Open(zipPath);
			if (error != Error.Ok) {
				if (RetryCounter < 0) {
					Game.CreateAlert("Unzip image generation download failed" +
					"\nError Code: " + error.ToString());
					SetStatus(false, true);
				}
			}
			else {
				byte[] imageBytes = reader.ReadFile("image_0.png");
				reader.Close();
				if (imageBytes == null || imageBytes.Length == 0) {
					if (RetryCounter < 0) {
						Game.CreateAlert("Unzipped PNG file read failed" +
						"\nError Code: " + error.ToString());
						SetStatus(false, true);
					}
					else {
						GenerateImage(RerollIndex >= 0 ? RerollIndex : ImageIndex, RerollIndex >= 0);
					}
				}
				else if (RerollIndex < 0) {
					Images[ImageIndex] = new ImageContainer {
						Index = ImageIndex,
						Tag = TagList[ImageIndex],
						Bytes = imageBytes,
						Container = CreateImageContainer(ImageIndex)
					};
					LoadImage(ImageIndex);
				}
				else if (Images.ContainsKey(RerollIndex)) {
					Images[RerollIndex].Bytes = imageBytes;
					LoadImage(RerollIndex);
				}
				else {
					RerollIndex = -1;
					UpdateImageCounterLabel();
					SetStatus(false, true);
				}
			}
		}
	}

	private ImagePanelContainer CreateImageContainer(int index) {
		ImagePanelContainer container = (ImagePanelContainer)GD.Load<PackedScene>("res://ImagePanelContainer.tscn").Instantiate();
		((GridContainer)Game.UI["Images_GridContainer"]).AddChild(container);
		container.Initialize(index, TagList[index]);
		return container;
	}

	public void LoadImage(int index) {
		var image = new Image();
		Error error = image.LoadPngFromBuffer(Images[index].Bytes);
		if (error != Error.Ok) {
			DeleteIndex(index);
			if (RetryCounter < 0) {
				Game.CreateAlert("Load byte buffer to PNG format failed" +
				"\nError Code: " + error.ToString());
				SetStatus(false, true);
			}
			else {
				GenerateImage(index, RerollIndex >= 0);
			}
		}
		else {
			var texture = ImageTexture.CreateFromImage(image);
			if (texture == null) {
				DeleteIndex(index);
				if (RetryCounter < 0) {
					Game.CreateAlert("Convert loaded PNG to texture failed" +
					"\nError Code: " + error.ToString());
					SetStatus(false, true);
				}
				else {
					GenerateImage(index, RerollIndex >= 0);
				}
			}
			else {
				Images[index].Container.TextureRect.Texture = texture;
				RetryCounter = 3;

				if (RerollIndex < 0) {
					ImageIndex++;
					if (IsRunning && ImageIndex < TagList.Count) {
						GenerateImage(ImageIndex);
					}
					else {
						UpdateImageCounterLabel();
						SetStatus(false, true);
					}
				}
				else {
					RerollIndex = -1;
					UpdateImageCounterLabel();
					SetStatus(false, true);
				}
			}
		}
	}

	public void RerollImage(int index) {
		if (IsRunning) {
			Game.CreateAlert("An image generation is currently in progress.");
			return;
		}
		if (httpBusy) {
			Game.CreateAlert("An API call is currently in progress.");
			return;
		}
		RerollIndex = index;
		GenerateImage(index, true);
	}

	public void SaveImage(int index) {
		if (!DirAccess.DirExistsAbsolute(ProjectSettings.GlobalizePath(Game.imagesRootPath))) {
			Error error = DirAccess.MakeDirAbsolute(ProjectSettings.GlobalizePath(Game.imagesRootPath));
			if (error != Error.Ok) {
				Game.CreateAlert("Failed to create /SavedImages/ folder" +
					"\nFolder Path: " + ProjectSettings.GlobalizePath(Game.imagesRootPath) +
					"\nError Code: " + error.ToString());
				return;
			}
		}
		string pngName = Godot.Time.GetDatetimeStringFromSystem().Replace(':', '-').Replace('T', '-') + "_" + Images[index].Tag + ".png";
		string filePath = Game.imagesRootPath + "Image_" + pngName;
		FileAccess file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		if (file == null) {
			Game.CreateAlert("Failed to create destination file" +
				"\nImage Index: " + index.ToString() +
				"\nFile Path: " + filePath);
			return;
		}
		if (Images[index] == null) {
			Game.CreateAlert("Image bytes is empty" +
				"\nImage Index: " + index.ToString() +
				"\nFile Path: " + filePath);
			file.Close();
			return;
		}
		file.StoreBuffer(Images[index].Bytes);
		file.Close();
		Game.CreatePopup("Succesfully saved image.\nFile Path: " + pngName);
	}

	public void DeleteIndex(int index) {
		if (Images.ContainsKey(index)) {
			List<string> tagList = ((TextEdit)Game.Instance.UI["TagList_TextEdit"]).Text.Split("\n").ToList();
			if (tagList.Remove(Images[index].Tag)) {
				((TextEdit)Game.Instance.UI["TagList_TextEdit"]).Text = tagList.ToArray().Join("\n");
			}
			Images[index].Container?.QueueFree();
			Images.Remove(index);
			UpdateImageCounterLabel();
		}
	}

	public void SetStatus(bool running, bool alert = false) {
		IsRunning = running;
		((Button)Game.Instance.UI["Start_Button"]).Text = IsRunning ? "Pause" : "Start";
		((Label)Game.UI["Loading_Label"]).Visible = running || httpBusy;

		if (running == false && alert == true) {
			DisplayServer.WindowRequestAttention();
		}
	}

	private void UpdateImageCounterLabel() {
		((Label)Game.UI["Index_Label"]).Text = ImageIndex.ToString() + " / " + TagList.Count.ToString();
	}
}