using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> returnList = new List<Vector2Int>();

        int[] x = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] y = { 1, 1, 0, -1, -1, -1, 0, 1 };

        for(int i = 0; i < x.Length; i++)
        {
            if (currentX + x[i] >= 0 && currentX + x[i] < tileCountX && currentY + y[i] >= 0 && currentY + y[i] < tileCountY)
                if(board[currentX + x[i], currentY + y[i]] == null || board[currentX + x[i], currentY + y[i]].team != team)
                    returnList.Add(new Vector2Int(currentX + x[i], currentY + y[i]));
        }

        return returnList;
    }
    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availaibleMoves) {
        SpecialMove r = SpecialMove.None;
        return r;
    }
}
