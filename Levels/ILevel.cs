using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public interface ILevel 
    {
        Rectangle Bounds { get; }
        void Initialize();
        void UpdateLevel(float deltaTime, float gameTime, ref Player player);
        void DrawBackground();
        void SpawnEnemy(Vector2 playerPosition, float gameTime);
    }
}
