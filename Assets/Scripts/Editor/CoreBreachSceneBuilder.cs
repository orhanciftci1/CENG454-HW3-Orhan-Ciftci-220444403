#if UNITY_EDITOR
using CoreBreach.Combat;
using CoreBreach.Core;
using CoreBreach.Enemies;
using CoreBreach.Player;
using CoreBreach.Pooling;
using CoreBreach.Strategies;
using CoreBreach.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CoreBreach.Editor
{
    public static class CoreBreachSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/CoreBreachPrototype.unity";

        [MenuItem("Tools/Core Breach/Build Prototype Scene")]
        public static void BuildPrototypeScene()
        {
            EnsureFolder("Assets/ScriptableObjects");
            EnsureFolder("Assets/Prefabs");
            EnsureFolder("Assets/Scenes");

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Physics2D.gravity = Vector2.zero;

            int playerLayer = EnsureLayer("Player");
            int enemyLayer = EnsureLayer("Enemy");
            int objectiveLayer = EnsureLayer("Objective");
            int projectileLayer = EnsureLayer("Projectile");

            Camera camera = CreateCamera();
            CoreHealth core = CreateCore(objectiveLayer);
            Projectile projectilePrefab = CreateProjectilePrefab(projectileLayer, enemyLayer);
            ProjectilePool pool = CreateProjectilePool(projectilePrefab);
            WeaponAssets weapons = CreateWeaponAssets();
            PlayerWeapon playerWeapon = CreatePlayer(playerLayer, pool, weapons);
            EnemyUnit directPrefab = CreateEnemyPrefab("DirectEnemy", enemyLayer, true);
            EnemyUnit strafePrefab = CreateEnemyPrefab("StrafeEnemy", enemyLayer, false);
            Transform[] spawnPoints = CreateSpawnPoints();
            WaveSpawner spawner = CreateSpawner(core, directPrefab, strafePrefab, spawnPoints);
            GameStateController gameState = CreateGameState(core, spawner);
            CreateHud(core, spawner, playerWeapon, gameState, camera);
            CreateArenaBounds();

            Selection.activeObject = core.gameObject;
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Core Breach prototype scene built at {ScenePath}");
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObject = new("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.05f, 0.07f, 0.08f);
            camera.orthographic = true;
            camera.orthographicSize = 7.5f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            return camera;
        }

        private static CoreHealth CreateCore(int layer)
        {
            GameObject coreObject = new("Energy Core");
            coreObject.layer = layer;
            coreObject.transform.position = Vector3.zero;
            SpriteRenderer renderer = coreObject.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSquareSprite();
            renderer.color = new Color(0.25f, 0.95f, 1f);
            coreObject.transform.localScale = Vector3.one * 1.4f;
            coreObject.AddComponent<BoxCollider2D>();
            Rigidbody2D body = coreObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            CoreHealth core = coreObject.AddComponent<CoreHealth>();
            CoreDamageFlash flash = coreObject.AddComponent<CoreDamageFlash>();
            SetObjectField(flash, "coreHealth", core);
            return core;
        }

        private static Projectile CreateProjectilePrefab(int projectileLayer, int enemyLayer)
        {
            GameObject projectileObject = new("PlayerProjectile");
            projectileObject.layer = projectileLayer;
            SpriteRenderer renderer = projectileObject.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSquareSprite();
            renderer.color = new Color(1f, 0.88f, 0.28f);
            projectileObject.transform.localScale = new Vector3(0.35f, 0.12f, 1f);
            CircleCollider2D collider = projectileObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            Rigidbody2D body = projectileObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Projectile projectile = projectileObject.AddComponent<Projectile>();
            SetLayerMask(projectile, "damageMask", 1 << enemyLayer);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectileObject, "Assets/Prefabs/PlayerProjectile.prefab");
            Object.DestroyImmediate(projectileObject);
            return prefab.GetComponent<Projectile>();
        }

        private static ProjectilePool CreateProjectilePool(Projectile prefab)
        {
            GameObject poolObject = new("Projectile Pool");
            ProjectilePool pool = poolObject.AddComponent<ProjectilePool>();
            SetObjectField(pool, "projectilePrefab", prefab);
            SetIntField(pool, "initialSize", 36);
            return pool;
        }

        private static WeaponAssets CreateWeaponAssets()
        {
            SingleShotWeaponStrategy single = ScriptableObject.CreateInstance<SingleShotWeaponStrategy>();
            SpreadShotWeaponStrategy spread = ScriptableObject.CreateInstance<SpreadShotWeaponStrategy>();
            DamageBoostWeaponDecorator boost = ScriptableObject.CreateInstance<DamageBoostWeaponDecorator>();

            SetStringField(single, "displayName", "Pulse Rifle");
            SetFloatField(single, "cooldown", 0.18f);
            SetFloatField(single, "damage", 14f);

            SetStringField(spread, "displayName", "Arc Spread");
            SetFloatField(spread, "cooldown", 0.55f);
            SetFloatField(spread, "damage", 8f);

            SetStringField(boost, "displayName", "Boosted Pulse");
            SetFloatField(boost, "cooldown", 0.18f);
            SetFloatField(boost, "damage", 0f);
            SetObjectField(boost, "wrappedWeapon", single);
            SetFloatField(boost, "extraDamageMultiplier", 1.75f);

            AssetDatabase.CreateAsset(single, "Assets/ScriptableObjects/PulseRifle.asset");
            AssetDatabase.CreateAsset(spread, "Assets/ScriptableObjects/ArcSpread.asset");
            AssetDatabase.CreateAsset(boost, "Assets/ScriptableObjects/BoostedPulseDecorator.asset");

            return new WeaponAssets(single, spread, boost);
        }

        private static PlayerWeapon CreatePlayer(int layer, ProjectilePool pool, WeaponAssets weapons)
        {
            GameObject player = new("Defender");
            player.layer = layer;
            player.transform.position = new Vector3(0f, -3.3f, 0f);
            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSquareSprite();
            renderer.color = new Color(0.95f, 0.95f, 0.95f);
            player.transform.localScale = new Vector3(0.8f, 0.55f, 1f);
            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            player.AddComponent<BoxCollider2D>();
            player.AddComponent<PlayerController>();

            GameObject muzzle = new("Muzzle");
            muzzle.transform.SetParent(player.transform);
            muzzle.transform.localPosition = new Vector3(0.65f, 0f, 0f);

            PlayerWeapon weapon = player.AddComponent<PlayerWeapon>();
            SetObjectField(weapon, "muzzle", muzzle.transform);
            SetObjectField(weapon, "projectilePool", pool);
            SetObjectField(weapon, "primaryWeapon", weapons.Single);
            SetObjectField(weapon, "alternateWeapon", weapons.Spread);
            SetObjectField(weapon, "boostedWeapon", weapons.Boosted);
            return weapon;
        }

        private static EnemyUnit CreateEnemyPrefab(string name, int layer, bool direct)
        {
            GameObject enemy = new(name);
            enemy.layer = layer;
            SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSquareSprite();
            renderer.color = direct ? new Color(1f, 0.35f, 0.28f) : new Color(1f, 0.62f, 0.18f);
            enemy.transform.localScale = direct ? Vector3.one * 0.62f : new Vector3(0.78f, 0.44f, 1f);
            Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            enemy.AddComponent<BoxCollider2D>();
            EnemyUnit unit = enemy.AddComponent<EnemyUnit>();
            MonoBehaviour strategy = direct
                ? enemy.AddComponent<DirectCoreMovementStrategy>()
                : enemy.AddComponent<StrafeCoreMovementStrategy>();
            SetObjectField(unit, "movementStrategyComponent", strategy);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(enemy, $"Assets/Prefabs/{name}.prefab");
            Object.DestroyImmediate(enemy);
            return prefab.GetComponent<EnemyUnit>();
        }

        private static Transform[] CreateSpawnPoints()
        {
            GameObject root = new("Spawn Points");
            Vector3[] positions =
            {
                new(-8.5f, 4.5f, 0f),
                new(8.5f, 4.2f, 0f),
                new(-8.5f, -4.2f, 0f),
                new(8.5f, -4.5f, 0f),
                new(0f, 6.5f, 0f)
            };

            Transform[] points = new Transform[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject point = new($"Spawn Point {i + 1}");
                point.transform.SetParent(root.transform);
                point.transform.position = positions[i];
                points[i] = point.transform;
            }

            return points;
        }

        private static WaveSpawner CreateSpawner(CoreHealth core, EnemyUnit directPrefab, EnemyUnit strafePrefab, Transform[] spawnPoints)
        {
            GameObject spawnerObject = new("Wave Spawner");
            WaveSpawner spawner = spawnerObject.AddComponent<WaveSpawner>();
            SetObjectField(spawner, "core", core);
            SetObjectField(spawner, "directEnemyPrefab", directPrefab);
            SetObjectField(spawner, "strafeEnemyPrefab", strafePrefab);
            SetObjectArrayField(spawner, "spawnPoints", spawnPoints);
            SetIntField(spawner, "totalEnemies", 28);
            SetFloatField(spawner, "spawnInterval", 2.1f);
            return spawner;
        }

        private static GameStateController CreateGameState(CoreHealth core, WaveSpawner spawner)
        {
            GameObject stateObject = new("Game State");
            GameStateController state = stateObject.AddComponent<GameStateController>();
            SetObjectField(state, "core", core);
            SetObjectField(state, "waveSpawner", spawner);
            return state;
        }

        private static void CreateHud(CoreHealth core, WaveSpawner spawner, PlayerWeapon weapon, GameStateController state, Camera camera)
        {
            GameObject canvasObject = new("HUD Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject textObject = new("Status Text");
            textObject.transform.SetParent(canvasObject.transform);
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.alignment = TextAnchor.UpperLeft;
            text.color = Color.white;
            RectTransform rect = text.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(18f, -18f);
            rect.sizeDelta = new Vector2(520f, 180f);

            GameHud hud = canvasObject.AddComponent<GameHud>();
            SetObjectField(hud, "core", core);
            SetObjectField(hud, "waveSpawner", spawner);
            SetObjectField(hud, "playerWeapon", weapon);
            SetObjectField(hud, "gameState", state);
            SetObjectField(hud, "statusText", text);
        }

        private static void CreateArenaBounds()
        {
            CreateWall("North Wall", new Vector2(0f, 6.9f), new Vector2(18f, 0.2f));
            CreateWall("South Wall", new Vector2(0f, -6.9f), new Vector2(18f, 0.2f));
            CreateWall("West Wall", new Vector2(-9.2f, 0f), new Vector2(0.2f, 14f));
            CreateWall("East Wall", new Vector2(9.2f, 0f), new Vector2(0.2f, 14f));
        }

        private static void CreateWall(string name, Vector2 position, Vector2 scale)
        {
            GameObject wall = new(name);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateSquareSprite();
            renderer.color = new Color(0.18f, 0.22f, 0.24f);
            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        private static Sprite CreateSquareSprite()
        {
            return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }

        private static int EnsureLayer(string layerName)
        {
            SerializedObject tagManager = new(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 0; i < layers.arraySize; i++)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                {
                    return i;
                }
            }

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(layer.stringValue))
                {
                    layer.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    return i;
                }
            }

            return 0;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/");
            string folder = System.IO.Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, folder);
        }

        private static void SetObjectField(Object target, string fieldName, Object value)
        {
            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).objectReferenceValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetObjectArrayField(Object target, string fieldName, Object[] values)
        {
            SerializedObject serialized = new(target);
            SerializedProperty property = serialized.FindProperty(fieldName);
            property.arraySize = values.Length;
            for (int i = 0; i < values.Length; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetIntField(Object target, string fieldName, int value)
        {
            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloatField(Object target, string fieldName, float value)
        {
            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).floatValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetStringField(Object target, string fieldName, string value)
        {
            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).stringValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetLayerMask(Object target, string fieldName, int value)
        {
            SerializedObject serialized = new(target);
            serialized.FindProperty(fieldName).intValue = value;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private readonly struct WeaponAssets
        {
            public WeaponAssets(WeaponStrategyBase single, WeaponStrategyBase spread, WeaponStrategyBase boosted)
            {
                Single = single;
                Spread = spread;
                Boosted = boosted;
            }

            public WeaponStrategyBase Single { get; }
            public WeaponStrategyBase Spread { get; }
            public WeaponStrategyBase Boosted { get; }
        }
    }
}
#endif
