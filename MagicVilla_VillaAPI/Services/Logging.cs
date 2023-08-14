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
        }
    }
}
