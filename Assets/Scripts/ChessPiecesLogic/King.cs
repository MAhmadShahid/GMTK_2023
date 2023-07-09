using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece {
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY) {
        List<Vector2Int> returnList = new List<Vector2Int>();

        int[] x = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] y = { 1, 1, 0, -1, -1, -1, 0, 1 };

        for (int i = 0; i < x.Length; i++) {
            if (currentX + x[i] >= 0 && currentX + x[i] < tileCountX && currentY + y[i] >= 0 && currentY + y[i] < tileCountY)
                if (board[currentX + x[i], currentY + y[i]] == null || board[currentX + x[i], currentY + y[i]].team != team)
                    returnList.Add(new Vector2Int(currentX + x[i], currentY + y[i]));
        }

        return returnList;
    }
    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availaibleMoves) {
        SpecialMove r = SpecialMove.None;
        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == ((team == 0) ? 0 : 7));
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == ((team == 0) ? 0 : 7));
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == ((team == 0) ? 0 : 7));
        if (kingMove == null && currentX == 4) {
            //white team
            if (team == 0) {
                //left rook
                if (leftRook == null) {

                    if (board[0, 0].type == ChessPieceType.Rook)

                        if (board[0, 0].team == 0)

                            if (board[3, 0] == null)

                                if (board[2, 0] == null)

                                    if (board[1, 0] == null) {
                                        availaibleMoves.Add(new Vector2Int(2, 0));
                                        r = SpecialMove.Castling;
                                    }

                }
                //right rook
                if (rightRook == null) {
                    if (board[7, 0].type == ChessPieceType.Rook)
                        if (board[7, 0].team == 0)
                            if (board[5, 0] == null)
                                if (board[6, 0] == null) {
                                    availaibleMoves.Add(new Vector2Int(6, 0));
                                    r = SpecialMove.Castling;
                                }

                }
            }
            else {
                //left rook
                if (leftRook == null) {
                    if (board[0, 7].type == ChessPieceType.Rook)
                        if (board[0, 7].team == 1)
                            if (board[3, 7] == null)
                                if (board[2, 7] == null)
                                    if (board[1, 7] == null) {
                                        availaibleMoves.Add(new Vector2Int(2, 7));
                                        r = SpecialMove.Castling;
                                    }

                }
                //right rook
                if (rightRook == null) {
                    if (board[7, 7].type == ChessPieceType.Rook)

                        if (board[7, 7].team == 1)

                            if (board[5, 7] == null)

                                if (board[6, 7] == null) {

                                    availaibleMoves.Add(new Vector2Int(6, 7));
                                    r = SpecialMove.Castling;

                                }

                }
            }

        }
        return r;
    }
}
