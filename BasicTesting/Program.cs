using Quest.ML.Clustering.Neural;

namespace BasicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int? a = null;
            int? b = 4;

            Console.WriteLine(GetMinimum(3,5));
            Console.WriteLine(GetMinimum(null,null));
            Console.WriteLine(GetMinimum(4,null));
            Console.WriteLine(GetMinimum(null,6));
            Console.WriteLine(GetMinimum(5,3));

            //Console.WriteLine(x);
            Console.ReadKey();
        }
        static int? GetMinimum(int? a, int? b)
        {
            return (a ?? int.MaxValue) < (b ?? int.MaxValue) ? a : b;
        }
    }
}
