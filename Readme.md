# Sobreviviente Callejero - Multiverso 3D

Un clon de Vampire Survivors con arquitectura Data-Oriented Design (DOD) en C#, que incluye modos de cámara 2D (Top-Down) y 3D (FPS), dificultad dinámica y soporte para jefes.

## Requisitos Previos

Para compilar y correr este juego desde la terminal, necesitas instalar:
**Para Linux (Ubuntu):**
* Abre la terminal y ejecuta: `sudo apt-get install -y dotnet-sdk-8.0`

**Para Windows:**
* Descarga el instalador del ".NET 8.0 SDK" desde la página oficial de Microsoft (https://dotnet.microsoft.com/download/dotnet/8.0) e instálalo con "Siguiente > Siguiente".
* Alternativamente, si usas Windows 11, puedes abrir PowerShell y ejecutar: `winget install Microsoft.DotNet.SDK.8`
## Estructura de Assets Requerida

El juego requiere una carpeta `Assets/` en el directorio raíz con los siguientes archivos multimedia para poder iniciar sin errores:

* **Imágenes (.png):** `player.png`, `menu_bg.png`, `bg.png`, y sprites de enemigos (`enemy_0.png` al `enemy_8.png`).
* **Audios (.wav):** `shoot.wav`, `machete.wav`, `hit.wav`, `pickup.wav`.

## Cómo Jugar

Clona el repositorio, abre la terminal (o PowerShell en Windows) en la carpeta del proyecto y ejecuta:

`dotnet run`
