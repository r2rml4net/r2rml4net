using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.RDB
{
    public class UniqueKeyCollection : IEnumerable<ColumnCollection>
    {
        private readonly IList<ColumnCollection> _uniqueKeys = new List<ColumnCollection>();

        #region Implementation of IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ColumnCollection> GetEnumerator()
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

        public void Add(ColumnCollection uniqueKey)
        {
            if(uniqueKey == null)
                throw new ArgumentNullException("uniqueKey");

            _uniqueKeys.Add(uniqueKey);
        }
    }
}