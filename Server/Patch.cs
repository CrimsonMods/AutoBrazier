using HarmonyLib;
using ProjectM;
using System;

namespace AutoBrazier.Server
{
    [HarmonyPatch]
    internal class Patch
    {
        private static DayNightCycleTracker _dayNightCycleTracker;
        private const string InitialErrorMessage =
        "System.InvalidOperationException: GetSingleton<ProjectM.DayNightCycle>() requires that exactly one ProjectM.DayNightCycle exist that match this query, but there are 0.";
        private static byte _currentDay = 0;
        private static bool _isDnInitialized = false;

        public static void Load()
        {
            _dayNightCycleTracker = new DayNightCycleTracker();
            _dayNightCycleTracker.OnTimeOfDayChanged += AutoToggle.OnTimeOfDayChanged;
        }

        [HarmonyPatch(typeof(BonfireSystemUpdateCloud), nameof(BonfireSystemUpdateCloud.OnUpdate))]
        [HarmonyPostfix]
        private static void OnUpdate(BonfireSystemUpdateCloud __instance)
        {
            
            if (!Plugin.AutoToggleEnabled.Value || !Core.HasInitialized)
            {
                Plugin.Log($"Returning because either Plugin.AutoToggleEnabled is False ({Plugin.AutoToggleEnabled.Value}) or Core.HasInitialized is False {Core.HasInitialized})");
                return;
            } 

            if (Core.ServerGameManager.HasDayNightCycle)
            {
                try
                {
                    var dayNightCycle = Core.ServerGameManager.DayNightCycle;
                  
                    if (!_isDnInitialized)
                    {
                        _currentDay = dayNightCycle.GameDateTimeNow.Day;
                        _isDnInitialized = true;
                        _dayNightCycleTracker.Update(dayNightCycle);
                    }
                    else
                    {
                        _currentDay = dayNightCycle.GameDateTimeNow.Day;
                        _dayNightCycleTracker.Update(dayNightCycle);
                    }
                    
                }
                catch (Exception e)
                {
                    if (e.Message.StartsWith(InitialErrorMessage))
                    {
                        Plugin.Log($"DNC singleton not yet ready. This error should only appear on the initial creation/startup of the server.");
                    }
                    else
                    {
                        Plugin.Log( $"DayNightCycle: {e.Message}");
                    }
                }
            }
            else
            {
                Plugin.Log($"Core.ServerGameManager.HasDayNightCycle = False", Plugin.LogSystem.Core, BepInEx.Logging.LogLevel.Warning);
            }
        }
    }
}
