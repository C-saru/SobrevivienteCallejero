using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class EnemySystem
    {
        public static void Update(float deltaTime, ref Player player)
        {
            for (int i = 0; i < GameManager.enemies.Length; i++)
            {
                if (GameManager.enemies[i].IsActive)
                {
                    if (GameManager.enemies[i].HitFlashTimer > 0)
                    {
                        GameManager.enemies[i].HitFlashTimer -= deltaTime;
                    }

                    if (GameManager.enemies[i].FreezeTimer > 0)
                    {
                        GameManager.enemies[i].FreezeTimer -= deltaTime;
                    }
                    else
                    {
                        GameManager.enemies[i].StateTimer += deltaTime;
                        Vector2 dirToPlayer = Vector2.Normalize(player.Position - GameManager.enemies[i].Position);
                        float distToPlayer = Vector2.Distance(player.Position, GameManager.enemies[i].Position);
                        
                        float enemySpeed = GameManager.enemies[i].BaseSpeed;
                        Vector2 movementDir = dirToPlayer;

                        if (GameManager.enemies[i].IsHoraPico)
                        {
                            movementDir = GameManager.enemies[i].DashDirection;
                            enemySpeed = GameManager.enemies[i].BaseSpeed;
                        }
                        else
                        {
                            switch (GameManager.enemies[i].Type)
                        {
                            case 0: // Normal
                            case 1: // Motorizado
                            case 2: // Tanque
                                movementDir = dirToPlayer;
                                break;
                            case 3: // Jefe
                                enemySpeed *= 1.5f;
                                movementDir = dirToPlayer;
                                break;
                            case 4: // Bachaquero (Zigzag)
                                Vector2 perp = new Vector2(-dirToPlayer.Y, dirToPlayer.X);
                                movementDir = Vector2.Normalize(dirToPlayer + perp * (float)Math.Sin(GameManager.enemies[i].StateTimer * 5.0f));
                                break;
                            case 5: // Malandro (Acechador)
                                float dot = Vector2.Dot(player.FacingDirection, -dirToPlayer);
                                if (dot > 0.5f) 
                                    enemySpeed = 0;
                                else 
                                    movementDir = dirToPlayer;
                                break;
                            case 6: // Borrachito (Errático)
                                if (GameManager.enemies[i].StateTimer > 0.5f) 
                                {
                                    GameManager.enemies[i].StateTimer = 0;
                                    float randomAngle = (float)(GameManager.random.NextDouble() * Math.PI * 2);
                                    GameManager.enemies[i].DashDirection = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));
                                }
                                movementDir = Vector2.Normalize(dirToPlayer * 0.4f + GameManager.enemies[i].DashDirection * 0.6f);
                                break;
                            case 7: // Lanza-Piedras (Orbitador)
                                if (distToPlayer < 250f)
                                    movementDir = -dirToPlayer; 
                                else if (distToPlayer > 350f)
                                    movementDir = dirToPlayer; 
                                else
                                    movementDir = new Vector2(-dirToPlayer.Y, dirToPlayer.X); 
                                break;
                            case 8: // Camionetica (Embestidor)
                                if (!GameManager.enemies[i].IsDashing)
                                {
                                    enemySpeed = 0; 
                                    if (GameManager.enemies[i].StateTimer > 2.0f) 
                                    {
                                        GameManager.enemies[i].IsDashing = true;
                                        GameManager.enemies[i].DashDirection = dirToPlayer; 
                                        GameManager.enemies[i].StateTimer = 0;
                                    }
                                }
                                else
                                {
                                    movementDir = GameManager.enemies[i].DashDirection; 
                                    if (GameManager.enemies[i].StateTimer > 1.0f) 
                                    {
                                        GameManager.enemies[i].IsDashing = false;
                                        GameManager.enemies[i].StateTimer = 0;
                                    }
                                }
                                break;
                        }
                        }

                        if (GameManager.DrunkTimer > 0) { movementDir = -movementDir; }
                        GameManager.enemies[i].Position += movementDir * enemySpeed * deltaTime;

                        if (movementDir != Vector2.Zero)
                        {
                            GameManager.enemies[i].FrameTimer += deltaTime;
                            if (GameManager.enemies[i].FrameTimer >= 0.15f)
                            {
                                GameManager.enemies[i].FrameTimer = 0.0f;
                                GameManager.enemies[i].CurrentFrame = (GameManager.enemies[i].CurrentFrame + 1) % 4;
                            }
                            if (Math.Abs(movementDir.X) > Math.Abs(movementDir.Y))
                                GameManager.enemies[i].CurrentRow = movementDir.X > 0 ? 2 : 3;
                            else
                                GameManager.enemies[i].CurrentRow = movementDir.Y > 0 ? 0 : 1;
                        }
                    }

                    GameManager.enemies[i].Position.X = Math.Clamp(GameManager.enemies[i].Position.X, GameManager.MapBounds.X, GameManager.MapBounds.X + GameManager.MapBounds.Width - GameManager.enemies[i].Size);
                    GameManager.enemies[i].Position.Y = Math.Clamp(GameManager.enemies[i].Position.Y, GameManager.MapBounds.Y, GameManager.MapBounds.Y + GameManager.MapBounds.Height - GameManager.enemies[i].Size);
                }
            }
        }
    }
}
