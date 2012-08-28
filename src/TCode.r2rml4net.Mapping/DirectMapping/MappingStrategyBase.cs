using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    /// <summary>
    /// Base class for mapping strategy classes
    /// </summary>
    public abstract class MappingStrategyBase
    {
        private readonly MappingOptions _options;
        private readonly MappingHelper _mappingHelper;

        /// <summary>
        /// </summary>
        protected MappingStrategyBase(MappingOptions options)
        {
            _options = options;
            _mappingHelper = new MappingHelper(options);
        }

        /// <summary>
        /// <see cref="MappingOptions"/>
        /// </summary>
        protected MappingOptions Options
        {
            get { return _options; }
        }

        /// <summary>
        /// <see cref="MappingHelper"/>
        /// </summary>
        protected MappingHelper MappingHelper
        {
            get { return _mappingHelper; }
        }

        /// <summary>
        /// Creates a template for column names in the form of "TableName;{col1};{col2};{col3}", where the separator is taken
        /// from <see cref="MappingOptions.BlankNodeTemplateSeparator"/>
        /// </summary>
        protected string CreateBlankNodeTemplate(string tableName, IEnumerable<string> columnsArray)
        {
            var joinedColumnNames = string.Join(Options.BlankNodeTemplateSeparator,
                                                columnsArray.Select(MappingHelper.EncloseColumnName));
            return string.Format("{0}{1}{2}", tableName, Options.BlankNodeTemplateSeparator, joinedColumnNames);
        }
    }
}