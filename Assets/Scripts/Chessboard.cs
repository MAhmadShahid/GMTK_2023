using System.Collections.Generic;
using UnityEngine;

public enum SpecialMove {
    None = 0,
    Enpassant,
    Castling,
    Promotion
}
public class Chessboard : MonoBehaviour {

    [Header("Art Section")]
    [SerializeField] private Material _tileMaterial;
    [SerializeField] private Material _hoverMaterial;
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private float tileSize = 0.7f;
    [SerializeField] private float yOffSet = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathScale = 10.0f;
    [SerializeField] private float deathSpacing = 5.0f;
    [SerializeField] private float dragOffset = 1.0f;

    [Header("Pieces Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;


    // Logic Section

    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    private SpecialMove specialMove;
    private GameObject[,] _tiles;
    private ChessPiece[,] chessPieces;
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();

    private bool isWhiteTurn;
    private Vector3 bounds;


    // raycast section
    private Camera _currentCamera;
    private Vector2Int _currentHover;

    //UI Section
    [SerializeField] private GameObject victoryScreen;

    private void Awake() {
        isWhiteTurn = true;
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }

    // Update is called once per frame
    void Update() {
        if (!_currentCamera) {
            _currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = _currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight"))) {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);
            Debug.Log($"Tile: ({hitPosition.x}, {hitPosition.y})");

            // If we're hovering a tile after not hovering any tiles
            if (_currentHover == -Vector2Int.one) {
                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                _tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = _hoverMaterial;
            }

            // If we were already hovering a tile, change the previous one
            if (_currentHover != hitPosition) {
                if (!ContainsValidMove(ref availableMoves, _currentHover)) {
                    _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    _tiles[_currentHover.x, _currentHover.y].GetComponent<MeshRenderer>().material = _tileMaterial;
                }
                else// if(ContainsValidMove(ref availableMoves, _currentHover) && Input.GetMouseButtonDown(0))
                {
                    _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Highlight");
                    _tiles[_currentHover.x, _currentHover.y].GetComponent<MeshRenderer>().material = _highlightMaterial;
                }

                _currentHover = hitPosition;
                _tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                _tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = _hoverMaterial;
            }


            // on mouse button pressed
            if (Input.GetMouseButtonDown(0)) {
                if (chessPieces[hitPosition.x, hitPosition.y] != null) {
                    // Is it our turn?
                    if ((chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn)) {
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                        // get list of allowed tiles we can move to, also highlight them
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        //Get a list of special moves 
                        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);

                        HighlightTiles();
                    }
                }
            }

            // on mouse button release
            if (currentlyDragging != null && Input.GetMouseButtonUp(0)) {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                if (!validMove)
                    currentlyDragging.SetPosition(GetTileCentre(previousPosition.x, previousPosition.y));

                currentlyDragging = null;
                RemoveHighlightTiles();

            }

        }
        else {
            if (_currentHover != -Vector2Int.one) {
                if (!ContainsValidMove(ref availableMoves, _currentHover)) {
                    _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    _tiles[_currentHover.x, _currentHover.y].GetComponent<MeshRenderer>().material = _tileMaterial;
                }
                else {
                    _tiles[_currentHover.x, _currentHover.y].layer = LayerMask.NameToLayer("Highlight");
                    _tiles[_currentHover.x, _currentHover.y].GetComponent<MeshRenderer>().material = _highlightMaterial;
                }

                _currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0)) {
                currentlyDragging.SetPosition(GetTileCentre(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }

        if (currentlyDragging) {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffSet);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
        }

    }



    // Generate the board
    void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY) {
        yOffSet += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountY / 2) * tileSize) + boardCenter;



        // generate the tile grid
        _tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                _tiles[x, y] = GenerateSingleTile(tileSize, x, y);

    }

    GameObject GenerateSingleTile(float tileSize, int x, int y) {
        // object will be created at position 0, 0
        GameObject tile = new GameObject($"X:{x}, Y:{y}");
        tile.transform.parent = transform;


        Mesh mesh = new Mesh();
        tile.AddComponent<MeshFilter>().mesh = mesh;
        tile.AddComponent<MeshRenderer>().material = _tileMaterial;


        // setting up the vertices for our square tile
        Vector3[] vertices = new Vector3[4];

        // objects rendered mesh will be created at its respective position defined by these vertices
        vertices[0] = new Vector3(x * tileSize, yOffSet, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffSet, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffSet, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffSet, (y + 1) * tileSize) - bounds;

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

    // Spawining Pieces
    private void SpawnAllPieces() {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        int whiteTeam = 0, blackTeam = 1;

        //whiteTeam
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        //BlackTeam
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team) {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        if (team == 0)
            cp.transform.Rotate(0, 0, 180);

        return cp;
    }

    // Positioning
    private void PositionAllPieces() {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }
    private void PositionSinglePiece(int x, int y, bool force = false) {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCentre(x, y), force);
    }
    private Vector3 GetTileCentre(int x, int y) {
        return new Vector3(x * tileSize, yOffSet, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    // Highlighting tiles
    private void HighlightTiles() {
        for (int i = 0; i < availableMoves.Count; i++) {
            _tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
            _tiles[availableMoves[i].x, availableMoves[i].y].GetComponent<MeshRenderer>().material = _highlightMaterial;
        }
    }

    private void RemoveHighlightTiles() {
        for (int i = 0; i < availableMoves.Count; i++) {
            _tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
            _tiles[availableMoves[i].x, availableMoves[i].y].GetComponent<MeshRenderer>().material = _tileMaterial;
        }

        availableMoves.Clear();
    }

    // Operations

    //Special Moves 
    private void ProcessSpecialMove() {
        if (specialMove == SpecialMove.Enpassant) {
            var newMove = moveList[moveList.Count - 1];
            ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece enemePawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

            if (myPawn.currentX == enemePawn.currentX) {
                if (myPawn.currentY == enemePawn.currentY - 1 || myPawn.currentY == enemePawn.currentY + 1) {
                    if (enemePawn.team == 0) {
                        deadWhites.Add(enemePawn);
                        enemePawn.SetScale(Vector3.one * deathScale);
                        enemePawn.SetPosition(new Vector3(8 * tileSize, yOffSet, -1 * tileSize)
                            - bounds
                            + new Vector3(tileSize / 2, 0, tileSize / 2)
                            + (Vector3.forward * deathSpacing) * deadWhites.Count);
                    }
                    else {
                        deadBlacks.Add(enemePawn);
                        enemePawn.SetScale(Vector3.one * deathScale);
                        enemePawn.SetPosition(new Vector3(8 * tileSize, yOffSet, -1 * tileSize)
                            - bounds
                            + new Vector3(tileSize / 2, 0, tileSize / 2)
                            + (Vector3.forward * deathSpacing) * deadBlacks.Count);
                    }
                    chessPieces[enemePawn.currentX, enemePawn.currentY] = null;
                }
            }
        }
        if (specialMove == SpecialMove.Promotion) {

            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

            if (targetPawn.type == ChessPieceType.Pawn) {

                if (targetPawn.team == 0 && lastMove[1].y == 7) {

                    ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 0);
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);

                }

                if (targetPawn.team == 1 && lastMove[1].y == 0) {

                    ChessPiece newQueen = SpawnSinglePiece(ChessPieceType.Queen, 0);
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position;
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);

                }
            }
        }
        if (specialMove == SpecialMove.Castling) {
            var lastMove = moveList[moveList.Count - 1];
            //Left rook
            if (lastMove[1].x == 2) {
                if (lastMove[1].y == 0) { //white side

                    ChessPiece rook = chessPieces[0, 0];
                    chessPieces[3, 0] = rook;
                    PositionSinglePiece(3, 0);
                    chessPieces[0, 0] = null;
                }
                else if (lastMove[1].y == 7) { //blackside
                    ChessPiece rook = chessPieces[0, 7];
                    chessPieces[3, 7] = rook;
                    PositionSinglePiece(3, 7);
                    chessPieces[0, 7] = null;
                }

            }
            //right rook
            else if (lastMove[1].x == 6) {
                if (lastMove[1].y == 0) { //white side

                    ChessPiece rook = chessPieces[7, 0];
                    chessPieces[5, 0] = rook;
                    PositionSinglePiece(5, 0);
                    chessPieces[7, 0] = null;
                }
                else if (lastMove[1].y == 7) { //blackside
                    ChessPiece rook = chessPieces[7, 7];
                    chessPieces[5, 7] = rook;
                    PositionSinglePiece(5, 7);
                    chessPieces[7, 7] = null;
                }

            }
        }
    }
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos) {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }
    private bool MoveTo(ChessPiece cp, int x, int y) {

        if (!ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
            return false;

        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        // is there another chess piece in that position ?
        if (chessPieces[x, y] != null) {
            ChessPiece otherChessPiece = chessPieces[x, y];

            if (cp.team == otherChessPiece.team)
                return false;


            // if its the enemy team
            if (otherChessPiece.team == 0) {
                if (otherChessPiece.type == ChessPieceType.King)
                    CheckMate(1);
                deadWhites.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathScale);
                otherChessPiece.SetPosition(new Vector3(8 * tileSize, yOffSet, -1 * tileSize)
                    - bounds
                    + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.forward * deathSpacing) * deadWhites.Count);
            }
            else {
                if (otherChessPiece.type == ChessPieceType.King)
                    CheckMate(0);
                deadBlacks.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathScale);
                otherChessPiece.SetPosition(new Vector3(-1 * tileSize, yOffSet, 8 * tileSize)
                    - bounds
                    + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.back * deathSpacing) * deadBlacks.Count);
            }
        }


        chessPieces[x, y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSinglePiece(x, y);

        isWhiteTurn = !isWhiteTurn;
        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });
        ProcessSpecialMove();
        return true;
    }
    private void CheckMate(int team) {
        DisplayVictory(team);
    }
    private void DisplayVictory(int winningTeam) {
        victoryScreen.SetActive(true);
        victoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }
    public void onResetButton() {
        //UI
        victoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        victoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        victoryScreen.SetActive(false);
        //Fields Reset
        currentlyDragging = null;
        availableMoves.Clear();
        moveList.Clear();
        //Cleanup
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++) {
                if (chessPieces[x, y] != null)
                    Destroy(chessPieces[x, y].gameObject);
                chessPieces[x, y] = null;
            }
        for (int i = 0; i < deadWhites.Count; i++)
            Destroy(deadWhites[i].gameObject);
        for (int i = 0; i < deadBlacks.Count; i++)
            Destroy(deadBlacks[i].gameObject);
        deadWhites.Clear();
        deadBlacks.Clear();

        //Spawining Again
        SpawnAllPieces();
        PositionAllPieces();
        isWhiteTurn = true;

    }
    public void onExitButton() {
        Application.Quit();
    }
    private Vector2Int LookUpTileIndex(GameObject hitInfo) {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (_tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        // for invalid return path
        return -Vector2Int.one;
    }
}
