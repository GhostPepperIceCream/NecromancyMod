using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System.Collections.Generic;

namespace NecromancyMod
{
    public class ControlMonsters
    {
        private static IMonitor Monitor;
        public bool isActive;
        private int baseDamage;
        private bool attackSlimesInHutch;
        private List<Monster> undeadMonsters = new List<Monster>();
        private List<Monster> otherMonsters = new List<Monster>();
        private List<Monster> summonedMonsters = new List<Monster>();

        public ControlMonsters()
        {
            isActive = false;
        }

        public ControlMonsters(IMonitor monitor, int baseDamage, bool attackSlimesInHutch)
        {
            isActive = true;
            Monitor = monitor;
            this.baseDamage = baseDamage;
            this.attackSlimesInHutch = attackSlimesInHutch;
            summonUndead();
            updateMonsters();
            foreach (Monster monster in undeadMonsters)
            {
                monster.doEmote(16);
            }
        }

        public void checkDamageCalculation()
        {
            if (Game1.currentLocation is SlimeHutch && !attackSlimesInHutch)
            {
                return;
            }

            foreach (Monster monster in undeadMonsters)
            {
                foreach (Monster otherMonster in otherMonsters)
                {
                    if (otherMonster.Tile == monster.Tile)
                    {
                        if (otherMonster.health.Value <= 0)
                        {
                            break;
                        }

                        Random r = new Random();

                        monster.setInvincibleCountdown(1);
                        Game1.currentLocation.damageMonster(otherMonster.GetBoundingBox(), CalculateDamage() - 5, CalculateDamage() + 5, false, Game1.player);
                        otherMonster.tryToMoveInDirection(r.Next(0, 4), false, 0, otherMonster.isGlider.Value);
                    }
                }
            }
        }

        public void changeMonsterMovementPatterns()
        {
            foreach (Monster monster in undeadMonsters)
            {
                monster.focusedOnFarmers = false;
                if (monster.withinPlayerThreshold())
                {
                    monster.IsWalkingTowardPlayer = false;
                    
                } else
                {
                    monster.IsWalkingTowardPlayer = true;
                }

                if (Game1.currentLocation is SlimeHutch && !attackSlimesInHutch)
                {
                    return;
                }

                foreach (Monster otherMonster in otherMonsters)
                {
                    if (withinMonsterThreshold(monster, otherMonster))
                    {
                        if (otherMonster.health.Value <= 0 || otherMonster == null || monster == null)
                        {
                            return;
                        }

                        moveTowardsOtherMonster(monster, otherMonster);
                    }
                }
            }
        }

        private bool withinMonsterThreshold(Monster attacker, Monster target)
        {
            Vector2 attackerLocation = attacker.Tile;
            Vector2 targetLocation = target.Tile;

            if (attackerLocation == null || targetLocation == null)
            {
                return false;
            }

            return Math.Abs(targetLocation.X - attackerLocation.X) <= 10.0 && Math.Abs(targetLocation.Y - attackerLocation.Y) <= 10.0;
        }

        private void moveTowardsOtherMonster(Monster attacker, Monster target)
        {
            if (attacker == null || target == null)
            {
                return;
            }

            Point targetPixel = target.StandingPixel;
            Point attackerPixel = attacker.StandingPixel;
            GameTime time = new GameTime();

            int xToGo = Math.Abs(targetPixel.X - attackerPixel.X);
            int yToGo = Math.Abs(targetPixel.Y - attackerPixel.Y);

            double chanceForX = (double)xToGo / (double)(xToGo + yToGo);
            int dx = ((targetPixel.X > attackerPixel.X) ? 1 : (-1));
            int dy = ((targetPixel.Y > attackerPixel.Y) ? 1 : (-1));
            if (Game1.random.NextDouble() < chanceForX)
            {
                attacker.tryToMoveInDirection((dx > 0) ? 1 : 3, isFarmer: false, attacker.damageToFarmer.Value, glider: attacker.isGlider.Value);
            }
            else
            {
                attacker.tryToMoveInDirection((dy > 0) ? 1 : 3, isFarmer: false, attacker.damageToFarmer.Value, glider: attacker.isGlider.Value);
            }

            //if (Math.Abs(targetPixel.Y - attackerPixel.Y) > 192)
            //{
            //    if (attackerPixel.X - targetPixel.X > 0)
            //    {
            //        attacker.SetMovingLeft(b: true);
            //    }
            //    else
            //    {
            //        attacker.SetMovingRight(b: true);
            //    }
            //}
            //else if (attackerPixel.Y - targetPixel.Y > 0)
            //{
            //    attacker.SetMovingUp(b: true);
            //}
            //else
            //{
            //    attacker.SetMovingDown(b: true);
            //}
            //attacker.MovePosition(time, Game1.viewport, attacker.currentLocation);
        }

        private int CalculateDamage()
        {
            return (Game1.player.combatLevel.Value + 1) * baseDamage;
        }

        private bool isUndead(Monster monster)
        {
            if (monster is Skeleton || monster is Ghost || monster is Mummy)
            {
                return true;
            } else if (monster is Bat)
            {
                if (((Bat)monster).hauntedSkull.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void updateMonsters()
        {
            undeadMonsters = getUndeadMonsterList();
            otherMonsters = getOtherMonstersList();

            foreach (Monster monster in undeadMonsters)
            {
                monster.farmerPassesThrough = true;
                monster.DamageToFarmer = 0;
                
            }
        }

        private List<Monster> getUndeadMonsterList()
        {
            Monster monster;
            List<Monster> monsters = new List<Monster>();
            for (int i = Game1.currentLocation.characters.Count - 1; i >= 0; i--)
            {
                if ((monster = Game1.currentLocation.characters[i] as Monster) != null && isUndead(monster))
                {
                    monsters.Add(monster);
                    monster.focusedOnFarmers = true;
                }
            }
            return monsters;
        }

        private List<Monster> getOtherMonstersList()
        {
            Monster monster;
            List<Monster> monsters = new List<Monster>();
            for (int i = Game1.currentLocation.characters.Count - 1; i >= 0; i--)
            {
                if ((monster = Game1.currentLocation.characters[i] as Monster) != null && !isUndead(monster))
                {
                    monsters.Add(monster);
                }
            }
            return monsters;
        }

        private void summonUndead()
        {
            int combatLevel = Game1.player.CombatLevel;

            int numberToSummon = 0;
            Random random = new Random();

            if (combatLevel < 3)
            {
                numberToSummon = random.Next(1, 3);
            } else if (combatLevel >= 3 && combatLevel < 7)
            {
                numberToSummon = random.Next(2, 5);
            } else if (combatLevel >= 7 && combatLevel < 10)
            {
                numberToSummon = random.Next(4, 8);
            } else if (combatLevel == 10)
            {
                numberToSummon = random.Next(7, 11);
            } else if (combatLevel > 10)
            {
                numberToSummon = random.Next(10, 16);
            }
            
            while (numberToSummon > 0)
            {
                int randomType = random.Next(1, 8);

                if (randomType == 1)
                {
                    Ghost ghost = new Ghost(Game1.player.Position);
                    summonedMonsters.Add(ghost);
                    Game1.currentLocation.addCharacter(ghost);
                } else if (randomType == 2)
                {
                    Ghost carbonGhost = new Ghost(Game1.player.Position, "Carbon Ghost");
                    summonedMonsters.Add(carbonGhost);
                    Game1.currentLocation.addCharacter(carbonGhost);
                } else if (randomType == 3)
                {
                    Ghost putridGhost = new Ghost(Game1.player.Position, "Putrid Ghost");
                    summonedMonsters.Add(putridGhost);
                    Game1.currentLocation.addCharacter(putridGhost);
                } else if (randomType == 4)
                {
                    Skeleton skeleton = new Skeleton(Game1.player.Position, false);
                    summonedMonsters.Add(skeleton);
                    Game1.currentLocation.addCharacter(skeleton);
                } else if (randomType == 5)
                {
                    Skeleton skeletonMage = new Skeleton(Game1.player.Position, true);
                    summonedMonsters.Add(skeletonMage);
                    Game1.currentLocation.addCharacter(skeletonMage);
                } else if (randomType == 6)
                {
                    Bat hauntedSkull = new Bat(Game1.player.Position, 77377);
                    summonedMonsters.Add(hauntedSkull);
                    Game1.currentLocation.addCharacter(hauntedSkull);
                } else if (randomType == 7)
                {
                    Mummy mummy = new Mummy(Game1.player.Position);
                    summonedMonsters.Add(mummy);
                    Game1.currentLocation.addCharacter(mummy);
                }
                
                numberToSummon--;
            }
        }

        public void removeSummons()
        {
            foreach (Monster summon in summonedMonsters)
            {
                Game1.currentLocation.characters.Remove(summon);
                summon.currentLocation.characters.Remove(summon);
                //Game1.removeCharacterFromItsLocation(summon);
                undeadMonsters.Remove(summon);
            }
            summonedMonsters = new List<Monster>();
        }
    }
}
