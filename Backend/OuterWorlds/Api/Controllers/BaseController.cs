using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OuterWorlds.CrossCutting.Exceptions;
using Serilog;
using System;
using System.Threading.Tasks;

namespace OuterWorlds.Api.Controllers
{
    public abstract class BaseController<TController> : Controller
    {
        protected IMediator MediatorService { get; }
        protected ILogger LogService { get; }
        protected IConfiguration Configuration { get; }

        protected BaseController(ILogger logService, IMediator mediatorService, IConfiguration configuration)
        {
            LogService = logService;
            MediatorService = mediatorService;
            Configuration = configuration;
        }

        protected async Task<IActionResult> GenerateResponseAsync<TRequest>(Func<Task<TRequest>> func)
        {
            try
            {
                var data = await func();
                
                return Ok(data);
            }
            catch(ApiCustomException customException)
            {
                return HandleExceptionResult(customException, (int)customException.ResponseCode);
            }
            catch(Exception ex)
            {
                return HandleExceptionResult(ex, 500);
            }
        }

        private IActionResult HandleExceptionResult(Exception ex, int responseCode)
        {
            var timestamp = DateTime.Now;
            var error = new { ex.Message, timestamp, ex.StackTrace };

            LogService.Error(JsonConvert.SerializeObject(error));

            return StatusCode(responseCode, error);
        }
    }
}
