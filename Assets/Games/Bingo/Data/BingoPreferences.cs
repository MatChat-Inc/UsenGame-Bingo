// Created by LunarEclipse on 2024-9-9 5:33.

using UnityEngine;

namespace USEN.Games.Bingo
{
    public static class BingoPreferences
    {
        public static BingoDisplayMode DisplayMode
        {
            get => (BingoDisplayMode) PlayerPrefs.GetInt("Bingo.DisplayMode", 0);
            set => PlayerPrefs.SetInt("Bingo.DisplayMode", (int) value);
        }
        
        public static int CommendationVideoOption
        {
            get => PlayerPrefs.GetInt("Bingo.CommendationVideoOption", 0);
            set => PlayerPrefs.SetInt("Bingo.CommendationVideoOption", value);
        }
        
        public static float BgmVolume
        {
            get => PlayerPrefs.GetFloat("Bingo.BgmVolume", 1);
            set => PlayerPrefs.SetFloat("Bingo.BgmVolume", value);
        }
        
        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat("Bingo.SfxVolume", 1);
            set => PlayerPrefs.SetFloat("Bingo.SfxVolume", value);
        }
    }
    
    public enum BingoDisplayMode
    {
        Normal,
        Random,
    }
}