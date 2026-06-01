using System.Reflection;
using CoreBreach.Combat;
using CoreBreach.Core;
using CoreBreach.Enemies;
using CoreBreach.Pickups;
using CoreBreach.Player;
using CoreBreach.Pooling;
using CoreBreach.Strategies;
using CoreBreach.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CoreBreach
{
    public static class CoreBreachRuntimeBootstrap
    {
        private static Sprite squareSprite;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BuildIfSceneIsEmpty()
        {
            if (Object.FindFirstObjectByType<CoreHealth>() != null)
            {
                EnhanceExistingScene();
                return;
            }

            Physics2D.gravity = Vector2.zero;
            Camera camera = CreateCamera();
            CoreHealth core = CreateCore();
            Projectile projectileTemplate = CreateProjectileTemplate();
            ProjectilePool pool = CreateProjectilePool(projectileTemplate);
            WeaponStrategyBase pulse = CreateSingleWeapon();
            WeaponStrategyBase spread = CreateSpreadWeapon();
            WeaponStrategyBase boost = CreateBoostedWeapon(pulse);
            PlayerWeapon playerWeapon = CreatePlayer(pool, pulse, spread, boost);
            EnemyUnit directEnemy = CreateEnemyTemplate("Direct Enemy Template", true);
            EnemyUnit strafeEnemy = CreateEnemyTemplate("Strafe Enemy Template", false);
            Transform[] spawnPoints = CreateSpawnPoints();
            WaveSpawner waveSpawner = CreateSpawner(core, directEnemy, strafeEnemy, spawnPoints);
            ScoreKeeper scoreKeeper = CreateScoreKeeper(waveSpawner);
            CreatePickupSpawner(waveSpawner, core);
            GameStateController gameState = CreateGameState(core, waveSpawner);
            CreateHud(core, waveSpawner, scoreKeeper, playerWeapon, gameState);
            CreateArenaBounds();

            camera.transform.position = new Vector3(0f, 0f, -10f);
        }

        private static void EnhanceExistingScene()
        {
            CoreHealth core = Object.FindFirstObjectByType<CoreHealth>();
            WaveSpawner waveSpawner = Object.FindFirstObjectByType<WaveSpawner>();
            PlayerWeapon playerWeapon = Object.FindFirstObjectByType<PlayerWeapon>();
            GameStateController gameState = Object.FindFirstObjectByType<GameStateController>();
            GameHud hud = Object.FindFirstObjectByType<GameHud>();

            if (core == null || waveSpawner == null)
            {
                return;
            }

            ScoreKeeper scoreKeeper = Object.FindFirstObjectByType<ScoreKeeper>();
            if (scoreKeeper == null)
            {
                scoreKeeper = CreateScoreKeeper(waveSpawner);
            }

            if (Object.FindFirstObjectByType<PickupSpawner>() == null)
            {
                CreatePickupSpawner(waveSpawner, core);
            }

            if (hud != null)
            {
                hud.enabled = false;
                SetField(hud, "core", core);
                SetField(hud, "waveSpawner", waveSpawner);
                SetField(hud, "scoreKeeper", scoreKeeper);
                SetField(hud, "playerWeapon", playerWeapon);
                SetField(hud, "gameState", gameState);
                hud.enabled = true;
            }
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObject = new("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 5.4f;
            camera.backgroundColor = new Color(0.05f, 0.07f, 0.08f);
            return camera;
        }

        private static CoreHealth CreateCore()
        {
            GameObject coreObject = new("Energy Core");
            coreObject.transform.position = Vector3.zero;
            coreObject.transform.localScale = Vector3.one * 2.0f;
            SpriteRenderer renderer = coreObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SquareSprite();
            renderer.color = new Color(0.25f, 0.95f, 1f);
            coreObject.AddComponent<BoxCollider2D>();
            Rigidbody2D body = coreObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            CoreHealth core = coreObject.AddComponent<CoreHealth>();
            CoreDamageFlash flash = coreObject.AddComponent<CoreDamageFlash>();
            SetField(flash, "coreHealth", core);
            return core;
        }

        private static Projectile CreateProjectileTemplate()
        {
            GameObject projectileObject = new("Player Projectile Template");
            projectileObject.SetActive(false);
            projectileObject.transform.localScale = new Vector3(0.55f, 0.18f, 1f);
            SpriteRenderer renderer = projectileObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SquareSprite();
            renderer.color = new Color(1f, 0.88f, 0.28f);
            CircleCollider2D collider = projectileObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            Rigidbody2D body = projectileObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            return projectileObject.AddComponent<Projectile>();
        }

        private static ProjectilePool CreateProjectilePool(Projectile projectileTemplate)
        {
            GameObject poolObject = new("Projectile Pool");
            poolObject.SetActive(false);
            ProjectilePool pool = poolObject.AddComponent<ProjectilePool>();
            SetField(pool, "projectilePrefab", projectileTemplate);
            SetField(pool, "initialSize", 36);
            poolObject.SetActive(true);
            return pool;
        }

        private static WeaponStrategyBase CreateSingleWeapon()
        {
            SingleShotWeaponStrategy weapon = ScriptableObject.CreateInstance<SingleShotWeaponStrategy>();
            SetField(weapon, "displayName", "Pulse Rifle");
            SetField(weapon, "cooldown", 0.18f);
            SetField(weapon, "damage", 14f);
            return weapon;
        }

        private static WeaponStrategyBase CreateSpreadWeapon()
        {
            SpreadShotWeaponStrategy weapon = ScriptableObject.CreateInstance<SpreadShotWeaponStrategy>();
            SetField(weapon, "displayName", "Arc Spread");
            SetField(weapon, "cooldown", 0.55f);
            SetField(weapon, "damage", 8f);
            return weapon;
        }

        private static WeaponStrategyBase CreateBoostedWeapon(WeaponStrategyBase wrapped)
        {
            DamageBoostWeaponDecorator weapon = ScriptableObject.CreateInstance<DamageBoostWeaponDecorator>();
            SetField(weapon, "displayName", "Boosted Pulse");
            SetField(weapon, "cooldown", 0.18f);
            SetField(weapon, "wrappedWeapon", wrapped);
            SetField(weapon, "extraDamageMultiplier", 1.75f);
            return weapon;
        }

        private static PlayerWeapon CreatePlayer(ProjectilePool pool, WeaponStrategyBase pulse, WeaponStrategyBase spread, WeaponStrategyBase boost)
        {
            GameObject player = new("Defender");
            player.transform.position = new Vector3(0f, -3.3f, 0f);
            player.transform.localScale = new Vector3(1.25f, 0.85f, 1f);
            SpriteRenderer renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = SquareSprite();
            renderer.color = Color.white;
            Rigidbody2D body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            player.AddComponent<BoxCollider2D>();
            player.AddComponent<PlayerController>();

            GameObject muzzle = new("Muzzle");
            muzzle.transform.SetParent(player.transform);
            muzzle.transform.localPosition = new Vector3(0.9f, 0f, 0f);

            PlayerWeapon weapon = player.AddComponent<PlayerWeapon>();
            SetField(weapon, "muzzle", muzzle.transform);
            SetField(weapon, "projectilePool", pool);
            SetField(weapon, "primaryWeapon", pulse);
            SetField(weapon, "alternateWeapon", spread);
            SetField(weapon, "boostedWeapon", boost);
            return weapon;
        }

        private static EnemyUnit CreateEnemyTemplate(string name, bool direct)
        {
            GameObject enemy = new(name);
            enemy.SetActive(false);
            enemy.transform.localScale = direct ? Vector3.one * 1.0f : new Vector3(1.15f, 0.75f, 1f);
            SpriteRenderer renderer = enemy.AddComponent<SpriteRenderer>();
            renderer.sprite = SquareSprite();
            renderer.color = direct ? new Color(1f, 0.35f, 0.28f) : new Color(1f, 0.62f, 0.18f);
            Rigidbody2D body = enemy.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            enemy.AddComponent<BoxCollider2D>();
            MonoBehaviour strategy = direct
                ? enemy.AddComponent<DirectCoreMovementStrategy>()
                : enemy.AddComponent<StrafeCoreMovementStrategy>();
            EnemyUnit unit = enemy.AddComponent<EnemyUnit>();
            SetField(unit, "movementStrategyComponent", strategy);
            return unit;
        }

        private static Transform[] CreateSpawnPoints()
        {
            Vector3[] positions =
            {
                new(-6.3f, 3.7f, 0f),
                new(6.3f, 3.5f, 0f),
                new(-6.3f, -3.5f, 0f),
                new(6.3f, -3.7f, 0f),
                new(0f, 5.0f, 0f)
            };
            Transform[] points = new Transform[positions.Length];
            GameObject root = new("Spawn Points");
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject point = new($"Spawn Point {i + 1}");
                point.transform.SetParent(root.transform);
                point.transform.position = positions[i];
                points[i] = point.transform;
            }
            return points;
        }

        private static WaveSpawner CreateSpawner(CoreHealth core, EnemyUnit directEnemy, EnemyUnit strafeEnemy, Transform[] spawnPoints)
        {
            GameObject spawnerObject = new("Wave Spawner");
            WaveSpawner spawner = spawnerObject.AddComponent<WaveSpawner>();
            SetField(spawner, "core", core);
            SetField(spawner, "directEnemyPrefab", directEnemy);
            SetField(spawner, "strafeEnemyPrefab", strafeEnemy);
            SetField(spawner, "spawnPoints", spawnPoints);
            SetField(spawner, "totalWaves", 3);
            SetField(spawner, "enemiesInFirstWave", 8);
            SetField(spawner, "enemiesAddedPerWave", 5);
            SetField(spawner, "spawnInterval", 1.45f);
            SetField(spawner, "timeBetweenWaves", 3f);
            return spawner;
        }

        private static ScoreKeeper CreateScoreKeeper(WaveSpawner waveSpawner)
        {
            GameObject scoreObject = new("Score Keeper");
            scoreObject.SetActive(false);
            ScoreKeeper scoreKeeper = scoreObject.AddComponent<ScoreKeeper>();
            SetField(scoreKeeper, "waveSpawner", waveSpawner);
            scoreObject.SetActive(true);
            return scoreKeeper;
        }

        private static void CreatePickupSpawner(WaveSpawner waveSpawner, CoreHealth core)
        {
            GameObject pickupObject = new("Pickup Spawner");
            pickupObject.SetActive(false);
            PickupSpawner pickupSpawner = pickupObject.AddComponent<PickupSpawner>();
            SetField(pickupSpawner, "waveSpawner", waveSpawner);
            SetField(pickupSpawner, "core", core);
            SetField(pickupSpawner, "repairPickupPrefab", CreateRepairPickupTemplate(core));
            SetField(pickupSpawner, "dropChance", 0.28f);
            pickupObject.SetActive(true);
        }

        private static RepairPickup CreateRepairPickupTemplate(CoreHealth core)
        {
            GameObject pickup = new("Repair Pickup Template");
            pickup.SetActive(false);
            pickup.transform.localScale = Vector3.one * 0.55f;
            SpriteRenderer renderer = pickup.AddComponent<SpriteRenderer>();
            renderer.sprite = SquareSprite();
            renderer.color = new Color(0.18f, 1f, 0.36f);
            CircleCollider2D collider = pickup.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            RepairPickup repairPickup = pickup.AddComponent<RepairPickup>();
            repairPickup.Configure(core);
            return repairPickup;
        }

        private static GameStateController CreateGameState(CoreHealth core, WaveSpawner waveSpawner)
        {
            GameObject stateObject = new("Game State");
            stateObject.SetActive(false);
            GameStateController state = stateObject.AddComponent<GameStateController>();
            SetField(state, "core", core);
            SetField(state, "waveSpawner", waveSpawner);
            stateObject.SetActive(true);
            return state;
        }

        private static void CreateHud(CoreHealth core, WaveSpawner waveSpawner, ScoreKeeper scoreKeeper, PlayerWeapon playerWeapon, GameStateController gameState)
        {
            GameObject canvasObject = new("HUD Canvas");
            canvasObject.SetActive(false);
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
            SetField(hud, "core", core);
            SetField(hud, "waveSpawner", waveSpawner);
            SetField(hud, "scoreKeeper", scoreKeeper);
            SetField(hud, "playerWeapon", playerWeapon);
            SetField(hud, "gameState", gameState);
            SetField(hud, "statusText", text);
            canvasObject.SetActive(true);
        }

        private static void CreateArenaBounds()
        {
            CreateWall("North Wall", new Vector2(0f, 5.25f), new Vector2(13.6f, 0.25f));
            CreateWall("South Wall", new Vector2(0f, -5.25f), new Vector2(13.6f, 0.25f));
            CreateWall("West Wall", new Vector2(-6.9f, 0f), new Vector2(0.25f, 10.6f));
            CreateWall("East Wall", new Vector2(6.9f, 0f), new Vector2(0.25f, 10.6f));
        }

        private static void CreateWall(string name, Vector2 position, Vector2 scale)
        {
            GameObject wall = new(name);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            SpriteRenderer renderer = wall.AddComponent<SpriteRenderer>();
            renderer.sprite = SquareSprite();
            renderer.color = new Color(0.18f, 0.22f, 0.24f);
            wall.AddComponent<BoxCollider2D>();
        }

        private static Sprite SquareSprite()
        {
            if (squareSprite != null)
            {
                return squareSprite;
            }

            Texture2D texture = new(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            squareSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return squareSprite;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            System.Type type = target.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(target, value);
                    return;
                }

                type = type.BaseType;
            }
        }
    }
}
