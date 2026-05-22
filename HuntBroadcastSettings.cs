#nullable disable
using ModSettings;
using System.Reflection;
using UnityEngine;

namespace ModTemplate
{
    // 颜色枚举
    public enum BroadcastColor
    {
        Red, Green, Blue, Yellow, Orange, White, Black
    }

    internal class HuntBroadcastSettings : JsonModSettings
    {
       [Section("Damage Broadcast Settings")]

        // ==========================================
        // Damage Broadcast
        // ==========================================
        [Name("Show Damage")]
        [Description("Whether to show damage notifications")]
        public bool ShowDamage = true;

        [Name("Damage Icon Index")]
        [Slider(0, 3)]
        [Description("Icons:0 = No Icon,1 = Blood Loss,2 = Food Poisoning,3 = Fear")]
        public int DamageIconIndex = 0;

        [Name("Damage Text Color")]
        [Description("Set the display color of damage text")]
        public BroadcastColor DamageTextColor = BroadcastColor.Red;

        [Name("Display Duration")]
        [Description("Time the damage text stays on screen (seconds)")]
        [Slider(1, 5, 5)]
        public float DisplayDuration = 5f;

        [Name("Fade Duration")]
        [Description("Fade-out time for damage text disappearance (seconds)")]
        [Slider(0f, 1f, 11)]
        public float FadeDuration = 0.6f;

        [Name("Show Current Damage")]
        [Description("Whether to show damage value from this attack")]
        public bool ShowCurrentDamage = true;

        [Name("Show Current Health")]
        [Description("Whether to show remaining current health of the target")]
        public bool ShowCurrentHealth = true;

        [Name("Show Max Health")]
        [Description("Whether to show max health of the target")]
        public bool ShowMaxHealth = true;

        [Name("Show Bleed Time")]
        [Description("Whether to show bleed countdown timer")]
        public bool ShowBleedTime = true;

        [Name("Show Last Shot")]
        [Description("If disabled, damage will not be shown on kill")]
        public bool ShowLastShot = true;


        // ==========================================
        // Kill Broadcast
        // ==========================================
        [Section("Kill Broadcast Settings")]
        [Name("Show Kill")]
        [Description("Whether to show kill notifications")]
        public bool ShowKill = true;

        [Name("Show Player Kills")]
        [Description("Whether to show kills by player")]
        public bool ShowPlayerKill = true;

        [Name("Show Animal Kills")]
        [Description("Whether to show kills by animals (Wolf)")]
        public bool ShowAnimalKill = true;

        [Name("Show Trap Kills")]
        [Description("Whether to show kills by traps")]
        public bool ShowTrapKill = true;

        [Name("Damage Icon Index")]
        [Slider(0, 3)]
        [Description("Icons:0 = No Icon,1 = Blood Loss,2 = Food Poisoning,3 = Fear")]
        public int KillIconIndex = 3;

        [Name("Kill Text Color")]
        [Description("Set the display color of kill text")]
        public BroadcastColor KillTextColor = BroadcastColor.Yellow;

        [Name("Kill Display Duration")]
        [Description("Time the kill text stays on screen (seconds)")]
        [Slider(1, 5, 5)]
        public float KillDisplayDuration = 5f;

        [Name("Kill Fade Duration")]
        [Description("Fade-out time for kill text disappearance (seconds)")]
        [Slider(0f, 1f, 11)]
        public float KillFadeDuration = 0.6f;

        // ==========================================
        // 颜色转换
        // ==========================================
        public Color GetColor(BroadcastColor color)
        {
            return color switch
            {
                BroadcastColor.Red => new Color(1f, 0f, 0f),
                BroadcastColor.Green => new Color(0f, 1f, 0f),
                BroadcastColor.Blue => new Color(0f, 0.6f, 1f),
                BroadcastColor.Yellow => new Color(1f, 1f, 0f),
                BroadcastColor.Orange => new Color(1f, 0.6f, 0f),
                BroadcastColor.White => Color.white,
                BroadcastColor.Black => Color.black,
                _ => Color.white
            };
        }

        // ==========================================
        // 折叠逻辑
        // ==========================================
        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            base.OnChange(field, oldValue, newValue);
            RefreshAll();
        }

        public void RefreshAll()
        {
            // 伤害折叠
            SetFieldVisible(nameof(DamageIconIndex), ShowDamage);
            SetFieldVisible(nameof(DamageTextColor), ShowDamage);
            SetFieldVisible(nameof(DisplayDuration), ShowDamage);
            SetFieldVisible(nameof(FadeDuration), ShowDamage);
            SetFieldVisible(nameof(ShowCurrentDamage), ShowDamage);
            SetFieldVisible(nameof(ShowCurrentHealth), ShowDamage);
            SetFieldVisible(nameof(ShowMaxHealth), ShowDamage);
            SetFieldVisible(nameof(ShowBleedTime), ShowDamage);

            // 击杀折叠
            SetFieldVisible(nameof(ShowPlayerKill), ShowKill);
            SetFieldVisible(nameof(ShowAnimalKill), ShowKill);
            SetFieldVisible(nameof(ShowTrapKill), ShowKill);
            SetFieldVisible(nameof(KillIconIndex), ShowKill);
            SetFieldVisible(nameof(KillTextColor), ShowKill);
            SetFieldVisible(nameof(KillDisplayDuration), ShowKill);
            SetFieldVisible(nameof(KillFadeDuration), ShowKill);
        }
    }

    // ==========================================
    // 设置加载
    // ==========================================
    internal static class Settings
    {
        public static HuntBroadcastSettings options;

        public static void OnLoad()
        {
            options = new HuntBroadcastSettings();
            // options.AddToModSettings("猎杀伤害显示");
            options.AddToModSettings("HuntBroadcast");
            options.RefreshAll();
        }
    }
}