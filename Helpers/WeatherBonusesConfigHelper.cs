using System;
using BepInEx.Configuration;
using LethalConfig;
using LethalConfig.ConfigItems;

namespace WeatherBonuses.Helpers;

public static class WeatherBonusesConfigHelper
{
    public static ConfigEntry<string> WeatherValues;
    public static ConfigEntry<OperationModes> OperationMode;

    public static void SetLethalConfig(ConfigFile config)
    {
        OperationMode = config.Bind("Weather Bonuses","OperationMode",OperationModes.BetterScrap,"Better scrap will adjust the value of scrap to be higher depending on the weather, more scrap will increase the amount of scrap that can spawn based on the weather");
        var OperationModeDropdown = new EnumDropDownConfigItem<OperationModes>(OperationMode,true);

        WeatherValues = config.Bind<string>("Weather Bonuses","WeatherValues","Foggy:1.05,Flooded:1.15,Rainy:1.1,Stormy:1.15,Eclipsed:1.25","Multipliers for weather, increases scrap value for moons with these weather effects");
        var WeatherValuesField = new TextInputFieldConfigItem(WeatherValues,true);

        LethalConfigManager.AddConfigItem(OperationModeDropdown);
        LethalConfigManager.AddConfigItem(WeatherValuesField);
        LethalConfigManager.SetModDescription("A mod for boosting scrap values on planets with specific weather");
    }

    public enum OperationModes
    {
        BetterScrap,
        MoreScrap
    }
}