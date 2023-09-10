using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.SystemTextJson;
using GraphQL.Transport;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SlackCloneGraphQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthorizeAttribute = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
using ApiService.Utils;

namespace ApiService.Controllers;

[ApiController]
[Route("graphql")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = "HasApiScopeClaim")]
public class ApiController : Controller
{
    private readonly IDocumentExecuter _documentExecuter;
    private readonly ISchema _schema;
    private readonly IOptions<GraphQLSettings> _graphQLOptions;

    public ApiController(
        IDocumentExecuter documentExecuter,
        ISchema schema,
        IOptions<GraphQLSettings> options
    )
    {
        _documentExecuter = documentExecuter;
        _schema = schema;
        _graphQLOptions = options;
    }

    [HttpPost]
    public async Task<IActionResult> GraphQLAsync(
        [FromBody] GraphQLRequest request
    )
    {
        var startTime = DateTime.UtcNow;

        var result = await _documentExecuter.ExecuteAsync(s =>
        {
            var userContext = new GraphQLUserContext
            {
                { "claims", HttpContext.User },
                { "queryName", FieldAnalyzer.GetQueryName(request.Query) },
                { "query", request.Query },
                {
                    "sub",
                    Guid.Parse(
                        AuthUtils.GetClaim("sub", HttpContext.User)!.Value
                    )
                }
            };
            s.Schema = _schema;
            s.Query = request.Query;
            s.Variables = request.Variables;
            s.OperationName = request.OperationName;
            s.RequestServices = HttpContext.RequestServices;
            s.UserContext = userContext!;
            s.CancellationToken = HttpContext.RequestAborted;
        });

        if (result.Errors?.Count > 0)
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error);
            }
        }

        if (_graphQLOptions.Value.EnableMetrics)
        {
            result.EnrichWithApolloTracing(startTime);
        }

        return new ExecutionResultActionResult(result);
    }
}
