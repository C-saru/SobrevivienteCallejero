using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class PlayerSystem
    {
        public static void Update(float deltaTime, ref Player player, GameMode mode, Obstacle[] obstacles)
        {
            Vector2 dir = InputManager.GetMovementDirection(ref player, deltaTime, mode);
            Vector2 oldPos = player.Position;

            Rectangle playerRec = new Rectangle(player.Position.X, player.Position.Y, player.Size, player.Size);
            bool inHole = false;

            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i].IsActive && obstacles[i].Type == 1 && Raylib.CheckCollisionRecs(playerRec, obstacles[i].Rect))
                {
                    inHole = true;
                    break;
                }
            }

            float currentSpeed = player.Speed * player.SpeedMult;
            if (inHole) currentSpeed *= 0.2f;
            if (player.IsSprinting && player.Stamina > 0) 
            { 
                currentSpeed *= 1.8f; 
                player.Stamina -= 30.0f * deltaTime; 
            }
            else 
            { 
                player.Stamina = Math.Min(player.Stamina + 15.0f * deltaTime, player.MaxStamina); 
            }

            player.Position += dir * currentSpeed * deltaTime;
            if (dir != Vector2.Zero)
            {
                player.HeadBobTimer += deltaTime * player.Speed * 0.05f;
            }

            playerRec.X = player.Position.X;
            playerRec.Y = player.Position.Y;

            for (int i = 0; i < obstacles.Length; i++)
            {
                if (obstacles[i].IsActive && obstacles[i].Type == 0 && Raylib.CheckCollisionRecs(playerRec, obstacles[i].Rect))
                {
                    player.Position = oldPos;
                    break;
                }
            }

            player.Position.X = Math.Clamp(player.Position.X, GameManager.MapBounds.X, GameManager.MapBounds.X + GameManager.MapBounds.Width - player.Size);
            player.Position.Y = Math.Clamp(player.Position.Y, GameManager.MapBounds.Y, GameManager.MapBounds.Y + GameManager.MapBounds.Height - player.Size);
        }
    }
}
