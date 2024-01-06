using MagicVilla_VillaAPI.Interfaces;

namespace MagicVilla_VillaAPI.Services
{
    public class Logging : ILogging
    {
        public void Log(string message, string type)
        {
            if (String.IsNullOrWhiteSpace(type))
            {
                Console.WriteLine(message);
            }
            else if (type == "error") { 
                Console.WriteLine("ERROR - " + message);
            }

            var numbers = Enumerable.Range(1, 50).ToArray(); // materializa
            var results = new int[50];


            var items = numbers.AsParallel().ForAll(x =>
            {
                int cubes = x * x * x;
                Console.WriteLine($"{cubes} ({Task.CurrentId})\t");
                results[x - 1] = cubes;
            });
        }
    }
}
