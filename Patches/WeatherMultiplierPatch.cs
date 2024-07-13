using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using WeatherBonuses.Helpers;

namespace WeatherBonuses.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class WeatherBonus
{   

    public static Dictionary<string, float> WeatherBonusDict()
    {
        Dictionary<string, float> weather_dict = new Dictionary<string, float>();

        string BonusesString = WeatherBonusesConfigHelper.WeatherValues.Value;
        List<string> entries = BonusesString.Split(",").ToList();

        foreach (string entry in entries)
        {
            if (entry == "") continue;

            string[] item = entry.Split(":");

            var weather_id = item[0];
            var bonus = item[1];

            weather_dict[weather_id] = float.Parse(bonus);
        }

        return weather_dict;
    }


    public static void AddWeatherEntry(string weather)
    {
        var weatherstring = WeatherBonusesConfigHelper.WeatherValues.Value;

        if (weatherstring == "")
        {
            WeatherBonusesConfigHelper.WeatherValues.Value = $"{weather}:1.0";
        } else {
            WeatherBonusesConfigHelper.WeatherValues.Value = $"{weatherstring},{weather}:1.0";
        }
    }

    [HarmonyPatch("SpawnScrapInLevel")]
    [HarmonyPrefix]
    private static void AdjustScrapMultiplier(RoundManager __instance)
    {
        var cur_weather = __instance.currentLevel.currentWeather.ToString();
        var weather_bonus = WeatherBonusDict();

        
        if (weather_bonus.ContainsKey(cur_weather))
        {
            var scrap_multiplier = weather_bonus[cur_weather];

            if (WeatherBonusesConfigHelper.OperationMode.Value == WeatherBonusesConfigHelper.OperationModes.BetterScrap) {
                __instance.scrapValueMultiplier *= scrap_multiplier;
            } else if (WeatherBonusesConfigHelper.OperationMode.Value == WeatherBonusesConfigHelper.OperationModes.MoreScrap) {
                __instance.scrapAmountMultiplier *= scrap_multiplier;
            }
            
        
        
        } else {
            AddWeatherEntry(cur_weather);
            WeatherBonuses.Logger.LogInfo($"{cur_weather} does not have a multiplier, creating new multiplier with default of 1!");
            HUDManager.Instance.DisplayTip("WeatherBonuses",$"{cur_weather} did not have a multiplier, created new multiplier with default of 1!",false,false,"LC_Tip1");
        }

    }

    [HarmonyPatch("SpawnScrapInLevel")]
    [HarmonyPostfix]
    private static void RevertScrapMultiplier(RoundManager __instance)
    {
        var cur_weather = __instance.currentLevel.currentWeather.ToString();
        var weather_bonus = WeatherBonusDict();

        if (weather_bonus.ContainsKey(cur_weather))
        {
            var scrap_multiplier = weather_bonus[cur_weather];

            if (WeatherBonusesConfigHelper.OperationMode.Value == WeatherBonusesConfigHelper.OperationModes.BetterScrap) {
                __instance.scrapValueMultiplier /= scrap_multiplier;
            } else if (WeatherBonusesConfigHelper.OperationMode.Value == WeatherBonusesConfigHelper.OperationModes.MoreScrap) {
                __instance.scrapAmountMultiplier /= scrap_multiplier;
            }
        }
    }
}