namespace KheaiGameEngine
{
    public static class KThreadManager
    {
        //Threading
        private static List<Thread> s_threads = new();

        public static Thread CreateThread(string name, ThreadStart start)
        {
            Thread thread = new Thread(start);
            thread.Name = name;
            KDebug.Log($"Thread created: {thread.Name}");

            RegisterThread(thread);
            return thread;
        }

        public static void RegisterThread(Thread thread)
        {
            KDebug.Log($"Registering thread: {thread.Name}");
            lock (thread)
            {
                s_threads.Add(thread);
            }
        }

        public static Thread GetThread(string name)
        {
            foreach (Thread thread in s_threads)
            {
                if (thread.Name == name)
                {
                    return thread;
                }
            }
            return null;
        }

        public static void JoinAllThreads()
        {
            lock (s_threads)
            {
                foreach (Thread t in s_threads)
                {
                    t.Join();
                }
            }
        }
    }
}
