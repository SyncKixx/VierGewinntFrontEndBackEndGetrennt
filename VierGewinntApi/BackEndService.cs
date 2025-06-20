using System;

namespace VierGewinntApi;

public class BackEndService
{   
    private VierGewinnt _vierGewinntData;
    
    public VierGewinnt MakeGameReady(int gamemode)
    {
        if (_vierGewinntData == null) // Nur initialisieren, wenn _vierGewinntData null ist
        {
            _vierGewinntData = new VierGewinnt
            {
                gamemode = gamemode,
                currentSpieler = 1,
                winner = 0,
                spielfeld = new string[6][]
            };

            for (int i = 0; i < _vierGewinntData.spielfeld.Length; i++)
            {
                _vierGewinntData.spielfeld[i] = new string[7];
                for (int j = 0; j < _vierGewinntData.spielfeld[i].Length; j++)
                {
                    _vierGewinntData.spielfeld[i][j] = "0";
                }
            }

            _vierGewinntData.zeilen = new int[6];
            _vierGewinntData.spalten = new int[7];
        }
        else
        {
            // Optional: Setze den Gamemode für ein neues Spiel zurück, falls gewünscht
            _vierGewinntData.gamemode = gamemode;
            _vierGewinntData.currentSpieler = 1;
            _vierGewinntData.winner = 0;
            // Optional: Spielfeld zurücksetzen, falls ein neues Spiel gestartet wird
            for (int i = 0; i < _vierGewinntData.spielfeld.Length; i++)
            {
                for (int j = 0; j < _vierGewinntData.spielfeld[i].Length; j++)
                {
                    _vierGewinntData.spielfeld[i][j] = "0";
                }
            }
        }

        return _vierGewinntData;
    }

    public VierGewinnt GetGame()
    {
        //hier wird das spielfeld zurückgegeben
        return _vierGewinntData;
    }

    public VierGewinnt HandleClick(int spalte, int zeile)
    {
        if (_vierGewinntData.spielfeld != null)
        {
            // Finde die tiefste leere Reihe in der angeklickten Spalte
            int r = GetLowestEmptyRow(spalte);

            if (r != -1 && _vierGewinntData.winner == 0) // Prüfe, ob die Spalte nicht voll ist und kein Gewinner feststeht
            {
                Console.WriteLine("Backendservice.HandleClick.ist in if");
                Console.WriteLine("Zeile: " + r);
                Console.WriteLine("Spalte: " + spalte);
                _vierGewinntData.spielfeld[r][spalte] = _vierGewinntData.currentSpieler.ToString();

                if (CheckWinForPlayer(_vierGewinntData.spielfeld, _vierGewinntData.currentSpieler.ToString()))
                {
                    _vierGewinntData.winner = _vierGewinntData.currentSpieler;
                }
                else
                {
                    // Spielerwechsel nur, wenn kein Gewinn vorliegt und der Zug gültig war
                    _vierGewinntData.currentSpieler = 3 - _vierGewinntData.currentSpieler;
                }
            }

            // KI-Zug, wenn der aktuelle Spieler 2 (KI) ist und kein Gewinner feststeht und der Modus Spieler gegen Computer ist
            if (_vierGewinntData.currentSpieler == 2 && _vierGewinntData.gamemode == 1 && _vierGewinntData.winner == 0)
            {
                int bestColumn = FindBestMove();
                int rowForAI = GetLowestEmptyRow(bestColumn);

                if (rowForAI != -1) // Prüfen, ob der KI-Zug gültig ist (Spalte nicht voll)
                {
                    _vierGewinntData.spielfeld[rowForAI][bestColumn] = _vierGewinntData.currentSpieler.ToString();
                    if (CheckWinForPlayer(_vierGewinntData.spielfeld, _vierGewinntData.currentSpieler.ToString()))
                    {
                        _vierGewinntData.winner = _vierGewinntData.currentSpieler;
                    }
                    // Spielerwechsel nach KI-Zug
                    _vierGewinntData.currentSpieler = 3 - _vierGewinntData.currentSpieler;
                }
                else
                {
                    // Fallbeispiel: KI kann keinen Zug machen, weil alle Spalten voll sind oder ein Fehler auftrat
                    Console.WriteLine("KI konnte keinen gültigen Zug finden oder alle Spalten sind voll.");
                    // Hier könnte man auch einen Unentschieden-Status setzen oder eine andere Logik implementieren
                }
            }
        }
        return _vierGewinntData;
    }

    public int GetLowestEmptyRow(int col)
    {
        string[][] board = _vierGewinntData.spielfeld; 
        if (board != null)
        {
            for (int i = 5; i >= 0; i--)
            {
                if (board[i][col] == "0") return i;
            }
        }
        return -1; // Spalte ist voll oder Board ist null
    }

    public int FindBestMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;
        string[][] board = CopyBoard(_vierGewinntData.spielfeld); // Arbeite mit einer Kopie

        if (board != null)
        {
            for (int col = 0; col < 7; col++)
            {
                int row = GetLowestEmptyRow(col);
                if (row != -1) // Wenn Spalte nicht voll ist
                {
                    board[row][col] = "2"; // Setze den Zug des Computers (Annahme: "2" ist der Computer)

                    int score = Minimax(board, 4, false); // Tiefe anpassen, 4 ist ein guter Startwert

                    board[row][col] = "0"; // Mache den Zug rückgängig

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = col;
                    }
                }
            }
        }
        return bestMove;
    }

    public int Minimax(string[][] board, int depth, bool isMaximizing)
    {
        // Prüfe auf Gewinnzustand für den KI-Spieler (2)
        if (CheckWinForPlayer(board, "2")) return 1000000; 
        // Prüfe auf Gewinnzustand für den menschlichen Spieler (1)
        if (CheckWinForPlayer(board, "1")) return -1000000;
        // Wenn das Spielfeld voll ist (Unentschieden)
        if (IsBoardFull(board)) return 0;
        // Maximale Suchtiefe erreicht
        if (depth == 0) return EvaluateBoard(board);

        if (isMaximizing) // KI (Spieler 2) versucht, den Score zu maximieren
        {
            int maxScore = int.MinValue;
            for (int col = 0; col < 7; col++)
            {
                int row = GetLowestEmptyRowForBoard(board, col); // Nutze die Board-Kopie
                if (row != -1)
                {
                    board[row][col] = "2";
                    maxScore = Math.Max(maxScore, Minimax(board, depth - 1, false));
                    board[row][col] = "0"; // Zug rückgängig machen
                }
            }
            return maxScore;
        }
        else // Menschlicher Spieler (Spieler 1) versucht, den Score zu minimieren
        {
            int minScore = int.MaxValue;
            for (int col = 0; col < 7; col++)
            {
                int row = GetLowestEmptyRowForBoard(board, col); // Nutze die Board-Kopie
                if (row != -1)
                {
                    board[row][col] = "1";
                    minScore = Math.Min(minScore, Minimax(board, depth - 1, true));
                    board[row][col] = "0"; // Zug rückgängig machen
                }
            }
            return minScore;
        }
    }

    // Hilfsmethode, um das Board zu kopieren
    private string[][] CopyBoard(string[][] original)
    {
        string[][] copy = new string[original.Length][];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = new string[original[i].Length];
            Array.Copy(original[i], copy[i], original[i].Length);
        }
        return copy;
    }

    // Hilfsmethode für GetLowestEmptyRow, die mit einem beliebigen Board arbeitet
    public int GetLowestEmptyRowForBoard(string[][] board, int col)
    {
        if (board != null)
        {
            for (int i = 5; i >= 0; i--)
            {
                if (board[i][col] == "0") return i;
            }
        }
        return -1; // Spalte ist voll oder Board ist null
    }

    // Hilfsmethode, um zu prüfen, ob das Spielfeld voll ist (Unentschieden)
    private bool IsBoardFull(string[][] board)
    {
        for (int col = 0; col < 7; col++)
        {
            if (board[0][col] == "0") // Wenn die oberste Reihe in irgendeiner Spalte leer ist
            {
                return false;
            }
        }
        return true; // Alle Spalten sind voll
    }


    public int EvaluateBoard(string[][] board)
    {
        int score = 0;

        // Bewerte horizontale Sequenzen
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                score += EvaluateSequence(
                    board[row][col],
                    board[row][col + 1],
                    board[row][col + 2],
                    board[row][col + 3]
                );
            }
        }

        // Bewerte vertikale Sequenzen
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                score += EvaluateSequence(
                    board[row][col],
                    board[row + 1][col],
                    board[row + 2][col],
                    board[row + 3][col]
                );
            }
        }

        // Bewerte diagonale Sequenzen (von links unten nach rechts oben)
        for (int row = 3; row < 6; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                score += EvaluateSequence(
                    board[row][col],
                    board[row - 1][col + 1],
                    board[row - 2][col + 2],
                    board[row - 3][col + 3]
                );
            }
        }

        // Bewerte diagonale Sequenzen (von links oben nach rechts unten)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                score += EvaluateSequence(
                    board[row][col],
                    board[row + 1][col + 1],
                    board[row + 2][col + 2],
                    board[row + 3][col + 3]
                );
            }
        }

        return score;
    }

    private int EvaluateSequence(string a, string b, string c, string d)
    {
        int aiCount = 0;
        int playerCount = 0;
        int emptyCount = 0;

        string[] cells = { a, b, c, d };
        foreach (string cell in cells)
        {
            if (cell == "2") aiCount++;
            else if (cell == "1") playerCount++;
            else emptyCount++;
        }

        // Wenn die Sequenz nur leere und KI-Steine enthält
        if (playerCount == 0)
        {
            if (aiCount == 4) return 1000000; // Gewinn für KI
            if (aiCount == 3 && emptyCount == 1) return 100;
            if (aiCount == 2 && emptyCount == 2) return 10;
        }
        // Wenn die Sequenz nur leere und Spieler-Steine enthält
        else if (aiCount == 0)
        {
            if (playerCount == 4) return -1000000; // Gewinn für Spieler
            if (playerCount == 3 && emptyCount == 1) return -100;
            if (playerCount == 2 && emptyCount == 2) return -10;
        }
        return 0;
    }

    // Überarbeitete CheckWin Methode, die ein Spielfeld und den Spieler als Parameter nimmt
    public bool CheckWinForPlayer(string[][] board, string player)
    {
        if (board == null) return false;

        // Horizontal prüfen
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (board[row][col] == player && 
                    board[row][col+1] == player &&
                    board[row][col+2] == player &&
                    board[row][col+3] == player)
                {
                    return true;
                }
            }
        }

        // Vertikal prüfen
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                if (board[row][col] == player && 
                    board[row+1][col] == player &&
                    board[row+2][col] == player &&
                    board[row+3][col] == player)
                {
                    return true;
                }
            }
        }
           
        // Diagonal (unten links nach oben rechts)
        for (int row = 3; row < 6; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (board[row][col] == player && 
                    board[row-1][col+1] == player &&
                    board[row-2][col+2] == player &&
                    board[row-3][col+3] == player)
                {
                    return true;
                }
            }
        }

        // Diagonal (oben links nach unten rechts)
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (board[row][col] == player && 
                    board[row+1][col+1] == player &&
                    board[row+2][col+2] == player &&
                    board[row+3][col+3] == player)
                {
                    return true;
                }
            }
        }
        return false;
    }
    // Die ursprüngliche checkWin Methode ist nicht mehr direkt notwendig, wenn CheckWinForPlayer verwendet wird
    // Ich habe sie beibehalten, da sie in Ihrem Originalcode war, aber sie sollte zugunsten von CheckWinForPlayer
    // innerhalb der Minimax-Logik und EvaluateBoard überdacht werden.
    public bool checkWin(){
        if(_vierGewinntData.spielfeld != null){
            string spieler = _vierGewinntData.currentSpieler.ToString();
            return CheckWinForPlayer(_vierGewinntData.spielfeld, spieler);
        }
        return false;
    }
}