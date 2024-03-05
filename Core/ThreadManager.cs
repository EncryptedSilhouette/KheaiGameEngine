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
