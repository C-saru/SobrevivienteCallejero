using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public enum GameState { StartMenu, Playing, LevelUpMenu, GameOver, GameWon, StoreMenu }
    public enum GameMode { Arcade2D, Story3D }

    public struct Upgrade
    {
        public string Title;
        public string Description;
        public int EffectId;
    }

    public struct Obstacle
    {
        public Rectangle Rect;
        public int Type; // 0 = Sólido/Carrito, 1 = Ralentización/Hueco
        public bool IsActive;
    }

    public struct Player
    {
        public Vector2 Position;
        public float Speed;
        public float Size;
        public Color Color;
        
        public float Health;
        public float MaxHealth;
        public int XP;
        public int MaxXP;
        public int Level;
        
        public float PickupRadius;
        public Vector2 FacingDirection;

        public int CurrentFrame;
        public float FrameTimer;
        public int CurrentRow;

        public float SpeedMult;
        public float MacheteCooldownMult;
        public float MagnetMult;
        public float WhipDamageMult;

        public bool HasPiedrazo;
        public bool HasMachete;
        public bool HasHalls;
        public bool HasWhip;
        public bool HasPuddle;

        public float ManualShootCooldown;
        public float DamageFlashTimer;
        public float ScreenShakeTimer;

        public float Stamina;
        public float MaxStamina;
        public bool IsSprinting;
        public int Coins;
    }

    public struct Pickup
    {
        public Vector2 Position;
        public int Type; // 0 = Empanada/Cura, 1 = Imán
        public bool IsActive;
        public float Size;
    }

    public struct DamageText
    {
        public Vector2 Position;
        public int Value;
        public float LifeTime;
        public bool IsActive;
    }

    public struct Enemy
    {
        public Vector2 Position;
        public float BaseSpeed;
        public float Speed; 
        public bool IsActive;
        public float Size;
        
        public float Health;
        public float InvincibilityTimer;
        
        public int Type; // 0=Normal, 1=Rápido, 2=Tanque, 3=Jefe, 4=Bachaquero, 5=Malandro, 6=Borrachito, 7=Lanza-Piedras, 8=Camionetica
        public float FreezeTimer;
        
        public float DashTimer;
        public Vector2 DashDirection;
        public bool IsDashing;

        public float PuddleDamageTimer;
        
        public float StateTimer; // Temporizador genérico para la máquina de estados de la IA
        
        public int CurrentFrame;
        public float FrameTimer;
        public int CurrentRow;
    }

    public struct Gem
    {
        public Vector2 Position;
        public bool IsActive;
        public float Size;
        public int Value;
    }

    public struct Projectile
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsActive;
        public float Size;
    }

    public struct BouncingProjectile
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsActive;
        public int BouncesLeft;
        public int TargetIndex;
        public float Size;
    }

    public struct Puddle
    {
        public Vector2 Position;
        public bool IsActive;
        public float Radius;
        public float LifeTime;
    }

    public struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float LifeTime;
        public float Size;
        public bool IsActive;
    }
}
