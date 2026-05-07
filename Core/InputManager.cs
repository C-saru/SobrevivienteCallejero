using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class InputManager
    {
        public static Vector2 GetMovementDirection(ref Player player, float deltaTime, GameMode mode)
        {
            Vector2 direction = Vector2.Zero;

            if (mode == GameMode.Story3D)
            {
                if (player.FacingDirection == Vector2.Zero) player.FacingDirection = new Vector2(1, 0);

                float angle = (float)Math.Atan2(player.FacingDirection.Y, player.FacingDirection.X);
                Vector2 mouseDelta = Raylib.GetMouseDelta();
                angle += mouseDelta.X * 0.003f; // Sensibilidad del ratón

                player.FacingDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2 forward = player.FacingDirection;
                Vector2 right = new Vector2(-forward.Y, forward.X);

                if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up)) direction += forward;
                if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down)) direction -= forward;
                if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left)) direction -= right;
                if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right)) direction += right;

                if (direction != Vector2.Zero)
                {
                    direction = Vector2.Normalize(direction);
                    player.FrameTimer += deltaTime;
                    if (player.FrameTimer >= 0.15f)
                    {
                        player.FrameTimer = 0.0f;
                        player.CurrentFrame = (player.CurrentFrame + 1) % 8;
                    }
                }
                else
                {
                    player.CurrentFrame = 0;
                    player.FrameTimer = 0.0f;
                }
            }
            else
            {
                if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up)) direction.Y -= 1;
                if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down)) direction.Y += 1;
                if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left)) direction.X -= 1;
                if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right)) direction.X += 1;

                if (direction != Vector2.Zero)
                {
                    direction = Vector2.Normalize(direction);
                    player.FacingDirection = direction;

                    if (Math.Abs(player.FacingDirection.X) > Math.Abs(player.FacingDirection.Y))
                    {
                        player.CurrentRow = player.FacingDirection.X > 0 ? 2 : 3;
                    }
                    else
                    {
                        player.CurrentRow = player.FacingDirection.Y > 0 ? 0 : 1;
                    }

                    player.FrameTimer += deltaTime;
                    if (player.FrameTimer >= 0.15f)
                    {
                        player.FrameTimer = 0.0f;
                        player.CurrentFrame = (player.CurrentFrame + 1) % 8;
                    }
                }
                else
                {
                    player.CurrentFrame = 0;
                    player.FrameTimer = 0.0f;
                }
            }

            player.IsSprinting = Raylib.IsKeyDown(KeyboardKey.LeftShift) && direction != Vector2.Zero;

            return direction;
        }

        public static void HandleManualWeapon(ref Player player, float deltaTime)
        {
            if (player.ManualShootCooldown > 0)
            {
                player.ManualShootCooldown -= deltaTime;
            }

            if (Raylib.IsKeyDown(KeyboardKey.Space) && player.ManualShootCooldown <= 0)
            {
                for (int i = 0; i < GameManager.manualProjectiles.Length; i++)
                {
                    if (!GameManager.manualProjectiles[i].IsActive)
                    {
                        GameManager.manualProjectiles[i].IsActive = true;
                        GameManager.manualProjectiles[i].Position = player.Position + new Vector2(player.Size / 2.0f);
                        GameManager.manualProjectiles[i].Velocity = player.FacingDirection * 800.0f;
                        GameManager.manualProjectiles[i].Size = 4.0f;
                        player.ManualShootCooldown = 0.15f;
                        break;
                    }
                }
            } // <-- ESTA ERA LA LLAVE QUE FALTABA

            if (Raylib.IsMouseButtonPressed(MouseButton.Right) && WeaponManager.parryCooldownTimer <= 0)
            {
                WeaponManager.isParrying = true;
                WeaponManager.parryWindowTimer = 0.2f;
                WeaponManager.parryCooldownTimer = 2.0f; 
            }
        }
    }
}
