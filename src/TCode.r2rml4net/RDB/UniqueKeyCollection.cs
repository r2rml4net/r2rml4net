using System;
using System.Collections;
using System.Collections.Generic;

namespace TCode.r2rml4net.RDB
{
    /// <summary>
    /// Represents a collection of unique keys
    /// </summary>
    public class UniqueKeyCollection : IEnumerable<UniqueKeyMetadata>
    {
        private readonly IList<UniqueKeyMetadata> _uniqueKeys = new List<UniqueKeyMetadata>();

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<UniqueKeyMetadata> GetEnumerator()
        {
            return _uniqueKeys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _uniqueKeys.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Adds a unique key to the collection
        /// </summary>
        public void Add(UniqueKeyMetadata uniqueKey)
        {
            if(uniqueKey == null)
                throw new ArgumentNullException("uniqueKey");

            _uniqueKeys.Add(uniqueKey);
        }
    }
}