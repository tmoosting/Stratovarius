# Stratovarius
## A Music-Weather Mapping Tool
Synesthesia: the transfiguration of senses. Stratovarius converts sound into weather: give it any song in .mp3 or .wav format and it will map pitch and volume data into cloud patterns and weather effects: rain, snow and lightning. 

### Demo (.mkv)
[Link](https://drive.google.com/file/d/1EY8I3Knvk_vnacbVdpzFNYOCKjZdGvwd/view?usp=sharing)

### WebGL
[Link](http://furion.net/stratovarius)

### Instructions
* Find the attached zip file, unzip into a folder.
* Use Unity (any recent version) to open the folder.
* Open the Main Scene, found in Assets/Scenes.
* Find the Assets/Audio folder and drag in any song in (.mp3 and .wav). 
* In the Hierarchy, on the left, click on the Equalizer GameObject. Then in the Inspector, on the right, find the Audio Source - Audio Clip field. Drag the desired song from Assets/Audio into this field. It will then play when the program is run.
* Primary tweaking can be done through WeatherController GameObject, found in the Hierarchy. Select a preset to match your song: Heavy Bass, Pop, or Classical. 
* Click the Run button to start. If you disable the Maximize on Play toggle, a qualizer-debug-line is drawn in the scene. 

### Code Overview
Work is done in the Update() functions of the Equalizer and WeatherController scripts. Equalizer assembles the spectrum data every frame, WeatherController controls the primary weather effects that use it. 

### Tweaking the Settings
* The four ‘zones’, as described above, are defined under the header Ranges, each of which can be directly visualized by entering the range number (1,2,3,4) in the Debug Range field (0 means disabled).
* The three presets’ relevant values are found under Threshold 1, 2, and 3. The minimum requirement for each weather phenomenon can be tweaked here.
* Cloud and Lightning are separate headers each with several specific behavioral tweaks. Feel free to mess with these, especially during runtime.  



</sub>In collaboration with [Freek van Heerikhuize](https://github.com/Free-k)</sub>
