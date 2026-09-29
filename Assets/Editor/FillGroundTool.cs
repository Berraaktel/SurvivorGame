using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class FillGroundTool : EditorWindow
{
    [MenuItem("Tools/Fill Ground Tilemap")]
    public static void ShowWindow()
    {
        GetWindow<FillGroundTool>("Fill Ground");
    }

    public Tilemap targetTilemap;
    public TileBase tile;
    public int width = 20;
    public int height = 20;

    void OnGUI()
    {
        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", targetTilemap, typeof(Tilemap), true);
        tile = (TileBase)EditorGUILayout.ObjectField("Tile", tile, typeof(TileBase), false);
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);

        if (GUILayout.Button("Fill"))
        {
            Fill();
        }
    }

    void Fill()
    {
        if (targetTilemap == null || tile == null) return;

        int halfW = width / 2;
        int halfH = height / 2;

        for (int x = -halfW; x < halfW; x++)
        {
            for (int y = -halfH; y < halfH; y++)
            {
                targetTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        EditorUtility.SetDirty(targetTilemap);
    }
}