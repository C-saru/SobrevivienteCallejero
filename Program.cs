using System;
using System.Numerics;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    class Program
    {
        static void Main(string[] args)
        {
            const int screenWidth = 1280;
            const int screenHeight = 720;

            Raylib.InitWindow(screenWidth, screenHeight, "Sobreviviente Callejero - Multiverso 3D");
            Raylib.InitAudioDevice(); // Inicializa el dispositivo de audio
            Raylib.SetTargetFPS(60);
            Raylib.SetExitKey(KeyboardKey.Null);

            SoundManager.Init(); // Carga los sonidos en memoria

            Image playerImage = Raylib.LoadImage("Assets/player.png");
            Raylib.ImageColorReplace(ref playerImage, Color.White, new Color(0, 0, 0, 0));
            Texture2D playerTexture = Raylib.LoadTextureFromImage(playerImage);
            Raylib.UnloadImage(playerImage);

            Texture2D menuBg = Raylib.LoadTexture("Assets/menu_bg.png");
            
            Texture2D titleBgTexture = new Texture2D();
            if (System.IO.File.Exists("Assets/TitleScreen.png"))
            {
                titleBgTexture = Raylib.LoadTexture("Assets/TitleScreen.png");
            }
            
            Texture2D[] enemyTextures = new Texture2D[9];
            for (int i = 0; i < 9; i++) {
                string path = $"Assets/enemy_{i}.png";
                if (System.IO.File.Exists(path)) enemyTextures[i] = Raylib.LoadTexture(path);
            }
            Texture2D bgTexture = Raylib.LoadTexture("Assets/bg.png");

            Player player = new Player();
            GameManager.ResetGame(ref player);

            Camera2D camera = new Camera2D();
            camera.Offset = new Vector2(screenWidth / 2.0f, screenHeight / 2.0f);
            camera.Rotation = 0.0f;
            camera.Zoom = 1.0f;

            // --- NUEVA CÁMARA 3D ---
            Camera3D camera3D = new Camera3D();
            camera3D.Position = new Vector3(0.0f, 10.0f, 0.0f);
            camera3D.Target = new Vector3(0.0f, 10.0f, 1.0f);
            camera3D.Up = new Vector3(0.0f, 1.0f, 0.0f);
            camera3D.FovY = 60.0f;
            camera3D.Projection = CameraProjection.Perspective;

            while (!Raylib.WindowShouldClose() && !GameManager.QuitGame)
            {
                float deltaTime = Raylib.GetFrameTime();
                GameManager.Update(deltaTime, ref player, ref camera, screenWidth, screenHeight);
                RenderManager.Draw(ref player, ref camera, ref camera3D, ref playerTexture, ref menuBg, ref titleBgTexture, enemyTextures, ref bgTexture, screenWidth, screenHeight);
            }

            SoundManager.Unload(); // Libera la memoria de los sonidos
            Raylib.UnloadTexture(playerTexture);
            Raylib.UnloadTexture(menuBg);
            if (titleBgTexture.Id != 0) Raylib.UnloadTexture(titleBgTexture);
            for (int i = 0; i < 9; i++) { if (enemyTextures[i].Id != 0) Raylib.UnloadTexture(enemyTextures[i]); }
            Raylib.UnloadTexture(bgTexture);
            Raylib.CloseAudioDevice(); // Cierra el dispositivo de audio
            Raylib.CloseWindow();
        }
    }
}
