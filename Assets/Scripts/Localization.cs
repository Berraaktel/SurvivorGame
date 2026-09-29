using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
    public static string CurrentLanguage
    {
        get
        {
            return Application.systemLanguage == SystemLanguage.Turkish ? "tr" : "en";
        }
    }

    private static readonly Dictionary<string, Dictionary<string, string>> Strings = new Dictionary<string, Dictionary<string, string>>
    {
        { "game_over", new Dictionary<string, string> { { "tr", "OYUN BITTI" }, { "en", "GAME OVER" } } },
        { "main_menu_title", new Dictionary<string, string> { { "tr", "SURVIVORGAME" }, { "en", "SURVIVORGAME" } } },
        { "play_button", new Dictionary<string, string> { { "tr", "OYNA" }, { "en", "PLAY" } } },
        { "quit_button", new Dictionary<string, string> { { "tr", "CIKIS" }, { "en", "QUIT" } } },
        { "restart", new Dictionary<string, string> { { "tr", "Yeniden Basla" }, { "en", "Restart" } } },
        { "level_up", new Dictionary<string, string> { { "tr", "SEVIYE ATLADIN!" }, { "en", "LEVEL UP!" } } },

        { "upgrade_speed_title", new Dictionary<string, string> { { "tr", "Hareket Hizi" }, { "en", "Move Speed" } } },
        { "upgrade_speed_desc", new Dictionary<string, string> { { "tr", "Daha hizli hareket et" }, { "en", "Move faster" } } },

        { "upgrade_health_title", new Dictionary<string, string> { { "tr", "Azami Can" }, { "en", "Max Health" } } },
        { "upgrade_health_desc", new Dictionary<string, string> { { "tr", "+2 can, tam iyilesme" }, { "en", "+2 max health, full heal" } } },

        { "upgrade_damage_title", new Dictionary<string, string> { { "tr", "Saldiri Hasari" }, { "en", "Attack Damage" } } },
        { "upgrade_damage_desc", new Dictionary<string, string> { { "tr", "+1 saldiri hasari" }, { "en", "+1 attack damage" } } },

        { "upgrade_atkspeed_title", new Dictionary<string, string> { { "tr", "Saldiri Hizi" }, { "en", "Attack Speed" } } },
        { "upgrade_atkspeed_desc", new Dictionary<string, string> { { "tr", "Saldirilar daha sik olur" }, { "en", "Attacks more often" } } },

        { "upgrade_range_title", new Dictionary<string, string> { { "tr", "Saldiri Menzili" }, { "en", "Attack Range" } } },
        { "upgrade_range_desc", new Dictionary<string, string> { { "tr", "Menzil biraz artar" }, { "en", "Slightly longer range" } } },
    };

    public static string Get(string key)
    {
        Dictionary<string, string> langs;
        if (Strings.TryGetValue(key, out langs))
        {
            string value;
            if (langs.TryGetValue(CurrentLanguage, out value)) return value;
            if (langs.TryGetValue("en", out value)) return value;
        }
        return key;
    }
}
