#nullable disable
using MelonLoader;
using HarmonyLib;
using UnityEngine;
using System;
using Il2Cpp;

[assembly: MelonInfo(typeof(ModTemplate.HuntBroadcastMain), "HuntBroadcast", "1.0.0", "hzb1130")]
[assembly: MelonGame("Hinterland", "TheLongDark")]

namespace ModTemplate
{
    public static class IconManager
    {
        public static string[] IconList = new string[]
        {
            "",                // 0  No Icon
            "ico_injury_bloodLoss",    // 1  Blood Loss
            "ico_injury_foodPoisoning",// 2  Food Poisoning
            "ico_injury_eventEntity2", // 3  Fear
        };
    }

    public class HuntBroadcastMain : MelonMod
    {
        public override void OnInitializeMelon()
        {
            Settings.OnLoad();
        }
    }

// ==========================================
// Damage Broadcast
// ==========================================
[HarmonyPatch(typeof(BaseAi), nameof(BaseAi.ApplyDamage), new Type[] { typeof(float), typeof(float), typeof(DamageSource), typeof(string) })]
internal static class BaseAi_ApplyDamage_Patch
{
    private static void Postfix(BaseAi __instance, float damage, float bleedOutMintues, DamageSource damageSource)
    {
        if (!Settings.options.ShowDamage)
            return;

        if (damageSource != DamageSource.Player && damageSource != DamageSource.NoiseMaker)
            return;
        
        if (__instance == null || damage < 0)
            return;

        if (!__instance.m_NavMeshAgent?.enabled == true)
            return;
        

        float currentHealth = __instance.m_CurrentHP;
        
        if (!Settings.options.ShowLastShot && currentHealth <= 0)
                return;

        float maxHealth = __instance.m_MaxHP;
        string damageText = "";

        if (Settings.options.ShowCurrentDamage)
        {
            damageText += $"-{Mathf.Round(damage)}";
        }

        bool showCur = Settings.options.ShowCurrentHealth;
        bool showMax = Settings.options.ShowMaxHealth;
        if (showCur || showMax)
        {
            string hpText = "";
            if (showCur) hpText += Mathf.Round(currentHealth).ToString();
            if (showCur && showMax) hpText += "/";
            if (showMax) hpText += Mathf.Round(maxHealth).ToString();
            damageText += $" ({hpText})";
        }

        if (Settings.options.ShowBleedTime && bleedOutMintues > 0 && currentHealth > 0)
        {
            damageText += $" Bleed {bleedOutMintues:F0}min";
        }

        Color color = Settings.options.GetColor(Settings.options.DamageTextColor);
        float duration = Settings.options.DisplayDuration;
        float fade = Settings.options.FadeDuration;
        int iconIdx = Settings.options.DamageIconIndex;

        PlayerDamageEvent.SpawnDamageEvent(
            damageEventName: damageText,
            damageEventType: "",
            iconName: IconManager.IconList[iconIdx],
            tint: color,
            fadeout: true,
            duration,
            fade
        );
    }
}

    // ==========================================
    // Kill Broadcast
    // ==========================================
    [HarmonyPatch(typeof(BaseAi), nameof(BaseAi.EnterDead))]
    internal static class BaseAi_EnterDead_Patch
    {
        private static void Prefix(BaseAi __instance)
        {
            if (!Settings.options.ShowKill) return;
            if (__instance == null || __instance.m_CurrentMode == AiMode.Dead) return;

            DamageSource killerSource = __instance.m_DamageSource;
            if (killerSource == DamageSource.Unspecified) return;

            bool isValidSource = 
                killerSource == DamageSource.Player || 
                killerSource == DamageSource.NoiseMaker || 
                killerSource == DamageSource.Wolf;

            if (!isValidSource) return;

            if (killerSource is DamageSource.Player or DamageSource.NoiseMaker && !Settings.options.ShowPlayerKill)
                return;

            if (killerSource == DamageSource.Wolf && !Settings.options.ShowAnimalKill)
                return;

            string killerName = killerSource switch
            {
                DamageSource.Player or DamageSource.NoiseMaker => "Player",
                DamageSource.Wolf => "Wolf",
                _ => killerSource.ToString()
            };

            string name = __instance.gameObject?.name ?? "Unknown";
            string targetName = name switch
            {
                var n when n.Contains("Wolf") => "Wolf",
                var n when n.Contains("Bear") => "Bear",
                var n when n.Contains("Moose") => "Moose",
                var n when n.Contains("Doe") => "Doe",
                var n when n.Contains("Stag") => "Stag",
                var n when n.Contains("Rabbit") => "Rabbit",
                var n when n.Contains("Cougar") => "Cougar",
                var n when n.Contains("Ptarmigan") => "Ptarmigan",
                _ => name.Replace("(Clone)", "").Trim()
            };

            string killText = $"{killerName} Killed {targetName}";
            Color color = Settings.options.GetColor(Settings.options.KillTextColor);
            float duration = Settings.options.KillDisplayDuration;
            float fade = Settings.options.KillFadeDuration;
            int iconIdx = Settings.options.KillIconIndex;

            PlayerDamageEvent.SpawnDamageEvent(
                damageEventName: killText,
                damageEventType: "",
                iconName: IconManager.IconList[iconIdx],
                tint: color,
                fadeout: true,
                duration,
                fade
            );
        }
    }

    // ==========================================
    // Trap Kill
    // ==========================================
    [HarmonyPatch(typeof(SnareItem), nameof(SnareItem.SpawnDeadRabbitOnSnare))]
    public static class SnareItem_SpawnDeadRabbitOnSnare_Patch
    {
        static void Postfix(SnareItem __instance)
        {
            if (!Settings.options.ShowKill) return;
            if (!Settings.options.ShowTrapKill) return;

            string killText = "Snare Killed Rabbit";
            Color color = Settings.options.GetColor(Settings.options.KillTextColor);
            float duration = Settings.options.KillDisplayDuration;
            float fade = Settings.options.KillFadeDuration;
            int iconIdx = Settings.options.KillIconIndex;

            PlayerDamageEvent.SpawnDamageEvent(
                damageEventName: killText,
                damageEventType: "",
                iconName: IconManager.IconList[iconIdx],
                tint: color,
                fadeout: true,
                duration,
                fade
            );
        }
    }
}