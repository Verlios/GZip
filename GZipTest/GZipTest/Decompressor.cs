using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZipTest
{
    class Decompressor : GZip
    {
        int counter = 0;
        public Decompressor(string input, string output) : base(input, output)
        {


        }

        public override void Start()
        {
            Console.WriteLine("Decompressing...\n");

            Thread reader = new Thread(new ThreadStart(Read));
            reader.Start();

            for (int i = 0; i < threads; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(Decompress, i);
            }

            Thread writer = new Thread(new ThreadStart(Write));
            writer.Start();
            
            WaitHandle.WaitAll(doneEvents);
            
        }

        private void Read()
        {
            try
            {
                using (FileStream compressedFile = new FileStream(sFile, FileMode.Open))
                {
                    while (compressedFile.Position < compressedFile.Length)
                    {
                        byte[] lengthBuffer = new byte[8];
                        compressedFile.Read(lengthBuffer, 0, lengthBuffer.Length);
                        int blockLength = BitConverter.ToInt32(lengthBuffer, 4);
                        byte[] compressedData = new byte[blockLength];
                        lengthBuffer.CopyTo(compressedData, 0);

                        compressedFile.Read(compressedData, 8, blockLength - 8);
                        int dataSize = BitConverter.ToInt32(compressedData, blockLength - 4);
                        byte[] lastBuffer = new byte[dataSize];

                        ByteBlock block = new ByteBlock(counter, lastBuffer, compressedData);
                        qReader.EnqueueForWriting(block);
                        counter++;
                       // Console.WriteLine(compressedFile.Position);
                       // Console.WriteLine(compressedFile.Length);
                        if (compressedFile.Position == compressedFile.Length)
                        {
                            Console.WriteLine("\nDecompressing has been succesfully finished");
                            cancell = true;
                            success = true;
                        }
                        
                    }
                    qReader.Stop();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cancell = true;
            }
        }

        private void Decompress(object i)
        {
            try
            {
                while (!cancell)
                {
                    ByteBlock block = qReader.Dequeue();
                    if (block == null)
                        return;

                    using (MemoryStream ms = new MemoryStream(block.CompressedBuffer))
                    {
                        using (GZipStream gz = new GZipStream(ms, CompressionMode.Decompress))
                        {
                            gz.Read(block.Buffer, 0, block.Buffer.Length);
                            byte[] decompressedData = block.Buffer.ToArray();
                            ByteBlock nblock = new ByteBlock(block.ID, decompressedData);
                            qWriter.EnqueueForWriting(nblock);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in thread number {0}. \n Error description: {1}", i, ex.Message);
                cancell = true;
            }
        }

        private void Write()
        {
            try
            {
                using (FileStream decompressedFile = new FileStream(fFile, FileMode.Append))
                {
                    while (true && !cancell)
                    {
                        ByteBlock block = qWriter.Dequeue();
                        if (block == null)
                        { 
                            
                        return;
                        }
                        decompressedFile.Write(block.Buffer, 0, block.Buffer.Length);
                    }
                    
                    


                }
                
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cancell = true;
            }
        }
    }
}
