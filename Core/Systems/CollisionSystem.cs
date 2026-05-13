using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class CollisionSystem
    {
    // --- Spatial Grid DOD ---
    const int CELL_SIZE = 128;
    const int GRID_WIDTH = 24; // 3000/128 = 23.4
    const int GRID_HEIGHT = 24;
    static int[,] gridHeads = new int[GRID_WIDTH, GRID_HEIGHT];
    static int[] nextInCell = new int[GameManager.enemies.Length];
        public static void Update(float deltaTime, ref Player player)
        {
            // 1. LIMPIAR GRID (Una sola vez por frame)
            for (int gx = 0; gx < GRID_WIDTH; gx++)
                for (int gy = 0; gy < GRID_HEIGHT; gy++)
                    gridHeads[gx, gy] = -1;

            // 2. POBLAR GRID (Una sola vez por frame)
            for (int i = 0; i < GameManager.enemies.Length; i++)
            {
                if (!GameManager.enemies[i].IsActive) continue;
                int gx = (int)((GameManager.enemies[i].Position.X - GameManager.MapBounds.X) / CELL_SIZE);
                int gy = (int)((GameManager.enemies[i].Position.Y - GameManager.MapBounds.Y) / CELL_SIZE);
                if (gx < 0 || gy < 0 || gx >= GRID_WIDTH || gy >= GRID_HEIGHT) continue;
                nextInCell[i] = gridHeads[gx, gy];
                gridHeads[gx, gy] = i;
            }

            Rectangle playerRect = new Rectangle(player.Position.X, player.Position.Y, player.Size, player.Size);

            // 1. Recolección de Pickups (Empanadas, Imanes, etc)
            for (int i = 0; i < GameManager.pickups.Length; i++)
            {
                if (GameManager.pickups[i].IsActive)
                {
                    Rectangle pickupRect = new Rectangle(GameManager.pickups[i].Position.X, GameManager.pickups[i].Position.Y, GameManager.pickups[i].Size, GameManager.pickups[i].Size);

                    if (Raylib.CheckCollisionRecs(playerRect, pickupRect))
                    {
                        GameManager.pickups[i].IsActive = false;
                        if (GameManager.pickups[i].Type == 0)
                        {
                            player.Health = Math.Min(player.Health + 20, player.MaxHealth);
                        }
                        else if (GameManager.pickups[i].Type == 1)
                        {
                            for (int g = 0; g < GameManager.gems.Length; g++)
                            {
                                if (GameManager.gems[g].IsActive)
                                {
                                    GameManager.gems[g].Position = player.Position;
                                }
                            }
                        }
                        else if (GameManager.pickups[i].Type == 2) { player.Coins++; GameManager.GlobalCoins++; }
                        else if (GameManager.pickups[i].Type == 3) { GameManager.DrunkTimer = 10.0f; }
                    }
                }
            }

            // 2. Recolección de Gemas (Experiencia) e Imán
            for (int i = 0; i < GameManager.gems.Length; i++)
            {
                if (GameManager.gems[i].IsActive)
                {
                    Vector2 playerCenter = player.Position + new Vector2(player.Size / 2.0f);
                    Vector2 gemCenter = GameManager.gems[i].Position + new Vector2(GameManager.gems[i].Size / 2.0f);
                    float dist = Vector2.Distance(playerCenter, gemCenter);
                    if (dist < player.PickupRadius * player.MagnetMult)
                    {
                        Vector2 pullDir = Vector2.Normalize(playerCenter - gemCenter);
                        GameManager.gems[i].Position += pullDir * 400.0f * deltaTime;
                    }

                    Rectangle gemRect = new Rectangle(GameManager.gems[i].Position.X, GameManager.gems[i].Position.Y, GameManager.gems[i].Size, GameManager.gems[i].Size);

                    if (Raylib.CheckCollisionRecs(playerRect, gemRect))
                    {
                        player.XP += GameManager.gems[i].Value;
                        GameManager.gems[i].IsActive = false;

                        if (player.XP >= player.MaxXP)
                        {
                            player.XP -= player.MaxXP;
                            player.Level++;
                            player.MaxXP = (int)(player.MaxXP * 1.5f);
                            GameManager.State = GameState.LevelUpMenu;
                            GameManager.GenerateUpgrades(ref player); 
                        }
                    }
                }
            }

            // 3. Proyectiles Manuales (Espacio)
            for (int i = 0; i < GameManager.manualProjectiles.Length; i++)
            {
                if (GameManager.manualProjectiles[i].IsActive)
                {
                    for (int j = 0; j < GameManager.enemies.Length; j++)
                    {
                        if (GameManager.enemies[j].IsActive && Raylib.CheckCollisionCircles(GameManager.manualProjectiles[i].Position, GameManager.manualProjectiles[i].Size, GameManager.enemies[j].Position + new Vector2(GameManager.enemies[j].Size / 2), GameManager.enemies[j].Size / 2))
                        {
                            GameManager.enemies[j].Health -= 15.0f;
                            GameManager.SpawnDamageText(GameManager.enemies[j].Position, 15);
                            GameManager.manualProjectiles[i].IsActive = false;
                            GameManager.HitStopTimer = GameManager.enemies[j].Type == 3 ? 0.06f : 0.03f;

                            CheckEnemyDeath(j);
                            break;
                        }
                    }
                }
            }

            // 4. Proyectiles Automáticos (Piedrazos)
            for (int i = 0; i < GameManager.projectiles.Length; i++)
            {
                if (GameManager.projectiles[i].IsActive)
                {
                    for (int j = 0; j < GameManager.enemies.Length; j++)
                    {
                        if (GameManager.enemies[j].IsActive && Raylib.CheckCollisionCircles(GameManager.projectiles[i].Position, GameManager.projectiles[i].Size, GameManager.enemies[j].Position + new Vector2(GameManager.enemies[j].Size / 2), GameManager.enemies[j].Size / 2))
                        {
                            GameManager.enemies[j].Health -= 30.0f;
                            GameManager.SpawnDamageText(GameManager.enemies[j].Position, 30);
                            GameManager.projectiles[i].IsActive = false;
                            GameManager.HitStopTimer = GameManager.enemies[j].Type == 3 ? 0.06f : 0.03f;

                            CheckEnemyDeath(j);
                            break;
                        }
                    }
                }
            }

            // 5. Proyectiles Rebotadores (Halls)
            for (int i = 0; i < GameManager.bouncingProjectiles.Length; i++)
            {
                if (GameManager.bouncingProjectiles[i].IsActive)
                {
                    for (int j = 0; j < GameManager.enemies.Length; j++)
                    {
                        if (GameManager.enemies[j].IsActive && j != GameManager.bouncingProjectiles[i].TargetIndex && Raylib.CheckCollisionCircles(GameManager.bouncingProjectiles[i].Position, GameManager.bouncingProjectiles[i].Size, GameManager.enemies[j].Position + new Vector2(GameManager.enemies[j].Size / 2), GameManager.enemies[j].Size / 2))
                        {
                            GameManager.enemies[j].Health -= 20.0f;
                            GameManager.SpawnDamageText(GameManager.enemies[j].Position, 20);
                            GameManager.enemies[j].FreezeTimer = 1.0f;
                            GameManager.HitStopTimer = GameManager.enemies[j].Type == 3 ? 0.06f : 0.03f;

                            CheckEnemyDeath(j);

                            GameManager.bouncingProjectiles[i].BouncesLeft--;
                            if (GameManager.bouncingProjectiles[i].BouncesLeft <= 0)
                            {
                                GameManager.bouncingProjectiles[i].IsActive = false;
                            }
                            else
                            {
                                GameManager.bouncingProjectiles[i].TargetIndex = j;
                                int nextTarget = GameManager.GetClosestEnemyIndex(GameManager.bouncingProjectiles[i].Position);
                                if (nextTarget != -1 && nextTarget != j)
                                {
                                    GameManager.bouncingProjectiles[i].Velocity = Vector2.Normalize(GameManager.enemies[nextTarget].Position - GameManager.bouncingProjectiles[i].Position) * 600.0f;
                                }
                                else
                                {
                                    GameManager.bouncingProjectiles[i].Velocity = -GameManager.bouncingProjectiles[i].Velocity;
                                }
                            }
                            break;
                        }
                    }
                }
            }

            // 6. Charcos de Daño
            for (int i = 0; i < GameManager.puddles.Length; i++)
            {
                if (GameManager.puddles[i].IsActive)
                {
                    for (int j = 0; j < GameManager.enemies.Length; j++)
                    {
                        if (GameManager.enemies[j].IsActive && Raylib.CheckCollisionCircles(GameManager.puddles[i].Position, GameManager.puddles[i].Radius, GameManager.enemies[j].Position + new Vector2(GameManager.enemies[j].Size / 2), GameManager.enemies[j].Size / 2))
                        {
                            GameManager.enemies[j].Health -= 30.0f * deltaTime;
                            
                            if (GameManager.GameTime % 0.5f < deltaTime) 
                            {
                                GameManager.SpawnDamageText(GameManager.enemies[j].Position, 15);
                            }
                            CheckEnemyDeath(j);
                        }
                    }
                }
            }

            // 7. Colisiones Directas: Jugador contra Enemigos y Daño Fijo (Machete, Látigo)
            for (int i = 0; i < GameManager.enemies.Length; i++)
            {
                if (GameManager.enemies[i].IsActive)
                {
                    Rectangle enemyRect = new Rectangle(GameManager.enemies[i].Position.X, GameManager.enemies[i].Position.Y, GameManager.enemies[i].Size, GameManager.enemies[i].Size);

                    if (WeaponManager.isAttacking && Raylib.CheckCollisionRecs(WeaponManager.attackHitbox, enemyRect))
                    {
                        GameManager.enemies[i].Health -= 50.0f;
                        GameManager.SpawnDamageText(GameManager.enemies[i].Position, 50);
                        GameManager.enemies[i].Position -= Vector2.Normalize(player.Position - GameManager.enemies[i].Position) * 20.0f;
                        GameManager.HitStopTimer = GameManager.enemies[i].Type == 3 ? 0.06f : 0.03f;
                        CheckEnemyDeath(i);
                    }

                    if (GameManager.enemies[i].IsActive && player.HasWhip && WeaponManager.isWhipping && (Raylib.CheckCollisionRecs(WeaponManager.whipHitboxLeft, enemyRect) || Raylib.CheckCollisionRecs(WeaponManager.whipHitboxRight, enemyRect)))
                    {
                        int whipDmg = 40;
                        GameManager.enemies[i].Health -= whipDmg;
                        GameManager.SpawnDamageText(GameManager.enemies[i].Position, whipDmg);
                        GameManager.HitStopTimer = GameManager.enemies[i].Type == 3 ? 0.06f : 0.03f;
                        CheckEnemyDeath(i);
                    }

                    if (GameManager.enemies[i].IsActive)
                    {
                        if (Raylib.CheckCollisionRecs(playerRect, enemyRect))
                        {
                            if (WeaponManager.isParrying)
                            {
                                if (GameManager.enemies[i].FreezeTimer <= 0)
                                {
                                    WeaponManager.parrySuccessTimer = 0.3f;
                                }
                                GameManager.enemies[i].FreezeTimer = 1.5f; // Stunned status
                            }
                            else
                            {
                                player.Health -= 10.0f * deltaTime;
                                if (player.DamageFlashTimer <= 0)
                                {
                                    player.DamageFlashTimer = 0.2f;
                                    player.ScreenShakeTimer = 0.3f;
                                }
                                
                                if (player.Health <= 0)
                                {
                                    GameManager.State = GameState.GameOver;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckEnemyDeath(int index)
        {
            if (GameManager.enemies[index].Health <= 0)
            {
                GameManager.enemies[index].IsActive = false;
                
                // Efecto de Partículas al morir (sangre/polvo)
                Color deathColor = GameManager.enemies[index].Type == 3 ? Color.Purple : Color.Red;
                if (GameManager.enemies[index].Type == 8) deathColor = Color.DarkGray; // Camionetica
                GameManager.SpawnParticles(GameManager.enemies[index].Position, deathColor, 15);

                // Hit-Stop para Jefes o Mini-Jefes
                if (GameManager.enemies[index].Type == 3 || GameManager.enemies[index].Type == 1 || GameManager.enemies[index].Type == 2)
                {
                    GameManager.HitStopTimer = 0.15f;
                }

                // La recompensa ahora escala con la dificultad
                int baseValue = GameManager.enemies[index].Type == 3 ? 10 : (GameManager.enemies[index].Type == 2 ? 5 : 1);
                int scaledValue = (int)(baseValue * GameManager.DifficultyMultiplier);
                GameManager.SpawnGem(GameManager.enemies[index].Position, scaledValue);
                GameManager.SpawnPickup(GameManager.enemies[index].Position);
            }
        }
    }
}
