using System.IO;
using Raylib_cs;

namespace VampireSurvivorsClone
{
    public static class SoundManager
    {
        // Array fijo de sonidos para evitar asignaciones dinámicas (GC)
        public static Sound[] sounds = new Sound[10];

        public static void Init()
        {
            // Cargamos los sonidos solo si existen para evitar crashes
            if (File.Exists("Assets/shoot.wav")) sounds[0] = Raylib.LoadSound("Assets/shoot.wav");
            if (File.Exists("Assets/machete.wav")) sounds[1] = Raylib.LoadSound("Assets/machete.wav");
            if (File.Exists("Assets/hit.wav")) sounds[2] = Raylib.LoadSound("Assets/hit.wav");
            if (File.Exists("Assets/pickup.wav")) sounds[3] = Raylib.LoadSound("Assets/pickup.wav");
        }

        public static void Play(int index)
        {
            // Verificamos que el índice sea válido y que el sonido se haya cargado correctamente (FrameCount > 0)
            if (index >= 0 && index < sounds.Length && sounds[index].FrameCount > 0)
            {
                Raylib.PlaySound(sounds[index]);
            }
        }

        public static void Unload()
        {
            // Liberamos la memoria de todos los sonidos cargados
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].FrameCount > 0)
                {
                    Raylib.UnloadSound(sounds[i]);
                }
            }
        }
    }
}
