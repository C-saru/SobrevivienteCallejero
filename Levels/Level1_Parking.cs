using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public class Level1_Parking : ILevel
    {
        public Rectangle Bounds { get; } = new Rectangle(-1500, -1500, 3000, 3000);
        private bool event60sFired = false; 
        private bool event120sFired = false;
        private bool event180sFired = false;
        private bool event240sFired = false;
        private bool event300sFired = false;
        
        // Custom flags for special states
        public static bool IsDarknessEvent = false;

        public void Initialize()
        {
            // Generar Obstáculos
            for (int i = 0; i < GameManager.obstacles.Length; i++)
            {
                GameManager.obstacles[i].IsActive = true;
                GameManager.obstacles[i].Type = GameManager.random.Next(0, 2);
                
                float ox = 0, oy = 0;
                do {
                    ox = GameManager.random.Next(-2000, 2000);
                    oy = GameManager.random.Next(-2000, 2000);
                } while (Math.Abs(ox) < 300 && Math.Abs(oy) < 300); // Evitar el centro (spawn del jugador)
                
                if (GameManager.obstacles[i].Type == 0)
                    GameManager.obstacles[i].Rect = new Rectangle(ox, oy, 120, 80); // Carrito
                else
                    GameManager.obstacles[i].Rect = new Rectangle(ox, oy, 150, 100); // Hueco
            }
        }

        public void UpdateLevel(float deltaTime, float gameTime, ref Player player)
        {
            // Limitar posición del jugador al mapa
            player.Position.X = Math.Clamp(player.Position.X, Bounds.X, Bounds.X + Bounds.Width - player.Size);
            player.Position.Y = Math.Clamp(player.Position.Y, Bounds.Y, Bounds.Y + Bounds.Height - player.Size);

            // Eventos del Game-Loop por minuto

            // Minuto 1: Enjambre de Bachaqueros
            if (gameTime >= 60.0f && !event60sFired) 
            { 
                event60sFired = true; 
                SpawnCircleWave(player.Position, 4, 30, 400.0f); // 4 = Bachaquero
            }

            // Minuto 2: Mini-jefe "Fiscal de Tránsito" (usaremos tanque temporalmente o crearemos un tipo si existe)
            // Por ahora usamos el tipo 2 (Tanque) pero con estadísticas bufadas por ser mini-jefe
            if (gameTime >= 120.0f && !event120sFired) 
            { 
                event120sFired = true; 
                SpawnMiniBoss(player.Position, 2, 500f); 
            }

            // Minuto 3: Hora Pico (Camioneticas en fila)
            if (gameTime >= 180.0f && !event180sFired)
            {
                event180sFired = true;
                SpawnCamioneticaRush(player.Position);
            }

            // Minuto 4: Mini-jefe "Motorizado Lider"
            if (gameTime >= 240.0f && !event240sFired)
            {
                event240sFired = true;
                SpawnMiniBoss(player.Position, 1, 400f); // 1 = Motorizado
            }

            // Minuto 5: Jefe final con efecto visual (apagón)
            if (gameTime >= 300.0f && !event300sFired)
            {
                event300sFired = true;
                IsDarknessEvent = true; // Activa el efecto de apagón
                GameManager.BossSpawned = true;
                SpawnBoss(player.Position);
            }

            if (GameManager.BossAcosoMode && GameManager.BossSpawned)
            {
                bool bossIsActive = false;
                for (int i = 0; i < GameManager.enemies.Length; i++)
                {
                    if (GameManager.enemies[i].IsActive && GameManager.enemies[i].Type == 3)
                    {
                        bossIsActive = true;
                        break;
                    }
                }

                if (!bossIsActive)
                {
                    IsDarknessEvent = false; // Termina el apagón al morir el boss
                    GameManager.BossRespawnTimer += deltaTime;
                    if (GameManager.BossRespawnTimer >= 5.0f)
                    {
                        SpawnBoss(player.Position);
                        GameManager.BossRespawnTimer = 0.0f;
                    }
                }
            }
        }

        public void DrawBackground()
        {
            // Fondo verde (pasto) y rectángulo gris (estacionamiento)
            Raylib.ClearBackground(new Color(34, 139, 34, 255)); // Pasto
            Raylib.DrawRectangleRec(Bounds, new Color(100, 100, 100, 255)); // Estacionamiento
            Raylib.DrawRectangleLinesEx(Bounds, 10, new Color(50, 50, 50, 255)); // Muro

            // Líneas blancas de los puestos
            int laneWidth = 300;
            int dashLength = 80;
            int dashGap = 60;
            
            // Limitamos las líneas al área del estacionamiento
            int startX = (int)Bounds.X;
            int startY = (int)Bounds.Y;
            int endX = (int)(Bounds.X + Bounds.Width);
            int endY = (int)(Bounds.Y + Bounds.Height);

            for (int x = startX + laneWidth; x < endX; x += laneWidth)
            {
                for (int y = startY; y < endY; y += (dashLength + dashGap))
                {
                    if (y + dashLength <= endY)
                    {
                        Raylib.DrawRectangle(x, y, 10, dashLength, new Color(255, 204, 0, 100));
                    }
                }
            }

            // Dibujar obstáculos (Huecos primero, luego Carritos)
            for (int i = 0; i < GameManager.obstacles.Length; i++)
            {
                if (GameManager.obstacles[i].IsActive)
                {
                    if (GameManager.obstacles[i].Type == 1) // Hueco
                    {
                        Raylib.DrawEllipse((int)(GameManager.obstacles[i].Rect.X + GameManager.obstacles[i].Rect.Width / 2), 
                                           (int)(GameManager.obstacles[i].Rect.Y + GameManager.obstacles[i].Rect.Height / 2), 
                                           (float)GameManager.obstacles[i].Rect.Width / 2, 
                                           (float)GameManager.obstacles[i].Rect.Height / 2, 
                                           new Color(20, 20, 20, 200));
                    }
                    else if (GameManager.obstacles[i].Type == 0) // Carrito
                    {
                        Raylib.DrawRectangleRec(GameManager.obstacles[i].Rect, new Color(139, 69, 19, 255)); // Base marrón
                        Raylib.DrawRectangle((int)GameManager.obstacles[i].Rect.X, (int)GameManager.obstacles[i].Rect.Y, (int)GameManager.obstacles[i].Rect.Width, 20, Color.Red); // Techo rojo
                    }
                }
            }
        }

        public void SpawnEnemy(Vector2 playerPosition, float gameTime)
        {
            for (int i = 0; i < GameManager.enemies.Length; i++)
            {
                if (!GameManager.enemies[i].IsActive)
                {
                    GameManager.enemies[i].IsActive = true;
                    GameManager.enemies[i].InvincibilityTimer = 0.0f;
                    GameManager.enemies[i].FreezeTimer = 0.0f;
                    GameManager.enemies[i].DashTimer = 0.0f;
                    GameManager.enemies[i].IsDashing = false;
                    GameManager.enemies[i].StateTimer = 0.0f;

                    double rnd = GameManager.random.NextDouble();
                    int typeToSpawn = 0;
                    if (rnd < 0.05) typeToSpawn = 2; // 5% Tanque
                    else if (rnd < 0.15) typeToSpawn = 1; // 10% Motorizado
                    else if (rnd < 0.25) typeToSpawn = 4; // 10% Bachaquero
                    else if (rnd < 0.35) typeToSpawn = 5; // 10% Malandro
                    else if (rnd < 0.45) typeToSpawn = 6; // 10% Borrachito
                    else if (rnd < 0.55) typeToSpawn = 7; // 10% Lanza-Piedras
                    else if (rnd < 0.60) typeToSpawn = 8; // 5% Camionetica
                    else typeToSpawn = 0; // 40% Normal

                    GameManager.enemies[i].Type = typeToSpawn;
                    GameManager.enemies[i].Size = GameConfig.Enemies[typeToSpawn].Size;
                    GameManager.enemies[i].BaseSpeed = GameConfig.Enemies[typeToSpawn].BaseSpeed;
                    GameManager.enemies[i].Health = GameConfig.Enemies[typeToSpawn].Health;

                    float angle = (float)(GameManager.random.NextDouble() * Math.PI * 2);
                    float distance = GameManager.random.Next(600, 1000);
                    
                    Vector2 spawnPos = playerPosition + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance;
                    
                    // Clamping para que no aparezcan fuera del mapa
                    spawnPos.X = Math.Clamp(spawnPos.X, Bounds.X, Bounds.X + Bounds.Width - GameManager.enemies[i].Size);
                    spawnPos.Y = Math.Clamp(spawnPos.Y, Bounds.Y, Bounds.Y + Bounds.Height - GameManager.enemies[i].Size);
                    
                    GameManager.enemies[i].Position = spawnPos;
                    break;
                }
            }
        }

        private void SpawnBoss(Vector2 playerPosition)
        {
            for (int i = 0; i < GameManager.enemies.Length; i++)
            {
                if (!GameManager.enemies[i].IsActive)
                {
                    GameManager.enemies[i].IsActive = true;
                    GameManager.enemies[i].InvincibilityTimer = 0.0f;
                    GameManager.enemies[i].FreezeTimer = 0.0f;
                    GameManager.enemies[i].DashTimer = 0.0f;
                    GameManager.enemies[i].IsDashing = false;
                    GameManager.enemies[i].StateTimer = 0.0f;
                    
                    GameManager.enemies[i].Type = 3; // Jefe
                    GameManager.enemies[i].Size = GameConfig.Enemies[3].Size;
                    GameManager.enemies[i].BaseSpeed = GameConfig.Enemies[3].BaseSpeed;
                    GameManager.enemies[i].Health = GameConfig.Enemies[3].Health;

                    float angle = (float)(GameManager.random.NextDouble() * Math.PI * 2);
                    float distance = 1000.0f; // Aparece un poco más lejos
                    
                    Vector2 spawnPos = playerPosition + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance;
                    
                    // Clamping para que no aparezcan fuera del mapa
                    spawnPos.X = Math.Clamp(spawnPos.X, Bounds.X, Bounds.X + Bounds.Width - GameManager.enemies[i].Size);
                    spawnPos.Y = Math.Clamp(spawnPos.Y, Bounds.Y, Bounds.Y + Bounds.Height - GameManager.enemies[i].Size);

                    GameManager.enemies[i].Position = spawnPos;
                    break;
                }
            }
        }

        private void SpawnMiniBoss(Vector2 playerPosition, int baseType, float extraHealth)
        {
            for (int i = 0; i < GameManager.enemies.Length; i++)
            {
                if (!GameManager.enemies[i].IsActive)
                {
                    GameManager.enemies[i].IsActive = true;
                    GameManager.enemies[i].InvincibilityTimer = 0.0f;
                    GameManager.enemies[i].FreezeTimer = 0.0f;
                    GameManager.enemies[i].DashTimer = 0.0f;
                    GameManager.enemies[i].IsDashing = false;
                    GameManager.enemies[i].StateTimer = 0.0f;
                    
                    GameManager.enemies[i].Type = baseType;
                    GameManager.enemies[i].Size = GameConfig.Enemies[baseType].Size * 1.5f; // Más grande
                    GameManager.enemies[i].BaseSpeed = GameConfig.Enemies[baseType].BaseSpeed;
                    GameManager.enemies[i].Health = GameConfig.Enemies[baseType].Health + extraHealth; // Más vida

                    float angle = (float)(GameManager.random.NextDouble() * Math.PI * 2);
                    float distance = 800.0f;
                    
                    Vector2 spawnPos = playerPosition + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance;
                    spawnPos.X = Math.Clamp(spawnPos.X, Bounds.X, Bounds.X + Bounds.Width - GameManager.enemies[i].Size);
                    spawnPos.Y = Math.Clamp(spawnPos.Y, Bounds.Y, Bounds.Y + Bounds.Height - GameManager.enemies[i].Size);

                    GameManager.enemies[i].Position = spawnPos;
                    break;
                }
            }
        }

        private void SpawnCamioneticaRush(Vector2 playerPosition)
        {
            // Spawnea 5 camioneticas en fila horizontal arriba del jugador
            for(int k = 0; k < 5; k++)
            {
                for (int i = 0; i < GameManager.enemies.Length; i++)
                {
                    if (!GameManager.enemies[i].IsActive)
                    {
                        GameManager.enemies[i].IsActive = true;
                        GameManager.enemies[i].InvincibilityTimer = 0.0f;
                        GameManager.enemies[i].FreezeTimer = 0.0f;
                        GameManager.enemies[i].DashTimer = 0.0f;
                        GameManager.enemies[i].IsDashing = true; // Empiezan dasheando
                        GameManager.enemies[i].StateTimer = 0.0f;
                        
                        GameManager.enemies[i].Type = 8; // Camionetica
                        GameManager.enemies[i].Size = GameConfig.Enemies[8].Size;
                        GameManager.enemies[i].BaseSpeed = GameConfig.Enemies[8].BaseSpeed;
                        GameManager.enemies[i].Health = GameConfig.Enemies[8].Health;

                        // Posición en fila por encima de la pantalla
                        Vector2 spawnPos = playerPosition + new Vector2(-400 + (k * 200), -800);
                        GameManager.enemies[i].Position = spawnPos;
                        GameManager.enemies[i].DashDirection = new Vector2(0, 1); // Hacia abajo
                        break;
                    }
                }
            }
        }

        private void SpawnCircleWave(Vector2 center, int enemyType, int count, float radius) 
        { 
            float angleStep = (float)(Math.PI * 2) / count; 
            for (int k = 0; k < count; k++) 
            { 
                for (int i = 0; i < GameManager.enemies.Length; i++) 
                { 
                    if (!GameManager.enemies[i].IsActive) 
                    { 
                        GameManager.enemies[i].IsActive = true; 
                        GameManager.enemies[i].Type = enemyType; 
                        GameManager.enemies[i].Size = GameConfig.Enemies[enemyType].Size; 
                        GameManager.enemies[i].BaseSpeed = GameConfig.Enemies[enemyType].BaseSpeed; 
                        GameManager.enemies[i].Health = GameConfig.Enemies[enemyType].Health; 
                        
                        float angle = k * angleStep; 
                        Vector2 spawnPos = center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius; 
                        spawnPos.X = Math.Clamp(spawnPos.X, Bounds.X, Bounds.X + Bounds.Width - GameManager.enemies[i].Size); 
                        spawnPos.Y = Math.Clamp(spawnPos.Y, Bounds.Y, Bounds.Y + Bounds.Height - GameManager.enemies[i].Size); 
                        GameManager.enemies[i].Position = spawnPos; 
                        break; 
                    } 
                } 
            } 
        }
    }
}
