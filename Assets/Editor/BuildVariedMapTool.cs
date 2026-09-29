using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BuildVariedMapTool : EditorWindow
{
    [MenuItem("Tools/Build Varied Desert Map")]
    public static void ShowWindow()
    {
        GetWindow<BuildVariedMapTool>("Varied Map");
    }

    public Tilemap groundTilemap;
    public int width = 40;
    public int height = 40;
    [Range(0f, 0.15f)] public float propDensity = 0.05f;
    public bool placeLandmark = true;

    const string tilesFolder = "Assets/Sprites/PNG/Tiles/Tiles";

    static readonly int[] sandIndices = { 54, 55, 56, 64, 65, 72, 73, 74, 154, 155, 156, 157, 158, 172, 173, 174, 175, 176, 190, 191, 192, 193, 194 };
    static readonly int[] propIndices = { 57, 58, 59, 60, 61, 62, 63, 66, 75, 76, 80, 81, 82, 83, 84, 85 };

    Dictionary<int, Tile> tileCache = new Dictionary<int, Tile>();

    void OnEnable()
    {
        AutoFindGroundTilemap();
    }

    void AutoFindGroundTilemap()
    {
        if (groundTilemap != null) return;

        Tilemap[] all = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Tilemap tm in all)
        {
            if (tm.gameObject.name != "PropsTilemap")
            {
                groundTilemap = tm;
                tm.CompressBounds();
                BoundsInt b = tm.cellBounds;
                if (b.size.x > 0) width = b.size.x;
                if (b.size.y > 0) height = b.size.y;
                break;
            }
        }
    }

    void OnGUI()
    {
        AutoFindGroundTilemap();

        groundTilemap = (Tilemap)EditorGUILayout.ObjectField("Ground Tilemap", groundTilemap, typeof(Tilemap), true);
        if (groundTilemap == null)
        {
            EditorGUILayout.HelpBox("Sahnede Tilemap bulunamadi. Grid/Tilemap objesinin Hierarchy'de acik oldugundan emin ol.", MessageType.Warning);
        }
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        propDensity = EditorGUILayout.Slider("Prop Density", propDensity, 0f, 0.15f);
        placeLandmark = EditorGUILayout.Toggle("Place Tomb Landmark", placeLandmark);

        EditorGUILayout.Space();

        if (GUILayout.Button("Fix Sorting Layer Only"))
        {
            FixSortingLayer();
        }

        if (GUILayout.Button("Build Varied Map"))
        {
            BuildMap();
        }

        if (GUILayout.Button("Add Arena Boundary Walls"))
        {
            AddArenaBoundary();
        }
    }

    void AddArenaBoundary()
    {
        AutoFindGroundTilemap();
        if (groundTilemap == null)
        {
            Debug.LogWarning("Ground Tilemap atanmadi - sahnede Tilemap bulunamadi.");
            return;
        }

        groundTilemap.CompressBounds();
        BoundsInt cellBounds = groundTilemap.cellBounds;
        Vector3 worldMin = groundTilemap.CellToWorld(new Vector3Int(cellBounds.xMin, cellBounds.yMin, 0));
        Vector3 worldMax = groundTilemap.CellToWorld(new Vector3Int(cellBounds.xMax, cellBounds.yMax, 0));

        Transform parent = groundTilemap.transform.parent;
        if (parent == null) parent = groundTilemap.transform;

        Transform existing = parent.Find("ArenaBoundary");
        if (existing != null)
        {
            DestroyImmediate(existing.gameObject);
        }

        GameObject boundaryRoot = new GameObject("ArenaBoundary");
        boundaryRoot.transform.SetParent(parent, false);

        float thickness = 1f;
        float w = worldMax.x - worldMin.x;
        float h = worldMax.y - worldMin.y;
        Vector3 center = new Vector3((worldMin.x + worldMax.x) / 2f, (worldMin.y + worldMax.y) / 2f, 0);

        CreateWall(boundaryRoot.transform, "Wall_Bottom", new Vector3(center.x, worldMin.y - thickness / 2f, 0), new Vector2(w + thickness * 2, thickness));
        CreateWall(boundaryRoot.transform, "Wall_Top", new Vector3(center.x, worldMax.y + thickness / 2f, 0), new Vector2(w + thickness * 2, thickness));
        CreateWall(boundaryRoot.transform, "Wall_Left", new Vector3(worldMin.x - thickness / 2f, center.y, 0), new Vector2(thickness, h + thickness * 2));
        CreateWall(boundaryRoot.transform, "Wall_Right", new Vector3(worldMax.x + thickness / 2f, center.y, 0), new Vector2(thickness, h + thickness * 2));

        Debug.Log("Arena boundary eklendi.");
    }

    void CreateWall(Transform parent, string wallName, Vector3 position, Vector2 size)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.SetParent(parent, false);
        wall.transform.position = position;
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
    }

    Tilemap GetOrCreatePropsTilemap()
    {
        Transform grid = groundTilemap.transform.parent;
        if (grid == null)
        {
            Debug.LogWarning("Ground Tilemap bir Grid altinda degil.");
            return null;
        }

        Transform existing = grid.Find("PropsTilemap");
        if (existing != null)
        {
            return existing.GetComponent<Tilemap>();
        }

        GameObject go = new GameObject("PropsTilemap");
        go.transform.SetParent(grid, false);
        Tilemap tm = go.AddComponent<Tilemap>();
        TilemapRenderer tr = go.AddComponent<TilemapRenderer>();
        tr.sortingLayerName = "Background";
        tr.sortingOrder = 1;
        return tm;
    }

    void FixSortingLayer()
    {
        AutoFindGroundTilemap();
        if (groundTilemap == null)
        {
            Debug.LogWarning("Ground Tilemap atanmadi - sahnede Tilemap bulunamadi.");
            return;
        }

        TilemapRenderer groundRenderer = groundTilemap.GetComponent<TilemapRenderer>();
        groundRenderer.sortingLayerName = "Background";
        groundRenderer.sortingOrder = 0;

        Tilemap props = GetOrCreatePropsTilemap();
        if (props != null)
        {
            TilemapRenderer propsRenderer = props.GetComponent<TilemapRenderer>();
            propsRenderer.sortingLayerName = "Background";
            propsRenderer.sortingOrder = 1;
        }

        EditorUtility.SetDirty(groundTilemap);
        Debug.Log("Sorting layer duzeltildi: Background");
    }

    Tile LoadTileAsset(int index)
    {
        if (tileCache.TryGetValue(index, out Tile cached) && cached != null)
        {
            return cached;
        }

        string assetPath = string.Format("{0}/tile_{1:D4}_0.asset", tilesFolder, index);
        Tile existingAsset = AssetDatabase.LoadAssetAtPath<Tile>(assetPath);
        if (existingAsset != null)
        {
            tileCache[index] = existingAsset;
            return existingAsset;
        }

        string pngPath = string.Format("{0}/tile_{1:D4}.png", tilesFolder, index);
        Sprite sprite = null;
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(pngPath))
        {
            Sprite s = obj as Sprite;
            if (s != null)
            {
                sprite = s;
                break;
            }
        }

        if (sprite == null)
        {
            Debug.LogWarning("Sprite bulunamadi: " + pngPath);
            return null;
        }

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        AssetDatabase.CreateAsset(tile, assetPath);
        tileCache[index] = tile;
        return tile;
    }

    void BuildMap()
    {
        AutoFindGroundTilemap();
        if (groundTilemap == null)
        {
            Debug.LogWarning("Ground Tilemap atanmadi - sahnede Tilemap bulunamadi.");
            return;
        }

        FixSortingLayer();
        Tilemap propsTilemap = GetOrCreatePropsTilemap();

        int halfW = width / 2;
        int halfH = height / 2;

        int landmarkX = Mathf.Min(8, Mathf.Max(4, halfW - 3));
        int landmarkY = Mathf.Min(5, Mathf.Max(4, halfH - 5));
        bool canPlaceLandmark = placeLandmark && halfW > 12 && halfH > 10;

        for (int x = -halfW; x < halfW; x++)
        {
            for (int y = -halfH; y < halfH; y++)
            {
                int sandPick = sandIndices[Random.Range(0, sandIndices.Length)];
                Tile sandTile = LoadTileAsset(sandPick);
                groundTilemap.SetTile(new Vector3Int(x, y, 0), sandTile);

                bool inSafeZone = Mathf.Abs(x) <= 3 && Mathf.Abs(y) <= 3;
                bool inLandmarkZone = canPlaceLandmark && x >= landmarkX - 1 && x <= landmarkX + 2 && y >= landmarkY - 1 && y <= landmarkY + 4;

                if (propsTilemap != null && !inSafeZone && !inLandmarkZone && Random.value < propDensity)
                {
                    int propPick = propIndices[Random.Range(0, propIndices.Length)];
                    Tile propTile = LoadTileAsset(propPick);
                    propsTilemap.SetTile(new Vector3Int(x, y, 0), propTile);
                }
            }
        }

        if (canPlaceLandmark && propsTilemap != null)
        {
            PlaceLandmark(propsTilemap, landmarkX, landmarkY);
        }

        EditorUtility.SetDirty(groundTilemap);
        if (propsTilemap != null) EditorUtility.SetDirty(propsTilemap);
        AssetDatabase.SaveAssets();
        Debug.Log("Harita olusturuldu.");
    }

    void PlaceLandmark(Tilemap propsTilemap, int baseX, int baseY)
    {
        Tile t152 = LoadTileAsset(152);
        Tile t153 = LoadTileAsset(153);
        Tile t170 = LoadTileAsset(170);
        Tile t171 = LoadTileAsset(171);
        Tile t188 = LoadTileAsset(188);
        Tile t189 = LoadTileAsset(189);
        Tile t206 = LoadTileAsset(206);
        Tile t207 = LoadTileAsset(207);

        propsTilemap.SetTile(new Vector3Int(baseX, baseY + 3, 0), t152);
        propsTilemap.SetTile(new Vector3Int(baseX + 1, baseY + 3, 0), t153);
        propsTilemap.SetTile(new Vector3Int(baseX, baseY + 2, 0), t170);
        propsTilemap.SetTile(new Vector3Int(baseX + 1, baseY + 2, 0), t171);
        propsTilemap.SetTile(new Vector3Int(baseX, baseY + 1, 0), t188);
        propsTilemap.SetTile(new Vector3Int(baseX + 1, baseY + 1, 0), t189);
        propsTilemap.SetTile(new Vector3Int(baseX, baseY, 0), t206);
        propsTilemap.SetTile(new Vector3Int(baseX + 1, baseY, 0), t207);
    }
}
