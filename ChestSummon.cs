using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using StardewValley.Characters;

namespace NecromancyMod
{
    public class ChestSummon
    {
        private const String ChestKey = "NecromancyModChestID";

        public static void callFriendlyMonster(NPC chestFriend)
        {
            Game1.warpCharacter(chestFriend, Game1.currentLocation, Game1.player.Position);
        }

        public static void ViewFriendItems()
        {
            Chest chestToOpen = CheckForChest();
            chestToOpen.ShowMenu();
        }

        private static Chest CheckForChest()
        {
            int chestID = 0;

            if (Game1.player.modData.ContainsKey(ChestKey))
            {
                chestID = int.Parse(Game1.player.modData[ChestKey]);
                Vector2 chestLocation = new Vector2(0f, chestID);

                if (Game1.getFarm().Objects.ContainsKey(chestLocation))
                {
                    var chest = Game1.getFarm().Objects[chestLocation];
                    if (chest is Chest)
                    {
                        return (Chest)chest;
                    } else
                    {
                        return null;
                    }
                } else
                {
                    return null;
                }
            } else
            {
                return CreateChest();
            }
        }

        private static Chest CreateChest()
        {
            if (!Context.IsMainPlayer)
            {
                return null;
            }

            Chest friendStorage;

            int num = -1;

            for (int i = 0; i < 10; i++)
            {
                if (!Game1.getFarm().Objects.ContainsKey(new Vector2(0f, i)))
                {
                    num = i;
                    break;
                }
            }

            if (num == -1)
            {
                return null;
            }

            friendStorage = new Chest(true, new Vector2(0f, num));
            Game1.player.modData[ChestKey] = num.ToString();
            Game1.getFarm().Objects.Add(new Vector2(0f, num), friendStorage);

            return friendStorage;
        }
    }
}
