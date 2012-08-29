#region Licence
			
/* 
Copyright (C) 2012 Tomasz Pluskiewicz
http://r2rml.net/
r2rml@r2rml.net
	
------------------------------------------------------------------------
	
This file is part of r2rml4net.
	
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE.
	
------------------------------------------------------------------------

r2rml4net may alternatively be used under the LGPL licence

http://www.gnu.org/licenses/lgpl.html

If these licenses are not suitable for your intended use please contact
us at the above stated email address to discuss alternative
terms. */
			
#endregion

using System;
using VDS.RDF;

namespace TCode.r2rml4net.RDF
{
    /// <summary>
    /// Extension methods for <see cref="Triple"/> class
    /// </summary>
    public static class TripleExtensions
    {
        /// <summary>
        /// Clone a triple and optionally replace <see cref="Triple.Subject"/>, <see cref="Triple.Predicate"/>, 
        /// <see cref="Triple.Object"/> or <see cref="Triple.Graph"/>
        /// </summary>
        public static Triple CloneTriple(
            this Triple t, 
            INode replacedSubject = null, 
            INode replacedPredicate = null, 
            INode replacedObject = null, 
            IGraph replacedGraph = null,
            Uri replacedGraphUri = null)
        {
            Triple newTriple;
            INode newSubject = replacedSubject ?? t.Subject;
            INode newPredicate = replacedPredicate ?? t.Predicate;
            INode newObject = replacedObject ?? t.Object;
            IGraph newGraph = replacedGraph ?? t.Graph;
            Uri newGraphUri = replacedGraphUri ?? t.GraphUri;

            if (newGraph == null && newGraphUri == null)
            {
                newTriple = new Triple(newSubject, newPredicate, newObject);
            }
            else if(newGraph != null)
            {
                if (replacedGraphUri != null)
                {
                    var message = string.Format("Tried to repalce by setting both newGraph and newGraphUri. Actual values are {0} and {1} respectively", newGraph, newGraphUri);
                    throw new ArgumentException(message, "replacedGraphUri");
                }

                newTriple = new Triple(newSubject, newPredicate, newObject, newGraph);
            }
            else
            {
                newTriple = new Triple(newSubject, newPredicate, newObject, newGraphUri);
            }

            return newTriple;
        }
    }
}