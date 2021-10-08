using DMT;
using System;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

using static OCB.ElectricityUtils;

public class OcbElectricityOption
{

    // Entry class for Harmony patching
    public class OcbElectricityOverhaul_Init : IHarmony
    {
        public void Start()
        {
            Debug.Log("Loading OCB Electricity Option Patch: " + GetType().ToString());
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(GamePrefs))]
    [HarmonyPatch("initPropertyDecl")]
    public class GamePrefs_initPropertyDecl
    {
        static void Postfix(GamePrefs __instance)
        {
            int size = __instance.propertyList.Length;
            // Only apply once, check for our key
            for (int i = 0; i < size; i++)
            {
                GamePrefs.PropertyDecl pref = __instance.propertyList[i];
                if (pref.name == EnumGamePrefs.MinPowerForCharging) return;
            }

            // Otherwise add three new items
            Array.Resize(ref __instance.propertyList, size + 3);
            __instance.propertyList[size + 0] = new GamePrefs.PropertyDecl(EnumGamePrefs.BatterySelfCharge,
                true, GamePrefs.EnumType.Bool, (bool)false, (object)null, (object)null);
            __instance.propertyList[size + 1] = new GamePrefs.PropertyDecl(EnumGamePrefs.BatteryPowerPerUse,
                true, GamePrefs.EnumType.Int, (int)10, (object)null, (object)null);
            __instance.propertyList[size + 2] = new GamePrefs.PropertyDecl(EnumGamePrefs.MinPowerForCharging,
                true, GamePrefs.EnumType.Int, (int)20, (object)null, (object)null);

            size = __instance.propertyValues.Length;
            Array.Resize(ref __instance.propertyValues, size + 3);
            __instance.propertyValues[size + 0] = (bool) false;
            __instance.propertyValues[size + 1] = (int) 10;
            __instance.propertyValues[size + 2] = (int) 20;
        }
    }

    [HarmonyPatch(typeof(GameModeSurvival))]
    [HarmonyPatch("GetSupportedGamePrefsInfo")]
    public class GameModeSurvival_GetSupportedGamePrefsInfo
    {
        static void Postfix(GameModeSurvival __instance,
            ref GameMode.ModeGamePref[] __result)
        {
            int size = __result.Length;
            // Only apply once, check for our key
            for (int i = 0; i < size; i++)
            {
                GameMode.ModeGamePref pref = __result[i];
                if (pref.GamePref == EnumGamePrefs.MinPowerForCharging) return;
            }
            // Otherwise add three new keys
            Array.Resize(ref __result, size + 3);
            __result[size + 0] = new GameMode.ModeGamePref(EnumGamePrefs.BatterySelfCharge, GamePrefs.EnumType.Bool, (bool) false);
            __result[size + 1] = new GameMode.ModeGamePref(EnumGamePrefs.BatteryPowerPerUse, GamePrefs.EnumType.Int, (int) 10);
            __result[size + 2] = new GameMode.ModeGamePref(EnumGamePrefs.MinPowerForCharging, GamePrefs.EnumType.Int, (int) 20);
        }
    }

    

}
