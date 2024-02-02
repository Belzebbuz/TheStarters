using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Namotion.Reflection;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace TheStarters.Client.Common.OpenApi
{
    public class SwaggerGlobalAuthProcessor(string name) : IOperationProcessor
    {
        public SwaggerGlobalAuthProcessor()
            : this(JwtBearerDefaults.AuthenticationScheme)
        {
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
                        name,
                        Array.Empty<string>()
                    }
                });
            }

            return true;
        }
    }
}
