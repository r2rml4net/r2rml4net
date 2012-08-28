using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Log
{
    class NullLog : IDefaultMappingGenerationLog
    {
        static readonly object ClassLock = new object();
        private static NullLog _instance;

        private NullLog()
        {
        }

        internal static NullLog Instance
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

        #region Implementation of IDefaultMappingGenerationLog

        public void LogMultipleCompositeKeyReferences(TableMetadata table)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}