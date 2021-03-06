using System;
using System.Collections.Generic;
using JsonApiDotNetCore.Builders;
using JsonApiDotNetCore.Internal;
using JsonApiDotNetCore.Models;
using JsonApiDotNetCore.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApiDotNetCore.Configuration
{
    /// <summary>
    /// Global options.
    /// https://json-api-dotnet.github.io/#/global-options
    /// </summary>
    public class JsonApiOptions
    {
        /// <summary>
        /// Whether or not stack traces should be serialized in Error objects
        /// </summary>
        public static bool DisableErrorStackTraces { get; set; }
        
        /// <summary>
        /// Whether or not source URLs should be serialized in Error objects
        /// </summary>
        public static bool DisableErrorSource { get; set; }

        /// <summary>
        /// The base URL Namespace
        /// </summary>
        /// <example>
        /// <code>options.Namespace = "api/v1";</code>
        /// </example>
        public string Namespace { get; set; }

        /// <summary>
        /// The default page size for all resources
        /// </summary>
        /// <example>
        /// <code>options.DefaultPageSize = 10;</code>
        /// </example>
        public int DefaultPageSize { get; set; }

        /// <summary>
        /// Whether or not the total-record count should be included in all document
        /// level meta objects.
        /// Defaults to false.
        /// </summary>
        /// <example>
        /// <code>options.IncludeTotalRecordCount = true;</code>
        /// </example>
        public bool IncludeTotalRecordCount { get; set; }

        /// <summary>
        /// Whether or not clients can provide ids when creating resources.
        /// Defaults to false.  When disabled the application will respond 
        /// with a 403 Forbidden respponse if a client attempts to create a 
        /// resource with a defined id.
        /// </summary>
        /// <example>
        /// <code>options.AllowClientGeneratedIds = true;</code>
        /// </example>
        public bool AllowClientGeneratedIds { get; set; }

        /// <summary>
        /// The graph of all resources exposed by this application.
        /// </summary>
        public IContextGraph ContextGraph { get; set; }

        /// <summary>
        /// Use relative links for all resources.
        /// </summary>
        /// <example>
        /// <code>
        /// options.RelativeLinks = true;
        /// </code>
        /// <code>
        /// {
        ///   "type": "articles",
        ///   "id": "4309",
        ///   "relationships": {
        ///      "author": {
        ///        "links": {
        ///          "self": "/api/v1/articles/4309/relationships/author",
        ///          "related": "/api/v1/articles/4309/author"
        ///        }
        ///      }
        ///   }
        /// }
        /// </code>
        /// </example>
        public bool RelativeLinks { get; set; }

        /// <summary>
        /// Whether or not to allow all custom query parameters.
        /// </summary>
        /// <example>
        /// <code>
        /// options.AllowCustomQueryParameters = true;
        /// </code>
        /// </example>
        public bool AllowCustomQueryParameters { get; set; }

        /// <summary>
        /// The default behavior for serializing null attributes.
        /// </summary>
        /// <example>
        /// <code>
        /// options.NullAttributeResponseBehavior = new NullAttributeResponseBehavior {
        ///  // ...
        ///};
        /// </code>
        /// </example>
        public NullAttributeResponseBehavior NullAttributeResponseBehavior { get; set; }

        /// <summary>
        /// Whether or not to allow json:api v1.1 operation requests.
        /// This is a beta feature and there may be breaking changes
        /// in subsequent releases. For now, it should be considered
        /// experimental.
        /// </summary>
        /// <remarks>
        /// This will be enabled by default in a subsequent patch JsonApiDotNetCore v2.2.x
        /// </remarks>
        public bool EnableOperations { get; set; }

        /// <summary>
        /// Whether or not to validate model state.
        /// </summary>
        /// <example>
        /// <code>
        /// options.ValidateModelState = true;
        /// </code>
        /// </example>
        public bool ValidateModelState { get; set; }

        [Obsolete("JsonContract resolver can now be set on SerializerSettings.")]
        public IContractResolver JsonContractResolver
        {
            get => SerializerSettings.ContractResolver;
            set => SerializerSettings.ContractResolver = value;
        }
        public JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new DasherizedResolver()
        };

        public void BuildContextGraph<TContext>(Action<IContextGraphBuilder> builder) where TContext : DbContext
        {
            BuildContextGraph(builder);

            ContextGraphBuilder.AddDbContext<TContext>();

            ContextGraph = ContextGraphBuilder.Build();
        }

        public void BuildContextGraph(Action<IContextGraphBuilder> builder)
        {
            if (builder == null) return;

            builder(ContextGraphBuilder);

            ContextGraph = ContextGraphBuilder.Build();
        }

        public void EnableExtension(JsonApiExtension extension)
            => EnabledExtensions.Add(extension);

        internal IContextGraphBuilder ContextGraphBuilder { get; } = new ContextGraphBuilder();
        internal List<JsonApiExtension> EnabledExtensions { get; set; } = new List<JsonApiExtension>();
    }
}
