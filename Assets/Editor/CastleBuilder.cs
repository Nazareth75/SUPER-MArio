using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CastleBuilder
{
    const string Root = "Assets/CastilloRosado/";
    [MenuItem("Mario/Crear castillo y menú de niveles")]
    public static void Build()
    {
        foreach (string file in Directory.GetFiles(Root, "*.png", SearchOption.AllDirectories))
        {
            string path = file.Replace('\\', '/');
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        MakeCastle();
        MakeMenu();
        // El agua conduce al castillo; se conservan los otros diseños existentes.
        var water = EditorSceneManager.OpenScene("Assets/Scenes/1-3.unity");
        foreach (var exit in Object.FindObjectsByType<AdventureExit>(FindObjectsSortMode.None)) exit.lastLevel = false;
        EditorSceneManager.SaveScene(water);
        var end = EditorSceneManager.OpenScene("Assets/Scenes/Completed.unity");
        EditorSceneManager.SaveScene(end);
        string[] names = { "Menu", "1-1", "1-2", "1-3", "1-4", "GameOver", "Completed" };
        EditorBuildSettings.scenes = System.Array.ConvertAll(names, n => new EditorBuildSettingsScene("Assets/Scenes/" + n + ".unity", true));
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        AssetDatabase.SaveAssets();
        Debug.Log("CASTLE_BUILD_OK: castillo rosado, Bowser, Peach y menú de cuatro niveles listos.");
    }
    static Sprite Art(string file) => AssetDatabase.LoadAssetAtPath<Sprite>(Root + file + ".png");
    static GameObject Piece(string file, float x, float y, float width, float height, bool solid = false, int order = 0)
    {
        var obj = new GameObject(Path.GetFileName(file));
        obj.transform.position = new Vector3(x, y, 0);
        var image = obj.AddComponent<SpriteRenderer>(); image.sprite = Art(file); image.sortingOrder = order;
        if (!image.sprite) throw new System.Exception("Falta sprite: " + file);
        Vector2 size = image.sprite.bounds.size;
        obj.transform.localScale = new Vector3(width / size.x, height / size.y, 1);
        if (solid) obj.AddComponent<BoxCollider2D>().size = size;
        return obj;
    }
    static void MakeCastle()
    {
        AdventureBuilder.Begin("1-4", new Color(.21f, .035f, .12f), false, 116);
        Object.FindFirstObjectByType<AdventureHud>().title = "MUNDO 1-4 · CASTILLO ROSADO";
        var audio = Object.FindFirstObjectByType<AdventureAudio>();
        var castleMusic = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Castillo.mp3");
        if (castleMusic) audio.music = castleMusic;
        for (int x = 0; x < 116; x += 2)
        {
            bool gap = (x >= 20 && x < 26) || (x >= 44 && x < 52) || (x >= 68 && x < 74);
            if (!gap) Piece("Tiles/Floor_01", x + 1, -.5f, 2, 1, true);
            Piece("Tiles/BrickPink", x + 1, 12.5f, 2, 1, true);
        }
        for (int x = 7; x < 116; x += 12)
        {
            var arch = Piece("Tiles/Arch_01", x, 5.5f, 6, 10, false, -6);
            arch.GetComponent<SpriteRenderer>().color = new Color(.5f,.3f,.4f);
            Piece("Decor/Window", x, 7.2f, 2, 3, false, -5);
            Piece("Decor/Banner", x + 3.6f, 8, 1.2f, 3, false, -4);
            var torch = Piece("Decor/Torch", x - 3, 3.6f, .7f, 1.3f, false, -2);
            torch.AddComponent<CastleGlow>();
        }
        foreach (int x in new[] { 20, 44, 68 })
        {
            int width = x == 44 ? 8 : 6;
            var lava = Piece("Hazards/Lava_01", x + width / 2f, -.8f, width, .7f);
            lava.AddComponent<BoxCollider2D>().isTrigger = true;
            lava.AddComponent<CastleHazard>().frames = new[] { Art("Hazards/Lava_01"), Art("Hazards/Lava_02"), Art("Hazards/Lava_03") };
            var platform = Piece("Platforms/MovingPlatform_01", x + width / 2f, 1.7f, 3, .45f, true);
            platform.AddComponent<AdventurePlatform>().travel = 1.3f;
        }
        for (int i = 0; i < 3; i++)
        {
            Piece("Tiles/BrickPinkTop", 32 + i * 2, 1 + i, 2, .7f, true);
            Piece("Tiles/BrickPinkTop", 77 + i * 2, 1 + i, 2, .7f, true);
        }
        foreach (int x in new[] { 13, 31, 39, 57, 63, 78, 84 })
        {
            var enemy = AdventureBuilder.Spawn("Goomba", x, 1);
            enemy.GetComponent<AnimatedSprite>().sprites = new[] { Art("Enemies/GoombaPink_01"), Art("Enemies/GoombaPink_02") };
            // Estos recortes son mayores que los sprites del Mario original.
            enemy.GetComponent<SpriteRenderer>().sprite = Art("Enemies/GoombaPink_01");
            Vector2 size = Art("Enemies/GoombaPink_01").bounds.size;
            enemy.transform.localScale = Vector3.one * (1.1f / size.y);
            enemy.GetComponent<CircleCollider2D>().radius = size.y * .4f;
        }
        foreach (int x in new[] { 10, 17, 28, 35, 41, 54, 61, 76, 82, 87 })
            for (int i = 0; i < 3; i++) AdventureBuilder.Spawn("Coin", x + i, 4.7f);
        AdventureBuilder.Spawn("MagicMushroom", 9, 1);
        Piece("Doors/BossDoor", 89, 3, 4, 6, false, -3);
        var bowser = Piece("Bowser/Bowser_Idle_01", 98, 1.6f, 3.5f, 3.2f, true, 3);
        bowser.layer = LayerMask.NameToLayer("Enemy");
        var collider = bowser.GetComponent<BoxCollider2D>(); collider.size *= .8f;
        var boss = bowser.AddComponent<CastleBoss>();
        boss.idle = Art("Bowser/Bowser_Idle_01"); boss.idleAlternate = Art("Bowser/Bowser_Idle_02");
        boss.attack = Art("Bowser/Bowser_Attack"); boss.damage = Art("Bowser/Bowser_Damage"); boss.death = Art("Bowser/Bowser_Death");
        boss.fire = Art("Hazards/FireballSmall");
        boss.fireSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/FuegoBowser.wav");
        Piece("Decor/Throne", 110, 1.8f, 3, 3.6f, false, -1);
        var peach = Piece("Peach/Peach_Idle_01", 110, 1.2f, 1.7f, 2.4f, false, 2);
        peach.AddComponent<CastleGlow>();
        var gate = Piece("Doors/CastleGate", 106, 2, 2.5f, 4, true, 1);
        var rescue = new GameObject("Rescatar a Peach"); rescue.transform.position = new Vector3(109, 2);
        var trigger = rescue.AddComponent<BoxCollider2D>(); trigger.size = new Vector2(4, 4); trigger.isTrigger = true;
        var goal = rescue.AddComponent<CastleGoal>(); goal.boss = boss; goal.gate = gate; goal.peach = peach.transform;
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/1-4.unity");
    }
    static void MakeMenu()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Main Camera").AddComponent<Camera>(); camera.tag = "MainCamera";
        camera.transform.position = new Vector3(0, 4, -10); camera.orthographic = true; camera.orthographicSize = 7;
        camera.backgroundColor = new Color(.17f,.025f,.09f); camera.gameObject.AddComponent<AudioListener>();
        new GameObject("Game Manager").AddComponent<GameManager>();
        new GameObject("Elegir nivel").AddComponent<LevelMenu>();
        Piece("Doors/BossDoor", -8, 4, 5, 10, false, -1);
        Piece("Doors/BossDoor", 8, 4, 5, 10, false, -1);
        AdventureBuilder.AddAudio("Superficie");
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Scenes/Menu.unity");
    }
}
