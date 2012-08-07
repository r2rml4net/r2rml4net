using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public abstract class MappingStrategyBase
    {
        private readonly MappingOptions _options;
        private readonly MappingHelper _mappingHelper;

        protected MappingStrategyBase(MappingOptions options)
        {
            _options = options;
            _mappingHelper = new MappingHelper(options);
        }

        protected MappingOptions Options
        {
            get { return _options; }
        }

        protected MappingHelper MappingHelper
        {
            get { return _mappingHelper; }
        }

        protected string CreateBlankNodeTemplate(string tableName, IEnumerable<string> columnsArray)
        {
            var joinedColumnNames = string.Join(Options.TemplateSeparator,
                                                columnsArray.Select(MappingHelper.EncloseColumnName));
            return string.Format("{0}{1}{2}", tableName, Options.TemplateSeparator, joinedColumnNames);
        }
    }
}