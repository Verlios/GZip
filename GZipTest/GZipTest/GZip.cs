using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GZipTest
{
    public abstract class GZip
    {
        protected bool cancell = false;
        protected bool success = false;
        protected string sFile, fFile;
        protected static int threads = Environment.ProcessorCount;

        protected int blockSize = 1024 * 1024;
        protected QueueAdmin qReader = new QueueAdmin();
        protected QueueAdmin qWriter = new QueueAdmin();
        protected ManualResetEvent[] doneEvents = new ManualResetEvent[threads];

        public GZip()
        {

        }
        public GZip(string input, string output)
        {
            this.sFile = input;
            this.fFile = output;
        }

        public int CallBackResult()
        {
            if (success)
                return 0;
            return 1;
        }

        public void Cancel()
        {
            cancell = true;
        }

        public abstract void Start();
    }
}
