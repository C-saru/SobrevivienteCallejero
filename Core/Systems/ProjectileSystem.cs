using System;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class ProjectileSystem
    {
        public static void Update(float deltaTime)
        {
            // 1. Mover Piedrazos Automáticos
            for (int i = 0; i < GameManager.projectiles.Length; i++)
            {
                if (GameManager.projectiles[i].IsActive)
                {
                    GameManager.projectiles[i].Position += GameManager.projectiles[i].Velocity * deltaTime;
                    
                    // Si se salen del mapa, se desactivan para ahorrar memoria
                    if (!Raylib.CheckCollisionPointRec(GameManager.projectiles[i].Position, GameManager.MapBounds))
                        GameManager.projectiles[i].IsActive = false;
                }
            }

            // 2. Mover Halls Rebotadores
            for (int i = 0; i < GameManager.bouncingProjectiles.Length; i++)
            {
                if (GameManager.bouncingProjectiles[i].IsActive)
                {
                    GameManager.bouncingProjectiles[i].Position += GameManager.bouncingProjectiles[i].Velocity * deltaTime;
                    
                    if (!Raylib.CheckCollisionPointRec(GameManager.bouncingProjectiles[i].Position, GameManager.MapBounds))
                        GameManager.bouncingProjectiles[i].IsActive = false;
                }
            }

            // 3. Mover Disparos Manuales (Espacio)
            for (int i = 0; i < GameManager.manualProjectiles.Length; i++)
            {
                if (GameManager.manualProjectiles[i].IsActive)
                {
                    GameManager.manualProjectiles[i].Position += GameManager.manualProjectiles[i].Velocity * deltaTime;
                    
                    if (!Raylib.CheckCollisionPointRec(GameManager.manualProjectiles[i].Position, GameManager.MapBounds))
                        GameManager.manualProjectiles[i].IsActive = false;
                }
            }

            // 4. Reducir el tiempo de vida de los Charcos
            for (int i = 0; i < GameManager.puddles.Length; i++)
            {
                if (GameManager.puddles[i].IsActive)
                {
                    GameManager.puddles[i].LifeTime -= deltaTime;
                    if (GameManager.puddles[i].LifeTime <= 0)
                        GameManager.puddles[i].IsActive = false;
                }
            }
        }
    }
}
