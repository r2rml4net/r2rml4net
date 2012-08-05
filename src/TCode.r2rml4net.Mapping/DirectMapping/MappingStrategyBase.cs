using System.Collections.Generic;
using System.Linq;

namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public abstract class MappingStrategyBase
    {
        private readonly DirectMappingOptions _options;
        private readonly DirectMappingHelper _directMappingHelper;

        protected MappingStrategyBase(DirectMappingOptions options)
        {
            _options = options;
            _directMappingHelper = new DirectMappingHelper(options);
        }

        protected DirectMappingOptions Options
        {
            get { return _options; }
        }

        protected DirectMappingHelper DirectMappingHelper
        {
            get { return _directMappingHelper; }
        }

        protected string CreateBlankNodeTemplate(string tableName, IEnumerable<string> columnsArray)
        {
            var joinedColumnNames = string.Join(Options.TemplateSeparator,
                                                columnsArray.Select(DirectMappingHelper.EncloseColumnName));
            return string.Format("{0}{1}{2}", tableName, Options.TemplateSeparator, joinedColumnNames);
        }
    }
}