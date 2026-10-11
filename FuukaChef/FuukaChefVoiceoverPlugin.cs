using BepInEx;
using FuukaChef.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Security;
using System.Security.Permissions;
using BepInEx.Configuration;
using UnityEngine.AddressableAssets;
using Rewired.UI.ControlMapper;
using System.Reflection;
using System.Runtime.CompilerServices;
using BaseVoiceoverLib;
using FuukaChef.Components;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
namespace FuukaChef
{
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BaseVoiceoverLib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("com.Schale.FuukaChefVoiceover", "FuukaChefVoiceover", "1.0.4")]
    public class FuukaChefVoiceoverPlugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool> enableVoicelines;
        public static ConfigEntry<KeyboardShortcut> btnNyCafe1, btnThanks, btnTitle, btnEx1, btnEx2, btnEx3, btnExLv1, btnExLv2, btnExLv3, btnNYEx1, btnNYEx2, btnNYEx3, btnNYExLv1, btnNYExLv2, btnNYExLv3, btnDoushimashou;
        public static bool playedSeasonalVoiceline = false;
        public static AssetBundle assetBundle;
        public static SurvivorDef survivorDef = Addressables.LoadAssetAsync<SurvivorDef>("RoR2/DLC2/Chef/Chef.asset").WaitForCompletion();

        public void Awake()
        {
            Files.PluginInfo = this.Info;
            RoR2.RoR2Application.onLoad += OnLoad;
            new Content().Initialize();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FuukaChef.fuukachefvoiceoverbundle"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }

            SoundBanks.Init();

            InitNSE();

            enableVoicelines = base.Config.Bind<bool>(new ConfigDefinition("Settings", "Enable Voicelines"), true, new ConfigDescription("Enable voicelines when using the FuukaChef Skin."));
            enableVoicelines.SettingChanged += EnableVoicelines_SettingChanged;

            btnTitle = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Blue Archive"), KeyboardShortcut.Empty);
            btnEx1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 1"), KeyboardShortcut.Empty);
            btnEx2 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 2"), KeyboardShortcut.Empty);
            btnEx3 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 3"), KeyboardShortcut.Empty);
            btnExLv1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv 1"), KeyboardShortcut.Empty);
            btnExLv2 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv 2"), KeyboardShortcut.Empty);
            btnExLv3 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv 3"), KeyboardShortcut.Empty);
            btnNYEx1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 1 (NY)"), KeyboardShortcut.Empty);
            btnNYEx2 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 2 (NY)"), KeyboardShortcut.Empty);
            btnNYEx3 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX 3 (NY)"), KeyboardShortcut.Empty);
            btnNYExLv1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv 1 (NY)"), KeyboardShortcut.Empty);
            btnNYExLv2 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv 2 (NY)"), KeyboardShortcut.Empty);
            btnNYExLv3 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "EX Lv 3 (NY)"), KeyboardShortcut.Empty);
            btnDoushimashou = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Doushimashou"), KeyboardShortcut.Empty);
            btnThanks = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Thanks"), KeyboardShortcut.Empty);
            btnNyCafe1 = base.Config.Bind<KeyboardShortcut>(new ConfigDefinition("Keybinds", "Cafe 1 (NY)"), KeyboardShortcut.Empty);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                RiskOfOptionsCompat();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void RiskOfOptionsCompat()
        {
            RiskOfOptions.ModSettingsManager.SetModIcon(assetBundle.LoadAsset<Sprite>("fuuka"));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(enableVoicelines));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnTitle));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnEx1));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnEx2));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnEx3));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnExLv1));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnExLv2));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnExLv3));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnNYEx1));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnNYEx2));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnNYEx3));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnNYExLv1));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnNYExLv2));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnNYExLv3));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnDoushimashou));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnThanks));
            RiskOfOptions.ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(btnNyCafe1));
        }

        private void FuukaSelect(GameObject mannequinObject)
        {
            if (!enableVoicelines.Value) return;

            bool played = false;
            if (!playedSeasonalVoiceline)
            {
                if ((System.DateTime.Today.Month == 1 && System.DateTime.Today.Day == 1) || (System.DateTime.Today.Month == 12 && System.DateTime.Today.Day == 31))
                {
                    Util.PlaySound("Play_FuukaChef_Lobby_Newyear", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 4 && System.DateTime.Today.Day == 30)
                {
                    Util.PlaySound("Play_FuukaChef_Lobby_bday", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 10 && System.DateTime.Today.Day == 31)
                {
                    Util.PlaySound("Play_FuukaChef_Lobby_Halloween", mannequinObject);
                    played = true;
                }
                else if (System.DateTime.Today.Month == 12 && (System.DateTime.Today.Day == 25 || System.DateTime.Today.Day == 24))
                {
                    Util.PlaySound("Play_FuukaChef_Lobby_xmas", mannequinObject);
                    played = true;
                }

                if (played)
                {
                    playedSeasonalVoiceline = true;
                }
            }
            if (!played)
            {
                if (Util.CheckRoll(5f))
                {
                    Util.PlaySound("Play_FuukaChef_Title", mannequinObject);
                }
                else
                {
                    Util.PlaySound("Play_FuukaChef_Lobby", mannequinObject);
                }
            }
        }

        private void OnLoad()
        {
            SkinDef targetSkin = null;
            SkinDef[] skins = SkinCatalog.FindSkinsForBody(BodyCatalog.FindBodyIndex("ChefBody"));
            foreach (SkinDef skinDef in skins)
            {
                if (skinDef.name == "FuukaCartDef")
                {
                    targetSkin = skinDef;
                    break;
                }
            }

            if (!targetSkin)
            {
                Debug.LogError("FuukaChefVoiceover: Chef Fuuka SkinDef not found. Voicelines will not work!");
            }
            else
            {
                VoiceoverInfo voiceoverInfo = new VoiceoverInfo(typeof(FuukaChefVoiceoverComponent), targetSkin, "ChefBody");
                voiceoverInfo.selectActions += FuukaSelect;
            }
            RefreshNSE();
            PopulateFoodList();
        }

        private void PopulateFoodList()
        {
            AddEquipmentIfValid("Fruit");
            AddEquipmentIfValid("HealAndRevive");
            AddEquipmentIfValid("HealAndReviveConsumed");
            //AddEquipmentIfValid("CLASSICITEMSRETURNS_EQUIPMENT_THQWIBS");

            AddEggItemIfValid("ParentEgg");
            AddEggItemIfValid("HopooEgg");
            AddEggEquipmentIfValid("FireBallDash");
        }

        private void AddEquipmentIfValid(string equipmentName)
        {
            EquipmentIndex id = EquipmentCatalog.FindEquipmentIndex(equipmentName);
            if (id != EquipmentIndex.None) FuukaChefVoiceoverComponent.foodEquipment.Add(id);
        }

        private void AddEggItemIfValid(string itemName)
        {
            ItemIndex id = ItemCatalog.FindItemIndex(itemName);
            if (id != ItemIndex.None) FuukaChefVoiceoverComponent.eggItems.Add(id);
        }

        private void AddEggEquipmentIfValid(string equipmentName)
        {
            EquipmentIndex id = EquipmentCatalog.FindEquipmentIndex(equipmentName);
            if (id != EquipmentIndex.None) FuukaChefVoiceoverComponent.eggEquipment.Add(id);
        }

        private void EnableVoicelines_SettingChanged(object sender, EventArgs e)
        {
            RefreshNSE();
        }

        private NetworkSoundEventDef RegisterNSE(string eventName)
        {
            NetworkSoundEventDef nse = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            nse.eventName = eventName;
            Content.networkSoundEventDefs.Add(nse);
            nseList.Add(new NSEInfo(nse));
            return nse;
        }

        public void RefreshNSE()
        {
            foreach (NSEInfo nse in nseList)
            {
                nse.ValidateParams();
            }
        }

        private void InitNSE()
        {
            FuukaChefVoiceoverComponent.nseDoushimashou = RegisterNSE("Play_FuukaChef_Doushimashou");
            FuukaChefVoiceoverComponent.nseNYCommon = RegisterNSE("Play_FuukaChef_NY_Common");
            FuukaChefVoiceoverComponent.nseTitle = RegisterNSE("Play_FuukaChef_Title");
            FuukaChefVoiceoverComponent.nseNYEx1 = RegisterNSE("Play_FuukaChef_NY_Ex1");
            FuukaChefVoiceoverComponent.nseNYEx2 = RegisterNSE("Play_FuukaChef_NY_Ex2");
            FuukaChefVoiceoverComponent.nseNYEx3 = RegisterNSE("Play_FuukaChef_NY_Ex3");
            FuukaChefVoiceoverComponent.nseNYExLv1 = RegisterNSE("Play_FuukaChef_NY_Ex_Lv1");
            FuukaChefVoiceoverComponent.nseNYExLv2 = RegisterNSE("Play_FuukaChef_NY_Ex_Lv2");
            FuukaChefVoiceoverComponent.nseNYExLv3 = RegisterNSE("Play_FuukaChef_NY_Ex_Lv3");
            FuukaChefVoiceoverComponent.nseEx1 = RegisterNSE("Play_FuukaChef_Ex1");
            FuukaChefVoiceoverComponent.nseEx2 = RegisterNSE("Play_FuukaChef_Ex2");
            FuukaChefVoiceoverComponent.nseEx3 = RegisterNSE("Play_FuukaChef_Ex3");
            FuukaChefVoiceoverComponent.nseExLv1 = RegisterNSE("Play_FuukaChef_Ex_Lv1");
            FuukaChefVoiceoverComponent.nseExLv2 = RegisterNSE("Play_FuukaChef_Ex_Lv2");
            FuukaChefVoiceoverComponent.nseExLv3 = RegisterNSE("Play_FuukaChef_Ex_Lv3");
            FuukaChefVoiceoverComponent.nseNYCafe1 = RegisterNSE("Play_FuukaChef_NY_Cafe_1");
        }

        public static List<NSEInfo> nseList = new List<NSEInfo>();
        public class NSEInfo
        {
            public NetworkSoundEventDef nse;
            public uint akId = 0u;
            public string eventName = string.Empty;

            public NSEInfo(NetworkSoundEventDef source)
            {
                this.nse = source;
                this.akId = source.akId;
                this.eventName = source.eventName;
            }

            private void DisableSound()
            {
                nse.akId = 0u;
                nse.eventName = string.Empty;
            }

            private void EnableSound()
            {
                nse.akId = this.akId;
                nse.eventName = this.eventName;
            }

            public void ValidateParams()
            {
                if (this.akId == 0u) this.akId = nse.akId;
                if (this.eventName == string.Empty) this.eventName = nse.eventName;

                if (!enableVoicelines.Value)
                {
                    DisableSound();
                }
                else
                {
                    EnableSound();
                }
            }
        }
    }
}
