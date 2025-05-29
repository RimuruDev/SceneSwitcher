<p align="center"><h1>SceneSwitcher</h1></p>
<p align="center">
  <a>
    <img alt="Made With Unity" src="https://img.shields.io/badge/made%20with-Unity-57b9d3.svg?logo=Unity">
  </a>
  <a>
    <img alt="License" src="https://img.shields.io/github/license/RimuruDev/SceneSwitcher?logo=github">
  </a>
  <a>
    <img alt="Last Commit" src="https://img.shields.io/github/last-commit/destructive-crab/SceneSwitcher?logo=Mapbox&color=orange">
  </a>
  <a>
    <img alt="Last Release" src="https://img.shields.io/github/v/release/destructive-crab/SceneSwitcher?include_prereleases&logo=Dropbox&color=yellow">
  </a>

## Description

**SceneSwitcher** – is a handy Unity editor tool that helps you move between scenes.

![alt text](https://github.com/destructive-crab/SceneSwitcher/blob/main/Screenshots/SwitcherShowcase.png)

## Features

- **Fast switching between scenes**: You can switch to any scene just from tool's window.
- **Custom Play Mode Start Scene**: you can change from which scene you want to enter into Play Mode. Or disable this feature.
- **Return Button**: Scene Switcher remembers the previous scene and can return to it. Or you can disable this feature.
- **Different scene pools**: Scene Switcher provides three types of scene pools: from **build settings**, **all** from project or **custom list**.
- **Highly customizable**: every part of scene switcher window can be customized.
- **Safe**: scenes are saved on switching, scene switcher stuff is disabled on Play Mode.

## Installation
### With Unity Package 
1. Download latest release from github page.
2. Drag and drop unitypackage file in your project.
3. Check if SceneSwitcher source was imported correctly: in Editor folder.

## Usage

After **SceneSwitcher** installation tool appears in Unity menu:  
**Tools** → **Scene Switcher**.

Open **Scene Switcher** window. In Assets/Editor/SceneSwitcher settings asset will be created.
Then you can configure Scene Switcher as you like. Just open settings by pressing Open Settings button in Scene Switcher menu.

![alt text](https://github.com/destructive-crab/SceneSwitcher/blob/main/Screenshots/HowToOpenSettings.png)

In Behaviour menu you can edit:

![alt text](https://github.com/destructive-crab/SceneSwitcher/blob/main/Screenshots/BehaviourSettingsShowcase.png)

1. Which Scenes Collect - there are three options
  - Only From Build - it means that SceneSwitcher will show you only that scenes that you have in Build Settings.
  - Custom List - with this option you can configure your scenes list as you wish. Just add your Scene asset in list. 
  - All - Scene Switcher will show you all scenes that you have in your project.
2. Show Return To Previous Button - you can enable and disable Return button in Scene Switcher. By pressing it Scene Switcher will switch you to previously opened scene.
3. Custom Play Mode Start Scene Build Index - type here index of Play Mode Start Scene.
4. Auto Enable Custom Play Mode Start Scene - when enabled, it will automatically change play mode start scene when you open project. If not, you will need to switch it on manually.
5. Save Scene On Switch - not recommended to disable. If enabled, scene will be automaticly saved when switch.

In Button Style menu you can edit:

![alt text](https://github.com/destructive-crab/SceneSwitcher/blob/main/Screenshots/StyleSettingsShowcase.png)

1. Heights and button spacings 
2. Formatting of current scene butoon - type there what you want to see on current scene button: content of this field will be displayed on button, SCENE_NAME will be replaced with name of scene.
3. Formatting of Play Mode Start Scene - type there what you want to see on start scene button: content of this field will be displayed on button, SCENE_NAME will be replaced with name of scene. 
