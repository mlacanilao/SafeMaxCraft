using BepInEx;
using HarmonyLib;

namespace SafeMaxCraft
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.safemaxcraft";
        internal const string Name = "Safe Max Craft";
        internal const string Version = "1.0.0.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class SafeMaxCraft : BaseUnityPlugin
    {
        internal static SafeMaxCraft Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            var harmony = new Harmony(id: ModInfo.Guid);
            harmony.PatchAll();
        }

        public static void Log(object payload)
        {
            Instance.Logger.LogInfo(data: payload);
        }
    }

    [HarmonyPatch(typeof(Recipe), nameof(Recipe.GetMaxCount))]
    internal static class RecipePatch
    {
        [HarmonyPrefix]
        public static bool GetMaxCountPrefix(Recipe __instance, ref int __result)
        {
            int num = int.MaxValue;

            foreach (var ingredient in __instance.ingredients)
            {
                Thing thing = ingredient.thing;
                int count = 0;

                if (!ingredient.optional || thing != null)
                {
                    if (thing != null && !thing.isDestroyed)
                    {
                        count = thing.Num / ingredient.req;
                    }

                    if (count < num)
                    {
                        num = count;
                    }
                }
            }

            int spCost = __instance.source.GetSPCost(EMono.screen.tileSelector.summary.factory);
            if (spCost > 0)
            {
                while (num > 0 && spCost * num > EMono.pc.stamina.value)
                {
                    num--;
                }
            }

            __result = num;
            return false;
        }
    }
}
