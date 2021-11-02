using System;
namespace TicTacToe
{
    class Move
    {
        public int row, column;
    };

    class Program
    {
        public const int board_size = 3;

        public static bool[] usedSpots = new bool[board_size * board_size + 1];

        public static char[,] field = new char[board_size, board_size];

        public static int winner;

        public static int user = 1;

        public static int computer = 2;

        public static char userSign = 'O';

        public static char computerSign = 'X';

        public static int numOfMoves;

        static void Main(string[] args)
        {
            int currentPlayer;

            do
            {
                RestartField();
                PrintBoard();
                currentPlayer = user;

                do
                {
                    Play(currentPlayer);
                    numOfMoves++;
                    currentPlayer = currentPlayer == user ? computer : user;

                } while (!DoWeHaveAWinner() && (numOfMoves != (board_size * board_size)));

                if (numOfMoves == board_size * board_size)
                {
                    Console.WriteLine("IT'S A TIE!");
                }
                else
                {
                    Console.WriteLine("AND THE WINNER IS....");
                    Console.WriteLine("PLAYER {0} !!! BRAVO!", winner);
                }

                Console.WriteLine("-----------");
                Console.WriteLine("Press any key to restart ths game");
                Console.ReadLine();
                Console.Clear();
            }
            while (true);
        }

        public static void RestartField()
        {
            char cell_label = '1';
            for (int i = 0; i < board_size; i++)
            {
                for (int j = 0; j < board_size; j++)
                {
                    field[i, j] = cell_label;
                    cell_label++;
                }
            }

            for (int i = 0; i < usedSpots.Length; i++)
            {
                usedSpots[i] = false;
            }

            numOfMoves = 0;
        }

        public static void PrintBoard()
        {
            Console.WriteLine("     |     |     ");
            Console.WriteLine("  {0}  |  {1}  |  {2}  ", field[0, 0], field[0, 1], field[0, 2]);
            Console.WriteLine("_____|_____|_____");
            Console.WriteLine("     |     |     ");
            Console.WriteLine("  {0}  |  {1}  |  {2}  ", field[1, 0], field[1, 1], field[1, 2]);
            Console.WriteLine("_____|_____|_____");
            Console.WriteLine("     |     |     ");
            Console.WriteLine("  {0}  |  {1}  |  {2}  ", field[2, 0], field[2, 1], field[2, 2]);
            Console.WriteLine("     |     |     ");
            Console.WriteLine();
        }

        public static bool DoWeHaveAWinner()
        {
            return CheckRow() ? true : CheckColumn() ? true : CheckDiagonal() ? true : false;
        }

        public static bool CheckRow()
        {
            for (int i = 0; i < board_size; i++)
            {
                char sign = field[i, 0];
                if (field[i, 1] == sign && field[i, 2] == sign)
                {
                    winner = sign == userSign ? user : computer;
                    return true;
                }
            }

            return false;
        }

        public static bool CheckColumn()
        {
            for (int i = 0; i < board_size; i++)
            {
                char sign = field[0, i];
                if (field[1, i] == sign && field[2, i] == sign)
                {
                    winner = sign == userSign ? user : computer;
                    return true;
                }
            }

            return false;
        }

        public static bool CheckDiagonal()
        {
            char sign = field[1, 1];
            if (field[0, 0] == sign && field[2, 2] == sign)
            {
                winner = sign == userSign ? user : computer;
                return true;
            }
            if (field[0, 2] == sign && field[2, 0] == sign)
            {
                winner = sign == userSign ? user : computer;
                return true;
            }

            return false;
        }

        public static void Play(int player)
        {
            if (player == user)
            {
                Console.WriteLine("User, Choose your field!");
                string Sspot = Console.ReadLine();
                int Ispot;
                if (!Int32.TryParse(Sspot, out Ispot)) // not an Integer
                {
                    Console.WriteLine(" your input is illegal");
                    Play(player);
                }
                else if (Ispot < 1 || Ispot > (board_size * board_size) || usedSpots[Ispot]) // not in range
                {
                    Console.WriteLine(" your input is illegal");
                    Play(player);
                }
                else
                {
                    usedSpots[Ispot] = true;
                    updateField(Ispot);
                }
            }
            else // computer's turn
            {
                Move bestMove = findBestMove();
                field[bestMove.row, bestMove.column] = computerSign;
            }
            Console.Clear();
            PrintBoard();
        }

        public static void updateField(int spot)
        { 
            int row = (spot - 1) / board_size;
            int column = (spot - 1) % board_size;
            field[row, column] = userSign;
        }

        public static Move findBestMove()
        {
            Console.WriteLine("DEBUG findBestMove START");
            int bestVal = -1000;
            Move bestMove = new Move();
            bestMove.row = -1;
            bestMove.column = -1;

            for (int i = 0; i < board_size; i++)
            {
                for (int j = 0; j < board_size; j++)
                {
                    if (field[i, j] != userSign && field[i, j] != computerSign) // Check if cell is empty
                    {
                        char cuurSign = field[i, j]; // saves the curr sign
                        field[i, j] = computerSign; // try that move
                        numOfMoves++;
                        int moveVal = minimax(0, false); // compute evaluation function for this move
                        field[i, j] = cuurSign; // Undo the move
                        numOfMoves--;

                        if (moveVal > bestVal) // updating the value for the best move
                        {
                            bestMove.row = i;
                            bestMove.column = j;
                            bestVal = moveVal;
                        }
                    }
                }
            }
            return bestMove;
        }

        // with minimax alporithem we cheack all the possibole ways to continue the game
        // the function returns the value of the field
        public static int minimax(int depth, Boolean isMax)
        {
            int score = evaluate();

            // if Maximizer has won the game return his evaluated score
            if (score == 10)
                return score;

            // if Minimizer has won the game return his evaluated score
            if (score == -10)
                return score;

            // it's a tie, no more moves to do
            if (numOfMoves == board_size * board_size)
            {
                return 0;
            }

            // none of them won, there are more moves to do
            // if this maximizer's move
            if (isMax)
            {
                int best = -1000;

                for (int i = 0; i < board_size; i++)
                {
                    for (int j = 0; j < board_size; j++)
                    {
                        if (field[i, j] != userSign && field[i, j] != computerSign) // Check if cell is empty
                        {
                            char cuurSign = field[i, j]; // saves the curr sign
                            field[i, j] = computerSign; // try that move                        
                            numOfMoves++;
                            best = Math.Max(best, minimax(depth + 1, !isMax)); // chose recursively the maximum value
                            int moveVal = minimax(0, false); // compute evaluation function for this move
                            field[i, j] = cuurSign; // Undo the move
                            numOfMoves--;
                        }
                    }
                }
                return best;
            }

            // If this minimizer's move
            else
            {
                int best = 1000;

                for (int i = 0; i < board_size; i++)
                {
                    for (int j = 0; j < board_size; j++)
                    {
                        if (field[i, j] != userSign && field[i, j] != computerSign) // Check if cell is empty
                        {
                            char cuurSign = field[i, j]; // saves the curr sign
                            field[i, j] = userSign; // try that move                        
                            numOfMoves++;
                            best = Math.Min(best, minimax(depth + 1, !isMax)); // chose recursively the manimum value
                            int moveVal = minimax(0, false); // compute evaluation function for this move
                            field[i, j] = cuurSign; // Undo the move
                            numOfMoves--;
                        }
                    }
                }
                return best;
            }
        }

        public static int evaluate()
        {
            if (CheckColumn())
            {
                if (winner == computer)
                {
                    return +10;
                }
                else
                {
                    return -10;
                }
            }

            if (CheckRow())
            {
                if (winner == computer)
                {
                    return +10;
                }
                else
                {
                    return -10;
                }
            }

            if (CheckDiagonal())
            {
                if (winner == computer)
                {
                    return +10;
                }
                else
                {
                    return -10;
                }
            }
            // if none of the players have won
            return 0;
        }

    }
}
