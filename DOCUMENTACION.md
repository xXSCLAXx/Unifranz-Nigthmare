# FIVE NIGHTS IN UNIFRANZ — Documentación del Proyecto

## 1. Introducción

**Five Nights in Unifranz** es un videojuego de terror y supervivencia tipo _point-and-click_ desarrollado en **Unity**, inspirado en la mecánica de _Five Nights at Freddy's (FNaF)_ y adaptado con un efecto visual panorámico (Panorama Effect). El jugador debe sobrevivir una noche completa en la universidad Unifranz, completando tareas en una PC mientras gestiona múltiples amenazas que aparecen aleatoriamente. El proyecto incluye un sistema de vidas, minijuegos de reparación, un informe con temporizador global, y un efecto visual estilo _Panorama_ que emula la estética de los primeros títulos de FNaF. Es una adaptación del concepto original "FNaF Panorama" portado a Unity.

## 2. Objetivo

Desarrollar un videojuego completo en Unity que combine elementos de horror de supervivencia con mecánicas de gestión de recursos y resolución de tareas. El jugador debe completar 5 módulos de programación (HTML, CSS, JS, Python) en una PC antes de que se agote el tiempo o antes de ser eliminado por las distintas amenazas del juego (PC4, WiFi, Informe). El proyecto busca recrear la atmósfera tensa de los juegos FNaF en un entorno universitario, implementando un sistema de panorama shader para el estilo visual retro característico.

## 3. Herramientas Usadas

- **Motor:** Unity 2020.1.4f1
- **Lenguaje:** C# (scripts de game mechanics, UI, minijuegos)
- **Shader:** HLSL/ShaderLab (Panorama shader para efecto visual)
- **Plataforma destino:** Android (APK generado en `Builds/FNaF-Panorama.apk`)
- **Entorno de desarrollo:** Visual Studio / VS Code
- **Control de versiones:** Git
- **Assets:** Recursos propios (texturas, audio, video) almacenados en `Assets/Resources/` y `Assets/StreamingAssets/`

## 4. Instrucciones del Juego

### Paso 1 — Objetivo principal

Sobreviví la noche completando los **5 módulos de la PC** (tareas de programación HTML, CSS, JS y Python). Tenés **3 vidas**. Si las perdés todas → **Game Over**.

### Paso 2 — Navegación y áreas

- **Vista principal:** Movete con click izquierdo arrastrando o usando los bordes de la pantalla.
- **Puerta izquierda:** Click en la zona izquierda → accedés al **Aula 306** (allí está el Informe).
- **Puerta derecha:** Click en la zona derecha → accedés a la **Sala de Cómputo** (allí está la PC4).
- **PC:** Click en la computadora del centro para abrir los módulos.
- **Router/WiFi:** Click en el router al lado de la PC para revisarlo.
- **Notas (Post-it):** Click en el post-it al lado de la PC.

### Paso 3 — Amenazas a gestionar

- **PC4 (Sala de Cómputo):** Cada vez que hacés click en la sala principal hay 5-12% de chance de que se rompa. Si se rompe, entrá a repararla con el minijuego de cables antes de que explote (20s). Si explota → screamer + 1 vida.
- **WiFi (Router):** Si te olvidás del router, te agarra un screamer → perdés 1 vida.
- **Timer de la PC:** Mientras hacés una tarea, un timer interno de 10s corre. Si llega a 0 → screamer + pérdida de la tarea + 1 vida. Hacé tareas secundarias para extenderlo.
- **Informe (Aula 306):** Tiene un ciclo global de 40s de cooldown + 45s de peligro. Si no corregís el error a tiempo en los 45s → **Game Over instantáneo** (pantalla de sangre, perdés todas las vidas). El timer corre aunque cierres la ventana.

### Paso 4 — Tareas secundarias y victoria

- Completá los **5 módulos de la PC** para ganar la partida (pantalla de victoria).
- **Tareas secundarias (bonus de tiempo):**
  - *Informe:* Corregir error → +2.5s al timer de la PC.
  - *PC4:* Reparar → +2s.
  - *Router:* Reparar → +1s.
- Usá las tareas secundarias para ganar tiempo y no morir en el intento.
- Si ves el **overlay rojo** o escuchás la **alarma del Informe**, deja todo y corregí el error YA.
