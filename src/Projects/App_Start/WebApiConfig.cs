using System.Web.Http;

namespace Projects
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration cfg)
        {
            cfg.MapHttpAttributeRoutes();
        }
    }
}