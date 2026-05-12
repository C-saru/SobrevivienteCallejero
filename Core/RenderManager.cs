using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class RenderManager
    {
        public static void Draw(ref Player player, ref Camera2D camera, ref Camera3D camera3D, ref Texture2D playerTexture, ref Texture2D menuBg, ref Texture2D titleBgTexture, Texture2D[] enemyTextures, ref Texture2D bgTexture, int screenWidth, int screenHeight)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            if (GameManager.State == GameState.StartMenu)
            {
                if (titleBgTexture.Id != 0)
                {
                    Raylib.DrawTexturePro(titleBgTexture, new Rectangle(0, 0, titleBgTexture.Width, titleBgTexture.Height), new Rectangle(0, 0, screenWidth, screenHeight), Vector2.Zero, 0.0f, Color.White);
                }

                int startAlpha = (int)(127 + 128 * Math.Cos(Raylib.GetTime() * 3.0f));
                Color startTextColor = new Color(255, 255, 255, startAlpha);
                int textWidth = Raylib.MeasureText("Pulsa ENTER para entrar al barrio", 30);
                Raylib.DrawText("Pulsa ENTER para entrar al barrio", screenWidth / 2 - textWidth / 2, screenHeight - 100, 30, startTextColor);
            }
            else if (GameManager.State == GameState.MainMenu)
            {
                if (menuBg.Id != 0) Raylib.DrawTexturePro(menuBg, new Rectangle(0, 0, menuBg.Width, menuBg.Height), new Rectangle(0, 0, screenWidth, screenHeight), Vector2.Zero, 0.0f, Color.White);

                Raylib.DrawText("SOBREVIVIENTE CALLEJERO", screenWidth / 2 - 250, 100, 40, Color.Gold);

                string[] options = { "Modo Arcade", "Modo Historia", "Desafíos", "El Kiosko", "Salir" };
                int startY = 250;
                int spacing = 50;

                for (int i = 0; i < options.Length; i++)
                {
                    Color optionColor = (i == GameManager.MenuSelection) ? Color.Yellow : Color.DarkGray;
                    string optionText = (i == GameManager.MenuSelection) ? ">> " + options[i] : options[i];
                    int textWidth = Raylib.MeasureText(optionText, 30);
                    Raylib.DrawText(optionText, screenWidth / 2 - textWidth / 2, startY + (i * spacing), 30, optionColor);
                }

                Raylib.DrawText("Controles: WASD/Flechas para mover, ESPACIO para disparar", screenWidth / 2 - 280, screenHeight - 60, 20, Color.Gray);
            }
            else if (GameManager.State == GameState.Playing || GameManager.State == GameState.LevelUpMenu || GameManager.State == GameState.GameOver || GameManager.State == GameState.GameWon)
            {
                if (player.ScreenShakeTimer > 0)
                {
                    camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f) + new Vector2(GameManager.random.Next(-5, 5), GameManager.random.Next(-5, 5));
                }
                else
                {
                    camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
                }

                if (GameManager.CurrentMode == GameMode.Arcade2D)
                {
                    Raylib.BeginMode2D(camera);

                    if (bgTexture.Id != 0)
                    {
                        int bgSize = 512;
                        for (int x = -3000; x < 3000; x += bgSize)
                        {
                            for (int y = -3000; y < 3000; y += bgSize)
                            {
                                Raylib.DrawTexturePro(bgTexture, new Rectangle(0, 0, bgTexture.Width, bgTexture.Height), new Rectangle(x, y, bgSize, bgSize), Vector2.Zero, 0.0f, new Color(100, 100, 100, 255));
                            }
                        }
                    }
                    else
                    {
                        GameManager.CurrentLevel.DrawBackground();
                    }

                    for (int i = 0; i < GameManager.puddles.Length; i++)
                    {
                        if (GameManager.puddles[i].IsActive)
                            Raylib.DrawCircleV(GameManager.puddles[i].Position, GameManager.puddles[i].Radius, new Color(139, 69, 19, 150));
                    }

                    for (int i = 0; i < GameManager.gems.Length; i++)
                    {
                        if (GameManager.gems[i].IsActive)
                            Raylib.DrawRectangleV(GameManager.gems[i].Position, new Vector2(GameManager.gems[i].Size, GameManager.gems[i].Size), Color.Green);
                    }

                    for (int i = 0; i < GameManager.pickups.Length; i++)
                    {
                        if (GameManager.pickups[i].IsActive)
                        {
                            Color pickupColor = GameManager.pickups[i].Type == 0 ? Color.Beige : Color.Blue;
                            if (GameManager.pickups[i].Type == 2) pickupColor = Color.Yellow;
                            else if (GameManager.pickups[i].Type == 3) pickupColor = new Color(139, 69, 19, 255); 
                            Raylib.DrawCircleV(GameManager.pickups[i].Position, GameManager.pickups[i].Size, pickupColor);
                        }
                    }

                    for (int i = 0; i < GameManager.enemies.Length; i++)
                    {
                        if (GameManager.enemies[i].IsActive)
                        {
                            Color enemyColor = Color.Maroon;
                            switch (GameManager.enemies[i].Type)
                            {
                                case 1: enemyColor = Color.Gold; break; 
                                case 2: enemyColor = Color.Purple; break; 
                                case 3: enemyColor = Color.DarkGray; break; 
                                case 4: enemyColor = Color.Orange; break;
                                case 5: enemyColor = Color.DarkBlue; break; 
                                case 6: enemyColor = Color.Pink; break;
                                case 7: enemyColor = new Color(0, 255, 255, 255); break; 
                                case 8: enemyColor = Color.Red; break;
                            }
                            
                            if (GameManager.enemies[i].FreezeTimer > 0) 
                                enemyColor = Color.SkyBlue;
                            else if (GameManager.enemies[i].Type == 8 && !GameManager.enemies[i].IsDashing)
                                enemyColor = ((int)(GameManager.GameTime * 10) % 2 == 0) ? Color.Red : Color.White; 
                            
                            if (GameManager.enemies[i].HitFlashTimer > 0)
                            {
                                enemyColor = Color.White;
                            }

                            Texture2D currentEnemyTex = enemyTextures[GameManager.enemies[i].Type];

                            if (currentEnemyTex.Id == 0 || currentEnemyTex.Width == 0)
                            {
                                Raylib.DrawRectangleV(GameManager.enemies[i].Position, new Vector2(GameManager.enemies[i].Size, GameManager.enemies[i].Size), enemyColor);
                            }
                            else
                            {
                                float divisorWidth = GameManager.UseStaticSprites ? 1.0f : 4.0f;
                                float divisorHeight = GameManager.UseStaticSprites ? 1.0f : 4.0f;
                                float frameWidth = currentEnemyTex.Width / divisorWidth;
                                float frameHeight = currentEnemyTex.Height / divisorHeight;
                                int frameToDraw = GameManager.UseStaticSprites ? 0 : GameManager.enemies[i].CurrentFrame;
                                int rowToDraw = GameManager.UseStaticSprites ? 0 : GameManager.enemies[i].CurrentRow;
                                Rectangle sourceRec = new Rectangle(frameToDraw * frameWidth, rowToDraw * frameHeight, frameWidth, frameHeight);
                                Rectangle destRec = new Rectangle(GameManager.enemies[i].Position.X + GameManager.enemies[i].Size / 2.0f, GameManager.enemies[i].Position.Y + GameManager.enemies[i].Size / 2.0f, GameManager.enemies[i].Size * 1.5f, GameManager.enemies[i].Size * 1.5f);
                                Vector2 origin = new Vector2(destRec.Width / 2.0f, destRec.Height / 2.0f);
                                Color drawColor = GameManager.enemies[i].FreezeTimer > 0 ? Color.SkyBlue : Color.White;
                                if (GameManager.enemies[i].HitFlashTimer > 0) drawColor = Color.White;
                                Raylib.DrawTexturePro(currentEnemyTex, sourceRec, destRec, origin, 0.0f, drawColor);
                            }
                        }
                    }

                    for (int i = 0; i < GameManager.projectiles.Length; i++)
                    {
                        if (GameManager.projectiles[i].IsActive)
                            Raylib.DrawCircleV(GameManager.projectiles[i].Position, GameManager.projectiles[i].Size, Color.LightGray);
                    }

                    for (int i = 0; i < GameManager.bouncingProjectiles.Length; i++)
                    {
                        if (GameManager.bouncingProjectiles[i].IsActive)
                            Raylib.DrawCircleV(GameManager.bouncingProjectiles[i].Position, GameManager.bouncingProjectiles[i].Size, Color.Red);
                    }

                    for (int i = 0; i < GameManager.manualProjectiles.Length; i++)
                    {
                        if (GameManager.manualProjectiles[i].IsActive)
                            Raylib.DrawCircleV(GameManager.manualProjectiles[i].Position, GameManager.manualProjectiles[i].Size, Color.Yellow);
                    }

                    if (WeaponManager.isAttacking)
                        Raylib.DrawRectangleRec(WeaponManager.attackHitbox, new Color(255, 255, 255, 150));

                    if (player.HasWhip && WeaponManager.isWhipping)
                    {
                        Raylib.DrawRectangleRec(WeaponManager.whipHitboxLeft, new Color(255, 215, 0, 200));
                        Raylib.DrawRectangleRec(WeaponManager.whipHitboxRight, new Color(255, 215, 0, 200));
                    }

                    float playerScale = 1.0f;
                    if (WeaponManager.isAttacking)
                    {
                        playerScale = 1.0f + (float)Math.Sin((WeaponManager.attackDurationTimer / 0.2f) * Math.PI) * 0.3f;
                    }

                    if (playerTexture.Id == 0 || playerTexture.Width == 0)
                    {
                        Color fallbackColor = player.DamageFlashTimer > 0 ? Color.Red : Color.Blue;
                        if (WeaponManager.isAttacking && player.DamageFlashTimer <= 0) fallbackColor = Color.Yellow;
                        
                        float scaledSize = player.Size * playerScale;
                        float offset = (scaledSize - player.Size) / 2.0f;
                        
                        Raylib.DrawRectangle((int)(player.Position.X - offset), (int)(player.Position.Y - offset), (int)scaledSize, (int)scaledSize, fallbackColor);
                    }
                    else
                    {
                        float divisorWidth = GameManager.UseStaticSprites ? 1.0f : 8.0f;
                        float divisorHeight = GameManager.UseStaticSprites ? 1.0f : 4.0f;
                        float frameWidth = playerTexture.Width / divisorWidth;
                        float frameHeight = playerTexture.Height / divisorHeight;
                        int frameToDraw = GameManager.UseStaticSprites ? 0 : player.CurrentFrame;
                        int rowToDraw = GameManager.UseStaticSprites ? 0 : player.CurrentRow;
                        
                        Rectangle sourceRec = new Rectangle(frameToDraw * frameWidth, rowToDraw * frameHeight, frameWidth, frameHeight);
                        
                        float destWidth = 128.0f * playerScale;
                        float destHeight = 128.0f * playerScale;
                        
                        Rectangle destRec = new Rectangle(player.Position.X + player.Size / 2.0f, player.Position.Y + player.Size / 2.0f, destWidth, destHeight);
                        Vector2 origin = new Vector2(destWidth / 2.0f, destHeight / 2.0f);
                        
                        Color playerDrawColor = player.DamageFlashTimer > 0 ? Color.Red : Color.White;
                        if (WeaponManager.isAttacking && player.DamageFlashTimer <= 0) 
                        {
                            playerDrawColor = new Color(255, 255, 200, 255);
                        }
                        
                        Raylib.DrawTexturePro(playerTexture, sourceRec, destRec, origin, 0.0f, playerDrawColor);
                    }

                    if (player.HasPiedrazo)
                    {
                        float maxCooldown = 0.8f * player.MacheteCooldownMult;
                        float cooldownPercentage = Math.Clamp(WeaponManager.shootTimer / maxCooldown, 0.0f, 1.0f);
                        int barWidth = 40;
                        int barHeight = 4;
                        int barX = (int)(player.Position.X + player.Size / 2.0f - barWidth / 2.0f);
                        int barY = (int)(player.Position.Y + player.Size + 5);
                        Raylib.DrawRectangle(barX, barY, barWidth, barHeight, new Color(0, 0, 0, 150));
                        Raylib.DrawRectangle(barX, barY, (int)(barWidth * cooldownPercentage), barHeight, Color.Yellow);
                    }

                    if (player.HasMachete)
                    {
                        float maxMacheteCooldown = 1.0f * player.MacheteCooldownMult;
                        float machetePercentage = Math.Clamp(WeaponManager.attackCooldownTimer / maxMacheteCooldown, 0.0f, 1.0f);
                        int barWidth = 40;
                        int barHeight = 4;
                        int barX = (int)(player.Position.X + player.Size / 2.0f - barWidth / 2.0f);
                        int barY = (int)(player.Position.Y + player.Size + 11);
                        Raylib.DrawRectangle(barX, barY, barWidth, barHeight, new Color(0, 0, 0, 150));
                        Raylib.DrawRectangle(barX, barY, (int)(barWidth * machetePercentage), barHeight, Color.White);
                    }

                    for (int i = 0; i < GameManager.damageTexts.Length; i++)
                    {
                        if (GameManager.damageTexts[i].IsActive)
                        {
                            int fontSize = 20;
                            Color textColor = Color.Red;
                            if (GameManager.damageTexts[i].Value >= 40)
                            {
                                fontSize = 30;
                                textColor = Color.Orange;
                            }
                            Raylib.DrawText(GameManager.damageTexts[i].Value.ToString(), (int)GameManager.damageTexts[i].Position.X, (int)GameManager.damageTexts[i].Position.Y, fontSize, textColor);
                        }
                    }

                    for (int i = 0; i < GameManager.particles.Length; i++)
                    {
                        if (GameManager.particles[i].IsActive)
                        {
                            Raylib.DrawRectangle((int)GameManager.particles[i].Position.X, (int)GameManager.particles[i].Position.Y, (int)GameManager.particles[i].Size, (int)GameManager.particles[i].Size, GameManager.particles[i].Color);
                        }
                    }

                    Raylib.EndMode2D();
                }
                else if (GameManager.CurrentMode == GameMode.Story3D)
                {
                    camera3D.Position = new Vector3(player.Position.X, 20.0f + (float)Math.Sin(player.HeadBobTimer) * 1.5f, player.Position.Y);
                    Vector2 lookDir = player.FacingDirection == Vector2.Zero ? new Vector2(0, 1) : Vector2.Normalize(player.FacingDirection);
                    camera3D.Target = new Vector3(player.Position.X + lookDir.X * 100.0f, 20.0f, player.Position.Y + lookDir.Y * 100.0f);

                    Raylib.BeginMode3D(camera3D);
                    
                    int viewRadius = 400;
                    int tileSize = 40;
                    int startX = (int)(player.Position.X - viewRadius) / tileSize * tileSize;
                    int endX = (int)(player.Position.X + viewRadius) / tileSize * tileSize;
                    int startY = (int)(player.Position.Y - viewRadius) / tileSize * tileSize;
                    int endY = (int)(player.Position.Y + viewRadius) / tileSize * tileSize;

                    // --- NUEVA LÓGICA DE LINTERNA PARA EL PISO ---
                    Color baseFloorColor = new Color(30, 30, 30, 255);

                    for (int x = startX; x <= endX; x += tileSize)
                    {
                        for (int y = startY; y <= endY; y += tileSize)
                        {
                            // Center of the current tile
                            Vector2 targetPos = new Vector2(x + tileSize / 2.0f, y + tileSize / 2.0f);
                            float dist = Vector2.Distance(player.Position, targetPos);
                            
                            float dotProduct = dist > 0 ? Vector2.Dot(lookDir, (targetPos - player.Position) / dist) : 1.0f;
                            float currentMaxDist = (dotProduct >= 0.866f) ? 400.0f : 120.0f;

                            if (dist < currentMaxDist)
                            {
                                float alpha = 1.0f - (dist / currentMaxDist);
                                // Modulate the color by alpha (fade out with distance), keeping the alpha channel at 255
                                Color finalFloorColor = new Color(
                                    (int)(baseFloorColor.R * alpha),
                                    (int)(baseFloorColor.G * alpha),
                                    (int)(baseFloorColor.B * alpha),
                                    255
                                );

                                // Draw flattened cube for the floor tile
                                Raylib.DrawCube(new Vector3(targetPos.X, 0, targetPos.Y), tileSize, 0.1f, tileSize, finalFloorColor);
                            }
                        }
                    }

                    // --- NUEVA LÓGICA DE LINTERNA PARA LOS ENEMIGOS ---
                    for (int i = 0; i < GameManager.enemies.Length; i++)
                    {
                        if (GameManager.enemies[i].IsActive)
                        {
                            float dist = Vector2.Distance(player.Position, GameManager.enemies[i].Position);
                            float dotProduct = dist > 0 ? Vector2.Dot(lookDir, (GameManager.enemies[i].Position - player.Position) / dist) : 1.0f;
                            float currentMaxDist = (dotProduct >= 0.866f) ? 400.0f : 120.0f;

                            if (dist > currentMaxDist) continue; 

                            float alpha = 1.0f - (dist / currentMaxDist);
                            Vector3 enemyPos3D = new Vector3(GameManager.enemies[i].Position.X, 20.0f, GameManager.enemies[i].Position.Y);

                            // Fake DOD Shadow
                            Raylib.DrawCylinder(new Vector3(GameManager.enemies[i].Position.X, 0.1f, GameManager.enemies[i].Position.Y), GameManager.enemies[i].Size * 0.6f, GameManager.enemies[i].Size * 0.6f, 0.1f, 10, new Color(0, 0, 0, (int)(180 * alpha)));

                            Texture2D tex = enemyTextures[GameManager.enemies[i].Type];
                            
                            Color baseColor = GameManager.enemies[i].FreezeTimer > 0 ? Color.SkyBlue : Color.White;
                            if (GameManager.enemies[i].HitFlashTimer > 0) baseColor = Color.White;
                            
                            // AQUI REPARAMOS LA AMBIGUEDAD
                            Color fogColor = new Color((int)(baseColor.R * alpha), (int)(baseColor.G * alpha), (int)(baseColor.B * alpha), (int)255);

                            if (tex.Id != 0)
                            {
                                float divisorWidth = GameManager.UseStaticSprites ? 1.0f : 4.0f;
                                float divisorHeight = GameManager.UseStaticSprites ? 1.0f : 4.0f;
                                float fWidth = tex.Width / divisorWidth;
                                float fHeight = tex.Height / divisorHeight;

                                int frameToDraw = GameManager.UseStaticSprites ? 0 : GameManager.enemies[i].CurrentFrame;
                                int rowToDraw = GameManager.UseStaticSprites ? 0 : GameManager.enemies[i].CurrentRow;
                                Rectangle source = new Rectangle(frameToDraw * fWidth, rowToDraw * fHeight, fWidth, fHeight);
                                
                                Raylib.DrawBillboardRec(camera3D, tex, source, enemyPos3D, new Vector2(GameManager.enemies[i].Size * 1.5f, GameManager.enemies[i].Size * 1.5f), fogColor);
                            }
                            else
                            {
                                // AQUI REPARAMOS LA AMBIGUEDAD
                                Color solidColor = GameManager.enemies[i].HitFlashTimer > 0 ? Color.White : Color.Red;
                                Color cubeColor = new Color((int)(solidColor.R * alpha), (int)(solidColor.G * alpha), (int)(solidColor.B * alpha), (int)255);
                                Raylib.DrawCube(enemyPos3D, GameManager.enemies[i].Size, GameManager.enemies[i].Size, GameManager.enemies[i].Size, cubeColor);
                            }
                        }
                    }

                    // --- DIBUJAR PROYECTILES EN 3D ---
                    // Balas 3D ahora a baja altura sobre el piso para que sean visibles desde la cámara en 3D
                    for (int i = 0; i < GameManager.projectiles.Length; i++)
                    {
                        if (GameManager.projectiles[i].IsActive)
                        {
                            Vector3 projPos = new Vector3(GameManager.projectiles[i].Position.X, 6.0f, GameManager.projectiles[i].Position.Y);
                            Raylib.DrawSphere(projPos, MathF.Max(GameManager.projectiles[i].Size, 4.0f), Color.Yellow);
                            Raylib.DrawSphereWires(projPos, MathF.Max(GameManager.projectiles[i].Size, 4.0f), 8, 8, Color.Gold);
                        }
                    }

                    for (int i = 0; i < GameManager.bouncingProjectiles.Length; i++)
                    {
                        if (GameManager.bouncingProjectiles[i].IsActive)
                        {
                            Vector3 projPos = new Vector3(GameManager.bouncingProjectiles[i].Position.X, 6.0f, GameManager.bouncingProjectiles[i].Position.Y);
                            float radius = MathF.Max(GameManager.bouncingProjectiles[i].Size, 5.0f);
                            Raylib.DrawSphere(projPos, radius, Color.SkyBlue);
                            Raylib.DrawSphereWires(projPos, radius, 10, 10, Color.Blue);
                        }
                    }

                    for (int i = 0; i < GameManager.manualProjectiles.Length; i++)
                    {
                        if (GameManager.manualProjectiles[i].IsActive)
                        {
                            Vector3 projPos = new Vector3(GameManager.manualProjectiles[i].Position.X, 6.0f, GameManager.manualProjectiles[i].Position.Y);
                            float radius = MathF.Max(GameManager.manualProjectiles[i].Size, 5.0f);
                            Raylib.DrawSphere(projPos, radius, new Color(255, 160, 0, 255));
                            Raylib.DrawSphereWires(projPos, radius, 10, 10, Color.Orange);
                        }
                    }

                    // --- OBSTÁCULOS 3D ---
                    for (int i = 0; i < GameManager.obstacles.Length; i++)
                    {
                        if (GameManager.obstacles[i].IsActive)
                        {
                            if (GameManager.obstacles[i].Type == 1) // Hueco
                            {
                                Vector3 obsPos = new Vector3(GameManager.obstacles[i].Rect.X + GameManager.obstacles[i].Rect.Width / 2, 0.1f, GameManager.obstacles[i].Rect.Y + GameManager.obstacles[i].Rect.Height / 2);
                                float radius = MathF.Max(GameManager.obstacles[i].Rect.Width, GameManager.obstacles[i].Rect.Height) / 2.0f;
                                Raylib.DrawCylinder(obsPos, radius, radius, 0.5f, 16, new Color(20, 20, 20, 200));
                            }
                            else if (GameManager.obstacles[i].Type == 0) // Carrito
                            {
                                Vector3 obsPos = new Vector3(GameManager.obstacles[i].Rect.X + GameManager.obstacles[i].Rect.Width / 2, 20.0f, GameManager.obstacles[i].Rect.Y + GameManager.obstacles[i].Rect.Height / 2);
                                Raylib.DrawCube(obsPos, GameManager.obstacles[i].Rect.Width, 40.0f, GameManager.obstacles[i].Rect.Height, new Color(139, 69, 19, 255));
                            }
                        }
                    }

                    // --- MUROS DEL ESTACIONAMIENTO 3D ---
                    float wallHeight = 100.0f;
                    float wallThickness = 50.0f;
                    Rectangle bounds = GameManager.MapBounds;
                    
                    Raylib.DrawCube(new Vector3(bounds.X + bounds.Width / 2, wallHeight / 2, bounds.Y - wallThickness / 2), bounds.Width + wallThickness * 2, wallHeight, wallThickness, Color.DarkGray); // Norte
                    Raylib.DrawCube(new Vector3(bounds.X + bounds.Width / 2, wallHeight / 2, bounds.Y + bounds.Height + wallThickness / 2), bounds.Width + wallThickness * 2, wallHeight, wallThickness, Color.DarkGray); // Sur
                    Raylib.DrawCube(new Vector3(bounds.X + bounds.Width + wallThickness / 2, wallHeight / 2, bounds.Y + bounds.Height / 2), wallThickness, wallHeight, bounds.Height, Color.DarkGray); // Este
                    Raylib.DrawCube(new Vector3(bounds.X - wallThickness / 2, wallHeight / 2, bounds.Y + bounds.Height / 2), wallThickness, wallHeight, bounds.Height, Color.DarkGray); // Oeste

                    for (int i = 0; i < GameManager.particles.Length; i++)
                    {
                        if (GameManager.particles[i].IsActive)
                        {
                            Raylib.DrawCube(new Vector3(GameManager.particles[i].Position.X, 1.0f, GameManager.particles[i].Position.Y), GameManager.particles[i].Size, GameManager.particles[i].Size, GameManager.particles[i].Size, GameManager.particles[i].Color);
                        }
                    }

                    Raylib.EndMode3D();
                    DrawRadar(camera3D, screenWidth, GameManager.enemies);
                }

                DrawScreenEffects(screenWidth, screenHeight, ref player);
                
                // --- DEV MODE VISUAL (TAREA 1 y 2) ---
                if (GameManager.DevMode)
                {
                    Raylib.DrawText("DEV MODE ON", 20, 60, 20, Color.Magenta);
                }

                int minutes = (int)GameManager.GameTime / 60;
                int seconds = (int)GameManager.GameTime % 60;
                string timeText = $"{minutes:D2}:{seconds:D2}";
                int textWidth = Raylib.MeasureText(timeText, 30);
                Raylib.DrawText(timeText, (screenWidth - textWidth) / 2, 20, 30, Color.White);

                // Dificultad actual del spawn y nivel movidos a esquinas separadas
                Raylib.DrawText($"NIVEL: {player.Level}", 20, 20, 20, Color.Gold);
                string difficultyText = $"Dificultad: x{GameManager.DifficultyMultiplier:0.0}";
                Raylib.DrawText(difficultyText, screenWidth - Raylib.MeasureText(difficultyText, 20) - 20, 20, 20, Color.Yellow);

                Raylib.DrawRectangle(10, 40, 220, 140, new Color(0, 0, 0, 180));
                Raylib.DrawText("ARSENAL:", 20, 50, 15, Color.Gold);
                Raylib.DrawText("1. Piedrazo", 20, 75, 15, player.HasPiedrazo ? Color.White : Color.DarkGray);
                Raylib.DrawText("2. Machetazo", 20, 95, 15, player.HasMachete ? Color.White : Color.DarkGray);
                Raylib.DrawText("3. Halls Rebotador", 20, 115, 15, player.HasHalls ? Color.White : Color.DarkGray);
                Raylib.DrawText("4. Cadena Dorada", 20, 135, 15, player.HasWhip ? Color.White : Color.DarkGray);
                Raylib.DrawText("5. Charco", 20, 155, 15, player.HasPuddle ? Color.White : Color.DarkGray);

                float healthPercentage = Math.Max(player.Health / player.MaxHealth, 0);
                Raylib.DrawRectangle(10, screenHeight - 40, screenWidth - 20, 15, Color.DarkGray);
                Raylib.DrawRectangle(10, screenHeight - 40, (int)((screenWidth - 20) * healthPercentage), 15, Color.Red);
                
                float xpPercentage = (float)player.XP / player.MaxXP;
                Raylib.DrawRectangle(10, screenHeight - 20, screenWidth - 20, 10, Color.DarkGray);
                Raylib.DrawRectangle(10, screenHeight - 20, (int)((screenWidth - 20) * xpPercentage), 10, Color.SkyBlue);

                float staminaPercentage = Math.Max(player.Stamina / player.MaxStamina, 0);
                Raylib.DrawRectangle(10, screenHeight - 60, screenWidth - 20, 10, Color.DarkGray);
                Raylib.DrawRectangle(10, screenHeight - 60, (int)((screenWidth - 20) * staminaPercentage), 10, Color.Green);

                Raylib.DrawText($"NIVEL: {player.Level}", 10, 10, 20, Color.Gold);
                Raylib.DrawFPS(screenWidth - 100, 10);

                // --- BARRA DE VIDA DEL JEFE ---
                for (int i = 0; i < GameManager.enemies.Length; i++)
                {
                    if (GameManager.enemies[i].IsActive && GameManager.enemies[i].Type == 3)
                    {
                        float bossHealthPct = Math.Max(GameManager.enemies[i].Health / GameConfig.Enemies[3].Health, 0);
                        int barWidth = screenWidth - 400;
                        int barHeight = 30;
                        int barX = (screenWidth - barWidth) / 2;
                        int barY = 60;

                        Raylib.DrawRectangle(barX - 5, barY - 5, barWidth + 10, barHeight + 10, Color.Black);
                        Raylib.DrawRectangle(barX, barY, barWidth, barHeight, Color.DarkGray);
                        Raylib.DrawRectangle(barX, barY, (int)(barWidth * bossHealthPct), barHeight, new Color(220, 20, 60, 255)); // Rojo Carmesí

                        string bossName = "HEISENBERG";
                        int nameWidth = Raylib.MeasureText(bossName, 20);
                        Raylib.DrawText(bossName, screenWidth / 2 - nameWidth / 2, barY + 5, 20, Color.White);
                        break;
                    }
                }

                if (GameManager.State == GameState.LevelUpMenu)
                {
                    Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 200));
                    float titleScale = 1.0f + 0.05f * (float)Math.Sin(Raylib.GetTime() * 5.0f);
                    int titleFontSize = (int)(40 * titleScale);
                    int titleWidth = Raylib.MeasureText("¡NIVEL AUMENTADO!", titleFontSize);
                    Raylib.DrawText("¡NIVEL AUMENTADO!", screenWidth / 2 - titleWidth / 2, screenHeight / 2 - 200, titleFontSize, Color.Gold);
                    
                    string subtitle = "Elige una mejora:";
                    int subWidth = Raylib.MeasureText(subtitle, 20);
                    Raylib.DrawText(subtitle, screenWidth / 2 - subWidth / 2, screenHeight / 2 - 140, 20, Color.White);

                    int baseCardWidth = 300;
                    int baseCardHeight = 180;
                    int cardSpacing = 40;
                    int totalWidth = (baseCardWidth * 3) + (cardSpacing * 2);
                    int startX = (screenWidth - totalWidth) / 2;
                    int cardY = screenHeight / 2 - baseCardHeight / 2 + 20;

                    Vector2 mousePos = Raylib.GetMousePosition();

                    for (int i = 0; i < GameManager.currentUpgrades.Length; i++)
                    {
                        Rectangle baseRec = new Rectangle(startX + i * (baseCardWidth + cardSpacing), cardY, baseCardWidth, baseCardHeight);
                        bool isHovering = Raylib.CheckCollisionPointRec(mousePos, baseRec);

                        float hoverOffset = isHovering ? 15.0f : 0.0f;
                        float floatOffset = (float)Math.Sin(Raylib.GetTime() * 3.0f + i) * 5.0f;
                        Rectangle cardRec = new Rectangle(
                            baseRec.X - (isHovering ? 12 : 0), 
                            baseRec.Y - hoverOffset + floatOffset, 
                            baseRec.Width + (isHovering ? 24 : 0), 
                            baseRec.Height + (isHovering ? 24 : 0)
                        );

                        int shadowOffset = isHovering ? 18 : 10;
                        Color shadowColor = isHovering ? new Color(0, 0, 0, 200) : new Color(0, 0, 0, 120);
                        Raylib.DrawRectangleRec(new Rectangle(cardRec.X + shadowOffset, cardRec.Y + shadowOffset, cardRec.Width, cardRec.Height), shadowColor);

                        Color cardBg = isHovering ? new Color(60, 60, 90, 255) : new Color(30, 30, 50, 255);
                        Color borderColor = isHovering ? Color.Gold : new Color(80, 80, 100, 255);

                        Raylib.DrawRectangleRec(cardRec, cardBg);
                        Raylib.DrawRectangleLinesEx(cardRec, isHovering ? 4 : 2, borderColor);

                        string title = GameManager.currentUpgrades[i].Title ?? "Mejora";
                        int tFontSize = isHovering ? 24 : 22;
                        int tWidth = Raylib.MeasureText(title, tFontSize);
                        int titleY = (int)(cardRec.Y + (isHovering ? 25 : 30));
                        Color titleColor = isHovering ? Color.Gold : new Color(218, 165, 32, 210);
                        Raylib.DrawText(title, (int)(cardRec.X + (cardRec.Width - tWidth) / 2), titleY, tFontSize, titleColor);

                        string desc = GameManager.currentUpgrades[i].Description ?? "";
                        int dWidth = Raylib.MeasureText(desc, 16);
                        int descY = (int)(cardRec.Y + (isHovering ? 75 : 80));
                        Color descColor = isHovering ? Color.White : new Color(200, 200, 200, 180);
                        Raylib.DrawText(desc, (int)(cardRec.X + (cardRec.Width - dWidth) / 2), descY, 16, descColor);

                        if (isHovering)
                        {
                            // AQUI REPARAMOS LA AMBIGUEDAD
                            int textAlpha = (int)(155 + 100 * Math.Sin(Raylib.GetTime() * 10.0f));
                            int clickWidth = Raylib.MeasureText(">> CLICK <<", 18);
                            Raylib.DrawText(">> CLICK <<", (int)(cardRec.X + (cardRec.Width - clickWidth) / 2), (int)(cardRec.Y + cardRec.Height - 40), 18, new Color((int)255, (int)255, (int)0, textAlpha));
                        }
                    }
                }
                else if (GameManager.State == GameState.GameOver)
                {
                    Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 200));
                    Raylib.DrawText("GAME OVER", screenWidth / 2 - 150, screenHeight / 2 - 50, 50, Color.Red);
                    Raylib.DrawText("Presiona ENTER para reiniciar", screenWidth / 2 - 180, screenHeight / 2 + 20, 20, Color.White);
                }
                else if (GameManager.State == GameState.GameWon)
                {
                    Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 200));
                    Raylib.DrawText("¡HAS SOBREVIVIDO!", screenWidth / 2 - 200, screenHeight / 2 - 50, 40, Color.Gold);
                    Raylib.DrawText("Presiona ENTER para jugar de nuevo", screenWidth / 2 - 180, screenHeight / 2 + 20, 20, Color.White);
                }
            }
            else if (GameManager.State == GameState.StoreMenu)
            {
                Raylib.DrawRectangle(0, 0, screenWidth, screenHeight, new Color(0, 0, 0, 255));
                if (menuBg.Id != 0) Raylib.DrawTexturePro(menuBg, new Rectangle(0, 0, menuBg.Width, menuBg.Height), new Rectangle(0, 0, screenWidth, screenHeight), Vector2.Zero, 0.0f, Color.White);
                Raylib.DrawText("PUESTO DE DULCES Y BISUTERÍA", screenWidth / 2 - 250, 50, 30, Color.Gold);
                Raylib.DrawText("Monedas Disponibles: " + GameManager.GlobalCoins, screenWidth / 2 - 150, 120, 20, Color.White);
                Raylib.DrawText("1. Comprar Paquete de Caramelos (+10 Vida Máxima Inicial) - 50 Monedas", screenWidth / 2 - 350, 200, 20, Color.Yellow);
                Raylib.DrawText("2. Comprar Zapatos de Goma (+5 Velocidad Base) - 100 Monedas", screenWidth / 2 - 350, 240, 20, Color.Yellow);
                Raylib.DrawText("3. Comprar Imán de Nevera (+5 Radio Imán Base) - 100 Monedas", screenWidth / 2 - 350, 280, 20, Color.Yellow);
                Raylib.DrawText("Presiona ESC para volver al menú", screenWidth / 2 - 180, screenHeight - 50, 20, Color.Gray);
            }

            Raylib.EndDrawing();
        }

        public static void DrawScreenEffects(int screenWidth, int screenHeight, ref Player player)
        {
            Color vignetteColor = new Color(0, 0, 0, 180);
            Color transparentColor = new Color(0, 0, 0, 0);

            if (player.MaxHealth > 0 && (player.Health / player.MaxHealth) < 0.3f)
            {
                float pulse = (float)(Math.Sin(Raylib.GetTime() * 10.0) * 0.5 + 0.5);
                
                // AQUI REPARAMOS LA AMBIGUEDAD
                int pulseAlpha = (int)(100 + pulse * 100); 
                vignetteColor = new Color((int)150, (int)0, (int)0, pulseAlpha); 
                transparentColor = new Color((int)150, (int)0, (int)0, (int)0);
            }

            Raylib.DrawRectangleGradientV(0, 0, screenWidth, 120, vignetteColor, transparentColor);
            Raylib.DrawRectangleGradientV(0, screenHeight - 120, screenWidth, 120, transparentColor, vignetteColor);
            Raylib.DrawRectangleGradientH(0, 0, 120, screenHeight, vignetteColor, transparentColor);
            Raylib.DrawRectangleGradientH(screenWidth - 120, 0, 120, screenHeight, transparentColor, vignetteColor);

            for (int y = 0; y < screenHeight; y += 4)
            {
                Raylib.DrawLine(0, y, screenWidth, y, new Color(0, 0, 0, 25));
            }
        }

        public static void DrawRadar(Camera3D camera, int screenWidth, Enemy[] enemies, float maxRadarDist = 1000.0f)
        {
            float radarRadius = 70.0f;
            Vector2 radarCenter = new Vector2(screenWidth - radarRadius - 20, radarRadius + 20);
            Raylib.DrawCircleV(radarCenter, radarRadius, new Color(0, 0, 0, 120));
            Raylib.DrawCircleLines((int)radarCenter.X, (int)radarCenter.Y, radarRadius, new Color(255, 255, 255, 100));
            Raylib.DrawCircleV(radarCenter, 4.0f, Color.Green);

            float camDx = camera.Target.X - camera.Position.X;
            float camDz = camera.Target.Z - camera.Position.Z;
            float camAngle = (float)Math.Atan2(camDz, camDx);

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].IsActive)
                {
                    float dx = enemies[i].Position.X - camera.Position.X;
                    float dy = enemies[i].Position.Y - camera.Position.Z; 
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (dist <= maxRadarDist)
                    {
                        float enemyAngle = (float)Math.Atan2(dy, dx);
                        float radarAngle = enemyAngle - camAngle - (float)(Math.PI / 2.0);
                        float radarDist = (dist / maxRadarDist) * radarRadius;
                        Vector2 dotPos = new Vector2(
                            radarCenter.X + (float)Math.Cos(radarAngle) * radarDist,
                            radarCenter.Y + (float)Math.Sin(radarAngle) * radarDist
                        );
                        Raylib.DrawCircleV(dotPos, 3.0f, Color.Red);
                    }
                }
            }
        }
    }
}
