# Audio de Dilan

Rama: feature/dilan-audio. Unity: 6000.3.16f1.
AudioManager contiene la logica; AdventureAudio hereda de el para conservar los componentes y referencias de las escenas. No agregar otro administrador a los niveles existentes.

## Recursos
- Salto y mejora: se conservan los clips ya asignados; Jump.wav y PowerUp.wav sirven como respaldo.
- Pisoton: Assets/Audio/Pisoton.wav, ya conectado a Goomba.
- Danio: Resources/DilanAudio/Damage.wav, procedente de smb_kick.wav.
- Muerte: Resources/DilanAudio/Death.wav, procedente de smb_mariodie.wav.
- Musica: la asignada a cada nivel del proyecto.

## Prueba en Unity
1. Abrir este repositorio con Unity 6000.3.16f1. Esperar que termine de importar y compilar.
2. Abrir Window > General > Console: no debe haber errores rojos.
3. Ejecutar Dilan > Validar recursos de audio. Debe aparecer DILAN_AUDIO_OK.
4. Abrir Assets/Scenes/1-1.unity y pulsar Play. Comprobar musica y salto.
5. Saltar encima de un Goomba: debe sonar el pisoton.
6. Recoger un hongo o estrella: debe sonar la mejora.
7. Siendo grande y sin estrella, tocar un enemigo lateralmente: debe sonar danio y Mario debe encogerse.
8. Siendo pequeno, tocar un enemigo: debe sonar muerte y detenerse la musica. Tras reiniciar, la musica vuelve una sola vez.
9. Repetir en 1-2, 1-3 y 1-4: cada nivel conserva su musica.
10. Durante Play, seleccionar el objeto con AdventureAudio (Sonidos y musica). En el menu de tres puntos del componente, usar Probar audio para oir cada efecto por separado. Death detiene la musica; detener y volver a iniciar Play para recuperarla.
11. Ajustar Music Volume a 0: se silencian solo la musica. Ajustar Effects Volume a 0: se silencian los efectos. Los cambios durante Play se descartan al salir.
12. No debe haber eco al reiniciar. El administrador rechaza otro administrador activo y llamadas al mismo clip separadas por 0.05 segundos o menos.

## Integracion con Jose
Metodos disponibles: AudioManager.Instance.PlayJump(), PlayStomp(), PlayPowerUp(), PlayDamage(), PlayDeath().
SetVolumes(musica, efectos) recibe valores entre 0 y 1.
Los eventos GameplayEvents aun no existen en esta rama. Los scripts actuales ya llaman al audio directamente. Cuando se integren eventos, sustituir esas llamadas por suscripciones; no mantener ambos caminos porque duplicarian los sonidos. No se modificaron las escenas ni se invento una API de eventos.

## Alcance
Se modifica Player.cs solo para separar danio y muerte; coordinar ese cambio con Jose.
Los archivos .vscode/settings.json y SUPER-MArio.slnx son cambios locales previos y no forman parte de esta entrega.
La validacion automatica verifica importacion, compilacion y referencias; la escucha y la prueba jugable requieren Unity con sonido.
## Panel de volumen independiente del menu
AudioVolumeMenu se agrega en ejecucion a Menu/MainMenu o una escena con LevelMenu. Aparece el boton Volumen abajo a la derecha; abre dos sliders y Guardar y cerrar. No se modificaron Menu.unity ni LevelMenu.cs.
Los valores globales se aplican a AudioManager, se conservan entre niveles y se guardan al cerrar el panel o salir.
Para un menu con otro nombre, agregar AudioVolumeMenu a un objeto manualmente.

Integracion con botones propios de Nazaret:
- Agregar AudioVolumeMenu a un objeto activo y desmarcar Draw Built In Controls.
- Boton de abrir: OnClick -> AudioVolumeMenu.Open o Toggle.
- Boton de cerrar: OnClick -> AudioVolumeMenu.Close.
- Slider musica: rango 0..1; OnValueChanged(float) dinamico -> SetMusicVolume.
- Slider efectos: rango 0..1; OnValueChanged(float) dinamico -> SetEffectsVolume.
- Al abrir su panel, inicializar sliders con SetValueWithoutNotify(AudioVolumeSettings.Music/Effects).
- IsOpen permite consultar el estado; el panel personalizado debe activar/desactivar su propia vista.
- Si Nazaret usa AudioSources propios para musica de menu, aplicar AudioVolumeSettings.Music y escuchar Changed; AudioManager ya lo hace para la partida.

Prueba: abrir Menu, pulsar Volumen, bajar musica a 0 y efectos a 50%, cerrar y entrar al nivel. No debe sonar musica pero si el salto. Volver al menu, invertir valores y comprobar el resultado inverso. Reiniciar el juego y verificar que se conservan. Restaurar los valores deseados. Los controles se guardan incluso si se ajustan en Play.
