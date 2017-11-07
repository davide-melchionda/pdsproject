using System.IO.Pipes;
using System.IO;
using FileShareConsole;

namespace FileChooser
{
    class Program
    {
        static void Main(string[] args)
        {
            PipeModule.Push(args[0]);
        }
    }
}