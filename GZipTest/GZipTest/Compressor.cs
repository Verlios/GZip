using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;

namespace GZipTest
{
    class Compressor : GZip
    {
        public Compressor(string input, string output) : base(input, output)
        {

        }

        public override void Start()
        {
            Console.WriteLine("Compressing...\n");

            Thread reader = new Thread(new ThreadStart(Read));
            reader.Start();

            for (int i = 0; i < threads; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(Compress, i);
            }

            Thread writer = new Thread(new ThreadStart(Write));
            writer.Start();

            WaitHandle.WaitAll(doneEvents);

            if (!cancell)
            {
                Console.WriteLine("\nCompressing has been succesfully finished");
                success = true;
            }
        }

        private void Read()
        {
            try
            {

                using (FileStream fileComp= new FileStream(sFile, FileMode.Open))
                {

                    int bytesRead;
                    byte[] lastBuffer;

                    while (fileComp.Position < fileComp.Length && !cancell)
                    {
                        if (fileComp.Length - fileComp.Position <= blockSize)
                        {
                            bytesRead = (int)(fileComp.Length - fileComp.Position);
                        }

                        else
                        {
                            bytesRead = blockSize;
                        }

                        lastBuffer = new byte[bytesRead];
                        fileComp.Read(lastBuffer, 0, bytesRead);
                        qReader.EnqueueForCompressing(lastBuffer);                        
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

        private void Compress(object i)
        {
            try
            {
                while (true && !cancell)
                {
                    ByteBlock block = qReader.Dequeue();

                    if (block == null)
                        return;

                    using (MemoryStream _memoryStream = new MemoryStream())
                    {
                        using (GZipStream cs = new GZipStream(_memoryStream, CompressionMode.Compress))
                        {

                            cs.Write(block.Buffer, 0, block.Buffer.Length);
                        }


                        byte[] compressedData = _memoryStream.ToArray();
                        ByteBlock _out = new ByteBlock(block.ID, compressedData);
                        qWriter.EnqueueForWriting(_out);
                    }
                    ManualResetEvent doneEvent = doneEvents[(int)i];
                    doneEvent.Set();
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
                using (FileStream fileComp= new FileStream(fFile, FileMode.Append))
                {
                    while (true && !cancell)
                    {
                        ByteBlock block = qWriter.Dequeue();
                        if (block == null)
                            return;

                        BitConverter.GetBytes(block.Buffer.Length).CopyTo(block.Buffer, 4);
                        fileComp.Write(block.Buffer, 0, block.Buffer.Length);
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
