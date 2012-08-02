namespace TCode.r2rml4net.Mapping.DirectMapping
{
    public abstract class DirectMappingStrategyBase
    {
        private readonly DirectMappingOptions _options;
        private readonly DirectMappingHelper _directMappingHelper;

        protected DirectMappingStrategyBase(DirectMappingOptions options)
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
    }
}