using StardewValley;

namespace NecromancyMod
{
    public class SummonAnimation
    {
        public static void Play(string animationType)
        {
            switch (animationType)
            {
                case "Flute":
                    Flute();
                    break;
                case "Mini Harp":
                    Miniharp();
                    break;
                case "Hand raised":
                    HandRaised();
                    break;
                default:
                    break;
            }
        }

        private static void Flute()
        {
            Game1.player.faceDirection(2);
            Game1.soundBank.PlayCue("horse_flute");
            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[6]
               {
                            new FarmerSprite.AnimationFrame(98, 400, secondaryArm: true, flip: false),
                            new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false),
                            new FarmerSprite.AnimationFrame(100, 200, secondaryArm: true, flip: false),
                            new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false),
                            new FarmerSprite.AnimationFrame(98, 400, secondaryArm: true, flip: false),
                            new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false)
               });
            Game1.player.freezePause = 1500;
        }

        private static void Miniharp()
        {
            Game1.player.faceDirection(2);

            int[] array = new int[4]
                {
                    800 + Game1.random.Next(4) * 100,
                    1200 + Game1.random.Next(4) * 100,
                    1600 + Game1.random.Next(4) * 100,
                    2000 + Game1.random.Next(4) * 100
                 };
            for (int i = 0; i < array.Length; i++)
            {
                DelayedAction.playSoundAfterDelay("miniharp_note", Game1.random.Next(60, 150) * i, Game1.currentLocation, Game1.player.Tile, array[i]);
                if (i > 1 && Game1.random.NextDouble() < 0.25)
                {
                    break;
                }
            }

            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[6]
            {
                new FarmerSprite.AnimationFrame(99, 150, secondaryArm: false, flip: false),
                new FarmerSprite.AnimationFrame(100, 150, secondaryArm: false, flip: false),
                new FarmerSprite.AnimationFrame(98, 150, secondaryArm: false, flip: false),
                new FarmerSprite.AnimationFrame(99, 150, secondaryArm: false, flip: false),
                new FarmerSprite.AnimationFrame(100, 150, secondaryArm: false, flip: false),
                new FarmerSprite.AnimationFrame(98, 150, secondaryArm: false, flip: false)
            });

            Game1.player.freezePause = 1500;
        }

        private static void HandRaised()
        {
            Game1.player.faceDirection(2);

            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
            {
                new FarmerSprite.AnimationFrame(15, 1500, secondaryArm: false, flip: false),
            }) ;
            Game1.player.freezePause = 1500;
        }
    }
}
