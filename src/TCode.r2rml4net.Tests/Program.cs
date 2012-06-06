using System.Reflection;

namespace TCode.r2rml4net.Tests
{
    public class Program
    {
        static int Main()
        {
            return NUnit.Gui.AppEntry.Main(new string[] { Assembly.GetExecutingAssembly().Location });
        }
    }
}
