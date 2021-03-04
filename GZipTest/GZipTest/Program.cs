using System;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    class Program
    {
        static GZip zip;
        static int Main(string[] args)
        {

            try
            {
                //Array for debuging and test from studio!
                args = new string[3];
                args[0] = @"decompress";
                args[1] = @"new.gz";
                args[2] = @"2.iso";


                CheckInput.StringInputCheck(args);

                switch (args[0].ToLower())
                {
                    case "compress":
                        zip = new Compressor(args[1], args[2]);
                        break;
                    case "decompress":
                        zip = new Decompressor(args[1], args[2]);
                        break;
                }

                zip.Start();
                
                return zip.CallBackResult();
               

            }

            catch (Exception ex)
            {
                Console.WriteLine("Error is occured!\n Method: {0}\n Error description {1}", ex.TargetSite, ex.Message);
                return 1;
            }

        }
            
    }
}
