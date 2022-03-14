using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MazeManager;

[CreateAssetMenu]
public class BackgroundTile : ScriptableObject
{
    public enum TileType { Fixed, Animated, Structure }

    [SerializeField] private TileType type;

    [Header("Fixed")]
    [SerializeField] private TileBase tile;

    [Header("Animated")]
    [SerializeField] private float animSpeed = 10;
    [SerializeField] private List<TileBase> upTiles;
    [SerializeField] private List<TileBase> downTiles;

    [Header("Structure")]
    [SerializeField] private int structureWidth = 1;
    [SerializeField] private int structureHeight = 1;
    [SerializeField] private List<TileBase> structureTiles;

    public TileType Type { get { return type; } }
    public int Width { get { return structureWidth; } }
    public int Height { get { return structureHeight; } }

    public List<(Vector3Int, bool)> animList = new List<(Vector3Int, bool)>();
    private List<IEnumerator> danceRoutineList = new List<IEnumerator>();

    public void Reset()
    {
        animList.Clear();

        foreach (IEnumerator routine in danceRoutineList)
            if (routine != null)
                instance.StopCoroutine(routine);

        danceRoutineList.Clear();
    }
    
    public List<Vector3Int> SetTile(Vector3Int pos)
    {
        List<Vector3Int> occupiedTiles = new List<Vector3Int>();
        
        if (type == TileType.Fixed)
            backgroundTilemap.SetTile(pos, tile);
        else if (type == TileType.Animated)
        {
            bool up = Random.Range(0, 2) == 1;
            animList.Add((pos, up));

            if (up)
                backgroundTilemap.SetTile(pos, upTiles[0]);
            else
                backgroundTilemap.SetTile(pos, downTiles[0]);

            occupiedTiles.Add(pos);
        }
        else
        {
            occupiedTiles = SetStructureTiles(pos);
        }

        return occupiedTiles;
    }

    public List<Vector3Int> GetStructureTiles(Vector3Int topRight)
    {
        List<Vector3Int> positions = new List<Vector3Int>();

        for (int i = 0; i < structureWidth; i++)
            for (int j = 0; j < structureHeight; j++)
                positions.Add(new Vector3Int(topRight.x - i, topRight.y - j, 0));

        return positions;
    }

    private List<Vector3Int> SetStructureTiles(Vector3Int topRight)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        int idx = 0;

        for (int i = 0; i < structureWidth; i++)
        {
            for (int j = 0; j < structureHeight; j++)
            {
                Vector3Int pos = new Vector3Int(topRight.x - i, topRight.y - j, 0);

                positions.Add(pos);
                backgroundTilemap.SetTile(pos, structureTiles[idx]);

                idx++;
            }
        }

        return positions;
    }

    public void Dance()
    {
        foreach (IEnumerator routine in danceRoutineList)
            if (routine != null)
                instance.StopCoroutine(routine);

        danceRoutineList.Clear();

        for (int i = 0; i < animList.Count; i++)
        {
            Vector3Int pos = animList[i].Item1;
            bool up = animList[i].Item2;
            IEnumerator danceRoutine;

            if (up)
                danceRoutine = UpRoutine(pos);
            else
                danceRoutine = DownRoutine(pos);

            animList[i] = (pos, !up);

            instance.StartCoroutine(danceRoutine);
            danceRoutineList.Add(danceRoutine);
        }
    }

    private IEnumerator UpRoutine(Vector3Int pos)
    {
        for (int i = 1; i < upTiles.Count; i++)
        {
            backgroundTilemap.SetTile(pos, upTiles[i]);
            yield return new WaitForSeconds(1 / animSpeed);
        }

        backgroundTilemap.SetTile(pos, downTiles[0]);
    }

    private IEnumerator DownRoutine(Vector3Int pos)
    {
        for (int i = 1; i < downTiles.Count; i++)
        {
            backgroundTilemap.SetTile(pos, downTiles[i]);
            yield return new WaitForSeconds(1 / animSpeed);
        }

        backgroundTilemap.SetTile(pos, upTiles[0]);
    }
}
