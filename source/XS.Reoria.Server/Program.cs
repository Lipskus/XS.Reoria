using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XS.Reoria.Server
{
    class Program
    {
        static bool run = true;
        static void Main(string[] args)
        {
            Thread input = new Thread(() => {
                if ((Console.ReadLine() == "exit") || (Console.ReadLine() == "stop"))
                {
                    run = false;
                }
            });

            input.Start();

            while (run)
            {
                Console.WriteLine("Hello!");
            }

            input = null;
        }
    }
}
