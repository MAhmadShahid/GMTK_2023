using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> returnList = new List<Vector2Int>();

        // rook 

        // down movement
        for (int i = currentY - 1; i >= 0; i--)
        {
            if (board[currentX, i] == null)
                returnList.Add(new Vector2Int(currentX, i));
            else
            {
                if (board[currentX, i].team != team)
                    returnList.Add(new Vector2Int(currentX, i));
                break;
            }
        }

        // forward movement
        for (int i = currentY + 1; i <= tileCountY - 1; i++)
        {
            if (board[currentX, i] == null)
                returnList.Add(new Vector2Int(currentX, i));
            else
            {
                if (board[currentX, i].team != team)
                    returnList.Add(new Vector2Int(currentX, i));

                break;
            }
        }

        // left movement
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (board[i, currentY] == null)
                returnList.Add(new Vector2Int(i, currentY));
            else
            {
                if (board[i, currentY].team != team)
                    returnList.Add(new Vector2Int(i, currentY));
                break;
            }
        }

        // right movement
        for (int i = currentX + 1; i <= tileCountX - 1; i++)
        {
            if (board[i, currentY] == null)
                returnList.Add(new Vector2Int(i, currentY));
            else
            {
                if (board[i, currentY].team != team)
                    returnList.Add(new Vector2Int(i, currentY));
                break;
            }
        }

        // bishop

        // top right
        for (int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y++)
        {
            if (board[x, y] == null)
                returnList.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    returnList.Add(new Vector2Int(x, y));
                break;
            }
        }

        // top left
        for (int x = currentX - 1, y = currentY + 1; x >= 0 && y < tileCountY; x--, y++)
        {
            if (board[x, y] == null)
                returnList.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    returnList.Add(new Vector2Int(x, y));
                break;
            }
        }

        // bottom right
        for (int x = currentX + 1, y = currentY - 1; x < tileCountX && y >= 0; x++, y--)
        {
            if (board[x, y] == null)
                returnList.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    returnList.Add(new Vector2Int(x, y));
                break;
            }
        }

        // bottom left
        for (int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y--)
        {
            if (board[x, y] == null)
                returnList.Add(new Vector2Int(x, y));
            else
            {
                if (board[x, y].team != team)
                    returnList.Add(new Vector2Int(x, y));
                break;
            }
        }


        return returnList;
    }
}
