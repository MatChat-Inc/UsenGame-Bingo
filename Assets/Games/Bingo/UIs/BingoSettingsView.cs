// Created by LunarEclipse on 2024-7-18 9:26.

using System;
using Luna;
using Luna.UI;
using Luna.UI.Navigation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using USEN.Games.Common;
using ToggleGroup = Modules.UI.ToggleGroup;

namespace USEN.Games.Bingo
{
    public class BingoSettingsView: Widget
    {
        public ToggleGroup basicDisplaySettingsToggles;
        public Slider basicDisplayShowSettingsSlider;
        public ToggleGroup commendationVideoSettingsToggles;
        public Slider commendationVideoSettingsSlider;
        public Slider bgmVolumeSlider;
        public Slider sfxVolumeSlider;
        public Text bgmVolumeText;
        public Text sfxVolumeText;
        public Button highLowTimeButton;
        public Text highLowTimeCountText;
        
        public Button appInfoButton;
        public BottomPanel bottomPanel;
        
        const int MAX_COUNT = 75;
        const int MIN_COUNT = 8;

        private void Start()
        {
            // Current display mode
            var selectedIndex = (int) BingoPreferences.DisplayMode;
            basicDisplayShowSettingsSlider.maxValue = basicDisplaySettingsToggles.Toggles.Count - 1;
            basicDisplayShowSettingsSlider.value = selectedIndex;
            basicDisplayShowSettingsSlider.onValueChanged.AddListener(OnBasicDisplayShowSettingsSliderValueChanged);
            
            basicDisplaySettingsToggles.ToggleOn(selectedIndex);
            basicDisplaySettingsToggles.Bind(basicDisplayShowSettingsSlider);
            
            // Commendation video settings
            commendationVideoSettingsSlider.onValueChanged.AddListener(OnCommendationVideoSettingsSliderValueChanged);
            commendationVideoSettingsSlider.value = BingoPreferences.CommendationVideoOption;
            commendationVideoSettingsToggles.ToggleOn(BingoPreferences.CommendationVideoOption);
            commendationVideoSettingsToggles.Bind(commendationVideoSettingsSlider);
            
            // Audio volume
            bgmVolumeText.text = $"{AppConfig.Instance.BGMVolume * 10:0}";
            bgmVolumeSlider.value = AppConfig.Instance.BGMVolume;
            bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
            
            sfxVolumeText.text = $"{AppConfig.Instance.EffectVolume * 10:0}";
            sfxVolumeSlider.value = AppConfig.Instance.EffectVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            
            highLowTimeCountText.text = AppConfig.Instance.MaxCellCount.ToString();
            
            // App info
            appInfoButton.onClick.AddListener(OnClickAppInfoButton);
            
            // Bottom panel
            bottomPanel.exitButton.onClick.AddListener(() => Navigator.Pop());
            
            EventSystem.current.SetSelectedGameObject(commendationVideoSettingsSlider.gameObject);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) ||
                Input.GetButtonDown("Cancel")) 
                Navigator.Pop();
            
            if (Input.GetButtonDown("Horizontal") && 
                EventSystem.current.currentSelectedGameObject == highLowTimeButton.gameObject) 
            {
                if (Input.GetKey(KeyCode.LeftArrow)) 
                    ChangeHighLowTimeValue(-1);
                else if (Input.GetKey(KeyCode.RightArrow)) 
                    ChangeHighLowTimeValue(+1);
            }
        }
        
        public void ChangeHighLowTimeValue(int value) 
        {
            AppConfig.Instance.MaxCellCount += value;
            if (value < 0 && AppConfig.Instance.MaxCellCount < MIN_COUNT) 
                AppConfig.Instance.MaxCellCount = MIN_COUNT;
            if (value > 0 && AppConfig.Instance.MaxCellCount > MAX_COUNT) 
                AppConfig.Instance.MaxCellCount = MAX_COUNT;
            
            highLowTimeCountText.text = AppConfig.Instance.MaxCellCount.ToString();
        }
        
        private void OnBasicDisplayShowSettingsSliderValueChanged(float arg0)
        {
            var index = Convert.ToInt32(arg0);
            AppConfig.Instance.ThemeSelectedIdx = index;
        }
        
        private void OnCommendationVideoSettingsSliderValueChanged(float arg0)
        {
            BingoPreferences.CommendationVideoOption = Convert.ToInt32(arg0);
        }
        
        private void OnBgmVolumeChanged(float value)
        {
            BgmManager.SetVolume(value * 0.1f);
            BingoPreferences.BgmVolume = value * 0.1f;
            AppConfig.Instance.BGMVolume = Convert.ToInt32(value);
            AudioManager.Instance.SetBgmVolume((int)value);
            bgmVolumeText.text = $"{value * 10:0}";
        }
        
        private void OnSfxVolumeChanged(float value)
        {
            SFXManager.SetVolume(value * 0.1f);
            BingoPreferences.SfxVolume = value * 0.1f;
            AppConfig.Instance.EffectVolume = Convert.ToInt32(value);
            AudioManager.Instance.SetEffectVolume((int)value);
            sfxVolumeText.text = $"{value * 10:0}";
        }
        
        void OnClickAppInfoButton() 
        {
            Navigator.Push<AppInfoView>();
        }
    }
}