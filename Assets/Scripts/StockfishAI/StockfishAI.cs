using UnityEngine;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;

public class StockfishAI : MonoBehaviour
{
    private Process stockfishProcess;

    private void Start()
    {
        StartStockfish();
        SendCommand("uci");
        SendCommand("isready");
        // You can send other necessary commands to initialize the engine here
    }

    private void OnDestroy()
    {
        StopStockfish();
    }

    private void StartStockfish()
    {
        stockfishProcess = new Process();
        stockfishProcess.StartInfo.FileName = Application.dataPath + "/Stockfish/stockfish-windows-x86-64-avx2.exe";
        stockfishProcess.StartInfo.UseShellExecute = false;
        stockfishProcess.StartInfo.RedirectStandardInput = true;
        stockfishProcess.StartInfo.RedirectStandardOutput = true;
        stockfishProcess.StartInfo.CreateNoWindow = true;
        stockfishProcess.OutputDataReceived += OnOutputDataReceived;
        stockfishProcess.Start();
        stockfishProcess.BeginOutputReadLine();
        Debug.Log("Stockfish Running");
    }

    private void StopStockfish()
    {
        if (stockfishProcess != null && !stockfishProcess.HasExited)
        {
            stockfishProcess.StandardInput.WriteLine("quit");
            stockfishProcess.WaitForExit();
            stockfishProcess.Close();
        }
    }

    private void SendCommand(string command)
    {
        if (stockfishProcess != null && stockfishProcess.HasExited == false)
        {
            stockfishProcess.StandardInput.WriteLine(command);
            stockfishProcess.StandardInput.Flush();
        }
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data != null)
        {
            // Handle the output data received from Stockfish here
            Debug.Log("Stockfish: " + e.Data);
            ProcessStockfishResponse(e.Data);
        }
    }

    private void ProcessStockfishResponse(string response)
    {
        // Implement logic to parse the response and handle different cases
        // For example, you can extract the best move, score, etc.

        // Example: Parsing the best move
        if (response.StartsWith("bestmove"))
        {
            string[] parts = response.Split(' ');
            if (parts.Length >= 2)
            {
                string bestMove = parts[1];
                Debug.Log("Best move: " + bestMove);

                // Integrate the best move into your game logic here
                // Update the chessboard state, move the pieces, etc.
            }
        }
    }

    // Example method to send a move to Stockfish
    public void SendMoveToStockfish(string move)
    {
        string command = "position startpos moves " + move;
        SendCommand(command);
        SendCommand("go");
    }

    //// Example usage in your game
    //private void PlayMove()
    //{
    //    string playerMove = GetPlayerMove(); // Get the player's move
    //    SendMoveToStockfish(playerMove);
    //}
}
