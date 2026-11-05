using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Construye escenas editables: los bloques y enemigos quedan guardados en la jerarquía.
public static class AdventureBuilder
{
    static string mode;
    static Transform scenery;
    [MenuItem("Mario/Crear niveles de la aventura")]
    public static void Build()
    {
        MakeSound("Cueva", new float[] { 130, 196, 155, 207, 130, 0, 98, 0 }, .22f);
        MakeSound("Agua", new float[] { 262, 330, 392, 523, 440, 392, 330, 294, 262, 330, 392, 294 }, .26f);
        MakeSound("Salto", new float[] { 280, 400, 560 }, .045f);
        MakeSound("Moneda", new float[] { 880, 1320 }, .09f);
        MakeSound("Golpe", new float[] { 330, 220, 165, 110 }, .12f);
        MakeSound("Salida", new float[] { 262, 330, 392, 523, 659, 784 }, .12f);
        MakeSound("Final", new float[] { 392, 330, 294, 196, 0, 196 }, .28f);
        PixelSprite("Fish", new[] { "....RRRR....", "...RRWWRR...", "..RRRWKWRR..", "RRRRRRRRRRR.", "RRRRRRRRRRR.", "..RRRRRRRR..", "...WWWWWW...", "....RRRR...." });
        PixelSprite("Coral", new[] { "...P....", "P..P..P.", "P..P..P.", "PP.P.PP.", ".PPPPPP.", "...PP...", "...PP...", "..PPPP.." });
        PixelSprite("Bubble", new[] { ".WWW.", "W...W", "WW..W", "W...W", ".WWW." });
        AssetDatabase.Refresh();
        MakeUnderground();
        MakeWater();
        MakeEnding("GameOver", true);
        MakeEnding("Completed", false);
        var first = EditorSceneManager.OpenScene("Assets/Scenes/1-1.unity");
        foreach (var flag in UnityEngine.Object.FindObjectsByType<FlagPole>(FindObjectsSortMode.None)) { flag.nextWorld = 1; flag.nextStage = 2; }
        if (!UnityEngine.Object.FindFirstObjectByType<AdventureHud>()) new GameObject("Marcador").AddComponent<AdventureHud>().title = "MUNDO 1-1";
        AddAudio("Superficie");
        EditorSceneManager.SaveScene(first);
        CastleBuilder.Build();
        string[] names = { "Menu", "1-1", "1-2", "1-3", "1-4", "GameOver", "Completed" };
        EditorBuildSettings.scenes = Array.ConvertAll(names, n => new EditorBuildSettingsScene("Assets/Scenes/" + n + ".unity", true));
        AssetDatabase.SaveAssets();
        Debug.Log("ADVENTURE_BUILD_OK: 5 escenas guardadas y conectadas");
    }
    internal static void Begin(string name, Color color, bool water, int end)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        mode = name;
        scenery = new GameObject("Escenario " + name).transform;
        var mario = Spawn("Mario", 3, 1);
        mario.GetComponent<PlayerMovement>().swimming = water;
        if (water) mario.GetComponent<PlayerMovement>().moveSpeed = 5;
        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(10, 6, -10);
        var camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true; camera.orthographicSize = 7; camera.backgroundColor = color;
        cameraObject.AddComponent<AudioListener>();
        var follow = cameraObject.AddComponent<SideScrolling>();
        follow.bounded = true; follow.leftBound = 12; follow.rightBound = end - 12;
        new GameObject("Game Manager").AddComponent<GameManager>();
        new GameObject("Marcador").AddComponent<AdventureHud>().title = water ? "MUNDO 1-3 · BAJO EL AGUA" : "MUNDO 1-2 · SUBTERRÁNEO";
        AddAudio(water ? "Agua" : "Cueva");
        var death = new GameObject("Caída fuera del nivel");
        death.transform.position = new Vector3(end / 2f, -5);
        var box = death.AddComponent<BoxCollider2D>(); box.size = new Vector2(end + 30, 2); box.isTrigger = true;
        death.AddComponent<DeathBarrier>();
        Blocks("HardBlock", -1, 0, 1, 14);
    }
    static void MakeUnderground()
    {
        Begin("1-2", new Color(.015f, .018f, .04f), false, 174);
        // Recorrido inspirado en 1-2: techo de ladrillo, columnas, fosos,
        // plataformas móviles y la escalera que conduce a la tubería final.
        for (int x = 0; x < 174; x++)
        {
            if (!(x >= 74 && x <= 77) && !(x >= 115 && x <= 119)) Blocks("UndergroundBlock", x, -2, 1, 2);
            if (x < 145 && !(x > 70 && x < 84) && !(x > 109 && x < 126)) Blocks("UndergroundBrick", x, 12, 1, 1);
        }
        for (int x = 12; x <= 24; x += 3) { Blocks("UndergroundBrick", x, 4, 2, 1); Coins(x, 6, 2); }
        for (int x = 31; x < 57; x += 4) { Blocks("UndergroundBlock", x, 0, 2, 2 + (x / 4 % 3)); Coins(x, 6, 2); }
        Blocks("UndergroundBrick", 61, 4, 9, 1); Coins(61, 6, 9);
        for (int x = 83; x < 106; x += 7) { Blocks("UndergroundBrick", x, 3 + x % 3, 4, 1); Coins(x, 7, 3); }
        Platform(76, 2, 4, 2.2f); Platform(117.5f, 3, 5, 3);
        Blocks("UndergroundBrick", 125, 4, 12, 1); Coins(125, 6, 12);
        for (int i = 0; i < 7; i++) Blocks("UndergroundBlock", 143 + i, 0, 1, i + 1);
        foreach (int x in new[] { 19, 28, 60, 69, 89, 101, 129, 138 }) Spawn("Goomba", x, 1);
        Spawn("MagicMushroom", 15, 5.5f);
        Exit(166, false);
        Save();
    }
    static void MakeWater()
    {
        Begin("1-3", new Color(.02f, .18f, .50f), true, 170);
        for (int x = 0; x < 170; x++)
        {
            if (!(x > 48 && x < 53) && !(x > 106 && x < 111)) Blocks("Ground", x, -2, 1, 2);
            Blocks("HardBlock", x, 13, 1, 1);
        }
        // Alterna pasos altos y bajos como el recorrido clásico de agua.
        foreach (int x in new[] { 20, 43, 67, 91, 119, 142 })
        {
            bool tall = x == 43 || x == 91 || x == 142;
            Blocks("HardBlock", x, 0, 3, tall ? 7 : 3);
            Blocks("HardBlock", x + 9, 9, 3, 4);
            Coins(x + 1, tall ? 9 : 5, 5);
            for (int j = 0; j < 3; j++) Decor("Coral", x + j * .7f, tall ? 7.5f : 3.5f, new Color(1,.45f,.6f), 1.5f);
        }
        for (int x = 14; x < 155; x += 13)
        {
            var fish = Decor("Fish", x, 5 + x % 4, Color.white, 1.8f);
            fish.AddComponent<BoxCollider2D>().isTrigger = true;
            var movement = fish.AddComponent<SwimmingHazard>(); movement.phase = x; movement.distance = 2;
        }
        for (int x = 6; x < 162; x += 5) Decor("Bubble", x, 2, new Color(1,1,1,.45f), .4f).AddComponent<WaterBubbles>();
        Blocks("HardBlock", 156, 0, 8, 3);
        Exit(164, false);
        Save();
    }
    static void MakeEnding(string name, bool lost)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var cam = new GameObject("Main Camera").AddComponent<Camera>();
        cam.tag = "MainCamera"; cam.backgroundColor = new Color(.025f,.035f,.08f); cam.clearFlags = CameraClearFlags.SolidColor;
        cam.gameObject.AddComponent<AudioListener>();
        new GameObject("Game Manager").AddComponent<GameManager>();
        var hud = new GameObject("Pantalla final").AddComponent<AdventureHud>();
        hud.gameOver = lost; hud.completed = !lost;
        AddAudio(lost ? "Final" : "Salida");
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/" + name + ".unity");
    }
    internal static GameObject Spawn(string prefab, float x, float y)
    {
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + prefab + ".prefab");
        if (!asset) throw new Exception("Falta prefab: " + prefab);
        var obj = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        obj.transform.position = new Vector3(x, y, 0); return obj;
    }
    static void Blocks(string prefab, int x, int y, int width, int height)
    {
        if (mode == "1-2") prefab = prefab == "UndergroundBlock" ? "Ground" : prefab == "UndergroundBrick" ? "Brick" : prefab;
        if (mode == "1-3" && (prefab == "Ground" || prefab == "HardBlock")) prefab = "UndergroundBlock";
        for (int dx = 0; dx < width; dx++) for (int dy = 0; dy < height; dy++)
        {
            var block = Spawn(prefab, x + dx + .5f, y + dy + .5f);
            block.transform.SetParent(scenery);
        }
    }
    static void Coins(int x, int y, int count) { for (int i = 0; i < count; i++) Spawn("Coin", x + i + .5f, y); }
    static void Platform(float x, float y, float width, float travel)
    {
        var obj = Spawn("UndergroundBlock", x, y);
        obj.transform.localScale = new Vector3(width, .5f, 1);
        var platform = obj.AddComponent<AdventurePlatform>(); platform.travel = travel;
    }
    static void Exit(int x, bool last)
    {
        for (int y = 0; y < 4; y++)
        {
            var part = Decor(y == 3 ? "PipeTop" : "PipeBottom", x + 2, y + .5f, Color.white, 1);
            part.transform.localScale = new Vector3(2, 1, 1);
        }
        var obj = new GameObject("Salida hacia " + (last ? "el final" : "el agua")); obj.transform.position = new Vector3(x, 2);
        var box = obj.AddComponent<BoxCollider2D>(); box.size = new Vector2(1, 5); box.isTrigger = true;
        obj.AddComponent<AdventureExit>().lastLevel = last;
    }
    static GameObject Decor(string sprite, float x, float y, Color color, float scale)
    {
        var obj = new GameObject(sprite); obj.transform.position = new Vector3(x,y,0); obj.transform.localScale = Vector3.one * scale;
        var render = obj.AddComponent<SpriteRenderer>();
        render.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Adventure/" + sprite + ".png") ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + sprite + ".png");
        render.color = color; return obj;
    }
    static void Save() { EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/" + mode + ".unity"); }
    internal static void AddAudio(string theme)
    {
        var audio = UnityEngine.Object.FindFirstObjectByType<AdventureAudio>();
        if (!audio) audio = new GameObject("Sonidos y música").AddComponent<AdventureAudio>();
        audio.music = Clip(theme); audio.jump = Clip("Salto"); audio.coin = Clip("Moneda"); audio.hurt = Clip("Golpe"); audio.finish = Clip("Salida");
        audio.life = Clip("Vida"); audio.pipe = Clip("Tuberia"); audio.powerup = Clip("Mejora"); audio.stomp = Clip("Pisoton"); audio.brick = Clip("Romper"); audio.flag = Clip("Bandera");
        audio.loopMusic = theme != "Final" && theme != "Salida";
    }
    static AudioClip Clip(string name) => AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name + ".mp3") ?? AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/" + name + ".wav");
    static void MakeSound(string name, float[] notes, float beat)
    {
        if (File.Exists("Assets/Audio/" + name + ".wav") || File.Exists("Assets/Audio/" + name + ".mp3")) return;
        const int rate = 22050;
        int perNote = (int)(rate * beat), samples = perNote * notes.Length;
        using (var writer = new BinaryWriter(File.Create("Assets/Audio/" + name + ".wav")))
        {
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + samples * 2);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt ")); writer.Write(16); writer.Write((short)1); writer.Write((short)1);
            writer.Write(rate); writer.Write(rate * 2); writer.Write((short)2); writer.Write((short)16);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(samples * 2);
            for (int i = 0; i < samples; i++)
            {
                float t = (i % perNote) / (float)rate;
                float envelope = Mathf.Min(t * 80, 1) * Mathf.Clamp01((beat - t) * 20);
                writer.Write((short)(Mathf.Sin(2 * Mathf.PI * notes[i / perNote] * t) * envelope * 10000));
            }
        }
    }
    static void PixelSprite(string name, string[] rows)
    {
        string path = "Assets/Sprites/Adventure/" + name + ".png";
        // Unity puede tener la textura abierta para generar su previsualización.
        // Los recursos existentes se conservan; no hace falta reescribirlos.
        if (File.Exists(path)) return;
        var texture = new Texture2D(rows[0].Length, rows.Length);
        for (int y=0; y<rows.Length; y++) for (int x=0; x<rows[y].Length; x++)
        {
            char c = rows[y][x];
            texture.SetPixel(x, rows.Length-y-1, c=='R' ? new Color(1,.25f,.15f) : c=='P' ? new Color(1,.35f,.6f) : c=='W' ? Color.white : c=='K' ? Color.black : Color.clear);
        }
        texture.Apply(); File.WriteAllBytes(path, texture.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite; importer.spritePixelsPerUnit = 16; importer.filterMode = FilterMode.Point; importer.textureCompression = TextureImporterCompression.Uncompressed; importer.SaveAndReimport();
    }
}
