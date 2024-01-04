using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace TheStarters.Clients.Web.Infrastructure.OpenApi
{
    public class SwaggerGlobalAuthProcessor : IOperationProcessor
    {
        private readonly string _name;

        public SwaggerGlobalAuthProcessor()
            : this(JwtBearerDefaults.AuthenticationScheme)
        {
        }

        public SwaggerGlobalAuthProcessor(string name)
        {
            _name = name;
        }

        public bool Process(OperationProcessorContext context)
        {
            IList<object>? list = ((AspNetCoreOperationProcessorContext)context).ApiDescription?.ActionDescriptor?.TryGetPropertyValue<IList<object>>("EndpointMetadata");
            if (list is not null)
            {
                if (list.OfType<AllowAnonymousAttribute>().Any())
                    return true;

                if (context.OperationDescription.Operation.Security?.Any() != true)
                    (context.OperationDescription.Operation.Security ??= new List<OpenApiSecurityRequirement>()).Add(new OpenApiSecurityRequirement
                {
                    {
                        _name,
                        Array.Empty<string>()
                    }
                });
            }

            return true;
        }
    }
}
