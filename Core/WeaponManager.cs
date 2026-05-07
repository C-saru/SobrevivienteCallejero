using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class WeaponManager
    {
        // Variables del Ataque 1 (El Machetazo)
        public static float attackCooldownTimer = 0.0f;
        public static float attackDurationTimer = 0.0f;
        public static bool isAttacking = false;
        public static Rectangle attackHitbox;

        // Variables del Ataque 2 (Piedrazos)
        public static float shootTimer = 0.0f;

        // Variables del Ataque 3 (Caramelo Rebotador)
        public static float bounceShootTimer = 0.0f;

        // Variables del Ataque 4 (Cadena Dorada / Látigo)
        public static float whipCooldownTimer = 0.0f;
        public static float whipDurationTimer = 0.0f;
        public static bool isWhipping = false;
        public static Rectangle whipHitboxLeft;
        public static Rectangle whipHitboxRight;

        // Variables del Ataque 5 (Charco)
        public static float puddleDropTimer = 0.0f;

        // Variables del Parry
        public static float parryWindowTimer = 0.0f;
        public static float parryCooldownTimer = 0.0f;
        public static bool isParrying = false;
        public static float parrySuccessTimer = 0.0f;

        public static void ResetWeapons()
        {
            attackCooldownTimer = 0.0f;
            attackDurationTimer = 0.0f;
            shootTimer = 0.0f;
            bounceShootTimer = 0.0f;
            whipCooldownTimer = 0.0f;
            whipDurationTimer = 0.0f;
            puddleDropTimer = 0.0f;
            parryWindowTimer = 0.0f;
            parryCooldownTimer = 0.0f;
            isAttacking = false;
            isWhipping = false;
            isParrying = false;
            parrySuccessTimer = 0.0f;
        }

        public static void UpdateWeapons(float deltaTime, ref Player player)
        {
            // --- PARRY ---
            if (parryCooldownTimer > 0) parryCooldownTimer -= deltaTime;
            if (isParrying)
            {
                parryWindowTimer -= deltaTime;
                if (parryWindowTimer <= 0) isParrying = false;
            }
            if (parrySuccessTimer > 0) parrySuccessTimer -= deltaTime;

            // --- ATAQUE 1: EL MACHETAZO ---
            if (player.HasMachete)
            {
                attackCooldownTimer += deltaTime;
                float currentMacheteCooldown = 1.0f * player.MacheteCooldownMult;

                if (attackCooldownTimer >= currentMacheteCooldown)
                {
                    isAttacking = true;
                    attackDurationTimer = 0.0f;
                    attackCooldownTimer = 0.0f;

                    SoundManager.Play(1); // Reproducir sonido de Machetazo

                    float hitboxWidth = 60.0f;
                    float hitboxHeight = 60.0f;
                    float offsetX = player.FacingDirection.X * 40.0f;
                    float offsetY = player.FacingDirection.Y * 40.0f;

                    attackHitbox = new Rectangle(
                        player.Position.X + (player.Size / 2) + offsetX - (hitboxWidth / 2),
                        player.Position.Y + (player.Size / 2) + offsetY - (hitboxHeight / 2),
                        hitboxWidth,
                        hitboxHeight
                    );
                }

                if (isAttacking)
                {
                    attackDurationTimer += deltaTime;
                    if (attackDurationTimer >= 0.2f)
                    {
                        isAttacking = false;
                    }
                }
            }

            // --- ATAQUE 2: PIEDRAZOS ---
            if (player.HasPiedrazo)
            {
                shootTimer += deltaTime;
                if (shootTimer >= 0.8f * player.MacheteCooldownMult)
                {
                    shootTimer = 0.0f;
                    int closestEnemyIndex = GameManager.GetClosestEnemyIndex(player.Position);
                    
                    if (closestEnemyIndex != -1)
                    {
                        Vector2 dirToEnemy = Vector2.Normalize(GameManager.enemies[closestEnemyIndex].Position - player.Position);
                        for (int i = 0; i < GameManager.projectiles.Length; i++)
                        {
                            if (!GameManager.projectiles[i].IsActive)
                            {
                                GameManager.projectiles[i].IsActive = true;
                                GameManager.projectiles[i].Position = player.Position + new Vector2(player.Size / 2.0f);
                                GameManager.projectiles[i].Velocity = dirToEnemy * 400.0f;
                                GameManager.projectiles[i].Size = 6.0f;
                                
                                SoundManager.Play(0); // Reproducir sonido de Piedrazo
                                break;
                            }
                        }
                    }
                }
            }

            // --- ATAQUE 3: HALLS REBOTADOR ---
            if (player.HasHalls)
            {
                bounceShootTimer += deltaTime;
                if (bounceShootTimer >= 2.0f)
                {
                    bounceShootTimer = 0.0f;
                    int closestEnemyIndex = GameManager.GetClosestEnemyIndex(player.Position);
                    if (closestEnemyIndex != -1)
                    {
                        Vector2 dirToTarget = Vector2.Normalize(GameManager.enemies[closestEnemyIndex].Position - player.Position);
                        for (int i = 0; i < GameManager.bouncingProjectiles.Length; i++)
                        {
                            if (!GameManager.bouncingProjectiles[i].IsActive)
                            {
                                GameManager.bouncingProjectiles[i].IsActive = true;
                                GameManager.bouncingProjectiles[i].Size = 10.0f;
                                GameManager.bouncingProjectiles[i].BouncesLeft = 5;
                                GameManager.bouncingProjectiles[i].TargetIndex = -1;
                                GameManager.bouncingProjectiles[i].Position = player.Position + new Vector2(player.Size / 2.0f);
                                GameManager.bouncingProjectiles[i].Velocity = dirToTarget * 600.0f;
                                break;
                            }
                        }
                    }
                }
            }

            // --- ATAQUE 4: CADENA DORADA (LÁTIGO) ---
            if (player.HasWhip)
            {
                whipCooldownTimer += deltaTime;
                if (whipCooldownTimer >= 1.5f)
                {
                    isWhipping = true;
                    whipDurationTimer = 0.0f;
                    whipCooldownTimer = 0.0f;

                    float whipWidth = 150.0f;
                    float whipHeight = 20.0f;

                    whipHitboxLeft = new Rectangle(
                        player.Position.X - whipWidth,
                        player.Position.Y + (player.Size / 2) - (whipHeight / 2),
                        whipWidth,
                        whipHeight
                    );

                    whipHitboxRight = new Rectangle(
                        player.Position.X + player.Size,
                        player.Position.Y + (player.Size / 2) - (whipHeight / 2),
                        whipWidth,
                        whipHeight
                    );
                }

                if (isWhipping)
                {
                    whipDurationTimer += deltaTime;
                    if (whipDurationTimer >= 0.3f)
                    {
                        isWhipping = false;
                    }
                }
            }

            // --- ATAQUE 5: CHARCO ---
            if (player.HasPuddle)
            {
                puddleDropTimer += deltaTime;
                if (puddleDropTimer >= 3.0f)
                {
                    puddleDropTimer = 0.0f;
                    for (int i = 0; i < GameManager.puddles.Length; i++)
                    {
                        if (!GameManager.puddles[i].IsActive)
                        {
                            GameManager.puddles[i].IsActive = true;
                            GameManager.puddles[i].Position = player.Position + new Vector2(player.Size / 2.0f);
                            GameManager.puddles[i].Radius = 60.0f;
                            GameManager.puddles[i].LifeTime = 4.0f;
                            break;
                        }
                    }
                }
            }
        }
    }
}
