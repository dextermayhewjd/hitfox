using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuController : MonoBehaviour
{
    // Graphics.
    [SerializeField] private TMP_Dropdown screenResolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    
    // Sound.
    private Slider masterVolumeSlider;
    private Toggle muteToggle; 

    // private FullScreenMode[] screenModes;
    private FullScreenMode[] screenModes = { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed };
    private Resolution[] resolutions;

    private FullScreenMode currentScreenMode;
    private Resolution currentResolution;

    public bool settingsMenuActive = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateScreenModes();
        UpdateScreenResolutions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateScreenResolutions()
    {
        screenResolutionDropdown.ClearOptions();

        resolutions = Screen.resolutions;

        List<string> dropdownList = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }

            string resolution = resolutions[i].width.ToString() + " x " + resolutions[i].height.ToString();
            dropdownList.Add(resolution);
        }

        screenResolutionDropdown.AddOptions(dropdownList);
        screenResolutionDropdown.value = currentResolutionIndex;
        screenResolutionDropdown.RefreshShownValue();

        currentResolution = resolutions[currentResolutionIndex];
    }

    private void UpdateScreenModes()
    {
        screenModeDropdown.ClearOptions();

        List<string> dropdownList = new List<string>();

        currentScreenMode = Screen.fullScreenMode;

        int currentScreenModeIndex = 0;
        for (int i = 0; i < screenModes.Length; i++)
        {
            if (screenModes[i] == Screen.fullScreenMode)
            {
                currentScreenModeIndex = i;
            }

            string screenMode = "";

            switch(screenModes[i])
            {
                case FullScreenMode.ExclusiveFullScreen:
                    screenMode = "Fullscreen";
                    break;
                case FullScreenMode.FullScreenWindow:
                    screenMode = "Borderless";
                    break;
                case FullScreenMode.MaximizedWindow:
                    screenMode = "Borderless";
                    break;
                case FullScreenMode.Windowed:
                    screenMode = "Windowed";
                    break;
            }

            dropdownList.Add(screenMode);
        }

        screenModeDropdown.AddOptions(dropdownList);
        screenModeDropdown.value = currentScreenModeIndex;
        screenModeDropdown.RefreshShownValue();

        currentScreenMode = screenModes[currentScreenModeIndex];
    }

    public void SetScreenResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, currentScreenMode);

        currentResolution = res;
    }

    public void SetScreenMode(int index)
    {
        FullScreenMode screenMode = screenModes[index];
        Screen.SetResolution(currentResolution.width, currentResolution.height, screenMode);
        currentScreenMode = screenMode;
    }
}
