using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider VolumeSlider;

    public void Start()
    {
        VolumeSlider.value = GameSettings.Volume;
        VolumeSlider.onValueChanged.AddListener(delegate { VolumeSliderValueChanged(); });
    }

    private void VolumeSliderValueChanged() => GameSettings.Volume = this.VolumeSlider.value;

    public void PlayerNumberSelected(int playerNumber) => GameSettings.PlayerNumber = playerNumber;
}
