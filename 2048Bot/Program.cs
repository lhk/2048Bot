using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Game test = new Game();
            Bot1 myBot = new Bot1(test);

            myBot.start();


            Console.Out.WriteLine();
            Console.Out.WriteLine("Score: " + test.score);
            Console.Out.WriteLine("Finished?: " + test.finished());
            Console.In.Read();
        }
    }
}
