using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

public class BuildGameUITool : EditorWindow
{
    private Sprite cachedFlatSprite;
    private Vector2 scrollPos;

    Sprite GetOrCreateFlatSprite()
    {
        if (cachedFlatSprite != null) return cachedFlatSprite;

        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(255, 255, 255, 255);
        tex.SetPixels32(pixels);
        tex.Apply();

        cachedFlatSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
        return cachedFlatSprite;
    }

    // Legacy UI Text looks thin/hard to read at any size. This adds a dark
    // outline + drop shadow so text pops against any background color.
    void MakeReadable(Text t, int fontSize, float outlineDistance = 1.5f)
    {
        if (t == null) return;
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.resizeTextForBestFit = false;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;

        Outline outline = t.GetComponent<Outline>();
        if (outline == null) outline = t.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.95f);
        outline.effectDistance = new Vector2(outlineDistance, -outlineDistance);

        Shadow shadow = t.GetComponent<Shadow>();
        if (shadow == null) shadow = t.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.6f);
        shadow.effectDistance = new Vector2(2f, -2f);
    }

    [MenuItem("Tools/Build Game UI (Health + Game Over)")]
    public static void ShowWindow()
    {
        GetWindow<BuildGameUITool>("Game UI");
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Play modundasiniz. Bu butonlar sadece Edit modunda calisir - Play modunda yapilan degisiklikler Stop'a basinca kaybolur ve sahneye hic kaydedilmez. Once Stop'a basin, sonra butona basin.", MessageType.Error);
            EditorGUI.BeginDisabledGroup(true);
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.HelpBox("Bu buton Canvas, can bari ve Game Over ekranini otomatik olusturur/gunceller.", MessageType.Info);
        if (GUILayout.Button("Build UI"))
        {
            Build();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton XP orbu, pool, PlayerXP ve XP barini kurar.", MessageType.Info);
        if (GUILayout.Button("Build XP System"))
        {
            BuildXPSystem();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton Pixel Perfect Camera'daki 'odd resolution' uyarisini kalici olarak kapatir.", MessageType.Info);
        if (GUILayout.Button("Fix Pixel Perfect Camera Warning"))
        {
            FixPixelPerfectCamera();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton 3 farkli dusman tipi (Scout/Grunt/Brute) olusturup Spawner'a atar.", MessageType.Info);
        if (GUILayout.Button("Build Enemy Variety"))
        {
            BuildEnemyVariety();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton seviye atlayinca cikan 3 secenekli yukseltme ekranini kurar.", MessageType.Info);
        if (GUILayout.Button("Build Upgrade System"))
        {
            BuildUpgradeSystem();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton oyuncunun saldirisini gorunur bicak firlatan bir projectile'a cevirir.", MessageType.Info);
        if (GUILayout.Button("Build Ranged Weapon (Thrown Knife)"))
        {
            BuildRangedWeapon();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton ekrana hayatta kalma suresini (MM:SS) gosteren bir sayac ekler.", MessageType.Info);
        if (GUILayout.Button("Build Survival Timer"))
        {
            BuildSurvivalTimer();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton oyuncuya basit bir yuruyus kare animasyonu ekler (Animator gerekmez).", MessageType.Info);
        if (GUILayout.Button("Build Player Walk Animation"))
        {
            BuildPlayerAnimation();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton dusmanlara da hareket ederken hafif zipla/salinma animasyonu ekler (ayri cizim karesi olmadigi icin).", MessageType.Info);
        if (GUILayout.Button("Build Enemy Animation"))
        {
            BuildEnemyAnimation();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton oyun acilinca cikan basit bir ana menu (Oyna/Cikis) ekler, oyunu menu kapaninca baslatir.", MessageType.Info);
        if (GUILayout.Button("Build Main Menu"))
        {
            BuildMainMenu();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton ses efektlerini (vurus, atis, olum, level up, game over, tik) ve arka plan muzigini baglar.", MessageType.Info);
        if (GUILayout.Button("Build Audio System"))
        {
            BuildAudioSystem();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton ESC ile acilan duraklatma menusunu (Devam Et / Ayarlar / Cikis) ve ses seviyesi kaydirmalarini kurar.", MessageType.Info);
        if (GUILayout.Button("Build Pause Menu"))
        {
            BuildPauseMenu();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Bu buton Build Settings'teki bos SampleScene'i cikarip gercek oyun sahnesini (player.unity) build listesine ekler, urun/sirket adini ayarlar.", MessageType.Info);
        if (GUILayout.Button("Setup Build Settings"))
        {
            SetupBuildSettings();
        }

        if (Application.isPlaying)
        {
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.EndScrollView();
    }

    // Called at the end of every build action so the result is always
    // written to disk immediately - relying on the user remembering to
    // press Ctrl+S was a real failure mode (a button "working" in the
    // editor but the .unity file on disk never actually changing).
    static void SaveScene()
    {
        EditorSceneManager.MarkAllScenesDirty();
        EditorSceneManager.SaveOpenScenes();
    }

    void BuildUpgradeSystem()
    {
        EnsurePlayerUpgrades();

        EnsureEventSystem();
        Canvas canvas = FindOrCreateCanvas();

        Text header;
        Text[] titles;
        Text[] descs;
        Button[] buttons;
        GameObject panel = CreateUpgradePanel(canvas.transform, out header, out titles, out descs, out buttons);

        UpgradeUI ui = canvas.GetComponent<UpgradeUI>();
        if (ui == null) ui = canvas.gameObject.AddComponent<UpgradeUI>();
        ui.panel = panel;
        ui.headerText = header;
        ui.optionTitles = titles;
        ui.optionDescriptions = descs;
        ui.optionButtons = buttons;

        Debug.Log("Upgrade sistemi kuruldu.");
        SaveScene();
    }

    void EnsurePlayerUpgrades()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("Player tagli obje bulunamadi.");
            return;
        }

        if (playerObj.GetComponent<PlayerUpgrades>() == null)
        {
            playerObj.AddComponent<PlayerUpgrades>();
        }
    }

    GameObject CreateUpgradePanel(Transform parent, out Text header, out Text[] titles, out Text[] descs, out Button[] buttons)
    {
        Transform existing = parent.Find("UpgradePanel");
        if (existing != null)
        {
            Transform existingHeader = existing.Find("UpgradeHeaderText");
            header = existingHeader != null ? existingHeader.GetComponent<Text>() : null;
            MakeReadable(header, 56, 2f);

            titles = new Text[3];
            descs = new Text[3];
            buttons = new Button[3];
            for (int i = 0; i < 3; i++)
            {
                Transform opt = existing.Find("OptionButton_" + i);
                if (opt != null)
                {
                    buttons[i] = opt.GetComponent<Button>();

                    RectTransform optRect = opt.GetComponent<RectTransform>();
                    if (optRect != null) optRect.sizeDelta = new Vector2(480f, 130f);

                    Transform t = opt.Find("TitleText");
                    Transform d = opt.Find("DescText");
                    titles[i] = t != null ? t.GetComponent<Text>() : null;
                    descs[i] = d != null ? d.GetComponent<Text>() : null;

                    MakeReadable(titles[i], 32, 1.5f);
                    if (titles[i] != null)
                    {
                        titles[i].alignment = TextAnchor.MiddleCenter;
                        RectTransform tr = titles[i].GetComponent<RectTransform>();
                        tr.anchorMin = new Vector2(0f, 0.52f);
                        tr.anchorMax = new Vector2(1f, 1f);
                        tr.offsetMin = new Vector2(12f, 0f);
                        tr.offsetMax = new Vector2(-12f, -6f);
                    }

                    MakeReadable(descs[i], 22, 1f);
                    if (descs[i] != null)
                    {
                        descs[i].fontStyle = FontStyle.Normal;
                        descs[i].color = new Color(0.92f, 0.92f, 0.92f, 1f);
                        descs[i].alignment = TextAnchor.MiddleCenter;
                        RectTransform dr = descs[i].GetComponent<RectTransform>();
                        dr.anchorMin = new Vector2(0f, 0f);
                        dr.anchorMax = new Vector2(1f, 0.52f);
                        dr.offsetMin = new Vector2(12f, 6f);
                        dr.offsetMax = new Vector2(-12f, 0f);
                    }
                }
            }
            return existing.gameObject;
        }

        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject panelGO = new GameObject("UpgradePanel", typeof(RectTransform));
        panelGO.transform.SetParent(parent, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.85f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject headerGO = new GameObject("UpgradeHeaderText", typeof(RectTransform));
        headerGO.transform.SetParent(panelGO.transform, false);
        Text headerText = headerGO.AddComponent<Text>();
        headerText.text = "LEVEL UP!";
        headerText.font = builtinFont;
        headerText.alignment = TextAnchor.MiddleCenter;
        headerText.color = Color.white;
        MakeReadable(headerText, 56, 2f);
        RectTransform headerRect = headerGO.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0.5f, 0.78f);
        headerRect.anchorMax = new Vector2(0.5f, 0.78f);
        headerRect.pivot = new Vector2(0.5f, 0.5f);
        headerRect.sizeDelta = new Vector2(600f, 90f);
        headerRect.anchoredPosition = Vector2.zero;

        titles = new Text[3];
        descs = new Text[3];
        buttons = new Button[3];

        float[] yPositions = { 0.58f, 0.42f, 0.26f };

        for (int i = 0; i < 3; i++)
        {
            GameObject btnGO = new GameObject("OptionButton_" + i, typeof(RectTransform));
            btnGO.transform.SetParent(panelGO.transform, false);
            Image btnImg = btnGO.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.25f, 0.35f, 1f);
            Button btn = btnGO.AddComponent<Button>();
            RectTransform btnRect = btnGO.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.5f, yPositions[i]);
            btnRect.anchorMax = new Vector2(0.5f, yPositions[i]);
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            btnRect.sizeDelta = new Vector2(480f, 130f);
            btnRect.anchoredPosition = Vector2.zero;

            GameObject titleGO = new GameObject("TitleText", typeof(RectTransform));
            titleGO.transform.SetParent(btnGO.transform, false);
            Text titleText = titleGO.AddComponent<Text>();
            titleText.text = "Upgrade";
            titleText.font = builtinFont;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            MakeReadable(titleText, 32, 1.5f);
            RectTransform titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.52f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(12f, 0f);
            titleRect.offsetMax = new Vector2(-12f, -6f);

            GameObject descGO = new GameObject("DescText", typeof(RectTransform));
            descGO.transform.SetParent(btnGO.transform, false);
            Text descText = descGO.AddComponent<Text>();
            descText.text = "Aciklama";
            descText.font = builtinFont;
            descText.alignment = TextAnchor.MiddleCenter;
            descText.color = new Color(0.92f, 0.92f, 0.92f, 1f);
            MakeReadable(descText, 22, 1f);
            descText.fontStyle = FontStyle.Normal;
            RectTransform descRect = descGO.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0f, 0f);
            descRect.anchorMax = new Vector2(1f, 0.52f);
            descRect.offsetMin = new Vector2(12f, 6f);
            descRect.offsetMax = new Vector2(-12f, 0f);

            titles[i] = titleText;
            descs[i] = descText;
            buttons[i] = btn;
        }

        panelGO.SetActive(false);
        header = headerText;
        return panelGO;
    }

    void BuildEnemyVariety()
    {
        GameObject basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab");
        if (basePrefab == null)
        {
            Debug.LogWarning("Assets/Prefabs/Enemy.prefab bulunamadi.");
            return;
        }

        GameObject scout = CreateEnemyVariant(basePrefab, "Scout", 8, 2.8f, 2, 1, 1, 0.8f);
        GameObject brute = CreateEnemyVariant(basePrefab, "Brute", 12, 1.2f, 6, 2, 3, 1.35f);
        // Boss: big, slow, tanky, deep-purple-tinted so it reads as a
        // singular threat rather than another species in the rotation.
        // It is deliberately NOT drawn into the normal random spawn pool
        // (see the very high unlock level below) - BossSpawner brings it
        // in on its own schedule via EnemyPool.GetEnemyOfType instead.
        GameObject boss = CreateEnemyVariant(basePrefab, "Boss", 4, 1.0f, 50, 2, 15, 2.2f, new Color(0.45f, 0.05f, 0.55f));

        GameObject spawnerObj = GameObject.Find("Spawner");
        if (spawnerObj == null)
        {
            Debug.LogWarning("Spawner objesi bulunamadi.");
            return;
        }

        EnemyPool pool = spawnerObj.GetComponent<EnemyPool>();
        if (pool == null)
        {
            Debug.LogWarning("EnemyPool komponenti bulunamadi.");
            return;
        }

        List<GameObject> prefabList = new List<GameObject> { basePrefab };
        List<int> unlockList = new List<int> { 1 };
        if (scout != null) { prefabList.Add(scout); unlockList.Add(2); }
        if (brute != null) { prefabList.Add(brute); unlockList.Add(3); }
        int bossTypeIndex = -1;
        if (boss != null) { prefabList.Add(boss); unlockList.Add(9999); bossTypeIndex = prefabList.Count - 1; }

        pool.enemyPrefabs = prefabList.ToArray();
        pool.unlockLevels = unlockList.ToArray();
        EditorUtility.SetDirty(pool);

        if (bossTypeIndex >= 0)
        {
            BossSpawner bossSpawner = spawnerObj.GetComponent<BossSpawner>();
            if (bossSpawner == null) bossSpawner = spawnerObj.AddComponent<BossSpawner>();
            bossSpawner.bossTypeIndex = bossTypeIndex;
            EditorUtility.SetDirty(bossSpawner);
        }

        AssetDatabase.SaveAssets();

        Debug.Log("Dusman cesitliligi kuruldu: Grunt (Lv1+), Scout (Lv2+), Brute (Lv3+), Boss (periyodik). Toplam tip: " + pool.enemyPrefabs.Length);
        SaveScene();
    }

    GameObject CreateEnemyVariant(GameObject basePrefab, string variantName, int spriteIndex, float moveSpeed, int maxHealth, int contactDamage, int xpValue, float scale = 1f, Color? tintColor = null)
    {
        string path = "Assets/Prefabs/Enemy_" + variantName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        GameObject instance = existing != null
            ? (GameObject)PrefabUtility.InstantiatePrefab(existing)
            : (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);

        // The visible sprite may live on a child "SpriteVisual" object
        // instead of this root, once Build Enemy Animation has restructured
        // it (same pattern as the player) - fall back to the root's own
        // SpriteRenderer for a variant that hasn't been through that yet.
        Transform visualT = instance.transform.Find("SpriteVisual");
        SpriteRenderer sr = visualT != null ? visualT.GetComponent<SpriteRenderer>() : instance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Sprite sprite = LoadEnemySprite(spriteIndex);
            if (sprite != null)
            {
                sr.sprite = sprite;
                // SpriteAnimator resets sr.sprite back to idleFrame every
                // idle frame - without updating this too, the species
                // sprite we just set would get immediately overwritten.
                SpriteAnimator anim = visualT != null ? visualT.GetComponent<SpriteAnimator>() : null;
                if (anim != null) anim.idleFrame = sprite;
            }
            // A boss needs to read as unmistakably different at a glance,
            // even before scale/silhouette register - a strong color tint
            // on top of the base species sprite does that cheaply.
            if (tintColor.HasValue)
            {
                sr.color = tintColor.Value;
            }
        }

        EnemyAI ai = instance.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.moveSpeed = moveSpeed;
            ai.contactDamage = contactDamage;
            // contactRange is a flat distance check, not derived from the
            // Collider2D, so it has to be scaled explicitly too or a large
            // variant's hitbox looks bigger than where it actually hurts you.
            ai.contactRange = 0.3f * scale;
        }

        EnemyHealth hp = instance.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.maxHealth = maxHealth;
            hp.xpValue = xpValue;
        }

        // Scaling the whole root (not just the sprite) also scales its
        // Collider2D, so a bigger Brute genuinely has a bigger hitbox and
        // a smaller Scout a smaller one - consistent with how they look,
        // not just a visual trick.
        if (!Mathf.Approximately(scale, 1f))
        {
            instance.transform.localScale = Vector3.one * scale;
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instance, path);
        Object.DestroyImmediate(instance);
        return prefab;
    }

    Sprite LoadEnemySprite(int index)
    {
        string pngPath = string.Format("Assets/Sprites/PNG/Enemies/Tiles/tile_{0:D4}.png", index);
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(pngPath))
        {
            Sprite s = obj as Sprite;
            if (s != null) return s;
        }
        return null;
    }

    void FixPixelPerfectCamera()
    {
        PixelPerfectCamera[] all = Object.FindObjectsByType<PixelPerfectCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (all.Length == 0)
        {
            Debug.LogWarning("Sahnede PixelPerfectCamera komponenti bulunamadi.");
            return;
        }

        foreach (PixelPerfectCamera ppc in all)
        {
            ppc.cropFrameX = true;
            ppc.cropFrameY = true;
            EditorUtility.SetDirty(ppc);
        }

        Debug.Log("Pixel Perfect Camera duzeltildi (" + all.Length + " adet), uyari kapanmali.");
        SaveScene();
    }

    void Build()
    {
        EnsureEventSystem();

        Canvas canvas = FindOrCreateCanvas();

        Image fillImage = CreateHealthBar(canvas.transform);
        HealthBarUI hb = canvas.GetComponent<HealthBarUI>();
        if (hb == null) hb = canvas.gameObject.AddComponent<HealthBarUI>();
        hb.fillImage = fillImage;

        Text gameOverText;
        Text restartButtonText;
        Text bestTimeText;
        GameObject panel = CreateGameOverPanel(canvas.transform, out gameOverText, out restartButtonText, out bestTimeText);
        GameOverUI go = canvas.GetComponent<GameOverUI>();
        if (go == null) go = canvas.gameObject.AddComponent<GameOverUI>();
        go.panel = panel;
        go.gameOverText = gameOverText;
        go.restartButtonText = restartButtonText;
        go.bestTimeText = bestTimeText;

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("Game UI olusturuldu: Canvas, HealthBar, GameOverPanel.");
        SaveScene();
    }

    void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;

        GameObject esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<StandaloneInputModule>();
    }

    Canvas FindOrCreateCanvas()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) return canvas;

        GameObject canvasGO = new GameObject("GameUI");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    Image CreateHealthBar(Transform parent)
    {
        Transform existingBg = parent.Find("HealthBarBackground");
        if (existingBg != null)
        {
            Transform existingFill = existingBg.Find("HealthBarFill");
            if (existingFill != null)
            {
                Image existingImg = existingFill.GetComponent<Image>();
                existingImg.sprite = GetOrCreateFlatSprite();
                existingImg.type = Image.Type.Filled;
                return existingImg;
            }
        }

        GameObject bgGO = new GameObject("HealthBarBackground", typeof(RectTransform));
        bgGO.transform.SetParent(parent, false);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 1f);
        bgRect.anchorMax = new Vector2(0f, 1f);
        bgRect.pivot = new Vector2(0f, 1f);
        bgRect.anchoredPosition = new Vector2(30f, -30f);
        bgRect.sizeDelta = new Vector2(300f, 40f);

        GameObject fillGO = new GameObject("HealthBarFill", typeof(RectTransform));
        fillGO.transform.SetParent(bgGO.transform, false);
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.sprite = GetOrCreateFlatSprite();
        fillImage.color = new Color(0.85f, 0.15f, 0.15f, 1f);
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        fillImage.fillOrigin = 0;
        fillImage.fillAmount = 1f;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(4f, 4f);
        fillRect.offsetMax = new Vector2(-4f, -4f);

        return fillImage;
    }

    GameObject CreateGameOverPanel(Transform parent, out Text gameOverText, out Text restartButtonText, out Text bestTimeText)
    {
        Transform existing = parent.Find("GameOverPanel");
        if (existing != null)
        {
            Transform existingText = existing.Find("GameOverText");
            gameOverText = existingText != null ? existingText.GetComponent<Text>() : null;
            MakeReadable(gameOverText, 76, 2.5f);

            Transform existingBtn = existing.Find("RestartButton");
            Transform existingBtnText = existingBtn != null ? existingBtn.Find("Text") : null;
            restartButtonText = existingBtnText != null ? existingBtnText.GetComponent<Text>() : null;
            MakeReadable(restartButtonText, 36, 1.5f);

            Transform existingBestTime = existing.Find("BestTimeText");
            Text bestTimeTxt = existingBestTime != null ? existingBestTime.GetComponent<Text>() : null;
            if (bestTimeTxt == null)
            {
                bestTimeTxt = CreateBestTimeLabel(existing, new Vector2(0.5f, 0.48f));
            }
            MakeReadable(bestTimeTxt, 30, 1.5f);
            bestTimeText = bestTimeTxt;

            return existing.gameObject;
        }

        GameObject panelGO = new GameObject("GameOverPanel", typeof(RectTransform));
        panelGO.transform.SetParent(parent, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject textGO = new GameObject("GameOverText", typeof(RectTransform));
        textGO.transform.SetParent(panelGO.transform, false);
        Text text = textGO.AddComponent<Text>();
        text.text = "GAME OVER";
        text.font = builtinFont;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        MakeReadable(text, 76, 2.5f);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.6f);
        textRect.anchorMax = new Vector2(0.5f, 0.6f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = new Vector2(700f, 120f);
        textRect.anchoredPosition = Vector2.zero;

        GameObject buttonGO = new GameObject("RestartButton", typeof(RectTransform));
        buttonGO.transform.SetParent(panelGO.transform, false);
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.9f, 1f);
        buttonGO.AddComponent<Button>();
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.4f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.4f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(250f, 80f);
        buttonRect.anchoredPosition = Vector2.zero;

        GameObject buttonTextGO = new GameObject("Text", typeof(RectTransform));
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        Text buttonText = buttonTextGO.AddComponent<Text>();
        buttonText.text = "Restart";
        buttonText.font = builtinFont;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        MakeReadable(buttonText, 36, 1.5f);
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;

        Text bestTimeTxtFresh = CreateBestTimeLabel(panelGO.transform, new Vector2(0.5f, 0.48f));

        panelGO.SetActive(false);
        gameOverText = text;
        restartButtonText = buttonText;
        bestTimeText = bestTimeTxtFresh;
        return panelGO;
    }

    // Small centered label used for the "best time" line on both the
    // Game Over panel and the main menu - same look, different parent.
    Text CreateBestTimeLabel(Transform parent, Vector2 anchor)
    {
        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject go = new GameObject("BestTimeText", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Text txt = go.AddComponent<Text>();
        txt.text = "";
        txt.font = builtinFont;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        MakeReadable(txt, 30, 1.5f);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600f, 60f);
        rect.anchoredPosition = Vector2.zero;
        return txt;
    }

    void BuildXPSystem()
    {
        GameObject orbPrefab = CreateOrLoadXPOrbPrefab();
        if (orbPrefab == null)
        {
            Debug.LogWarning("XP orb prefab olusturulamadi.");
            return;
        }

        EnsureXPOrbPool(orbPrefab);
        EnsurePlayerXP();

        EnsureEventSystem();
        Canvas canvas = FindOrCreateCanvas();

        Image xpFill;
        Text levelText;
        CreateXPBar(canvas.transform, out xpFill, out levelText);

        XPBarUI xpBarUI = canvas.GetComponent<XPBarUI>();
        if (xpBarUI == null) xpBarUI = canvas.gameObject.AddComponent<XPBarUI>();
        xpBarUI.fillImage = xpFill;
        xpBarUI.levelText = levelText;
        xpBarUI.xpText = CreateXPText(canvas.transform);

        AssetDatabase.SaveAssets();
        Debug.Log("XP sistemi kuruldu: Orb prefab, pool, PlayerXP, XP bari.");
        SaveScene();
    }

    GameObject CreateOrLoadXPOrbPrefab()
    {
        string prefabPath = "Assets/Prefabs/XPOrb.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null) return existing;

        Sprite orbSprite = LoadTileSprite(39);

        GameObject temp = new GameObject("XPOrb");
        SpriteRenderer sr = temp.AddComponent<SpriteRenderer>();
        sr.sprite = orbSprite;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 5;

        CircleCollider2D col = temp.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.3f;

        temp.AddComponent<XPOrb>();
        temp.transform.localScale = Vector3.one * 1.5f;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    Sprite LoadTileSprite(int index)
    {
        string pngPath = string.Format("Assets/Sprites/PNG/Tiles/Tiles/tile_{0:D4}.png", index);
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(pngPath))
        {
            Sprite s = obj as Sprite;
            if (s != null) return s;
        }
        return null;
    }

    void EnsureXPOrbPool(GameObject orbPrefab)
    {
        XPOrbPool pool = Object.FindFirstObjectByType<XPOrbPool>();
        if (pool == null)
        {
            GameObject poolGO = new GameObject("XPOrbPool");
            pool = poolGO.AddComponent<XPOrbPool>();
        }
        pool.orbPrefab = orbPrefab;
        EditorUtility.SetDirty(pool);
    }

    void EnsurePlayerXP()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("Player tagli obje bulunamadi.");
            return;
        }

        if (playerObj.GetComponent<PlayerXP>() == null)
        {
            playerObj.AddComponent<PlayerXP>();
        }
    }

    void CreateXPBar(Transform parent, out Image fillImage, out Text levelText)
    {
        Transform existingBg = parent.Find("XPBarBackground");
        if (existingBg != null)
        {
            Transform existingFill = existingBg.Find("XPBarFill");
            fillImage = existingFill != null ? existingFill.GetComponent<Image>() : null;
            if (fillImage != null)
            {
                fillImage.sprite = GetOrCreateFlatSprite();
                fillImage.type = Image.Type.Filled;
            }

            Transform existingLevel = parent.Find("LevelText");
            levelText = existingLevel != null ? existingLevel.GetComponent<Text>() : null;
            MakeReadable(levelText, 24, 1.5f);
            return;
        }

        GameObject bgGO = new GameObject("XPBarBackground", typeof(RectTransform));
        bgGO.transform.SetParent(parent, false);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 1f);
        bgRect.anchorMax = new Vector2(0f, 1f);
        bgRect.pivot = new Vector2(0f, 1f);
        bgRect.anchoredPosition = new Vector2(30f, -80f);
        bgRect.sizeDelta = new Vector2(300f, 20f);

        GameObject fillGO = new GameObject("XPBarFill", typeof(RectTransform));
        fillGO.transform.SetParent(bgGO.transform, false);
        Image fill = fillGO.AddComponent<Image>();
        fill.sprite = GetOrCreateFlatSprite();
        fill.color = new Color(0.55f, 0.25f, 0.85f, 1f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 0f;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(3f, 3f);
        fillRect.offsetMax = new Vector2(-3f, -3f);

        GameObject textGO = new GameObject("LevelText", typeof(RectTransform));
        textGO.transform.SetParent(parent, false);
        Text text = textGO.AddComponent<Text>();
        text.text = "Lv. 1";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        MakeReadable(text, 24, 1.5f);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(0f, 1f);
        textRect.pivot = new Vector2(0f, 1f);
        textRect.anchoredPosition = new Vector2(340f, -83f);
        textRect.sizeDelta = new Vector2(120f, 28f);

        fillImage = fill;
        levelText = text;
    }

    Text CreateXPText(Transform parent)
    {
        Transform existing = parent.Find("XPText");
        if (existing != null)
        {
            Text t = existing.GetComponent<Text>();
            MakeReadable(t, 20, 1f);
            return t;
        }

        GameObject textGO = new GameObject("XPText", typeof(RectTransform));
        textGO.transform.SetParent(parent, false);
        Text text = textGO.AddComponent<Text>();
        text.text = "0 / 5";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        MakeReadable(text, 20, 1f);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(0f, 1f);
        textRect.pivot = new Vector2(0f, 1f);
        textRect.anchoredPosition = new Vector2(30f, -103f);
        textRect.sizeDelta = new Vector2(220f, 24f);

        return text;
    }

    void BuildRangedWeapon()
    {
        GameObject projectilePrefab = CreateOrLoadProjectilePrefab();
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile prefab olusturulamadi.");
            return;
        }

        EnsureProjectilePool(projectilePrefab);

        AssetDatabase.SaveAssets();
        Debug.Log("Menzilli silah kuruldu: oyuncu artik bicak firlatiyor.");
        SaveScene();
    }

    GameObject CreateOrLoadProjectilePrefab()
    {
        string prefabPath = "Assets/Prefabs/Projectile.prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existing != null) return existing;

        Sprite knifeSprite = LoadWeaponSprite(8);

        GameObject temp = new GameObject("Projectile");
        SpriteRenderer sr = temp.AddComponent<SpriteRenderer>();
        sr.sprite = knifeSprite;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 6;

        temp.AddComponent<Projectile>();
        temp.transform.localScale = Vector3.one * 1.6f;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(temp, prefabPath);
        Object.DestroyImmediate(temp);
        return prefab;
    }

    Sprite LoadWeaponSprite(int index)
    {
        string pngPath = string.Format("Assets/Sprites/PNG/Weapons/Tiles/tile_{0:D4}.png", index);
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(pngPath))
        {
            Sprite s = obj as Sprite;
            if (s != null) return s;
        }
        return null;
    }

    void EnsureProjectilePool(GameObject projectilePrefab)
    {
        ProjectilePool pool = Object.FindFirstObjectByType<ProjectilePool>();
        if (pool == null)
        {
            GameObject poolGO = new GameObject("ProjectilePool");
            pool = poolGO.AddComponent<ProjectilePool>();
        }
        pool.projectilePrefab = projectilePrefab;
        EditorUtility.SetDirty(pool);
    }

    void BuildSurvivalTimer()
    {
        EnsureEventSystem();
        Canvas canvas = FindOrCreateCanvas();

        Text timerText = CreateSurvivalTimer(canvas.transform);

        SurvivalTimerUI timerUI = canvas.GetComponent<SurvivalTimerUI>();
        if (timerUI == null) timerUI = canvas.gameObject.AddComponent<SurvivalTimerUI>();
        timerUI.timerText = timerText;

        Debug.Log("Hayatta kalma sayaci kuruldu.");
        SaveScene();
    }

    Text CreateSurvivalTimer(Transform parent)
    {
        Transform existing = parent.Find("SurvivalTimerText");
        if (existing != null)
        {
            Text t = existing.GetComponent<Text>();
            MakeReadable(t, 30, 1.5f);
            return t;
        }

        GameObject textGO = new GameObject("SurvivalTimerText", typeof(RectTransform));
        textGO.transform.SetParent(parent, false);
        Text text = textGO.AddComponent<Text>();
        text.text = "00:00";
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        MakeReadable(text, 30, 1.5f);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 1f);
        textRect.anchorMax = new Vector2(0.5f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.anchoredPosition = new Vector2(0f, -30f);
        textRect.sizeDelta = new Vector2(160f, 40f);

        return text;
    }

    void BuildPlayerAnimation()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("Player tagli obje bulunamadi.");
            return;
        }

        // The animated sprite lives on a separate child object, not on the
        // Player root. The root's Transform is driven by Rigidbody2D
        // (MovePosition) and carries the BoxCollider2D - animating scale or
        // position directly on it would fight the physics engine and also
        // resize the collider. A plain child has neither concern.
        Transform visualT = playerObj.transform.Find("SpriteVisual");
        GameObject visualGO = visualT != null ? visualT.gameObject : null;
        if (visualGO == null)
        {
            visualGO = new GameObject("SpriteVisual");
            visualGO.transform.SetParent(playerObj.transform, false);
        }

        SpriteRenderer rootSR = playerObj.GetComponent<SpriteRenderer>();
        SpriteRenderer childSR = visualGO.GetComponent<SpriteRenderer>();
        if (childSR == null) childSR = visualGO.AddComponent<SpriteRenderer>();

        if (rootSR != null)
        {
            childSR.sprite = rootSR.sprite;
            childSR.sortingLayerID = rootSR.sortingLayerID;
            childSR.sortingOrder = rootSR.sortingOrder;
            childSR.color = rootSR.color;
            childSR.flipX = rootSR.flipX;
            // Disabled, not removed/destroyed - keeps GetComponent<SpriteRenderer>()
            // calls elsewhere from breaking, it just no longer renders (avoids
            // double-drawing the character on top of itself).
            rootSR.enabled = false;
        }

        // Clean up a leftover SpriteAnimator from the earlier (root-based)
        // version of this tool, if present.
        SpriteAnimator rootAnim = playerObj.GetComponent<SpriteAnimator>();
        if (rootAnim != null) Object.DestroyImmediate(rootAnim);

        SpriteAnimator anim = visualGO.GetComponent<SpriteAnimator>();
        if (anim == null) anim = visualGO.AddComponent<SpriteAnimator>();

        // Player currently uses Players/Tiles/tile_0000 (front-facing pose).
        // tile_0001 and tile_0002 are the same character's other walk-cycle
        // poses from the same Kenney sheet, so this is a real 3-frame walk
        // cycle, not just a random sprite swap.
        anim.idleFrame = LoadPlayerSprite(0);
        anim.moveFrames = new Sprite[] { LoadPlayerSprite(1), LoadPlayerSprite(2) };
        anim.frameRate = 8f;

        EditorUtility.SetDirty(playerObj);
        Debug.Log("Oyuncu yuruyus animasyonu kuruldu (ayri SpriteVisual objesi ile).");
        SaveScene();
    }

    void BuildEnemyAnimation()
    {
        string[] paths = {
            "Assets/Prefabs/Enemy.prefab",
            "Assets/Prefabs/Enemy_Scout.prefab",
            "Assets/Prefabs/Enemy_Brute.prefab"
        };

        int done = 0;
        foreach (string path in paths)
        {
            if (RestructureEnemyPrefabForAnimation(path)) done++;
        }

        Debug.Log("Dusman hareket animasyonu kuruldu (" + done + " prefab).");
        SaveScene();
    }

    // Same restructuring BuildPlayerAnimation does for the player, applied
    // directly to a prefab ASSET rather than a live scene object (enemies
    // are spawned from these prefabs by EnemyPool, so the prefab itself
    // needs the child object, not just whatever instance happens to be in
    // the scene). Enemies don't have a drawn walk-cycle (each sprite index
    // is a different enemy species, not a pose of the same one like the
    // player's tiles are) - so moveFrames is left empty and SpriteAnimator
    // falls back to its tilt+hop-only motion cue, which needs no extra art.
    bool RestructureEnemyPrefabForAnimation(string path)
    {
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefabAsset == null) return false;

        GameObject root = PrefabUtility.LoadPrefabContents(path);

        Transform visualT = root.transform.Find("SpriteVisual");
        GameObject visualGO = visualT != null ? visualT.gameObject : null;
        if (visualGO == null)
        {
            visualGO = new GameObject("SpriteVisual");
            visualGO.transform.SetParent(root.transform, false);
        }

        SpriteRenderer rootSR = root.GetComponent<SpriteRenderer>();
        SpriteRenderer childSR = visualGO.GetComponent<SpriteRenderer>();
        if (childSR == null) childSR = visualGO.AddComponent<SpriteRenderer>();

        if (rootSR != null && rootSR.enabled)
        {
            childSR.sprite = rootSR.sprite;
            childSR.sortingLayerID = rootSR.sortingLayerID;
            childSR.sortingOrder = rootSR.sortingOrder;
            childSR.color = rootSR.color;
            rootSR.enabled = false;
        }

        SpriteAnimator anim = visualGO.GetComponent<SpriteAnimator>();
        if (anim == null) anim = visualGO.AddComponent<SpriteAnimator>();
        if (anim.idleFrame == null) anim.idleFrame = childSR.sprite;
        anim.moveFrames = new Sprite[0];
        anim.frameRate = 6f;
        anim.tiltAngle = 6f;

        PrefabUtility.SaveAsPrefabAsset(root, path);
        PrefabUtility.UnloadPrefabContents(root);
        return true;
    }

    void BuildMainMenu()
    {
        EnsureEventSystem();
        Canvas canvas = FindOrCreateCanvas();

        Text title, playText, quitText, bestTimeText;
        GameObject panel = CreateMainMenuPanel(canvas.transform, out title, out playText, out quitText, out bestTimeText);

        MainMenuUI menu = canvas.GetComponent<MainMenuUI>();
        if (menu == null) menu = canvas.gameObject.AddComponent<MainMenuUI>();
        menu.panel = panel;
        menu.titleText = title;
        menu.playButtonText = playText;
        menu.quitButtonText = quitText;
        menu.bestTimeText = bestTimeText;

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("Ana menu kuruldu (Oyna / Cikis).");
        SaveScene();
    }

    GameObject CreateMainMenuPanel(Transform parent, out Text title, out Text playText, out Text quitText, out Text bestTimeText)
    {
        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Desert/pixel palette - warm sand + rust + a near-black brown for
        // the chunky pixel-style borders (two flat-color rects nested
        // inside each other, bigger one behind, is the cheap way to fake a
        // pixel-art bordered panel/button without needing a 9-sliced
        // texture asset).
        Color borderColor = new Color(0.18f, 0.11f, 0.06f, 1f);
        Color backdropColor = new Color(0.16f, 0.10f, 0.06f, 0.9f);
        Color sandColor = new Color(0.86f, 0.66f, 0.38f, 1f);
        Color rustColor = new Color(0.76f, 0.38f, 0.16f, 1f);
        Color oliveColor = new Color(0.46f, 0.38f, 0.22f, 1f);

        Transform existingOld = parent.Find("MainMenuPanel");
        if (existingOld != null) Object.DestroyImmediate(existingOld.gameObject);

        // Full-screen dark backdrop so the desert menu frame pops against
        // whatever is behind it.
        GameObject panelGO = new GameObject("MainMenuPanel", typeof(RectTransform));
        panelGO.transform.SetParent(parent, false);
        panelGO.transform.SetAsLastSibling();
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = backdropColor;
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // The actual bordered "frame" - dark border rect with a smaller
        // sand-colored fill inset inside it.
        GameObject frameGO = new GameObject("MenuFrame", typeof(RectTransform));
        frameGO.transform.SetParent(panelGO.transform, false);
        Image frameBorder = frameGO.AddComponent<Image>();
        frameBorder.color = borderColor;
        RectTransform frameRect = frameGO.GetComponent<RectTransform>();
        frameRect.anchorMin = new Vector2(0.5f, 0.5f);
        frameRect.anchorMax = new Vector2(0.5f, 0.5f);
        frameRect.pivot = new Vector2(0.5f, 0.5f);
        frameRect.sizeDelta = new Vector2(680f, 480f);
        frameRect.anchoredPosition = Vector2.zero;

        GameObject frameFillGO = new GameObject("Fill", typeof(RectTransform));
        frameFillGO.transform.SetParent(frameGO.transform, false);
        Image frameFill = frameFillGO.AddComponent<Image>();
        frameFill.color = sandColor;
        RectTransform frameFillRect = frameFillGO.GetComponent<RectTransform>();
        frameFillRect.anchorMin = Vector2.zero;
        frameFillRect.anchorMax = Vector2.one;
        frameFillRect.offsetMin = new Vector2(14f, 14f);
        frameFillRect.offsetMax = new Vector2(-14f, -14f);

        GameObject titleGO = new GameObject("TitleText", typeof(RectTransform));
        titleGO.transform.SetParent(frameGO.transform, false);
        Text titleTxt = titleGO.AddComponent<Text>();
        titleTxt.text = "SURVIVORGAME";
        titleTxt.font = builtinFont;
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.color = Color.white;
        MakeReadable(titleTxt, 56, 2.5f);
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.78f);
        titleRect.anchorMax = new Vector2(0.5f, 0.78f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(620f, 120f);
        titleRect.anchoredPosition = Vector2.zero;

        Button playBtn;
        Text playTxt = CreatePixelButton(frameGO.transform, "PlayButton", "OYNA", 0.46f,
            new Vector2(340f, 92f), borderColor, rustColor, builtinFont, 36, out playBtn);

        Button quitBtn;
        Text quitTxt = CreatePixelButton(frameGO.transform, "QuitButton", "CIKIS", 0.22f,
            new Vector2(340f, 80f), borderColor, oliveColor, builtinFont, 30, out quitBtn);

        Text bestTimeTxt = CreateBestTimeLabel(frameGO.transform, new Vector2(0.5f, 0.09f));
        MakeReadable(bestTimeTxt, 24, 1.5f);

        title = titleTxt;
        playText = playTxt;
        quitText = quitTxt;
        bestTimeText = bestTimeTxt;
        return panelGO;
    }

    // Builds one "chunky pixel" button: an outer border-colored rect with a
    // smaller colored fill rect inset inside it (the same cheap 2-layer
    // trick as the menu frame itself) plus a Button + centered label.
    // yAnchor positions it vertically within its parent (0 = bottom of
    // parent, 1 = top), horizontally centered.
    Text CreatePixelButton(Transform parent, string name, string label, float yAnchor, Vector2 size, Color borderColor, Color fillColor, Font font, int fontSize, out Button button)
    {
        GameObject btnGO = new GameObject(name, typeof(RectTransform));
        btnGO.transform.SetParent(parent, false);
        Image btnBorder = btnGO.AddComponent<Image>();
        btnBorder.color = borderColor;
        button = btnGO.AddComponent<Button>();
        RectTransform btnRect = btnGO.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, yAnchor);
        btnRect.anchorMax = new Vector2(0.5f, yAnchor);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.sizeDelta = size;
        btnRect.anchoredPosition = Vector2.zero;

        GameObject fillGO = new GameObject("Fill", typeof(RectTransform));
        fillGO.transform.SetParent(btnGO.transform, false);
        Image fillImg = fillGO.AddComponent<Image>();
        fillImg.color = fillColor;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(6f, 6f);
        fillRect.offsetMax = new Vector2(-6f, -6f);
        // The Button needs *a* graphic to receive clicks reliably across
        // its whole rect - the outer border image already covers that, so
        // the fill image doesn't need to (and shouldn't) block raycasts.
        fillImg.raycastTarget = false;

        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(btnGO.transform, false);
        Text txt = textGO.AddComponent<Text>();
        txt.text = label;
        txt.font = font;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        MakeReadable(txt, fontSize, 1.5f);
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        return txt;
    }

    void BuildAudioSystem()
    {
        GameObject audioGO = GameObject.Find("AudioManager");
        if (audioGO == null)
        {
            audioGO = new GameObject("AudioManager");
        }

        AudioManager mgr = audioGO.GetComponent<AudioManager>();
        if (mgr == null) mgr = audioGO.AddComponent<AudioManager>();

        mgr.clickClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/click.wav");
        mgr.pickupClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/pickup.wav");
        mgr.throwClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/throw.wav");
        mgr.hitClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/hit.wav");
        mgr.enemyDeathClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/enemy_death.wav");
        mgr.levelUpClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/level_up.wav");
        mgr.gameOverClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/game_over.wav");
        mgr.musicClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/bg_music.wav");

        EditorUtility.SetDirty(audioGO);
        Debug.Log("Ses sistemi kuruldu (AudioManager + " +
            (mgr.musicClip != null ? "muzik baglandi" : "muzik dosyasi bulunamadi") + ").");
        SaveScene();
    }

    void SetupBuildSettings()
    {
        string gameScenePath = "Assets/player.unity";
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(gameScenePath) == null)
        {
            Debug.LogWarning("Assets/player.unity bulunamadi, Build Settings guncellenemedi.");
            return;
        }

        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(gameScenePath, true)
        };

        PlayerSettings.productName = "SurvivorGame";
        if (string.IsNullOrEmpty(PlayerSettings.companyName) || PlayerSettings.companyName == "DefaultCompany")
        {
            PlayerSettings.companyName = "Berra Aktel";
        }

        Debug.Log("Build Settings guncellendi: sahne = " + gameScenePath + ", urun adi = " + PlayerSettings.productName + ", sirket adi = " + PlayerSettings.companyName);
    }

    Sprite LoadPlayerSprite(int index)
    {
        string pngPath = string.Format("Assets/Sprites/PNG/Players/Tiles/tile_{0:D4}.png", index);
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(pngPath))
        {
            Sprite s = obj as Sprite;
            if (s != null) return s;
        }
        return null;
    }

    void BuildPauseMenu()
    {
        EnsureEventSystem();
        Canvas canvas = FindOrCreateCanvas();

        Text title, resumeText, settingsText, quitText;
        GameObject pausePanel = CreatePausePanel(canvas.transform, out title, out resumeText, out settingsText, out quitText);

        Text settingsTitle, sfxLabel, musicLabel, backText;
        Slider sfxSlider, musicSlider;
        GameObject settingsPanel = CreateSettingsPanel(canvas.transform, out settingsTitle, out sfxLabel, out musicLabel, out backText, out sfxSlider, out musicSlider);

        PauseMenuUI pause = canvas.GetComponent<PauseMenuUI>();
        if (pause == null) pause = canvas.gameObject.AddComponent<PauseMenuUI>();
        pause.pausePanel = pausePanel;
        pause.settingsPanel = settingsPanel;
        pause.titleText = title;
        pause.resumeButtonText = resumeText;
        pause.settingsButtonText = settingsText;
        pause.quitButtonText = quitText;
        pause.settingsTitleText = settingsTitle;
        pause.sfxLabel = sfxLabel;
        pause.musicLabel = musicLabel;
        pause.backButtonText = backText;
        pause.sfxSlider = sfxSlider;
        pause.musicSlider = musicSlider;

        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("Duraklatma menusu kuruldu (ESC ile ac/kapa, Devam Et / Ayarlar / Cikis).");
        SaveScene();
    }

    GameObject CreatePausePanel(Transform parent, out Text title, out Text resumeText, out Text settingsText, out Text quitText)
    {
        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Color borderColor = new Color(0.18f, 0.11f, 0.06f, 1f);
        Color backdropColor = new Color(0.16f, 0.10f, 0.06f, 0.9f);
        Color sandColor = new Color(0.86f, 0.66f, 0.38f, 1f);
        Color rustColor = new Color(0.76f, 0.38f, 0.16f, 1f);
        Color oliveColor = new Color(0.46f, 0.38f, 0.22f, 1f);
        Color slateColor = new Color(0.34f, 0.40f, 0.42f, 1f);

        Transform existingOld = parent.Find("PausePanel");
        if (existingOld != null) Object.DestroyImmediate(existingOld.gameObject);

        GameObject panelGO = new GameObject("PausePanel", typeof(RectTransform));
        panelGO.transform.SetParent(parent, false);
        panelGO.transform.SetAsLastSibling();
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = backdropColor;
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject frameGO = new GameObject("MenuFrame", typeof(RectTransform));
        frameGO.transform.SetParent(panelGO.transform, false);
        Image frameBorder = frameGO.AddComponent<Image>();
        frameBorder.color = borderColor;
        RectTransform frameRect = frameGO.GetComponent<RectTransform>();
        frameRect.anchorMin = new Vector2(0.5f, 0.5f);
        frameRect.anchorMax = new Vector2(0.5f, 0.5f);
        frameRect.pivot = new Vector2(0.5f, 0.5f);
        frameRect.sizeDelta = new Vector2(560f, 500f);
        frameRect.anchoredPosition = Vector2.zero;

        GameObject frameFillGO = new GameObject("Fill", typeof(RectTransform));
        frameFillGO.transform.SetParent(frameGO.transform, false);
        Image frameFill = frameFillGO.AddComponent<Image>();
        frameFill.color = sandColor;
        RectTransform frameFillRect = frameFillGO.GetComponent<RectTransform>();
        frameFillRect.anchorMin = Vector2.zero;
        frameFillRect.anchorMax = Vector2.one;
        frameFillRect.offsetMin = new Vector2(14f, 14f);
        frameFillRect.offsetMax = new Vector2(-14f, -14f);

        GameObject titleGO = new GameObject("TitleText", typeof(RectTransform));
        titleGO.transform.SetParent(frameGO.transform, false);
        Text titleTxt = titleGO.AddComponent<Text>();
        titleTxt.text = "DURAKLATILDI";
        titleTxt.font = builtinFont;
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.color = Color.white;
        MakeReadable(titleTxt, 42, 2f);
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.84f);
        titleRect.anchorMax = new Vector2(0.5f, 0.84f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(500f, 90f);
        titleRect.anchoredPosition = Vector2.zero;

        Button resumeBtn;
        Text resumeTxt = CreatePixelButton(frameGO.transform, "ResumeButton", "DEVAM ET", 0.60f,
            new Vector2(320f, 84f), borderColor, rustColor, builtinFont, 30, out resumeBtn);

        Button settingsBtn;
        Text settingsTxt = CreatePixelButton(frameGO.transform, "SettingsButton", "AYARLAR", 0.37f,
            new Vector2(320f, 84f), borderColor, slateColor, builtinFont, 30, out settingsBtn);

        Button quitBtn;
        Text quitTxt = CreatePixelButton(frameGO.transform, "QuitButton", "CIKIS", 0.14f,
            new Vector2(320f, 76f), borderColor, oliveColor, builtinFont, 28, out quitBtn);

        title = titleTxt;
        resumeText = resumeTxt;
        settingsText = settingsTxt;
        quitText = quitTxt;
        return panelGO;
    }

    GameObject CreateSettingsPanel(Transform parent, out Text title, out Text sfxLabel, out Text musicLabel, out Text backText, out Slider sfxSlider, out Slider musicSlider)
    {
        Font builtinFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        Color borderColor = new Color(0.18f, 0.11f, 0.06f, 1f);
        Color backdropColor = new Color(0.16f, 0.10f, 0.06f, 0.9f);
        Color sandColor = new Color(0.86f, 0.66f, 0.38f, 1f);
        Color rustColor = new Color(0.76f, 0.38f, 0.16f, 1f);
        Color oliveColor = new Color(0.46f, 0.38f, 0.22f, 1f);
        Color trackColor = new Color(0.30f, 0.20f, 0.11f, 1f);

        Transform existingOld = parent.Find("PauseSettingsPanel");
        if (existingOld != null) Object.DestroyImmediate(existingOld.gameObject);

        GameObject panelGO = new GameObject("PauseSettingsPanel", typeof(RectTransform));
        panelGO.transform.SetParent(parent, false);
        panelGO.transform.SetAsLastSibling();
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = backdropColor;
        RectTransform panelRect = panelGO.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        GameObject frameGO = new GameObject("MenuFrame", typeof(RectTransform));
        frameGO.transform.SetParent(panelGO.transform, false);
        Image frameBorder = frameGO.AddComponent<Image>();
        frameBorder.color = borderColor;
        RectTransform frameRect = frameGO.GetComponent<RectTransform>();
        frameRect.anchorMin = new Vector2(0.5f, 0.5f);
        frameRect.anchorMax = new Vector2(0.5f, 0.5f);
        frameRect.pivot = new Vector2(0.5f, 0.5f);
        frameRect.sizeDelta = new Vector2(560f, 460f);
        frameRect.anchoredPosition = Vector2.zero;

        GameObject frameFillGO = new GameObject("Fill", typeof(RectTransform));
        frameFillGO.transform.SetParent(frameGO.transform, false);
        Image frameFill = frameFillGO.AddComponent<Image>();
        frameFill.color = sandColor;
        RectTransform frameFillRect = frameFillGO.GetComponent<RectTransform>();
        frameFillRect.anchorMin = Vector2.zero;
        frameFillRect.anchorMax = Vector2.one;
        frameFillRect.offsetMin = new Vector2(14f, 14f);
        frameFillRect.offsetMax = new Vector2(-14f, -14f);

        GameObject titleGO = new GameObject("TitleText", typeof(RectTransform));
        titleGO.transform.SetParent(frameGO.transform, false);
        Text titleTxt = titleGO.AddComponent<Text>();
        titleTxt.text = "AYARLAR";
        titleTxt.font = builtinFont;
        titleTxt.alignment = TextAnchor.MiddleCenter;
        titleTxt.color = Color.white;
        MakeReadable(titleTxt, 38, 2f);
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.85f);
        titleRect.anchorMax = new Vector2(0.5f, 0.85f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(500f, 80f);
        titleRect.anchoredPosition = Vector2.zero;

        Text sfxLbl;
        Slider sfxSl = CreatePixelSlider(frameGO.transform, "SfxSlider", "EFEKT SESI", 0.63f, borderColor, trackColor, rustColor, builtinFont, out sfxLbl);

        Text musicLbl;
        Slider musicSl = CreatePixelSlider(frameGO.transform, "MusicSlider", "MUZIK SESI", 0.44f, borderColor, trackColor, rustColor, builtinFont, out musicLbl);

        Button backBtn;
        Text backTxt = CreatePixelButton(frameGO.transform, "BackButton", "GERI", 0.16f,
            new Vector2(280f, 76f), borderColor, oliveColor, builtinFont, 28, out backBtn);

        title = titleTxt;
        sfxLabel = sfxLbl;
        musicLabel = musicLbl;
        backText = backTxt;
        sfxSlider = sfxSl;
        musicSlider = musicSl;
        return panelGO;
    }

    // Builds one labeled pixel-style volume slider: a small label above a
    // bordered track, with a colored fill that grows/shrinks as the value
    // changes. No drag handle - clicking/dragging anywhere on the track
    // sets the value directly, which is easier on a trackpad than grabbing
    // a tiny thumb.
    Slider CreatePixelSlider(Transform parent, string name, string label, float yAnchor, Color borderColor, Color trackColor, Color fillColor, Font font, out Text labelText)
    {
        GameObject labelGO = new GameObject(name + "Label", typeof(RectTransform));
        labelGO.transform.SetParent(parent, false);
        Text labelTxt = labelGO.AddComponent<Text>();
        labelTxt.text = label;
        labelTxt.font = font;
        labelTxt.alignment = TextAnchor.MiddleCenter;
        labelTxt.color = Color.white;
        MakeReadable(labelTxt, 22, 1.5f);
        RectTransform labelRect = labelGO.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, yAnchor + 0.10f);
        labelRect.anchorMax = new Vector2(0.5f, yAnchor + 0.10f);
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.sizeDelta = new Vector2(420f, 44f);
        labelRect.anchoredPosition = Vector2.zero;

        GameObject sliderGO = new GameObject(name, typeof(RectTransform));
        sliderGO.transform.SetParent(parent, false);
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, yAnchor);
        sliderRect.anchorMax = new Vector2(0.5f, yAnchor);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.sizeDelta = new Vector2(420f, 36f);
        sliderRect.anchoredPosition = Vector2.zero;

        Image trackBorder = sliderGO.AddComponent<Image>();
        trackBorder.color = borderColor;
        trackBorder.raycastTarget = true;

        Slider slider = sliderGO.AddComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        GameObject trackFillGO = new GameObject("Track", typeof(RectTransform));
        trackFillGO.transform.SetParent(sliderGO.transform, false);
        Image trackFillImg = trackFillGO.AddComponent<Image>();
        trackFillImg.color = trackColor;
        trackFillImg.raycastTarget = false;
        RectTransform trackFillRect = trackFillGO.GetComponent<RectTransform>();
        trackFillRect.anchorMin = Vector2.zero;
        trackFillRect.anchorMax = Vector2.one;
        trackFillRect.offsetMin = new Vector2(4f, 4f);
        trackFillRect.offsetMax = new Vector2(-4f, -4f);

        GameObject fillAreaGO = new GameObject("FillArea", typeof(RectTransform));
        fillAreaGO.transform.SetParent(trackFillGO.transform, false);
        RectTransform fillAreaRect = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = Vector2.zero;
        fillAreaRect.offsetMax = Vector2.zero;

        GameObject fillGO = new GameObject("Fill", typeof(RectTransform));
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        Image fillImg = fillGO.AddComponent<Image>();
        fillImg.color = fillColor;
        fillImg.raycastTarget = false;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;
        slider.targetGraphic = trackBorder;
        slider.value = 0.8f;

        labelText = labelTxt;
        return slider;
    }
}
