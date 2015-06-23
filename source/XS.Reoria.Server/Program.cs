using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XS.Reoria.Framework;
using XS.Reoria.Framework.IO;

namespace XS.Reoria.Server
{
    class Program
    {
        static volatile Application game;
        static bool run = true;
        static void Main(string[] args)
        {
            game = new Application();
            FileSystem fs = new FileSystem();

            Thread input = new Thread(() =>
            {
                while (run)
                {
                    switch (Console.ReadLine())
                    {
                        case "stop":
                            game.Stop();
                            run = false;
                            break;
                        case "exit":
                            game.Stop();
                            run = false;
                            break;
                        case "resume":
                            game.Resume();
                            break;
                        case "pause":
                            game.Pause();
                            break;
                        default:
                            Console.WriteLine("Invalid command input.");
                            break;
                    }
                }
            });

            input.Start();
            game.Start();

            while (game.IsAlive || run)
            {
                // Keep alive loop.
            }

            game = null;
            input = null;
        }
    }
}
