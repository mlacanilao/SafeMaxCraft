using BepInEx;
using HarmonyLib;

namespace SafeMaxCraft
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.safemaxcraft";
        internal const string Name = "Safe Max Craft";
        internal const string Version = "1.1.0.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class SafeMaxCraft : BaseUnityPlugin
    {
        internal static SafeMaxCraft Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            SafeMaxCraftConfig.LoadConfig(config: Config);
            var harmony = new Harmony(id: ModInfo.Guid);
            harmony.PatchAll();
        }
        
        private void Update()
        {
            if (SafeMaxCraftConfig.ToggleSafeMaxCraftKey.Value.IsDown())
            {
                if (EClass.core?.IsGameStarted == false)
                {
                    return;
                }

                SafeMaxCraftConfig.EnableSafeMaxCraft.Value = !SafeMaxCraftConfig.EnableSafeMaxCraft.Value;

                string status = SafeMaxCraftConfig.EnableSafeMaxCraft.Value
                    ? __(ja: "有効", en: "enabled", cn: "启用")
                    : __(ja: "無効", en: "disabled", cn: "禁用");

                ELayer.pc.TalkRaw(
                    text: $"Safe Max Craft {status}.",
                    ref1: null, ref2: null, forceSync: false);
            }
        }
        
        private static string __(string ja = "", string en = "", string cn = "")
        {
            if (Lang.langCode == "JP")
            {
                return ja ?? en;
            }

            if (Lang.langCode == "CN")
            {
                return cn ?? en;
            }

            return en;
        }

        public static void Log(object payload)
        {
            Instance.Logger.LogInfo(data: payload);
        }
    }

    [HarmonyPatch(declaringType: typeof(Recipe), methodName: nameof(Recipe.GetMaxCount))]
    internal static class RecipePatch
    {
        [HarmonyPrefix]
        public static bool GetMaxCountPrefix(Recipe __instance, ref int __result)
        {
            bool enableSafeMaxCraft = SafeMaxCraftConfig.EnableSafeMaxCraft?.Value ?? true;

            if (enableSafeMaxCraft == false)
            {
                return true;
            }
            
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

            int spCost = __instance.source.GetSPCost(factory: EMono.screen.tileSelector.summary.factory);
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
