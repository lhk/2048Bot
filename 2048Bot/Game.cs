using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Bot
{
    class Game
    {
        public int[,] board;
        public Stack<int[,]> backup_board;
        public int score;
        public Stack<int> backup_score;

        static Random rnd;


        public Game()
        {
            board = new int[4, 4];
            backup_board = new Stack<int[,]>();

            score = 0;
            backup_score = new Stack<int>();

            rnd = new Random();

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    board[i, j] = 0;

            backup_board.Push((int[,])board.Clone());
            backup_score.Push(score);
        }

        public void print()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Out.Write(board[i, j]);
                    Console.Out.Write("     ");
                }
                Console.Out.WriteLine();
            }
        }

        //undo the last move
        public void revert()
        {
            board = backup_board.Pop();
            score = backup_score.Pop();
        }

        public void set(int i, int j, int number)
        {
            if (board[i, j] != 0)
            {
                Console.Error.WriteLine("trying to place a tile on a not-empty position");
                return;
            }
            backup_board.Push((int[,])board.Clone());
            backup_score.Push(score);

            board[i, j] = number;
        }

        // this array stores a flag for each tile of the game: has the tile already been doubled ?
        // with this array the following situation can be prevented:
        // we only look at one row
        // current      8 4 4 0 
        // move right   8 4 0 4
        // move right   8 0 4 4
        // move right   8 0 0 8  set the flag to true.
        // move right   0 8 0 8
        // move right   0 0 8 8  now we need to stop.
        // normally the two eights would be added. but the flag says that one of them already has been modified.
        bool[,] doubled = new bool[4, 4];

        public bool up() {
            bool changed = false;
            backup_board.Push((int[,])board.Clone());
            backup_score.Push(score);

            // this is a small sanity check. by moving the tiles, the sum of values must not change
            // the loop is also used to initialize the mask doubled
            int value_before = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    value_before += board[i, j];
                    doubled[i, j] = false;
                }

            // for each row, start with the second
            for (int i = 1; i < 4; i++)
            {
                // for each column
                for (int j = 0; j < 4; j++)
                {
                    int number = board[i, j];
                    if (number == 0)
                        continue;

                    // for each space above this
                    for (int k = i - 1; k >= 0; k--)
                    {
                        // if the space is empty, move the element into it
                        if (board[k, j] == 0)
                        {
                            changed = true;
                            board[k, j] = number;
                            board[k + 1, j] = 0;
                        }
                        // if the space contains the same number
                        // and hasn't been modified yet, add them up
                        else if (board[k, j] == number)
                        {
                            if (doubled[k,j] == false)
                            {
                                changed = true;
                                board[k, j] = number * 2;
                                doubled[k, j] = true;
                                score += number * 2;
                                board[k + 1, j] = 0;
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        // if the space contains a different number, stop
                        else
                            break;
                    }
                }
            }

            int value_after = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    value_after += board[i, j];
            if (value_before != value_after)
                Console.Error.WriteLine("up has created an error");

            return changed;
        }
        public bool down()
        {
            bool changed = false;
            backup_board.Push((int[,])board.Clone());
            backup_score.Push(score);

            int value_before = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    value_before += board[i, j];
                    doubled[i, j] = false;
                }

            // for each row, start with the second-last
            for (int i = 2; i >=0; i--)
            {
                // for each column
                for (int j = 0; j < 4; j++)
                {
                    int number = board[i, j];
                    if (number == 0)
                        continue;

                    // for each space below this
                    for (int k = i + 1; k <=3; k++)
                    {
                        // if the space is empty, move the element into it
                        if (board[k, j] == 0)
                        {
                            changed = true;
                            board[k, j] = number;
                            board[k - 1, j] = 0;
                        }
                        // if the space contains the same number, add them up
                        else if (board[k, j] == number)
                        {
                            if (doubled[k, j] == false)
                            {
                                changed = true;
                                board[k, j] = number * 2;
                                doubled[k, j] = true;
                                score += number * 2;
                                board[k - 1, j] = 0;
                                break;
                            }
                            else
                                break;
                        }
                        // if the space contains a different number, stop
                        else
                            break;
                    }
                }
            }

            int value_after = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    value_after += board[i, j];
            if (value_before != value_after)
                Console.Error.WriteLine("down has created an error");

            return changed;
        }

        public bool left()
        {
            bool changed = false;

            backup_board.Push((int[,])board.Clone());
            backup_score.Push(score);

            // there have been a few nasty errors, this is a small insurance
            int value_before = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    value_before += board[i, j];
                    doubled[i, j] = false;
                }

            // for each column, start with the second
            for (int j = 1; j <=3; j++)
            {
                // for each row
                for (int i = 0; i < 4; i++)
                {
                    int number = board[i, j];
                    if (number == 0)
                        continue;

                    // for each space left of this
                    for (int k = j - 1; k >=0; k--)
                    {
                        // if the space is empty, move the element into it
                        if (board[i, k] == 0)
                        {
                            changed = true;
                            board[i, k] = number;
                            board[i, k + 1] = 0;
                        }
                        // if the space contains the same number, add them up
                        else if (board[i, k] == number)
                        {
                            if (doubled[i, k] == false)
                            {
                                changed = true;
                                board[i, k] = number * 2;
                                doubled[i, k] = true;
                                score += number * 2;
                                board[i, k + 1] = 0;
                                break;
                            }
                            else
                                break;
                        }
                        // if the space contains a different number, stop
                        else
                            break;
                    }
                }
            }
            int value_after = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    value_after += board[i, j];
            if (value_before != value_after)
                Console.Error.WriteLine("left has created an error");

            return changed;
        }

        public bool right()
        {
            bool changed = false;

            backup_board.Push((int[,])board.Clone());
            backup_score.Push(score);

            // there have been a few nasty errors, this is a small insurance
            int value_before = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    value_before += board[i, j];
                    doubled[i, j] = false;
                }
            // for each column, start with the second-last
            for (int j = 2; j >=0; j--)
            {
                // for each row
                for (int i = 0; i < 4; i++)
                {
                    int number = board[i, j];
                    if (number == 0)
                        continue;

                    // for each right of this
                    for (int k = j +1 ; k<=3 ; k++)
                    {
                        // if the space is empty, move the element into it
                        if (board[i, k] == 0)
                        {
                            changed = true;
                            board[i, k] = number;
                            board[i, k - 1] = 0;
                        }
                        // if the space contains the same number, add them up
                        else if (board[i, k] == number)
                        {
                            if (doubled[i, k] == false)
                            {
                                changed = true;
                                board[i, k] = number * 2;
                                doubled[i, k] = true;
                                score += number * 2;
                                board[i, k - 1] = 0;
                                break;
                            }
                            else break;
                        }
                        // if the space contains a different number, stop
                        else
                            break;
                    }
                }
            }

            int value_after = 0;
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    value_after += board[i, j];

            if (value_before != value_after)
                Console.Error.WriteLine("right has created an error");
            return changed;
        }

        // puts either 2 or 4 on some random empty field
        public bool addRandom()
        {
            backup_board.Push((int[,])board.Clone());
            backup_score.Push(score);
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            for(int i=0; i<4; i++)
                for (int j = 0; j < 4; j++)
                    if (board[i, j] == 0)
                        list.Add(new Tuple<int, int>(i, j));

            if (list.Count == 0)
                return false;

            Tuple<int,int> position=list.ElementAt(rnd.Next(list.Count));

            if (rnd.NextDouble() > 0.5)
                board[position.Item1, position.Item2] = 2;
            else
                board[position.Item1, position.Item2] = 4;

            return true;
        }

        public bool finished()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (board[i, j] == 0)
                        return false;
                    if (i > 0)
                        if (board[i - 1, j] == board[i, j])
                            return false;
                    if (i < 3)
                        if (board[i + 1, j] == board[i, j])
                            return false;
                    if (j > 0)
                        if (board[i, j - 1] == board[i, j])
                            return false;
                    if (j < 3)
                        if (board[i, j + 1] == board[i, j])
                            return false;
                }
            }
            return true;
        }
    }
}
