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
    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availaibleMoves) {
        int direction = (team == 0) ? 1 : -1;
        if ((team == 0 && currentY == 6) || (team == 1 && currentY == 1)) {
            return SpecialMove.Promotion;
        }
        //En passant
        if(moveList.Count > 0) {
          
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
           
            if (board[lastMove[1].x,lastMove[1].y].type == ChessPieceType.Pawn) { //if the last piece moved was a pawn

                if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2) { //if the last move a +2 in either direction

                    if (board[lastMove[1].x, lastMove[1].y].team != team) {//if the move was from other team

                        if (lastMove[1].y == currentY) { //if both pawn are on the same y

                            if (lastMove[1].x == currentX - 1) { //Landed left

                                availaibleMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                                return SpecialMove.Enpassant;
                            }

                            if (lastMove[1].x == currentX + 1) { //Landed right

                                availaibleMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                                return SpecialMove.Enpassant;
                            }
                        }
                    }
                }
            }
        }

            return SpecialMove.None;
    }
}
