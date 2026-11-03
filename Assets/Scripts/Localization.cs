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
        { "victory_title", new Dictionary<string, string> { { "tr", "ZAFER!" }, { "en", "VICTORY!" } } },

        { "evo_knife", new Dictionary<string, string> { { "tr", "BICAK -> BIN BICAK!" }, { "en", "KNIFE -> THOUSAND KNIVES!" } } },
        { "evo_orbit", new Dictionary<string, string> { { "tr", "DONEN BICAKLAR -> OLUM CEMBERI!" }, { "en", "SPINNING BLADES -> DEATH RING!" } } },
        { "evo_rockbomb", new Dictionary<string, string> { { "tr", "KAYA BOMBASI -> METEOR YAGMURU!" }, { "en", "ROCK BOMB -> METEOR SHOWER!" } } },
        { "evo_dustnova", new Dictionary<string, string> { { "tr", "TOZ PATLAMASI -> KUM FIRTINASI!" }, { "en", "DUST NOVA -> SANDSTORM!" } } },
        { "evo_lightning", new Dictionary<string, string> { { "tr", "YILDIRIM ZINCIRI -> SIMSEK FIRTINASI!" }, { "en", "LIGHTNING CHAIN -> THUNDERSTORM!" } } },

        { "upgrade_knife_lvl_title", new Dictionary<string, string> { { "tr", "Bicagi Guclendir" }, { "en", "Strengthen Knife" } } },
        { "upgrade_knife_lvl_desc", new Dictionary<string, string> { { "tr", "Bicak daha sik ve guclu atilir. Max seviyede Zirh ile Bin Bicak'a evrimlesir." }, { "en", "The knife throws faster and hits harder. At max level, evolves into Thousand Knives with Armor." } } },
        { "upgrade_orbit_lvl_title", new Dictionary<string, string> { { "tr", "Donen Bicaklari Guclendir" }, { "en", "Strengthen Spinning Blades" } } },
        { "upgrade_orbit_lvl_desc", new Dictionary<string, string> { { "tr", "Bicaklar daha hizli doner, daha cok hasar verir. Max seviyede Miknatis ile Olum Cemberi'ne evrimlesir." }, { "en", "The blades spin faster and hit harder. At max level, evolves into Death Ring with Magnet." } } },
        { "upgrade_rockbomb_lvl_title", new Dictionary<string, string> { { "tr", "Kaya Bombasini Guclendir" }, { "en", "Strengthen Rock Bomb" } } },
        { "upgrade_rockbomb_lvl_desc", new Dictionary<string, string> { { "tr", "Bomba daha sik atilir, daha cok hasar verir. Max seviyede Can Yenileme ile Meteor Yagmuru'na evrimlesir." }, { "en", "The bomb throws more often and hits harder. At max level, evolves into Meteor Shower with Health Regen." } } },
        { "upgrade_dustnova_lvl_title", new Dictionary<string, string> { { "tr", "Toz Patlamasini Guclendir" }, { "en", "Strengthen Dust Nova" } } },
        { "upgrade_dustnova_lvl_desc", new Dictionary<string, string> { { "tr", "Patlama daha sik ve genis olur. Max seviyede Zirh ile Kum Firtinasi'na evrimlesir." }, { "en", "The pulse fires more often over a wider area. At max level, evolves into Sandstorm with Armor." } } },
        { "upgrade_lightning_lvl_title", new Dictionary<string, string> { { "tr", "Yildirim Zincirini Guclendir" }, { "en", "Strengthen Lightning Chain" } } },
        { "upgrade_lightning_lvl_desc", new Dictionary<string, string> { { "tr", "Yildirim daha sik carpar, daha cok hasar verir. Max seviyede Miknatis ile Simsek Firtinasi'na evrimlesir." }, { "en", "The lightning strikes more often and hits harder. At max level, evolves into Thunderstorm with Magnet." } } },
        { "main_menu_title", new Dictionary<string, string> { { "tr", "SURVIVORGAME" }, { "en", "SURVIVORGAME" } } },
        { "play_button", new Dictionary<string, string> { { "tr", "OYNA" }, { "en", "PLAY" } } },
        { "quit_button", new Dictionary<string, string> { { "tr", "CIKIS" }, { "en", "QUIT" } } },
        { "paused_title", new Dictionary<string, string> { { "tr", "DURAKLATILDI" }, { "en", "PAUSED" } } },
        { "resume_button", new Dictionary<string, string> { { "tr", "DEVAM ET" }, { "en", "RESUME" } } },
        { "settings_button", new Dictionary<string, string> { { "tr", "AYARLAR" }, { "en", "SETTINGS" } } },
        { "settings_title", new Dictionary<string, string> { { "tr", "AYARLAR" }, { "en", "SETTINGS" } } },
        { "sfx_volume", new Dictionary<string, string> { { "tr", "Efekt Sesi" }, { "en", "SFX Volume" } } },
        { "music_volume", new Dictionary<string, string> { { "tr", "Muzik Sesi" }, { "en", "Music Volume" } } },
        { "back_button", new Dictionary<string, string> { { "tr", "GERI" }, { "en", "BACK" } } },
        { "score_label", new Dictionary<string, string> { { "tr", "SKOR" }, { "en", "SCORE" } } },
        { "record_score_label", new Dictionary<string, string> { { "tr", "REKOR SKOR" }, { "en", "RECORD SCORE" } } },
        { "restart", new Dictionary<string, string> { { "tr", "Yeniden Basla" }, { "en", "Restart" } } },
        { "level_up", new Dictionary<string, string> { { "tr", "SEVIYE ATLADIN!" }, { "en", "LEVEL UP!" } } },
        { "boss_label", new Dictionary<string, string> { { "tr", "BOSS" }, { "en", "BOSS" } } },

        { "gold_earned_label", new Dictionary<string, string> { { "tr", "Kazanilan Altin" }, { "en", "Gold Earned" } } },
        { "total_gold_label", new Dictionary<string, string> { { "tr", "Toplam Altin" }, { "en", "Total Gold" } } },
        { "gold_balance_label", new Dictionary<string, string> { { "tr", "Altin" }, { "en", "Gold" } } },
        { "shop_button", new Dictionary<string, string> { { "tr", "MAGAZA" }, { "en", "SHOP" } } },
        { "shop_title", new Dictionary<string, string> { { "tr", "MAGAZA" }, { "en", "SHOP" } } },
        { "buy_button", new Dictionary<string, string> { { "tr", "SATIN AL" }, { "en", "BUY" } } },
        { "maxed_label", new Dictionary<string, string> { { "tr", "MAKSIMUM" }, { "en", "MAXED" } } },
        { "owned_label", new Dictionary<string, string> { { "tr", "SAHIPSIN" }, { "en", "OWNED" } } },

        { "shop_health_title", new Dictionary<string, string> { { "tr", "Kalici Can" }, { "en", "Permanent Health" } } },
        { "shop_health_desc", new Dictionary<string, string> { { "tr", "Her kosuda +2 baslangic cani" }, { "en", "+2 starting health, every run" } } },
        { "shop_knife_desc_format", new Dictionary<string, string> { { "tr", "Sonraki: {0} (+{1} hasar)" }, { "en", "Next: {0} (+{1} damage)" } } },
        { "shop_knife_maxed_desc", new Dictionary<string, string> { { "tr", "Maksimum seviye" }, { "en", "Maximum level" } } },
        { "shop_skin_title", new Dictionary<string, string> { { "tr", "Altin Kahraman" }, { "en", "Golden Hero" } } },
        { "shop_skin_desc", new Dictionary<string, string> { { "tr", "Karaktere altin renk temasi" }, { "en", "Gold color theme for your character" } } },

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

        { "upgrade_rockbomb_title", new Dictionary<string, string> { { "tr", "Patlayan Kaya" }, { "en", "Exploding Rock" } } },
        { "upgrade_rockbomb_desc", new Dictionary<string, string> { { "tr", "Yeni silah: firlatilan kaya carptigi yerde patlayip alan hasari verir" }, { "en", "New weapon: a thrown rock explodes on impact, dealing area damage" } } },

        { "upgrade_dustnova_title", new Dictionary<string, string> { { "tr", "Toz Firtinasi" }, { "en", "Dust Nova" } } },
        { "upgrade_dustnova_desc", new Dictionary<string, string> { { "tr", "Yeni silah: etrafina periyodik toz dalgasi yayarak yakindaki dusmanlara hasar verir" }, { "en", "New weapon: periodically releases a dust wave, damaging nearby enemies" } } },

        { "upgrade_lightning_title", new Dictionary<string, string> { { "tr", "Yildirim Zinciri" }, { "en", "Chain Lightning" } } },
        { "upgrade_lightning_desc", new Dictionary<string, string> { { "tr", "Yeni silah: en yakin dusmana anlik carpar, yakindaki digerlerine zincirleme sicrar" }, { "en", "New weapon: strikes the nearest enemy instantly and chains to others nearby" } } },

        { "upgrade_regen_title", new Dictionary<string, string> { { "tr", "Can Yenileme" }, { "en", "Health Regen" } } },
        { "upgrade_regen_desc", new Dictionary<string, string> { { "tr", "Yeni pasif: zamanla kendiliginden can yenilenir" }, { "en", "New passive: slowly heals you back over time" } } },

        { "upgrade_magnet_title", new Dictionary<string, string> { { "tr", "Miknatis" }, { "en", "Magnet" } } },
        { "upgrade_magnet_desc", new Dictionary<string, string> { { "tr", "Yeni pasif: XP ve altin toplama menzili artar" }, { "en", "New passive: wider XP and gold pickup range" } } },

        { "upgrade_armor_title", new Dictionary<string, string> { { "tr", "Zirh" }, { "en", "Armor" } } },
        { "upgrade_armor_desc", new Dictionary<string, string> { { "tr", "Yeni pasif: gelen hasar biraz azalir" }, { "en", "New passive: slightly reduces incoming damage" } } },
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
