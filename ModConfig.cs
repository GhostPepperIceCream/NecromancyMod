using StardewModdingAPI;
namespace NecromancyMod
{
    public class ModConfig
    {
        public SButton summonItemFriendKey = SButton.J;
        public SButton controlMonstersKey = SButton.K;
        public string summonAnimation = "Mini Harp";
        public string itemSummon = "Ghost";
        public bool attackSlimesInHutch = false;
        public int baseDamage = 10;

        internal static string[] summonAnimationChoices = new string[] { "Mini Harp", "Flute" };
        internal static string[] itemSummonChoices = new string[] { "Ghost", "Skeleton", "Haunted Skull", "Mummy" };
    }
}
