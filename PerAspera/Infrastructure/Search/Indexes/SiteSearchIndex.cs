using Examine;
using Examine.Lucene;
using Microsoft.Extensions.Options;
using PerAspera.Extensions;
using System.Collections.Concurrent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
using PerAspera.Infrastructure.Search.ValueTypes;
using PerAspera.Infrastructure.Search.Configurations;

namespace PerAspera.Infrastructure.Search.Indexes
{
    public sealed class SiteSearchIndex : UmbracoExamineIndex, IUmbracoContentIndex, IDisposable
    {
        private readonly ISiteSearchConfiguration _siteSearchConfiguration;

        public SiteSearchIndex(
            ILoggerFactory loggerFactory,
            string name,
            IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions,
            Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
            IRuntimeState runtimeState,
            ISiteSearchConfiguration siteSearchConfiguration)
            : base(loggerFactory, name, indexOptions, hostingEnvironment, runtimeState)
        {
            LuceneDirectoryIndexOptions namedOptions = indexOptions.Get(name);
            if (namedOptions == null)
            {
                throw new InvalidOperationException($"No named {typeof(LuceneDirectoryIndexOptions)} options with name {name}");
            }

            var fieldDefinitions = namedOptions.FieldDefinitions;
            var valueTypeFactories = new ConcurrentDictionary<string, IFieldValueTypeFactory>();
            namedOptions.IndexValueTypesFactory.ForEach(valueType => valueTypeFactories.AddOrUpdate(valueType.Key, valueType.Value, (key, value) => valueType.Value));

            var jsonFieldValueTypeFactory = new DelegateFieldValueTypeFactory(field => new JsonValueType(field, loggerFactory));
            _ = valueTypeFactories
                    .AddOrUpdate(
                        JsonValueType.ValueTypeName,
                        jsonFieldValueTypeFactory,
                        (key, value) => jsonFieldValueTypeFactory);

            namedOptions.IndexValueTypesFactory = valueTypeFactories;

            siteSearchConfiguration
                .JsonSearchableFields
                .ForEach(
                    field => fieldDefinitions.AddOrUpdate(new FieldDefinition(field, JsonValueType.ValueTypeName))
                );

            if (namedOptions.Validator is IContentValueSetValidator contentValueSetValidator)
            {
                PublishedValuesOnly = contentValueSetValidator.PublishedValuesOnly;
            }

            _siteSearchConfiguration = siteSearchConfiguration.ThrowIfNull();
        }

        void IIndex.IndexItems(IEnumerable<ValueSet> values)
            => PerformIndexItems(
                values.Where(v => _siteSearchConfiguration.SearchableDocumentTypes.Contains(v.ItemType)),
                OnIndexOperationComplete);
    }
}
