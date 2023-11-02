# NAI Tester
NAI Tester is a standalone Godot (.NET) desktop game/app to streamline testing a list of tags into a **Prompt** and/or **Undesired Content** for NovelAI's image generator.

The [`config.json`](config.json) contains the base API payload information sent to each image generation API call, with the `input`, `negative_prompt` and `seed` properties being supplied values at runtime using the provided configurations of the in-game text fields.

The `config.json` is copied from the application content to the user directory when the application is ran for the first time, and can be browsed to using the in-game "Open user directory" button (<img src="Icons/icons8-open-folder-in-new-tab-50.png" width="20">). The `config.json` can be modified in a text editor and reloaded in-game during runtime to instantly reflect any changes made.

## Functionality
Provide a **Prompt** and **Undesired Content** in the appropriate text fields, **either or both** of the fields should contain the text: **`%TAG%`**. Given a list of tags (split by new lines) in the **Tag List** field, iterate over each tag to replace the string `%TAG%` with the tag to provide a series of images with variations according to the tag replacement text.

*Example:*
**Prompt:**
1girl, {artist:%TAG%}
**Tag List:**
Aaron Douglas
Adam Martinakis
Adolph Gottlieb

Result: 3 images will be generated with the following prompts: `1girl, {artist:Aaron Douglas}`, `1girl, {artist:Adam Martinakis}`, and `1girl, {artist:Adolph Gottlieb}`.

## In-Game UI
|Name|Icon|Functionality|
|-|-|-|
|Resize Slider||The knob at the top of the screen horizontally resizes the image grid|
|NovelAI API Key||Text field for supplying a NovelAI API Key<br />Browse to NovelAI site > User Settings > Account > Get Persistent API Token|
|||
|Prompt||Text field of the base image generation **Prompt**|
|Undesired Content||Text field of the base image generation **Undesired Content**|
|Seed||An optional whole number of the image generation<br />A random value is provided if left blank|
|Tag List||A list of tags to inject into the **Prompt** and/or **Undesired Content**<br >See [Functionality](#functionality) for example formatting|
||||
|Columns slider||Number of columns of the image grid|
|Reload config.json|<img src="Icons/icons8-reload-50.png" width="20">|Re-import the user folder `config.json` and its configurations|
|Reset config.json to default|<img src="Icons/icons8-reset-50.png" width="20">|Restore the user `config.json` to the default resource `config.json` contents|
|Open user directory|<img src="Icons/icons8-open-folder-in-new-tab-50.png" width="20">|Open a shell window to the user directory that contains the local `config.json`, `README.md`, and any `/SavedImages/`|
|Toggle FullScreen Mode|<img src="Icons/icons8-fit-to-width-50.png" width="20">|Switch between Windows and FullScreen display mode|
|Exit to Desktop|<img src="Icons/icons8-quit-50.png" width="20">|Quit the game|
||||
|Start/Pause||Start or pause the image generation, resuming from the last generated image|
|#/#||Number of current images loaded versus number of total tags provided|
|Restart||Clear all loaded images and restart image generation from the first tag|
||||
|Tag Label||The tag used for the loaded image|
|Reroll tag with new seed|<img src="Icons/icons8-reload-50.png" width="20">|Replace the image with a new generation, using the same tag with a new random seed|
|Save image (with metadata)|<img src="Icons/icons8-save-50.png" width="20">|Save the image to a timestamped `.png` file with full NovelAI metadata|
|Delete image and remove tag from Tag List|<img src="Icons/icons8-delete-48.png" width="20">|Delete the loaded image from the image grid and remove the image's tag from the **Tag List** text field|

## Modifying the API Payload
The base API payload is visible in the [`config.json`](config.json) file. To assuredly modify values (such as `model` name): open the NovelAI image generator website in the browser, select your preferred image and model settings, open the Developer Tools (F12?), open the Network tab, and generate an image using normal means. The Network tab will add a line with the name `/api/generate-image` (or just `generate-image`), select that API call, select the Payload tab, and copy desired values to the `config.json`.

The following two functionality are existing logic of the NovelAI image generation in the web browser:
- The `qualityToggle` property determines whether the `QualityToggle` list of tags should be prepended to the `input` prompt.
- The `ucPreset` property is a value of 0, 1, or 2 to determine which index of the `UCPresets` array should be prepended to the `negative_prompt`.
