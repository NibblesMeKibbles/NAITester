# NAI Tester
NAI Tester is a standalone Godot (.NET) desktop game/app to streamline testing a list of tags into a **Prompt** and/or **Undesired Content** for NovelAI's image generator.

**NOTE:** It is the user's responsibility to manage and monitor their **Anlas** credits spendage. Providing API payload configurations that would consume Anlas using the same configurations in the NovelAI image generator site will still consume Anlas.

The [`config.json`](config.json) contains the base API payload information sent to each image generation API call, with the `input`, `negative_prompt` and `seed` properties being supplied values at runtime using the provided configurations of the in-game text fields.

The `config.json` is copied from the application content to the user directory when the application is ran for the first time, and can be browsed to using the in-game "Open user directory" button (<img src="Icons/icons8-open-folder-in-new-tab-50.png" width="20">). The `config.json` can be modified in a text editor and reloaded in-game during runtime to instantly reflect any changes made.

## Functionality
Provide a **Prompt** and **Undesired Content** in the appropriate text fields, **either or both** of the fields should contain the text: **`%TAG%`**. Given a list of tags (split by new lines) in the **Tag List** field, iterate over each tag to replace the string `%TAG%` with the tag to provide a series of images with variations according to the tag replacement text.

*Example:*<br >
**Prompt:**<br >
1girl, {artist:%TAG%}<br >
**Tag List:**<br >
Aaron Douglas<br >
Adam Martinakis<br >
Adolph Gottlieb

Result: 3 images will be generated with the following prompts: `1girl, {artist:Aaron Douglas}`, `1girl, {artist:Adam Martinakis}`, and `1girl, {artist:Adolph Gottlieb}`.

Note that the **Tag List** does not need to strictly adhere to a single tag per line and can consist of any length of text.

### Remove Mode
Generate 1 image for each tag, using the same seed for all images.

Use the Delete (<img src="Icons/icons8-delete-48.png" width="20">) button on each image to both remove the image from the image grid and its tag from the Tag List.

Great for quickly generating a batch of images and/or narrowing down a large number of tags.

### Vote Mode
Generate # images, each image using a random tag from the Tag List and a random seed.

Use the vote buttons (++, +, -, --) on each image to compile an unbiased rating (0-10) for each tag.

After voting on enough images, a subjective rating can be viewed using the View Votes button to identify a preferred or disliked tag.

### Sequence Mode
Given 2-10 tags, each row of images use the same seed to create an ordered sequence per row.

Creates image sequences with a unique seed per row for multiple sequences in a single execution.

## In-Game UI
|Name|Icon|Functionality|
|-|-|-|
|Resize Slider||The knob at the top of the screen horizontally resizes the image grid|
|Mode
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
