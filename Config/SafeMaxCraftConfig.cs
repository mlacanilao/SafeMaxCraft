using BepInEx.Configuration;
using UnityEngine;

namespace SafeMaxCraft
{
    internal static class SafeMaxCraftConfig
    {
        internal static ConfigEntry<bool> EnableSafeMaxCraft;
        internal static ConfigEntry<KeyboardShortcut> ToggleSafeMaxCraftKey;

        internal static void LoadConfig(ConfigFile config)
        {
            EnableSafeMaxCraft = config.Bind(
                section: ModInfo.Name,
                key: "Enable Safe Max Craft",
                defaultValue: true,
                description: "Enable or disable the Safe Max Craft mod.\n" +
                             "Set to 'false' to allow crafting regardless of stamina.\n" +
                             "Safe Max Craft MOD の有効/無効を切り替えます。\n" +
                             "'false' に設定すると、スタミナに関係なくクラフトが可能になります。\n" +
                             "启用或禁用 Safe Max Craft 模组。\n" +
                             "设置为 'false' 可在体力不足时仍可进行制作。"
            );
            
            ToggleSafeMaxCraftKey = config.Bind(
                section: ModInfo.Name,
                key: "Toggle Safe Max Craft Key",
                defaultValue: new KeyboardShortcut(mainKey: KeyCode.C, modifiers: KeyCode.RightControl),
                description: "Key to toggle Safe Max Craft on/off during gameplay.\n" +
                             "Press this key to enable or disable the stamina-based crafting limiter.\n" +
                             "ゲーム中にSafe Max Craftをオン/オフ切り替えるキーを設定します。\n" +
                             "このキーを押すと、スタミナベースのクラフト制限を有効または無効にできます。\n" +
                             "游戏过程中用于切换Safe Max Craft开关的按键。\n" +
                             "按下此键可启用或禁用基于体力的制作限制。"
            );
        }
    }
}