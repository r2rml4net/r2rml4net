#region Licence

/* 
Copyright (C) 2013 Tomasz Pluskiewicz
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using TCode.r2rml4net.RDB;

namespace TCode.r2rml4net.Excel
{
    public class ExcelSchemaProvider : IDatabaseMetadata
    {
        private readonly string _excelFile;
        private readonly ExcelFormat _excelFormat;
        private TableCollection _tables;

        public ExcelSchemaProvider(string excelFile, ExcelFormat excelFormat)
        {
            _excelFile = excelFile;
            _excelFormat = excelFormat;

            DataProvider = "Microsoft.ACE.OLEDB.12.0";
            ColumnNamesInHeader = true;
        }

        public TableCollection Tables
        {
            get
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    if (_tables == null)
                    {
                        _tables = new TableCollection();
                        DataTable tablesSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[0]);

                        foreach (DataRow row in tablesSchema.Rows)
                        {
                            TableMetadata table = ReadTableMetadata(row);
                            foreach (var column in ReadColumns(table.Name, connection))
                            {
                                table.Add(column);
                            }

                            _tables.Add(table);
                        }
                    }
                }

                return _tables;
            }
        }

        public string DataProvider { get; set; }

        public bool ColumnNamesInHeader { get; set; }

        public IDbConnection Connection
        {
            get { return GetConnection(); }
        }

        private OleDbConnection GetConnection()
        {
            var builder = new OleDbConnectionStringBuilder
            {
                Provider = DataProvider,
                DataSource = _excelFile
            };
            builder.Add("Extended Properties", GetExtendedProperties(_excelFormat));

            return new OleDbConnection(builder.ConnectionString);
        }

        private string GetExtendedProperties(ExcelFormat excelFormat)
        {
            string properties;
            switch (excelFormat)
            {
                case ExcelFormat.BIFF8:
                    properties= "Excel 12.0 Xml";
                    break;
                case ExcelFormat.OpenXML:
                    properties= "Excel 8.0";
                    break;
                default:
                    throw new ArgumentException("Unrecognized format", "excelFormat");
            }

            if (ColumnNamesInHeader)
            {
                properties += ";HDR=Yes";
            }
            return properties;
        }

        private static IEnumerable<ColumnMetadata> ReadColumns(string sheetName, OleDbConnection connection)
        {
            using (var adapter = new OleDbDataAdapter(string.Format("select top 1 * from [{0}]", sheetName), connection))
            {
                DataTable select = new DataTable();
                adapter.Fill(select);

                foreach (DataColumn column in select.Columns)
                {
                    yield return new ColumnMetadata
                        {
                            Name = column.ColumnName,
                            Type = GetColumnType(column)
                        };
                }
            }
        }

        private static R2RMLType GetColumnType(DataColumn column)
        {
            if (column.DataType == typeof (DateTime))
            {
                return R2RMLType.DateTime;
            }
            if (column.DataType == typeof(float))
            {
                return R2RMLType.FloatingPoint;
            }
            if (column.DataType == typeof(double))
            {
                return R2RMLType.FloatingPoint;
            } 
            if (column.DataType == typeof(int))
            {
                return R2RMLType.Integer;
            }

            return R2RMLType.String;
        }

        private static TableMetadata ReadTableMetadata(DataRow row)
        {
            return new TableMetadata
                {
                    Name = (string)row["TABLE_NAME"]
                };
        }
    }
}
