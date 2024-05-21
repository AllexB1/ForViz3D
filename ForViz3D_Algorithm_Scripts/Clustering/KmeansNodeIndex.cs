
namespace Clustering
{
    public class KmeansNodeIndex 
    {
        public static int X { get; private set; }
        public static int Y { get; private set; }

        public KmeansNodeIndex(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
