using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewModdingAPI.Enums;
using StardewValley.Characters;
using System.IO;
using HarmonyLib;
using StardewValley.Monsters;

namespace NecromancyMod
{
    public class ModEntry : Mod
    {
        public ModConfig config;
        public ControlMonsters monsterController = new ControlMonsters();
        public NPC chestFriend;

        private SButton summonItemFriendKey;
        private SButton controlMonstersKey;
        private string summonAnimationType;
        private string chestType;
        private bool attackSlimesInHutch;
        private int baseDamage;

        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += onLaunched;
            helper.Events.Input.ButtonPressed += OnButtonsChanged;
            helper.Events.Player.Warped += Warped;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += Saving;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            //harmony.Patch(
            //    original: Game1.player.applyBuff(string id),
            //    prefix: new HarmonyMethod(typeof(ModEntry), 
            //    ) ;
        }

        private void onLaunched(object sender, GameLaunchedEventArgs e)
        {
            config = Helper.ReadConfig<ModConfig>();
            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            api.RegisterModConfig(ModManifest, () => config = new ModConfig(), () => Helper.WriteConfig(config));
            api.RegisterLabel(ModManifest, "Keybinds", "");
            api.RegisterSimpleOption(ModManifest, "Item Friend Summon Key", "Keybind to summon your item carrier for you.", () => config.summonItemFriendKey, (SButton val) => config.summonItemFriendKey = val);
            api.RegisterSimpleOption(ModManifest, "Control Monsters Key", "Keybind to summon/control undead.", () => config.controlMonstersKey, (SButton val) => config.controlMonstersKey = val);
            api.RegisterLabel(ModManifest, "Cosmetics", "");
            api.RegisterChoiceOption(ModManifest, "Summon Animation", "Animation that plays when you cast spells.", () => config.summonAnimation, (string val) => config.summonAnimation = val, ModConfig.summonAnimationChoices);
            api.RegisterChoiceOption(ModManifest, "Item Friend Choice", "Changing this does NOT affect your stored items.", () => config.itemSummon, (string val) => config.itemSummon = val, ModConfig.itemSummonChoices);
            api.RegisterLabel(ModManifest, "Mechanics", "");
            api.RegisterSimpleOption(ModManifest, "Attack Hutch Slimes", "Whether your summons should attack slimes in your slime hutch.", () => config.attackSlimesInHutch, (bool val) => config.attackSlimesInHutch = val);
            api.RegisterClampedOption(ModManifest, "Base Damage", "Damage summons do to other monsters. Scales with combat level. Default is 10.", () => config.baseDamage, (int val) => config.baseDamage = val, 1, 100);

            modOptions();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            modOptions();
            chestFriend = new NPC(new AnimatedSprite("Characters\\Monsters\\" + chestType), new Vector2(0f,0f), 0, "Item Friend", null);
            chestFriend.farmerPassesThrough = true;
            chestFriend.IsWalkingTowardPlayer = true;
            chestFriend.willDestroyObjectsUnderfoot = false;
            chestFriend.currentLocation = Game1.currentLocation;
            chestFriend.Sprite.Animate(Game1.currentGameTime, 1, 5, (float)20.0);
        }

        private void OnButtonsChanged(object sender, ButtonPressedEventArgs e)
        {

            if (!Context.CanPlayerMove)
                return;

            if (e.Button == this.summonItemFriendKey)
            {
                SummonAnimation.Play(this.summonAnimationType);
                DelayedAction.functionAfterDelay(summonItems, 1500);
                return;
            }

            if (e.Button == this.controlMonstersKey)
            {
                SummonAnimation.Play(this.summonAnimationType);
                DelayedAction.functionAfterDelay(summon, 1500);
                return;
            }
        }

        private void summon()
        {
            if (monsterController.isActive)
            {
                monsterController.updateMonsters();
            }
            else
            {
                Game1.currentLocation.playSound("wand", Game1.player.Tile);
                monsterController = new ControlMonsters(Monitor, baseDamage, attackSlimesInHutch);
            }
        }

        private void summonItems()
        {
            Game1.currentLocation.playSound("wand", Game1.player.Tile);
            ChestSummon.callFriendlyMonster(chestFriend);
        }

        private void Warped(object sender, WarpedEventArgs e)
        {
            monsterController.removeSummons();
            monsterController.isActive = false;
        }

        private void Saving(object sender, SavingEventArgs e)
        {
            monsterController.removeSummons();
            monsterController = new ControlMonsters();
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!monsterController.isActive)
            {
                return;
            }

            monsterController.updateMonsters();
            monsterController.checkDamageCalculation();
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!monsterController.isActive)
            {
                return;
            }
            monsterController.changeMonsterMovementPatterns();
        }

        private void modOptions()
        {
            this.summonItemFriendKey = this.config.summonItemFriendKey;
            this.controlMonstersKey = this.config.controlMonstersKey;
            this.summonAnimationType = this.config.summonAnimation;
            this.chestType = this.config.itemSummon;
            this.attackSlimesInHutch = this.config.attackSlimesInHutch;
            this.baseDamage = this.config.baseDamage;
        }
    }
}
