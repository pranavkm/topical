using System.Web.Http;

namespace Topical
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            DIConfig.Configure(GlobalConfiguration.Configuration);
        }
    }
}
