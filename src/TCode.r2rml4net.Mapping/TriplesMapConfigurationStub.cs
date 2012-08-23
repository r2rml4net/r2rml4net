using TCode.r2rml4net.Validation;
using VDS.RDF;

namespace TCode.r2rml4net.Mapping
{
    internal class TriplesMapConfigurationStub
    {
        private readonly IR2RMLConfiguration _r2RMLConfiguration;
        private readonly IGraph _r2RMLMappings;
        private readonly MappingOptions _options;
        private readonly ISqlVersionValidator _sqlVersionValidator;

        public TriplesMapConfigurationStub(IR2RMLConfiguration r2RMLConfiguration, IGraph r2RMLMappings, MappingOptions options, ISqlVersionValidator sqlVersionValidator)
        {
            _r2RMLConfiguration = r2RMLConfiguration;
            _r2RMLMappings = r2RMLMappings;
            _options = options;
            _sqlVersionValidator = sqlVersionValidator;
        }

        public IR2RMLConfiguration R2RMLConfiguration
        {
            get { return _r2RMLConfiguration; }
        }

        public IGraph R2RMLMappings
        {
            get { return _r2RMLMappings; }
        }

        public MappingOptions Options
        {
            get { return _options; }
        }

        public ISqlVersionValidator SQLVersionValidator
        {
            get { return _sqlVersionValidator; }
        }
    }
}