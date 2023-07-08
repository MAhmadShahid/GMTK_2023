using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{

    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> returnList = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        if (currentY == tileCountY - 1 || currentY == 0)
            return returnList;

        // one in front
        if (board[currentX, currentY + direction] == null ) 
        {
            returnList.Add(new Vector2Int(currentX, currentY + direction));
        }

        // two in front
        if (board[currentX, currentY + direction] == null)
        {
            // for white team
            if( team == 0 && currentY == 1 && board[currentX, currentY + direction*2] == null)
                returnList.Add(new Vector2Int(currentX, currentY + direction*2));

            // for the black team
            if (team == 1 && currentY == 6 && board[currentX, currentY + direction * 2] == null)
                returnList.Add(new Vector2Int(currentX, currentY + direction*2));
        }

        // kill move
        if(currentX != tileCountX - 1 && board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                returnList.Add(new Vector2Int(currentX + 1, currentY + direction));


        if(currentX != 0 && board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                returnList.Add(new Vector2Int(currentX - 1, currentY + direction));



        return returnList;
    }
}
