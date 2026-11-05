# Super Mario Bros. — Proyecto grupal

Juego de plataformas 2D en Unity con cuatro niveles, selección de Mario o Peach, enemigos y combate contra Bowser.

## Integrantes

| Integrante | Aporte principal | Rama |
|---|---|---|
| Kathlen | HUD, monedas y puntuación | feature/kathlen |
| Jose | Temporizador y muerte al agotarse el tiempo | feature/jose |
| Dilan | Música, efectos, volumen y correcciones de colisiones y enemigos | feature/dilan-audio |
| Nazareth | Menú, personajes y ajustes de plataformas | feature/nazaret-menu |

## Cómo jugar

1. Abrir el proyecto con **Unity 6000.3.16f1** y esperar la importación.
2. Abrir **Assets/Scenes/Menu.unity** y pulsar **Play**.
3. Elegir **Jugar**, un personaje y el nivel.
4. Usar **A/D o flechas** para moverse y **Espacio** para saltar o impulsarse en el agua.
5. En el menú, **Volumen** permite ajustar música y efectos por separado.

## Funciones integradas

- Cuatro niveles: superficie, subterráneo, acuático y castillo.
- HUD con puntuación, monedas, vidas, mundo y tiempo.
- Temporizador de 400 segundos por nivel; al llegar a cero el personaje muere.
- Música y efectos con preferencias de volumen guardadas.
- Pisotones, caparazones, enemigos que cambian de rumbo y rescate de Peach.

## Trabajo en equipo

**develop** integra las cuatro ramas conservando sus commits. Cada integrante sigue trabajando en su rama y propone sus cambios hacia develop. **main** se mantiene como base hasta que el equipo decida publicar la integración.

Para revisar la integración, ejecutar en orden:

    git fetch origin
    git switch develop
    git pull --ff-only origin develop

La integración conserva puntuación y protección contra daño repetido, conecta el tiempo al HUD y mantiene las opciones de volumen independientes del menú.

## Créditos

Proyecto basado en el [tutorial de Super Mario de Zigurous](https://github.com/zigurous/unity-super-mario-tutorial). Se conservan los recursos y créditos incluidos en el repositorio.