#region Licence
// Copyright (C) 2013 Tomasz Pluskiewicz
// http://r2rml.net/
// r2rml@r2rml.net
// 	
// ------------------------------------------------------------------------
// 	
// This file is part of r2rml4net.
// 	
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
// OR OTHER DEALINGS IN THE SOFTWARE.
// 	
// ------------------------------------------------------------------------
// 
// r2rml4net may alternatively be used under the LGPL licence
// 
// http://www.gnu.org/licenses/lgpl.html
// 
// If these licenses are not suitable for your intended use please contact
// us at the above stated email address to discuss alternative
// terms.
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using NUnit.Framework;
using TCode.r2rml4net.Excel;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Mapping.Tests.Excel
{
    [TestFixture]
    public class ExcelSchemaProviderTestsBase
    {
        private readonly IList<string> _expectedColumns = new List<string> { "OrderDate", "Region", "Rep", "Item", "Units", "Unit Cost", "Total" };

        private readonly IList<R2RMLType> _expectedTypes = new List<R2RMLType>
            {
                R2RMLType.DateTime,
                R2RMLType.String,
                R2RMLType.String,
                R2RMLType.String,
                R2RMLType.FloatingPoint,
                R2RMLType.FloatingPoint,
                R2RMLType.FloatingPoint
            };

        [TestCase("Excel\\SampleData.xls", ExcelFormat.BIFF8)]
        [TestCase("Excel\\SampleData.xlsx", ExcelFormat.OpenXML)]
        public void CanReadSchemaFromExcel(string fileName, ExcelFormat format)
        {
            // given
            var provider = new ExcelSchemaProvider(fileName, format);

            // then
            Assert.AreEqual(1, provider.Tables.Count);
            var tableMeta = provider.Tables["SalesOrders$"];
            Assert.AreEqual(7, tableMeta.ColumnsCount);
            Assert.IsTrue(tableMeta.Select((column, i) => column.Name == _expectedColumns[i]).All(namesAreSame => namesAreSame));
            Assert.IsTrue(tableMeta.Select((column, i) => column.Type == _expectedTypes[i]).All(typesAreSame => typesAreSame));
        }
    }
}