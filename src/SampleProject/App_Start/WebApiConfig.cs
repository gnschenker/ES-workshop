using System.Web.Http;

namespace SampleProject
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration cfg)
        {
            cfg.MapHttpAttributeRoutes();
        }
    }
}