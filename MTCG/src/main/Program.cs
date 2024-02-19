using MTCG.Server;

namespace MTCG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new HTTPServer().Start(); 
        }
    }
}
