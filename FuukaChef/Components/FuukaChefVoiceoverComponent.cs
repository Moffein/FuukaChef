using BaseVoiceoverLib;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static FuukaChef.FuukaChefVoiceoverPlugin;

namespace FuukaChef.Components
{
    public class FuukaChefVoiceoverComponent : BaseVoiceoverComponent
    {
        private bool acquiredScepter = false;

        private float specialCooldown = 0f;
        private float utilityCooldown = 0f;
        private float secondaryCooldown = 0f;
        private float levelCooldown = 0f;
        private float shrineFailCooldown = 0f;
        private float lowHealthCooldown = 0f;
        private float foodCommentCooldown = 0f;

        public static NetworkSoundEventDef nseNYCafe1 ,nseThanks, nseDoushimashou, nseNYCommon, nseNYEx1, nseNYEx2, nseNYEx3, nseNYExLv1, nseNYExLv2, nseNYExLv3, nseEx1, nseEx2, nseEx3, nseExLv1, nseExLv2, nseExLv3, nseTitle;

        public static HashSet<ItemIndex> eggItems = new HashSet<ItemIndex>();
        public static HashSet<EquipmentIndex> foodEquipment = new HashSet<EquipmentIndex>();
        public static HashSet<EquipmentIndex> eggEquipment = new HashSet<EquipmentIndex>();

        protected override void Start()
        {
            base.Start();
            if (inventory)
            {
                if (inventory.GetItemCount(scepterIndex) > 0) acquiredScepter = true;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (specialCooldown > 0f) specialCooldown -= Time.fixedDeltaTime;
            if (utilityCooldown > 0f) utilityCooldown -= Time.fixedDeltaTime;
            if (secondaryCooldown > 0f) secondaryCooldown -= Time.fixedDeltaTime;
            if (levelCooldown > 0f) levelCooldown -= Time.fixedDeltaTime;
            if (lowHealthCooldown > 0f) lowHealthCooldown -= Time.fixedDeltaTime;
            if (shrineFailCooldown > 0f) shrineFailCooldown -= Time.fixedDeltaTime;
            if (foodCommentCooldown > 0f) foodCommentCooldown -= Time.fixedDeltaTime;
        }

        public override void PlayLevelUp()
        {
            if (levelCooldown > 0f) return;
            bool played;
            if (Util.CheckRoll(50f))
            {
                played = TryPlaySound("Play_FuukaChef_LevelUp_Short", 2.1f, false);
            }
            else
            {
                played = TryPlaySound("Play_FuukaChef_LevelUp_Long", 3.3f, false);
            }
            if (played) levelCooldown = 60f;
        }

        protected override void Inventory_onEquipmentChangedClient(EquipmentIndex equipmentIndex, uint equipmentSlot)
        {
            base.Inventory_onEquipmentChangedClient(equipmentIndex, equipmentSlot);
            if (eggEquipment.Contains(equipmentIndex))
            {
                PlayEggGet();
            }
            else if (foodEquipment.Contains(equipmentIndex))
            {
                PlayFoodGet();
            }
        }

        protected override void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            base.Inventory_onItemAddedClient(itemIndex);
            if (scepterIndex != ItemIndex.None && itemIndex == scepterIndex)
            {
                PlayAcquireScepter();
            }
            else
            {
                ItemDef id = ItemCatalog.GetItemDef(itemIndex);
                if (id && id.tier == ItemTier.Tier3)
                {
                    PlayAcquireLegendary();
                }
                else if (eggItems.Contains(itemIndex))
                {
                    PlayEggGet();
                }
                else if (id.ContainsTag(ItemTag.FoodRelated))
                {
                    PlayFoodGet();
                }
            }
        }

        public void PlayAcquireScepter()
        {
            if (acquiredScepter) return;
            TryPlaySound("Play_FuukaChef_AcquireScepter", 8f, true);
            acquiredScepter = true;
        }

        public void PlayEggGet()
        {
            if (foodCommentCooldown > 0f) return;
            bool played = TryPlaySound("Play_FuukaChef_Egg", 7f, true);
            if (played) foodCommentCooldown = 30f;
        }

        public void PlayFoodGet()
        {
            if (foodCommentCooldown > 0f) return;
            bool played = false;
            int rand = UnityEngine.Random.Range(1, 6);
            switch (rand)
            {
                case 1:
                    played = TryPlaySound("Play_FuukaChef_Lobby_2", 4f, false);
                    break;
                case 2:
                    played = TryPlaySound("Play_FuukaChef_Lobby_5", 7f, false);
                    break;
                case 3:
                    played = TryPlaySound("Play_FuukaChef_Cafe_1", 2.1f, false);
                    break;
                case 4:
                    played = TryPlaySound("Play_FuukaChef_Cafe_3", 2.6f, false);
                    break;
                default:
                    played = TryPlaySound("Play_FuukaChef_NY_Cafe_1", 3.5f, false);
                    break;
            }
            if (played) foodCommentCooldown = 30f;
        }

        public void PlayAcquireLegendary()
        {
            int rand = UnityEngine.Random.Range(1, 5);
            switch (rand)
            {
                case 1:
                    TryPlaySound("Play_FuukaChef_Relationship_1", 4f, false);
                    break;
                case 2:
                    TryPlaySound("Play_FuukaChef_Relationship_2", 3.1f, false);
                    break;
                case 3:
                    TryPlaySound("Play_FuukaChef_Relationship_3", 3.3f, false);
                    break;
                default:
                    TryPlaySound("Play_FuukaChef_Relationship_4", 6.25f, false);
                    break;
            }
        }

        public override void PlayLowHealth()
        {
            if (lowHealthCooldown > 0f) return;
            bool playedSound = TryPlaySound("Play_FuukaChef_Lobby_3", 4.2f, false);
            if (playedSound) lowHealthCooldown = 60f;
        }

        public override void PlaySpawn()
        {
            int rand = UnityEngine.Random.Range(1, 6);
            switch (rand)
            {
                case 1:
                    TryPlaySound("Play_FuukaChef_Battle_In_1", 2.3f, true);
                    break;
                case 2:
                    TryPlaySound("Play_FuukaChef_Battle_TSA_1", 4.3f, true);
                    break;
                case 3:
                    TryPlaySound("Play_FuukaChef_Battle_TSA_2", 2.7f, true);
                    break;
                case 4:
                    TryPlaySound("Play_FuukaChef_Cafe_Act_2", 1.45f, true);
                    break;
                default:
                    TryPlaySound("Play_FuukaChef_CookingSkills", 4.7f, true);
                    break;
            }
        }

        public override void PlayTeleporterStart()
        {
            int rand = UnityEngine.Random.Range(1, 6);
            switch (rand)
            {
                case 1:
                    TryPlaySound("Play_FuukaChef_Battle_In_1", 2.3f, false);
                    break;
                case 2:
                    TryPlaySound("Play_FuukaChef_Battle_TSA_1", 4.3f, false);
                    break;
                case 3:
                    TryPlaySound("Play_FuukaChef_Battle_TSA_2", 2.7f, true);
                    break;
                case 4:
                    TryPlaySound("Play_FuukaChef_Cafe_Act_2", 1.45f, false);
                    break;
                default:
                    TryPlaySound("Play_FuukaChef_CookingSkills", 4.7f, false);
                    break;
            }
        }

        public override void PlayTeleporterFinish()
        {
            int rand = UnityEngine.Random.Range(1, 5);
            switch (rand)
            {
                case 1:
                    TryPlaySound("Play_FuukaChef_EventEnd", 7.5f, false);
                    break;
                case 2:
                    TryPlaySound("Play_FuukaChef_CommonSkill", 1.5f, false);
                    break;
                case 3:
                    TryPlaySound("Play_FuukaChef_Ex2", 1.55f, false);
                    break;
                default:
                    TryPlaySound("Play_FuukaChef_Ex3", 2.3f, false);
                    break;
            }
        }

        public override void PlayVictory()
        {
            TryPlaySound("Play_FuukaChef_Memorial5", 17f, true);
        }

        public override void PlayShrineOfChanceFailServer()
        {
            if (shrineFailCooldown > 0f) return;
            if (Util.CheckRoll(15f))
            {
                bool played = TryPlayNetworkSound(nseDoushimashou, 1.5f, false);
                if (played) shrineFailCooldown = 60f;
            }
        }

        public override void PlaySecondaryAuthority(GenericSkill skill)
        {
            if (secondaryCooldown > 0f) return;
            bool played = false;

            int rand = UnityEngine.Random.Range(1, 5);
            switch (rand)
            {
                case 1:
                    TryPlayNetworkSound(nseEx1, 1f, false);
                    break;
                case 2:
                    TryPlayNetworkSound(nseExLv1, 1.7f, false);
                    break;
                case 3:
                    TryPlayNetworkSound(nseExLv2, 1.5f, false);
                    break;
                default:
                    TryPlayNetworkSound(nseExLv3, 1.65f, false);
                    break;
            }

            if (played) secondaryCooldown = 12f;
        }

        public override void PlayUtilityAuthority(GenericSkill skill)
        {
            if (utilityCooldown > 0f) return;
            bool played = false;

            int rand = UnityEngine.Random.Range(1, 4);
            switch (rand)
            {
                case 1:
                    TryPlayNetworkSound(nseNYCommon, 1.3f, false);
                    break;
                case 2:
                    TryPlayNetworkSound(nseNYEx1, 1.5f, false);
                    break;
                default:
                    TryPlayNetworkSound(nseNYEx3, 1f, false);
                    break;
            }

            if (played) utilityCooldown = 30f;
        }

        public override void PlaySpecialAuthority(GenericSkill skill)
        {
            if (specialCooldown > 0f) return;
            bool played = false;

            int rand = UnityEngine.Random.Range(1, 4);
            switch (rand)
            {
                case 1:
                    TryPlayNetworkSound(nseNYEx2, 1.65f, false);
                    break;
                case 2:
                    TryPlayNetworkSound(nseNYExLv2, 1.45f, false);
                    break;
                default:
                    TryPlayNetworkSound(nseNYExLv3, 2.05f, false);
                    break;
            }

            if (played) specialCooldown = 12f;
        }

        protected override void CheckInputs()
        {
            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnTitle))
            {
                TryPlayNetworkSound(nseTitle, 1.7f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnEx1))
            {
                TryPlayNetworkSound(nseEx1, 1f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnEx2))
            {
                TryPlayNetworkSound(nseEx2, 1.55f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnEx3))
            {
                TryPlayNetworkSound(nseEx3, 2.3f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnExLv1))
            {
                TryPlayNetworkSound(nseExLv1, 1.7f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnExLv2))
            {
                TryPlayNetworkSound(nseExLv2, 1.5f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnExLv3))
            {
                TryPlayNetworkSound(nseExLv3, 1.65f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnNYEx1))
            {
                TryPlayNetworkSound(nseNYEx1, 1.5f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnNYEx2))
            {
                TryPlayNetworkSound(nseNYEx2, 1.65f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnNYEx3))
            {
                TryPlayNetworkSound(nseNYEx3, 1f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnNYExLv1))
            {
                TryPlayNetworkSound(nseNYExLv1, 1.85f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnNYExLv2))
            {
                TryPlayNetworkSound(nseNYExLv2, 1.45f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnNYExLv3))
            {
                TryPlayNetworkSound(nseNYExLv3, 2.05f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnDoushimashou))
            {
                TryPlayNetworkSound(nseDoushimashou, 1.2f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnThanks))
            {
                TryPlayNetworkSound(nseThanks, 1.2f, false);
                return;
            }

            if (BaseVoiceoverLib.Utils.GetKeyPressed(FuukaChefVoiceoverPlugin.btnNyCafe1))
            {
                TryPlayNetworkSound(nseNYCafe1, 3.5f, false);
                return;
            }
        }
    }
}
