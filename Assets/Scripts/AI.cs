using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] StockfishAI stockfish;
    [SerializeField] Chessboard chessboard;

    Dictionary<char, int> rowUCI = new Dictionary<char, int>();

    private void Start()
    {
        rowUCI.Add('a', 0);
        rowUCI.Add('b', 1);
        rowUCI.Add('c', 2);
        rowUCI.Add('d', 3);
        rowUCI.Add('e', 4);
        rowUCI.Add('f', 5);
        rowUCI.Add('g', 6);
        rowUCI.Add('h', 7);
    }



    public async void PlayAIMove(string playersMoveUCI, bool roleReverse)
    {
        chessboard.aiTurnInProcess = true;
        Debug.Log("AI's Turn");
        chessboard.RestrictPlayerToMove();
        string aiMove = await stockfish.BestMoveAsync(playersMoveUCI);
        Debug.Log($"Players Move: {playersMoveUCI}");
        RecieveAndPerformAIMove(aiMove, roleReverse);
    }

    public void RecieveAndPerformAIMove(string aiMoveUCI, bool roleReverse)
    {
        Debug.Log($"AI Move: {aiMoveUCI}");
        Vector2Int currentlySelectedPiece = new Vector2Int(rowUCI[aiMoveUCI[0]], (int)aiMoveUCI[1] - 1 - 48);
        Vector2Int newPosition = new Vector2Int(rowUCI[aiMoveUCI[2]], (int)aiMoveUCI[3] - 1 - 48);

        Debug.Log(currentlySelectedPiece.ToString());
        Debug.Log(newPosition.ToString());
        chessboard.MoveTo(chessboard.chessPieces[currentlySelectedPiece.x, currentlySelectedPiece.y], newPosition.x, newPosition.y, true, roleReverse); ;
        chessboard.AllowPlayerToMove();
        chessboard.totalTurn++;
        chessboard.aiTurnInProcess = false;
    }




}
