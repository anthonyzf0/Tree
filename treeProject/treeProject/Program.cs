using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeProject
{
    class Program
    {
        static Random rand = new Random();
        
        static void Main(string[] args) {

            Stopwatch watch = new Stopwatch();
            ADSTree t = new ADSTree();

            Console.ReadLine();

            watch.Start();

            for (int i = 0; i <= 10; i++)
            {
                t.insert(i);
                long ms = watch.ElapsedMilliseconds;
                //Console.WriteLine(i + ": " + ms);
                //watch.Restart();

                t.printTree(TraverseOrder.InOrder);
                Console.ReadLine();
                Console.Clear();
            }


            Console.WriteLine("Done "+watch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
