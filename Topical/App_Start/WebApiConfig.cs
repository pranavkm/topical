using System.Web.Http;
using Topical.Infrastructure;
using Topical.Services;

namespace Topical
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var json =  config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new SnakeCaseContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Add(new BasicAuthHandler(config.DependencyResolver));
        }
    }
}
