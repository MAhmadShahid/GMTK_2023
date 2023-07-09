using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField] StockfishAI stockfish;
    [SerializeField] Chessboard chessboard;

    private void Start()
    {
        
    }

    public void PlayAIMove(string playersMoveUCI)
    {
        chessboard.RestrictPlayerToMove();



        chessboard.AllowPlayerToMove();
    }


}
