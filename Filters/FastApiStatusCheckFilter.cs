using Dermainsight.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Dermainsight.Filters
{
    public class FastApiStatusCheckFilter : IActionFilter
    {
        private readonly FastApiService _fastApiService;

        public FastApiStatusCheckFilter(FastApiService fastApiService)
        {
            _fastApiService = fastApiService;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the FastAPI service is running
            var result = _fastApiService.checkService().Result;

            if (!result.IsSuccess)
            {
                var controller = context.RouteData.Values["controller"]?.ToString();
                var action = context.RouteData.Values["action"]?.ToString();
                var id = context.RouteData.Values["id"]?.ToString();

                string requestedPath;

                if (controller != null && action != null)
                {
                    requestedPath = $"/{controller}/{action}" + (id != null ? $"/{id}" : "");
                }
                else
                {
                    // fallback - tam path + query göster
                    requestedPath = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                }

                var errModel = new Models.Error
                {
                    Title = "FastAPI Servisine Ulaşılamıyor",
                    Message = "Sunucu şu anda FastAPI servisine bağlanamıyor.",
                    StatusCode = result.StatusCode,
                    RequestedPath = requestedPath,
                    ExceptionDetails = result.Message
                };

                var metadataProvider = context.HttpContext.RequestServices
                    .GetService(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.IModelMetadataProvider))
                        as Microsoft.AspNetCore.Mvc.ModelBinding.IModelMetadataProvider;


                context.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/_ErrorLayout.cshtml",
                    ViewData = new ViewDataDictionary<Models.Error>(metadataProvider, context.ModelState)
                    {
                        Model = errModel
                    }
                };
            }
        }
    }
}
