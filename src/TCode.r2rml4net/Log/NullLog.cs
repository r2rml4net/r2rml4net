using TCode.r2rml4net.Mapping;

namespace TCode.r2rml4net.Log
{
    class NullLog : ITriplesGenerationLog
    {
        static readonly object ClassLock = new object();
        private static NullLog _instance;

        private NullLog()
        {
        }

        internal static ITriplesGenerationLog Instance
        {
            get
            {
                lock(ClassLock)
                {
                    if(_instance == null)
                    {
                        lock (ClassLock)
                        {
                          _instance = new NullLog();  
                        }
                    }
                }

                return _instance;
            }
        }

        #region Implementation of ITriplesGenerationLog

        public void LogMissingSubject(ITriplesMap triplesMap)
        {
        }

        #endregion
    }
}