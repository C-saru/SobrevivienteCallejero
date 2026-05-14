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
                if (titleBgTexture.Id != 0) Raylib.DrawTexturePro(titleBgTexture, new Rectangle(0, 0, titleBgTexture.Width, titleBgTexture.Height), new Rectangle(0, 0, screenWidth, screenHeight), Vector2.Zero, 0.0f, new Color(150, 150, 150, 255));
                
                // Animación Logo cayendo
                float logoY = (float)Math.Min(screenHeight / 2 - 150, GameManager.GameTime * 800 - 400); 
                if (logoY < screenHeight / 2 - 150) GameManager.GameTime += Raylib.GetFrameTime(); // Pequeño hack visual inicial
                
                float logoScale = 1.0f + 0.05f * (float)Math.Sin(Raylib.GetTime() * 4.0f);
                int titleFontSize = (int)(60 * logoScale);
                int titleWidth = Raylib.MeasureText("SOBREVIVIENTE CALLEJERO", titleFontSize);
                
                // Sombra del texto
                Raylib.DrawText("SOBREVIVIENTE CALLEJERO", screenWidth / 2 - titleWidth / 2 + 5, (int)logoY + 5, titleFontSize, Color.Black);
                Raylib.DrawText("SOBREVIVIENTE CALLEJERO", screenWidth / 2 - titleWidth / 2, (int)logoY, titleFontSize, Color.Gold);
                
                // Texto parpadeante (Alpha dinámico)
                int startAlpha = (int)(155 + 100 * Math.Sin(Raylib.GetTime() * 6.0f));
                Raylib.DrawText("Usa Flechas/WS para seleccionar, ENTER para confirmar", screenWidth / 2 - 280, screenHeight / 2 - 10, 20, new Color(255, 255, 255, startAlpha));
                
                // Menú opciones
                string[] options = { "Modo Arcade", "Modo Historia", "Desafíos", "Minijuegos", "El Kiosko", "Salir" };
                int menuStartY = screenHeight / 2 + 30;
                Raylib.DrawRectangle(screenWidth / 2 - 300, menuStartY - 10, 600, 230, new Color(0, 0, 0, 200));
                Raylib.DrawRectangleLines(screenWidth / 2 - 300, menuStartY - 10, 600, 230, Color.Gold);

                for (int i = 0; i < options.Length; i++)
                {
                    Color optionColor = (i == GameManager.MenuSelection) ? Color.Gold : Color.LightGray;
                    string prefix = (i == GameManager.MenuSelection) ? ">> " : "   ";
                    string text = prefix + options[i];
                    int optWidth = Raylib.MeasureText(text, 22);
                    Raylib.DrawText(text, screenWidth / 2 - optWidth / 2, menuStartY + 10 + i * 30, 22, optionColor);
                }

                Raylib.DrawText("Controles: WASD/Flechas mover, ESPACIO disparar", screenWidth / 2 - 230, menuStartY + 200, 18, Color.Gray);
                Raylib.DrawText("[ M ] Modo: " + GameManager.CurrentMode.ToString() + "  |  [ F ] Sprites: " + (GameManager.UseStaticSprites ? "OFF" : "ON"), screenWidth / 2 - 190, menuStartY + 245, 16, Color.DarkGray);
            }
            else if (GameManager.State == GameState.ChallengesMenu)
            {
                Raylib.ClearBackground(Color.Black);
                Raylib.DrawText("SELECCIONA UN DESAFÍO", screenWidth / 2 - 220, 150, 40, Color.Red);
                
                string[] challOptions = { "1. Vida de Cristal (Un golpe = Muerte)", "2. Pacifista (Sin armas nuevas)", "3. Volver al Menú" };
                for (int i = 0; i < challOptions.Length; i++)
                {
                    Color cColor = (i == GameManager.ChallengeMenuSelection) ? Color.Gold : Color.LightGray;
                    string text = ((i == GameManager.ChallengeMenuSelection) ? ">> " : "   ") + challOptions[i];
                    int optWidth = Raylib.MeasureText(text, 24);
                    Raylib.DrawText(text, screenWidth / 2 - optWidth / 2, 300 + i * 50, 24, cColor);
                }
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
                        // Optimización Zero-GC: Reutilizamos instancias de Rectangle y Color sacándolas del bucle anidado
                        Rectangle sourceRect = new Rectangle(0, 0, bgTexture.Width, bgTexture.Height);
                        Rectangle destRect = new Rectangle(0, 0, bgSize, bgSize);
                        Color tint = new Color(100, 100, 100, 255);

                        for (int x = -3000; x < 3000; x += bgSize)
                        {
                            for (int y = -3000; y < 3000; y += bgSize)
                            {
                                destRect.X = x;
                                destRect.Y = y;
                                Raylib.DrawTexturePro(bgTexture, sourceRect, destRect, Vector2.Zero, 0.0f, tint);
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
                            
                            // Hit Flash logic
                            if (GameManager.enemies[i].Health < GameConfig.Enemies[GameManager.enemies[i].Type].Health)
                            {
                                // We don't have a specific hit timer per enemy yet, so we use a quick flash based on health loss 
                                // Alternatively, we can check if Health is very close to MaxHealth (meaning it hasn't taken damage)
                                // or add a simple flash when taking damage.
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

                    for (int i = 0; i < GameManager.particles.Length; i++)
                    {
                        if (GameManager.particles[i].IsActive)
                        {
                            // Hacemos que se desvanezcan con el tiempo
                            byte alpha = (byte)(255 * (GameManager.particles[i].LifeTime / GameManager.particles[i].MaxLifeTime));
                            Color pColor = GameManager.particles[i].Color;
                            pColor.A = alpha;
                            Raylib.DrawRectangle((int)GameManager.particles[i].Position.X, (int)GameManager.particles[i].Position.Y, (int)GameManager.particles[i].Size, (int)GameManager.particles[i].Size, pColor);
                        }
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
                            Raylib.DrawText(GameManager.damageTexts[i].Value.ToString(), (int)GameManager.damageTexts[i].Position.X, (int)GameManager.damageTexts[i].Position.Y, 20, Color.Red);
                    }

                    Raylib.EndMode2D();
                }
                else if (GameManager.CurrentMode == GameMode.Story3D)
                {
                    // Head-bobbing effect
                    float bobbingAmount = 0.0f;
                    if (player.SpeedMult > 0 && player.FacingDirection != Vector2.Zero)
                    {
                        bobbingAmount = (float)Math.Sin(GameManager.GameTime * 15.0f) * 2.0f;
                        if (player.IsSprinting) bobbingAmount *= 2.0f;
                    }
                    
                    camera3D.Position = new Vector3(player.Position.X, 20.0f + bobbingAmount, player.Position.Y);
                    Vector2 lookDir = player.FacingDirection == Vector2.Zero ? new Vector2(0, 1) : Vector2.Normalize(player.FacingDirection);
                    camera3D.Target = new Vector3(player.Position.X + lookDir.X * 100.0f, 20.0f + bobbingAmount, player.Position.Y + lookDir.Y * 100.0f);

                    Raylib.BeginMode3D(camera3D);
                    
                    int viewRadius = 400;
                    int tileSize = 40;
                    int startX = (int)(player.Position.X - viewRadius) / tileSize * tileSize;
                    int endX = (int)(player.Position.X + viewRadius) / tileSize * tileSize;
                    int startY = (int)(player.Position.Y - viewRadius) / tileSize * tileSize;
                    int endY = (int)(player.Position.Y + viewRadius) / tileSize * tileSize;

                    // --- NUEVA LÓGICA DE LINTERNA PARA EL PISO ---
                    Color floorColor = new Color(0, 100, 0, 255);
                    if (GameManager.DifficultyMultiplier >= 3.0f) floorColor = Color.Red;
                    else if (GameManager.DifficultyMultiplier >= 2.0f) floorColor = Color.Orange;

                    // Optimización Zero-GC: Reutilizamos Color y calculamos la distancia al cuadrado en el bucle de piso 3D
                    Color finalLineColor = floorColor;

                    for (int x = startX; x <= endX; x += tileSize)
                    {
                        for (int y = startY; y <= endY; y += tileSize)
                        {
                            Vector2 targetPos = new Vector2(x, y);
                            float distSquared = Vector2.DistanceSquared(player.Position, targetPos);
                            float dist = (float)Math.Sqrt(distSquared); // Fallback a Sqrt solo si es necesario para Alpha
                            
                            float dotProduct = dist > 0 ? Vector2.Dot(lookDir, (targetPos - player.Position) / dist) : 1.0f;
                            float currentMaxDist = (dotProduct >= 0.866f) ? 400.0f : 120.0f;

                            // Si es evento de oscuridad, limitamos brutalmente la visión
                            if (Level1_Parking.IsDarknessEvent) 
                            {
                                currentMaxDist *= 0.4f; 
                            }

                            if (distSquared < currentMaxDist * currentMaxDist)
                            {
                                float alpha = 1.0f - (dist / currentMaxDist);
                                finalLineColor.A = (byte)(255 * alpha); // Mutamos la propiedad A en lugar de instanciar un new Color
                                Raylib.DrawLine3D(new Vector3(x, 0, y), new Vector3(x + tileSize, 0, y), finalLineColor);
                                Raylib.DrawLine3D(new Vector3(x, 0, y), new Vector3(x, 0, y + tileSize), finalLineColor);
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

                            // Si es evento de oscuridad, limitamos brutalmente la visión
                            if (Level1_Parking.IsDarknessEvent) 
                            {
                                currentMaxDist *= 0.4f; 
                            }

                            if (dist > currentMaxDist) continue; 

                            float alpha = 1.0f - (dist / currentMaxDist);
                            Vector3 enemyPos3D = new Vector3(GameManager.enemies[i].Position.X, 20.0f, GameManager.enemies[i].Position.Y);
                            Texture2D tex = enemyTextures[GameManager.enemies[i].Type];
                            
                            Color baseColor = GameManager.enemies[i].FreezeTimer > 0 ? Color.SkyBlue : Color.White;
                            
                            // Color de niebla (fog) para los enemigos, se mezclan con negro a lo lejos
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
                                
                                // Sombra circular proyectada debajo del enemigo (mejora la inmersión)
                                Raylib.DrawCircle3D(new Vector3(enemyPos3D.X, 0.5f, enemyPos3D.Z), GameManager.enemies[i].Size * 0.8f, new Vector3(1, 0, 0), 90.0f, new Color(0, 0, 0, (int)(100 * alpha)));
                                
                                Raylib.DrawBillboardRec(camera3D, tex, source, enemyPos3D, new Vector2(GameManager.enemies[i].Size * 1.5f, GameManager.enemies[i].Size * 1.5f), fogColor);
                            }
                            else
                            {
                                Color cubeColor = new Color((int)(Color.Red.R * alpha), (int)0, (int)0, (int)255);
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
                if (menuBg.Id != 0) Raylib.DrawTexturePro(menuBg, new Rectangle(0, 0, menuBg.Width, menuBg.Height), new Rectangle(0, 0, screenWidth, screenHeight), Vector2.Zero, 0.0f, new Color(100, 100, 100, 255)); // Fondo oscurecido
                
                // Animación de título
                float titleScale = 1.0f + 0.05f * (float)Math.Sin(Raylib.GetTime() * 3.0f);
                int titleFontSize = (int)(40 * titleScale);
                int titleWidth = Raylib.MeasureText("EL KIOSKO DE MEJORAS", titleFontSize);
                Raylib.DrawText("EL KIOSKO DE MEJORAS", screenWidth / 2 - titleWidth / 2, 40, titleFontSize, Color.Gold);
                
                Raylib.DrawText("Monedas Disponibles: " + GameManager.GlobalCoins, screenWidth / 2 - 150, 100, 24, Color.Green);

                // Cuadro de Tienda
                Rectangle storeBox = new Rectangle(screenWidth / 2 - 400, 150, 800, 350);
                Raylib.DrawRectangleRec(storeBox, new Color(20, 20, 30, 220));
                Raylib.DrawRectangleLinesEx(storeBox, 4, Color.Gold);

                // Opciones de compra
                int startY = 180;
                int lineSpacing = 60;

                // Opción 1: Vida
                Color c1 = GameManager.GlobalCoins >= 50 ? Color.White : Color.DarkGray;
                Raylib.DrawText("[1] Piel de Caimán (+20 Vida Base) - 50 Monedas", (int)storeBox.X + 30, startY, 20, c1);
                Raylib.DrawText("Nivel Actual: " + (GameManager.BonusHealth / 20), (int)storeBox.X + 600, startY, 20, Color.SkyBlue);

                // Opción 2: Velocidad
                Color c2 = GameManager.GlobalCoins >= 100 ? Color.White : Color.DarkGray;
                Raylib.DrawText("[2] Suela Gastada (+15 Velocidad Base) - 100 Monedas", (int)storeBox.X + 30, startY + lineSpacing, 20, c2);
                Raylib.DrawText("Nivel Actual: " + (GameManager.BonusSpeed / 15), (int)storeBox.X + 600, startY + lineSpacing, 20, Color.SkyBlue);

                // Opción 3: Daño Látigo
                Color c3 = GameManager.GlobalCoins >= 150 ? Color.White : Color.DarkGray;
                Raylib.DrawText("[3] Pesa de Cemento (+10% Daño Látigo) - 150 Monedas", (int)storeBox.X + 30, startY + lineSpacing * 2, 20, c3);
                Raylib.DrawText("Nivel Actual: " + GameManager.BonusDamage, (int)storeBox.X + 600, startY + lineSpacing * 2, 20, Color.SkyBlue);

                // Botón volver
                int flashAlpha = (int)(155 + 100 * Math.Sin(Raylib.GetTime() * 5.0f));
                Raylib.DrawText("Presiona ESC para volver al menú", screenWidth / 2 - 180, screenHeight - 60, 20, new Color(200, 200, 200, flashAlpha));
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
                // Mutamos las propiedades en lugar de asignar new Color
                vignetteColor.R = 150; vignetteColor.G = 0; vignetteColor.B = 0; vignetteColor.A = (byte)pulseAlpha;
                transparentColor.R = 150; transparentColor.G = 0; transparentColor.B = 0; transparentColor.A = 0;
            }

            Raylib.DrawRectangleGradientV(0, 0, screenWidth, 120, vignetteColor, transparentColor);
            Raylib.DrawRectangleGradientV(0, screenHeight - 120, screenWidth, 120, transparentColor, vignetteColor);
            Raylib.DrawRectangleGradientH(0, 0, 120, screenHeight, vignetteColor, transparentColor);
            Raylib.DrawRectangleGradientH(screenWidth - 120, 0, 120, screenHeight, transparentColor, vignetteColor);

            // Optimización Zero-GC: Reutilizamos la instancia de color de línea de escaneo
            Color scanlineColor = new Color(0, 0, 0, 25);
            for (int y = 0; y < screenHeight; y += 4)
            {
                Raylib.DrawLine(0, y, screenWidth, y, scanlineColor);
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
