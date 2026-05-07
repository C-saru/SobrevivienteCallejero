using System.Collections.Generic;

namespace VampireSurvivorsClone
{
    public struct EnemyConfig
    {
        public float Size;
        public float BaseSpeed;
        public float Health;
    }

    public static class GameConfig
    {
        public static readonly Dictionary<int, EnemyConfig> Enemies = new Dictionary<int, EnemyConfig>()
        {
            { 0, new EnemyConfig { Size = 20f, BaseSpeed = 100f, Health = 30f } }, // Normal
            { 1, new EnemyConfig { Size = 15f, BaseSpeed = 180f, Health = 20f } }, // Motorizado
            { 2, new EnemyConfig { Size = 30f, BaseSpeed = 50f, Health = 100f } }, // Tanque
            { 3, new EnemyConfig { Size = 60f, BaseSpeed = 80f, Health = 3000f } },// Jefe
            { 4, new EnemyConfig { Size = 20f, BaseSpeed = 110f, Health = 30f } }, // Bachaquero
            { 5, new EnemyConfig { Size = 22f, BaseSpeed = 280f, Health = 25f } }, // Malandro
            { 6, new EnemyConfig { Size = 25f, BaseSpeed = 80f, Health = 40f } },  // Borrachito
            { 7, new EnemyConfig { Size = 18f, BaseSpeed = 130f, Health = 20f } }, // Lanza-Piedras
            { 8, new EnemyConfig { Size = 35f, BaseSpeed = 450f, Health = 80f } }  // Camionetica
        };
    }
}
