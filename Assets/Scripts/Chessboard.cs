using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{

    [Header("Art Section")]
    [SerializeField] private Material _tileMaterial;
    [SerializeField] private Material _hoverMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffSet = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    

    // Logic Section

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private GameObject[,] _tiles;

    private Vector3 bounds;


    // raycast section
    private Camera _currentCamera;
    private Vector2Int _currentHover;

    private void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_currentCamera)
        {
            _currentCamera = Camera.current;
            return;
        }

        RaycastHit info;
        Ray ray = _currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);
            Debug.Log($"Tile: ({hitPosition.x}, {hitPosition.y})");

            // If we're hovering a tile after not hovering any tiles
            if (_currentHover == -Vector2Int.one)
            {
                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                _tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = _hoverMaterial;
            }

            // If we were already hovering a tile, change the previous one
            if (_currentHover != hitPosition)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                _tiles[_currentHover.x, _currentHover.y].GetComponent<MeshRenderer>().material = _tileMaterial;
                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                _tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = _hoverMaterial;
            }
            
        }
        else
        {
            if (_currentHover != -Vector2Int.one)
            {
                _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                _tiles[_currentHover.x, _currentHover.y].GetComponent<MeshRenderer>().material = _tileMaterial;
                _currentHover = -Vector2Int.one;
            }
        }
    }


    // Generate the board
    void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffSet += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountY / 2) * tileSize) + boardCenter;



        // generate the tile grid
        _tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                _tiles[x, y] = GenerateSingleTile(tileSize, x, y);

    }

    GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        // object will be created at position 0, 0
        GameObject tile = new GameObject($"X:{x}, Y:{y}");
        tile.transform.parent = transform; 

        
        Mesh mesh = new Mesh();
        tile.AddComponent<MeshFilter>().mesh = mesh;
        tile.AddComponent<MeshRenderer>().material = _tileMaterial;


        // setting up the vertices for our square tile
        Vector3[] vertices = new Vector3[4];

        // objects rendered mesh will be created at its respective position defined by these vertices
        vertices[0] = new Vector3(x * tileSize, yOffSet, y * tileSize) - boardCenter;
        vertices[1] = new Vector3(x * tileSize, yOffSet, (y + 1) * tileSize) - boardCenter;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffSet, y * tileSize) - boardCenter;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffSet, (y + 1) * tileSize) - boardCenter;

        // grouping above vertices into two triangles
        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        // assigning defined vertices and triangles settings to our mesh
        mesh.vertices = vertices;
        mesh.triangles = tris;

        // adding box collider for raycast hits later
        tile.AddComponent<BoxCollider>();
        // add "Tile" layer to our tyle object
        tile.layer = LayerMask.NameToLayer("Tile");

        return tile;
    }


    private Vector2Int LookUpTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (_tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);
        
        // for invalid return path
        return -Vector2Int.one;
    }
}
