using System;
using System.Numerics;
using System.IO;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class GameManager
    {
        public static GameState State = GameState.StartMenu;
        public static GameMode CurrentMode = GameMode.Arcade2D;
        public static bool UseStaticSprites = true;
        public static ILevel CurrentLevel = null!;
        public static float GameTime = 0.0f;
        public static float spawnTimer = 0.0f;
        public static int MenuSelection = 0;
        public static bool QuitGame = false;
        public static bool IsStoryMode = false;
        public static int ActiveChallenge = 0;
        public static int ChallengeMenuSelection = 0;
        public static float spawnInterval = 2.0f;
        public static Random random = new Random();
        public static bool BossSpawned = false;

        public static Enemy[] enemies = new Enemy[500];
        public static Gem[] gems = new Gem[1000];
        public static Projectile[] projectiles = new Projectile[200];
        public static BouncingProjectile[] bouncingProjectiles = new BouncingProjectile[50];
        public static Puddle[] puddles = new Puddle[20];
        public static Projectile[] manualProjectiles = new Projectile[100];
        public static DamageText[] damageTexts = new DamageText[100];
        public static Obstacle[] obstacles = new Obstacle[30];
        public static Pickup[] pickups = new Pickup[50];

        public static float HitStopTimer = 0.0f;
        public static float DrunkTimer = 0.0f;
        public static Particle[] particles = new Particle[1000];
        public static int GlobalCoins = 0;
        public static int BonusHealth = 0;
        public static int BonusSpeed = 0;
        public static int BonusMagnet = 0;

        public static Upgrade[] currentUpgrades = new Upgrade[3];
        public static Rectangle MapBounds = new Rectangle(-1500, -1500, 3000, 3000);
        public static bool DevMode = false;
        public static float BaseDifficulty = 1.0f;
    public static float DifficultyMultiplier = 1.0f;
    public static bool BossAcosoMode = false;
    public static float BossRespawnTimer = 0.0f;

    public static void SaveGame() { File.WriteAllText("save.txt", $"{GlobalCoins},{BonusHealth},{BonusSpeed},{BonusMagnet}"); }
    public static void LoadGame() { if (File.Exists("save.txt")) { var data = File.ReadAllText("save.txt").Split(','); GlobalCoins = int.Parse(data[0]); BonusHealth = int.Parse(data[1]); if (data.Length > 2) { BonusSpeed = int.Parse(data[2]); BonusMagnet = int.Parse(data[3]); } } }

    public static void SpawnParticles(Vector2 pos, int count, Color color)
    {
        for (int i = 0; i < count; i++)
        {
            for (int j = 0; j < particles.Length; j++)
            {
                if (!particles[j].IsActive)
                {
                    particles[j].IsActive = true;
                    particles[j].Position = pos;
                    float angle = (float)(random.NextDouble() * Math.PI * 2);
                    float speed = random.Next(50, 201);
                    particles[j].Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;
                    particles[j].Color = color;
                    particles[j].LifeTime = (float)(0.2 + random.NextDouble() * 0.6);
                    particles[j].Size = random.Next(2, 6);
                    break;
                }
            }
        }
    }

    public static void ResetGame(ref Player player)
        {
            LoadGame();
            State = GameState.MainMenu;
            GameTime = 0.0f;
            spawnTimer = 0.0f;
            spawnInterval = 2.0f;
            BossSpawned = false;

            player.Position = new Vector2(0, 0);
            player.Size = 30.0f;
            player.MaxHealth = 100.0f + BonusHealth;
            player.Health = player.MaxHealth;
            player.Coins = 0;
            DrunkTimer = 0.0f;
            player.XP = 0;
            player.MaxXP = 10;
            player.Level = 1;
            player.Speed = 150.0f + BonusSpeed;
            player.SpeedMult = 1.0f;
            player.MacheteCooldownMult = 1.0f;
            player.MagnetMult = 1.0f;
            player.WhipDamageMult = 1.0f;
            player.PickupRadius = 60.0f + BonusMagnet;
            player.HasPiedrazo = true;
            player.HasMachete = false;
            player.HasHalls = false;
            player.HasWhip = false;
            player.HasPuddle = false;
            player.DamageFlashTimer = 0.0f;
            player.ManualShootCooldown = 0.0f;
            player.MaxStamina = 100.0f;
            player.Stamina = 100.0f;
            player.IsSprinting = false;
            HitStopTimer = 0;

            WeaponManager.ResetWeapons();

            for (int i = 0; i < enemies.Length; i++) enemies[i].IsActive = false;
            for (int i = 0; i < gems.Length; i++) gems[i].IsActive = false;
            for (int i = 0; i < projectiles.Length; i++) projectiles[i].IsActive = false;
            for (int i = 0; i < bouncingProjectiles.Length; i++) bouncingProjectiles[i].IsActive = false;
            for (int i = 0; i < puddles.Length; i++) puddles[i].IsActive = false;
            for (int i = 0; i < manualProjectiles.Length; i++) manualProjectiles[i].IsActive = false;
            for (int i = 0; i < damageTexts.Length; i++) damageTexts[i].IsActive = false;
            for (int i = 0; i < obstacles.Length; i++) obstacles[i].IsActive = false;
            for (int i = 0; i < pickups.Length; i++) pickups[i].IsActive = false;

            if (ActiveChallenge == 1)
            {
                player.MaxHealth = 1.0f;
                player.Health = 1.0f;
            }
        }

        public static void SpawnPickup(Vector2 pos)
        {
            pos.X = Math.Clamp(pos.X, MapBounds.X, MapBounds.X + MapBounds.Width - 12.0f);
            pos.Y = Math.Clamp(pos.Y, MapBounds.Y, MapBounds.Y + MapBounds.Height - 12.0f);

            if (random.NextDouble() < 0.10)
            {
                for (int i = 0; i < pickups.Length; i++)
                {
                    if (!pickups[i].IsActive)
                    {
                        pickups[i].IsActive = true;
                        pickups[i].Position = pos;
                        pickups[i].Size = 12.0f;
                        double rnd = random.NextDouble();
                        if (rnd < 0.60) pickups[i].Type = 2; // Moneda
                        else if (rnd < 0.90) pickups[i].Type = 0; // Empanada
                        else if (rnd < 0.98) pickups[i].Type = 1; // Imán
                        else pickups[i].Type = 3; // Perfume de Cacao
                        break;
                    }
                }
            }
        }

        public static void SpawnDamageText(Vector2 pos, int amount)
        {
            for (int i = 0; i < damageTexts.Length; i++)
            {
                if (!damageTexts[i].IsActive)
                {
                    damageTexts[i].IsActive = true;
                    damageTexts[i].Position = pos;
                    damageTexts[i].Value = amount;
                    damageTexts[i].LifeTime = 0.5f;
                    break;
                }
            }
        }

        public static int GetClosestEnemyIndex(Vector2 position)
        {
            int closestIndex = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].IsActive)
                {
                    float dist = Vector2.DistanceSquared(position, enemies[i].Position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestIndex = i;
                    }
                }
            }
            return closestIndex;
        }

        public static void SpawnGem(Vector2 position, int value)
        {
            position.X = Math.Clamp(position.X, MapBounds.X, MapBounds.X + MapBounds.Width - 8.0f);
            position.Y = Math.Clamp(position.Y, MapBounds.Y, MapBounds.Y + MapBounds.Height - 8.0f);

            for (int i = 0; i < gems.Length; i++)
            {
                if (!gems[i].IsActive)
                {
                    gems[i].IsActive = true;
                    gems[i].Position = position;
                    gems[i].Value = value;
                    gems[i].Size = 8.0f;
                    break;
                }
            }
        }

        public static void Update(float deltaTime, ref Player player, ref Camera2D camera, int screenWidth, int screenHeight)
        {
            // Dificultad Dinámica
            if (Raylib.IsKeyPressed(KeyboardKey.KpAdd)) BaseDifficulty += 0.1f;
            if (Raylib.IsKeyPressed(KeyboardKey.KpSubtract)) BaseDifficulty = Math.Max(0.1f, BaseDifficulty - 0.1f);
            DifficultyMultiplier = Math.Max(0.1f, BaseDifficulty + (GameTime / 60.0f) * 0.4f);

            // Tecla de Salida / Pausa
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                if (State == GameState.Playing) State = GameState.MainMenu;
                Raylib.EnableCursor();
            }
            // Gestión del cursor para el modo FPS
            if (State == GameState.Playing && CurrentMode == GameMode.Story3D)
            {
                if (!Raylib.IsCursorHidden()) Raylib.DisableCursor();
            }
            else
            {
                if (Raylib.IsCursorHidden()) Raylib.EnableCursor();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.F9))
            {
                DevMode = !DevMode;
                if (DevMode)
                {
                    player.MaxHealth = 5000.0f;
                    player.Health = player.MaxHealth;
                    GlobalCoins += 10000;
                    player.HasMachete = true;
                    player.HasHalls = true;
                    player.HasWhip = true;
                    player.HasPuddle = true;
                }
            }

            if (State == GameState.StartMenu)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    State = GameState.MainMenu;
                }
            }
            else if (State == GameState.MainMenu)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.F)) { UseStaticSprites = !UseStaticSprites; }

                if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
                {
                    MenuSelection--;
                    if (MenuSelection < 0) MenuSelection = 4;
                }
                if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
                {
                    MenuSelection++;
                    if (MenuSelection > 4) MenuSelection = 0;
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    switch (MenuSelection)
                    {
                        case 0:
                            State = GameState.Playing;
                            CurrentMode = GameMode.Arcade2D;
                            IsStoryMode = false;
                            ResetGame(ref player);
                            State = GameState.Playing; // ResetGame sets to MainMenu
                            CurrentLevel = new Level1_Parking();
                            CurrentLevel.Initialize();
                            break;
                        case 1:
                            State = GameState.Playing;
                            CurrentMode = GameMode.Story3D;
                            IsStoryMode = true;
                            ResetGame(ref player);
                            State = GameState.Playing;
                            CurrentLevel = new Level1_Parking();
                            CurrentLevel.Initialize();
                            break;
                        case 2:
                            State = GameState.ChallengesMenu;
                            break;
                        case 3:
                            State = GameState.StoreMenu;
                            break;
                        case 4:
                            QuitGame = true;
                            break;
                    }
                }
            }
            else if (State == GameState.Playing)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    if (particles[i].IsActive)
                    {
                        particles[i].Position += particles[i].Velocity * deltaTime;
                        particles[i].LifeTime -= deltaTime;
                        if (particles[i].LifeTime <= 0)
                        {
                            particles[i].IsActive = false;
                        }
                    }
                }

                GameTime += deltaTime;

                if (HitStopTimer > 0) { HitStopTimer -= deltaTime; return; }
                if (DrunkTimer > 0) { DrunkTimer -= deltaTime; }

                if (GameTime >= 300.0f) 
                {
                    State = GameState.GameWon;
                    return;
                }

                PlayerSystem.Update(deltaTime, ref player, CurrentMode, obstacles);
                camera.Target = player.Position + new Vector2(player.Size / 2.0f);

                if (player.DamageFlashTimer > 0)
                {
                    player.DamageFlashTimer -= deltaTime;
                }

                if (player.ScreenShakeTimer > 0)
                {
                    player.ScreenShakeTimer -= deltaTime;
                }

                // La recolección de Pickups ahora se procesa en CollisionSystem.Update

                float baseInterval = Math.Max(0.5f, 2.0f - (GameTime / 200.0f));
                float currentSpawnInterval = baseInterval / Math.Max(0.1f, GameManager.DifficultyMultiplier);
                spawnTimer += deltaTime;
                // Minuto 1 ('El Matraqueo'): Desactiva el spawn normal por 10 segundos
                if (GameTime >= 60.0f && GameTime < 70.0f)
                {
                    spawnTimer = 0.0f;
                }
                else if (spawnTimer >= currentSpawnInterval)
                {
                    CurrentLevel.SpawnEnemy(player.Position, GameTime);
                    spawnTimer = 0.0f;
                }

                CurrentLevel.UpdateLevel(deltaTime, GameTime, ref player);
                InputManager.HandleManualWeapon(ref player, deltaTime);
                WeaponManager.UpdateWeapons(deltaTime, ref player);

                for (int i = 0; i < damageTexts.Length; i++)
                {
                    if (damageTexts[i].IsActive)
                    {
                        damageTexts[i].LifeTime -= deltaTime;
                        damageTexts[i].Position.Y -= 50.0f * deltaTime;
                        if (damageTexts[i].LifeTime <= 0)
                        {
                            damageTexts[i].IsActive = false;
                        }
                    }
                }

                EnemySystem.Update(deltaTime, ref player);
                ProjectileSystem.Update(deltaTime); // <--- ESTA ES LA CONEXIÓN VITAL
                CollisionSystem.Update(deltaTime, ref player);
            }
            else if (State == GameState.LevelUpMenu)
            {
                int baseCardWidth = 300;
                int baseCardHeight = 180;
                int cardSpacing = 40;
                int totalWidth = (baseCardWidth * 3) + (cardSpacing * 2);
                int startX = (screenWidth - totalWidth) / 2;
                int cardY = screenHeight / 2 - baseCardHeight / 2 + 20;

                Vector2 mousePos = Raylib.GetMousePosition();

                for (int i = 0; i < 3; i++)
                {
                    Rectangle baseRec = new Rectangle(
                        startX + i * (baseCardWidth + cardSpacing),
                        cardY,
                        baseCardWidth,
                        baseCardHeight
                    );

                    if (Raylib.CheckCollisionPointRec(mousePos, baseRec) && Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        ApplyUpgrade(currentUpgrades[i].EffectId, ref player);
                        break;
                    }
                }
            }
            else if (State == GameState.GameOver || State == GameState.GameWon)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    SaveGame();
                    ResetGame(ref player);
                }
            }
            else if (State == GameState.ChallengesMenu)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W))
                {
                    ChallengeMenuSelection--;
                    if (ChallengeMenuSelection < 0) ChallengeMenuSelection = 2;
                }
                if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S))
                {
                    ChallengeMenuSelection++;
                    if (ChallengeMenuSelection > 2) ChallengeMenuSelection = 0;
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    switch (ChallengeMenuSelection)
                    {
                        case 0:
                            ActiveChallenge = 1;
                            State = GameState.Playing;
                            ResetGame(ref player);
                            State = GameState.Playing;
                            CurrentLevel = new Level1_Parking();
                            CurrentLevel.Initialize();
                            break;
                        case 1:
                            ActiveChallenge = 2;
                            State = GameState.Playing;
                            ResetGame(ref player);
                            State = GameState.Playing;
                            CurrentLevel = new Level1_Parking();
                            CurrentLevel.Initialize();
                            break;
                        case 2:
                            State = GameState.MainMenu;
                            break;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Escape)) State = GameState.MainMenu;
            }
            else if (State == GameState.StoreMenu)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.One) && GlobalCoins >= 50) { GlobalCoins -= 50; BonusHealth += 10; SaveGame(); }
                if (Raylib.IsKeyPressed(KeyboardKey.Two) && GlobalCoins >= 100) { GlobalCoins -= 100; BonusSpeed += 5; SaveGame(); }
                if (Raylib.IsKeyPressed(KeyboardKey.Three) && GlobalCoins >= 100) { GlobalCoins -= 100; BonusMagnet += 5; SaveGame(); }
                if (Raylib.IsKeyPressed(KeyboardKey.Escape)) State = GameState.MainMenu;
            }
        }

        public static void GenerateUpgrades(ref Player player)
        {
            Upgrade[] pool = new Upgrade[10];

            pool[0] = new Upgrade { Title = "Curación", Description = "Restaura 30 de vida", EffectId = 0 };
            pool[1] = new Upgrade { Title = "Velocidad", Description = "+20 velocidad de movimiento", EffectId = 1 };
            pool[2] = new Upgrade { Title = "Brazos Rápidos", Description = "Dispara piedras más rápido", EffectId = 2 };
            pool[3] = new Upgrade { Title = "+Vida Máxima", Description = "+20 de vida máxima", EffectId = 3 };
            pool[4] = new Upgrade { Title = "Imán Mejorado", Description = "+50 radio de recogida", EffectId = 4 };

            int poolCount = 5;

            if (ActiveChallenge != 2)
            {
                if (!player.HasMachete)
                {
                    pool[poolCount] = new Upgrade { Title = "¡Machetazo!", Description = "Desbloquea ataque cuerpo a cuerpo", EffectId = 10 };
                    poolCount++;
                }
                if (!player.HasHalls)
                {
                    pool[poolCount] = new Upgrade { Title = "¡Halls Rebotador!", Description = "Desbloquea proyectil congelante", EffectId = 11 };
                    poolCount++;
                }
                if (!player.HasWhip)
                {
                    pool[poolCount] = new Upgrade { Title = "¡Cadena Dorada!", Description = "Desbloquea látigo de doble lado", EffectId = 12 };
                    poolCount++;
                }
                if (!player.HasPuddle)
                {
                    pool[poolCount] = new Upgrade { Title = "¡Charco Ácido!", Description = "Desbloquea charco de daño en área", EffectId = 13 };
                    poolCount++;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int index = random.Next(poolCount);
                currentUpgrades[i] = pool[index];

                poolCount--;
                pool[index] = pool[poolCount];
            }
        }

        private static void ApplyUpgrade(int effectId, ref Player player)
        {
            switch (effectId)
            {
                case 0: player.Health = Math.Min(player.Health + 30, player.MaxHealth); break;
                case 1: player.Speed += 20; break;
                case 2: player.MacheteCooldownMult *= 0.8f; break;
                case 3: player.MaxHealth += 20; player.Health = Math.Min(player.Health + 20, player.MaxHealth); break;
                case 4: player.PickupRadius += 50; break;
                case 10: player.HasMachete = true; break;
                case 11: player.HasHalls = true; break;
                case 12: player.HasWhip = true; break;
                case 13: player.HasPuddle = true; break;
            }
            State = GameState.Playing;
        }
    }
}
