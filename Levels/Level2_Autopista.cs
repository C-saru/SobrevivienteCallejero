using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public class Level2_Autopista : ILevel
    {
        public Rectangle Bounds { get; } = new Rectangle(-1500, -1500, 3000, 3000);
        private float spawnTimer = 0.0f;

        public void Initialize() { /* El jugador empieza en el centro por defecto */ }

        public void UpdateLevel(float deltaTime, float gameTime, ref Player player)
        {
            // Condición de Victoria: Cruzar la autopista (llegar arriba)
            if (player.Position.Y < Bounds.Y + 200)
            {
                GameManager.GlobalCoins += 50;
                GameManager.State = GameState.GameWon;
            }

            // Spawn de camioneticas constante
            spawnTimer += deltaTime;
            if (spawnTimer >= 0.4f)
            {
                spawnTimer = 0.0f;
                SpawnEnemy(player.Position, gameTime);
            }
        }

        public void DrawBackground()
        {
            Raylib.ClearBackground(new Color(30, 30, 30, 255)); // Asfalto
            for (int i = -1500; i < 1500; i += 150)
            {
                Raylib.DrawRectangle(-1500, i, 3000, 5, new Color(255, 255, 0, 100)); // Líneas amarillas
            }
        }

        public void SpawnEnemy(Vector2 playerPosition, float gameTime)
        {
            for (int i = 0; i < GameManager.enemies.Length; i++)
            {
                if (!GameManager.enemies[i].IsActive)
                {
                    GameManager.enemies[i].IsActive = true;
                    GameManager.enemies[i].IsHoraPico = true;
                    GameManager.enemies[i].Type = 8;
                    GameManager.enemies[i].Size = GameConfig.Enemies[8].Size;
                    GameManager.enemies[i].BaseSpeed = GameConfig.Enemies[8].BaseSpeed * 2.5f;
                    GameManager.enemies[i].Health = 9999f; // Invencibles aquí

                    bool fromLeft = GameManager.random.Next(2) == 0;
                    float startX = fromLeft ? -1500 : 1500;
                    float startY = playerPosition.Y + GameManager.random.Next(-500, 500);
                    
                    GameManager.enemies[i].Position = new Vector2(startX, startY);
                    GameManager.enemies[i].DashDirection = fromLeft ? new Vector2(1, 0) : new Vector2(-1, 0);
                    break;
                }
            }
        }
    }
}
