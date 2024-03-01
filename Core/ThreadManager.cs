using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KheaiGameEngine.Core
{
    public class ThreadManager
    {
        int ThreadCount;
        List<Thread> threads = new();

        public ThreadManager() 
        {
            ThreadCount = Environment.ProcessorCount;
        }

        private void StartThread()
        {

        }
    }
}
