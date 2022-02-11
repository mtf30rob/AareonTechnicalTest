using System.Runtime.CompilerServices;
using AareonTechnicalTest.JsonConfiguration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AareonTechnicalTest.Controllers
{
    public class AareonControllerBase : ControllerBase
    {
        private readonly ILogger _logger;

        public AareonControllerBase(ILogger logger)
        {
            _logger = logger;
        }

        protected void AuditControllerAction([CallerMemberName] string methodName = null, params object[] paramters)
        {
            // In the real word the username would normally come from the ClaimsPrinciple on the HttpContext object (HttpContext.User)
            _logger.LogInformation("User {UserName} called {ControllerMethodName} with paramters {ControllerMethodParameters}.", "MadeUpUserName", methodName, paramters.Serialise());
        }
    }
}