# Sobreviviente Callejero - Multiverso 3D

Un clon de Vampire Survivors con arquitectura Data-Oriented Design (DOD) en C#, que incluye modos de cámara 2D (Top-Down) y 3D (FPS), dificultad dinámica y soporte para jefes.

## Requisitos Previos

Para compilar y correr este juego desde la terminal, necesitas instalar:

1. **.NET 8.0 SDK**: El framework base del proyecto. En Ubuntu, puedes instalarlo con `sudo apt-get install -y dotnet-sdk-8.0`.
2. **Raylib-cs (v6.0.0)**: La librería gráfica y de audio. Se descarga automáticamente al compilar porque ya está definida en el archivo `.csproj`.

## Estructura de Assets Requerida

El juego requiere una carpeta `Assets/` en el directorio raíz con los siguientes archivos multimedia para poder iniciar sin errores:

* **Imágenes (.png):** `player.png`, `menu_bg.png`, `bg.png`, y sprites de enemigos (`enemy_0.png` al `enemy_8.png`).
* **Audios (.wav):** `shoot.wav`, `machete.wav`, `hit.wav`, `pickup.wav`.

## Cómo Jugar

Clona el repositorio, abre la terminal en la carpeta del proyecto y ejecuta:

`dotnet run`
