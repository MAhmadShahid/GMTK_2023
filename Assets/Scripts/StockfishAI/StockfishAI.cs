//using UnityEngine;
//using System.Diagnostics;
//using System;
//using Debug = UnityEngine.Debug;

//public class StockfishAI : MonoBehaviour
//{
//    public event Action<string> OnAIMoveReceived;

//    private Process stockfishProcess;

//    private void Start()
//    {
//        StartStockfish();
//        SendCommand("uci");
//        SendCommand("isready");
//        // You can send other necessary commands to initialize the engine here
//    }


//    ~StockfishAI() 
//    {
//        StopStockfish();
//    }

//    private void StartStockfish()
//    {
//        stockfishProcess = new Process();
//        stockfishProcess.StartInfo.FileName = Application.dataPath + "/Stockfish/stockfish-windows-x86-64-avx2.exe";
//        stockfishProcess.StartInfo.UseShellExecute = false;
//        stockfishProcess.StartInfo.CreateNoWindow = true;
//        stockfishProcess.StartInfo.RedirectStandardInput = true;
//        stockfishProcess.StartInfo.RedirectStandardOutput = true;
//        stockfishProcess.OutputDataReceived += OnOutputDataReceived;
//        stockfishProcess.Start();
//    }

//    private void StopStockfish()
//    {
//        if (stockfishProcess != null && !stockfishProcess.HasExited)
//        {
//            Debug.Log("Quit");
//            stockfishProcess.StandardInput.WriteLine("quit");
//            stockfishProcess.WaitForExit();
//            stockfishProcess.Close();
//        }
//    }

//    private void SendCommand(string command)
//    {
//        if (stockfishProcess != null && stockfishProcess.HasExited == false)
//        {
//            stockfishProcess.StandardInput.WriteLine(command);
//            stockfishProcess.StandardInput.Flush();
//        }
//    }

//    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
//    {
//        if (e.Data != null)
//        {
//            // Handle the output data received from Stockfish here
//            Debug.Log("Stockfish: " + e.Data);
//            ProcessStockfishResponse(e.Data);
//        }
//    }

//    private void ProcessStockfishResponse(string response)
//    {
//        // Implement logic to parse the response and handle different cases
//        // For example, you can extract the best move, score, etc.

//        // Example: Parsing the best move
//        if (response.StartsWith("bestmove"))
//        {
//            string[] parts = response.Split(' ');
//            if (parts.Length >= 2)
//            {
//                string bestMove = parts[1];
//                Debug.Log("Best move: " + bestMove);

//                // Trigger the event and pass the AI's move to subscribers
//                OnAIMoveReceived?.Invoke(bestMove);
//            }
//        }
//    }

//    // Example method to send a move to Stockfish
//    public void SendMoveToStockfish(string move)
//    {
//        string command = "position startpos moves " + move;
//        SendCommand(command);
//        SendCommand("go");
//    }
//}


using UnityEngine;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class StockfishAI : MonoBehaviour
{

    private Process stockfishProcess;
    string aiMove = "";
    string command = "position startpos move";
    private void Start()
    {
        StartStockfish();
        SendCommand("uci");
        SendCommand("isready");
        // You can send other necessary commands to initialize the engine here
    }


    ~StockfishAI()
    {
        StopStockfish();
    }

    private void StartStockfish()
    {
        stockfishProcess = new Process();
        stockfishProcess.StartInfo.FileName = Application.dataPath + "/Stockfish/stockfish-windows-x86-64-avx2.exe";
        stockfishProcess.StartInfo.UseShellExecute = false;
        stockfishProcess.StartInfo.CreateNoWindow = true;
        stockfishProcess.StartInfo.RedirectStandardInput = true;
        stockfishProcess.StartInfo.RedirectStandardOutput = true;
        // stockfishProcess.OutputDataReceived += OnOutputDataReceived;
        stockfishProcess.Start();
        Debug.Log("Start");
    }

    private void StopStockfish()
    {
        stockfishProcess.Close();
    }

    private void SendCommand(string command)
    {
        stockfishProcess.StandardInput.WriteLine(command);
        // stockfishProcess.StandardInput.Flush();
    }

    public string BestMove(string playerMoveUCI)
    {
        command += (playerMoveUCI != "" ? (" " + playerMoveUCI) : playerMoveUCI);
        Debug.Log("Player: " + command);
        SendCommand(command);

        String processString = "go movetime 1000";
        SendCommand(processString);

        do
        {
            aiMove = stockfishProcess.StandardOutput.ReadLine();
            BestMove();
        }
        while (BestMove() == "none");

        // aiMove = ExtractAIMove(aiMove);
        // Debug.Log($"Returning Move: {aiMove}");
        MoveAI(playerMoveUCI);
        return ExtractAIMove(aiMove);
    }

    public Task<string> BestMoveAsync(string playerMoveUCI)
    {
        return Task.Run(() => BestMove(playerMoveUCI));
    }

    public Task<string> AIBestMoveAsync(string aiMoveUCI)
    {
        return Task.Run(() => BestMove(""));
    }


    public string BestMove()
    {

        if (aiMove.StartsWith("bestmove"))
        {
            string[] parts = aiMove.Split(' ');
            if (parts.Length >= 2)
            {
                return parts[1];
            }
        }

        return "none";
    }

    private string ExtractAIMove(string output)
    {

        if (output == "" || !output.StartsWith("bestmove"))
            return "";

        string[] parts = output.Split(' ');
        return parts[1];

    }

    private void MoveAI(string playerMoveUCI)
    {
        command += " " + ExtractAIMove(aiMove);
        Debug.Log("AI: " + command);
        SendCommand(command);
    }



}
