using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Bot
{
    class Bot1
    {
        Game game;
        
        public Bot1(Game game)
        {
            this.game = game;
        }

        public void start() {

            while(!game.finished()){

                Console.Out.WriteLine("-------------");
                Console.Out.WriteLine("current");
                game.print();

                Console.Out.WriteLine("-------------");
                Console.Out.WriteLine("addRandom");
                game.addRandom();
                game.print();

                Console.Out.WriteLine("-------------");
                Console.Out.WriteLine("move");
                max(game, 0);
                game.print();
            }
        }

        // the most interesting function. determines the value of a situation.
        // TODO: create a useful algorithm
        public float evaluate(Game game)
        {
            float score = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int value = game.board[i, j];
                    if (value > 4)
                    {
                        float deltascore = value;
                        // bonus points if the value is on a side, in a corner, or adjacent to a multiple

                        float cornerbonus = 4;
                        float sidebonus = 1f;
                        float multiplebonus = 2;

                        if (i == 0 && j == 0 || i == 0 && j == 3 || i == 3 && j == 0 || i == 3 && j == 3)
                            deltascore *= cornerbonus;

                        else if (i == 0 || j == 0 || i == 3 || j == 3)
                            deltascore *= sidebonus;


                        // multiples next to it?

                        if (i > 0)
                            if (game.board[i - 1, j] != 0)
                                if (game.board[i - 1, j] / value == 2 || value / game.board[i - 1, j] == 2)
                                    deltascore *= multiplebonus;
                        if (i < 3)
                            if (game.board[i + 1, j] != 0)
                                if (game.board[i + 1, j] / value == 2 || value / game.board[i + 1, j] == 2)
                                    deltascore *= multiplebonus;

                        if (j > 0)
                            if (game.board[i, j - 1] != 0)
                                if (game.board[i, j - 1] / value == 2 || value / game.board[i, j - 1] == 2)
                                    deltascore *= multiplebonus;
                        if (j < 3)
                            if (game.board[i, j + 1] != 0)
                                if (game.board[i, j + 1] / value == 0 || value / game.board[i, j + 1] == 2)
                                    deltascore *= multiplebonus;

                        score += deltascore;
                    }
                }
            }
            return score;
        }

        // now an implementation of minmax. 
        int maxdepth = 7;

        float min(Game game, int depth) {

            float minimum = evaluate(game);

            if (game.finished())
                return evaluate(game);

            if (depth >= maxdepth)
                return evaluate(game);

            // get all empty fields
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (game.board[i, j] == 0)
                        list.Add(new Tuple<int, int>(i, j));

            
            list.ForEach((Tuple<int, int> tuple) => {
                game.set(tuple.Item1, tuple.Item2, 2);
                float value = max(game, depth + 1);
                if (value < minimum) minimum = value;
                game.revert();
            });
            return minimum;
        }

        float max(Game game, int depth)
        {
            float maximum = 0;

            if (game.finished())
                return evaluate(game);

            if (depth > maxdepth)
                return evaluate(game);

            // test each possible move for the best value
            float value = 0;
            int dir = 0;     // 0 = up, 1 = down, 2 = left, 3 = right
            bool possible=false;

            possible=game.up();
            if(possible){
                value = min(game, depth + 1);
                if (value >= maximum) { 
                    maximum = value;
                    dir = 0;
                }
            }
            game.revert();

            possible=game.down();
            if (possible)
            {
                value = min(game, depth + 1);
                if (value >= maximum)
                {
                    maximum = value;
                    dir = 1;
                }
            }
            game.revert();

            possible=game.left();
            if (possible)
            {
                value = min(game, depth + 1);
                if (value >= maximum && possible)
                {
                    maximum = value;
                    dir = 2;
                }
            }
            game.revert();

            possible=game.right();
            if (possible)
            {
                value = min(game, depth + 1);
                if (value >= maximum && possible)
                {
                    maximum = value;
                    dir = 3;
                }
            }
            game.revert();


            if (depth == 0)
            {
                switch (dir)
                {
                    case 0:
                        game.up();
                        break;
                    case 1:
                        game.down();
                        break;
                    case 2:
                        game.left();
                        break;
                    case 3:
                        game.right();
                        break;
                }
            }

            return maximum;
        }
    }
}
