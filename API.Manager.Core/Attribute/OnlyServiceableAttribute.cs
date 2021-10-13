using API.Manager.Core;
using API.Manager.Core.Models;
using API.Manager.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Manager.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OnlyServiceableAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool isServiceable = true;

            var managerService = GetManagerService(context);

            if (managerService is not null)
            {
                var serviceArgs = PrepareServiceArgs(context);
                isServiceable = managerService.IsServiceable(serviceArgs).Result;
            }

            if (!isServiceable)
                context.Result = CreateObjectResult(context);

            return base.OnActionExecutionAsync(context, next);
        }

        private IManagerService GetManagerService(ActionExecutingContext context)
        {
            return (IManagerService)context.HttpContext.RequestServices.GetService(typeof(IManagerService));
        }

        private ApiManagerOptions GetApiManagerOptions(ActionExecutingContext context)
        {
            return (ApiManagerOptions)context.HttpContext.RequestServices.GetService(typeof(ApiManagerOptions));
        }

        private ObjectResult CreateObjectResult(ActionExecutingContext context)
        {
            var notAcceptableResult = new NotAcceptableResult();
            var options = (ApiManagerOptions)context.HttpContext.RequestServices.GetService(typeof(ApiManagerOptions));

            notAcceptableResult.Message = options.NotAcceptableMessage;

            var objectResult = new ObjectResult(notAcceptableResult);
            objectResult.StatusCode = StatusCodes.Status406NotAcceptable;

            return objectResult;
        }

        private Service PrepareServiceArgs(ActionExecutingContext context)
        {
            Service serviceArgs = new Service();

            serviceArgs.Channel = GetChannel(context);
            serviceArgs.Project = GetProject(context);
            serviceArgs.Controller = GetControllerName(context);
            serviceArgs.Method = GetMethod(context);
            serviceArgs.MethodType = GetMethodType(context);
            return serviceArgs;
        }

        private string GetChannel(ActionExecutingContext context)
        {
            string channel = string.Empty;

            var options = GetApiManagerOptions(context);

            if (options.Channels.Length == 1)
                channel = options.Channels.FirstOrDefault();
            else
                channel = context.HttpContext.Request.Headers.Where(c => c.Key == options.HeaderKey).FirstOrDefault().Value.ToString();

            return channel;
        }

        private string GetProject(ActionExecutingContext context)
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        private string GetControllerName(ActionExecutingContext context)
        {
            return context.Controller.GetType().Name;
        }

        private string GetMethod(ActionExecutingContext context)
        {
            string method;
            var attribute = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault();

            if (attribute is not null && !string.IsNullOrWhiteSpace(attribute.Template))
                method = attribute.Template;
            else
                method = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.Name;

            return method;
        }

        private string GetMethodType(ActionExecutingContext context)
        {
            string methodType;

            var attribute = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes<HttpMethodAttribute>().FirstOrDefault();

            if (attribute is not null && attribute.HttpMethods is not null && attribute.HttpMethods.Any())
                methodType = attribute.HttpMethods.FirstOrDefault();
            else
                methodType = context.HttpContext.Request.Method;

            return methodType;
        }
    }
}
