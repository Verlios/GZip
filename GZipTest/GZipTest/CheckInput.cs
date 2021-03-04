using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GZipTest
{
    public static class CheckInput
    {
        public static void StringInputCheck(string[] arg)
        {

            if (arg.Length == 0 || arg.Length > 3)
            {
                throw new Exception("Please enter arguments like in templates:\n compress(decompress) [The full name of source file] [Name finished file].");
            }

            if (arg[0].ToLower() != "compress" && arg[0].ToLower() != "decompress")
            {
                throw new Exception("First argument must be compress or decompress");
            }

            if (arg[1].Length == 0)
            {
                throw new Exception("Second argument must contain the name of source file");
            }

            if (!File.Exists(arg[1]))
            {
                throw new Exception("No source file was found");
            }

            FileInfo fSour = new FileInfo(arg[1]);
            FileInfo fFin = new FileInfo(arg[2]);

            if (arg[1] == arg[2])
            {
                throw new Exception("Source and finished files must be different.");
            }

            if (fFin.Exists)
            {
                throw new Exception("Finished file already exists. Please using another file name.");
            }

            if (fSour.Extension != ".gz" && arg[0] == "decompress")
            {
                throw new Exception("File to be decompressed must have .gz extension.");
            }

            if (arg[2].Length == 0)
            {
                throw new Exception("Third argument must contain the name of final file");
            }
        }
    }
}
