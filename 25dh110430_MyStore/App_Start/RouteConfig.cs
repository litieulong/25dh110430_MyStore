
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace _25dh110430_MyStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // ROUTE CAMERA
            routes.MapRoute(
                name: "CameraRoute",
                url: "Products/Camera",
                defaults: new
                {
                    controller = "Home",
                    action = "Camera"
                },
                namespaces: new[] { "_25dh110430_MyStore.Controllers" }
            );

            // ROUTE LAPTOP
            routes.MapRoute(
                name: "LaptopRoute",
                url: "Products/Laptop",
                defaults: new
                {
                    controller = "Home",
                    action = "Laptop"
                },
                namespaces: new[] { "_25dh110430_MyStore.Controllers" }
            );

            // ROUTE MẶC ĐỊNH
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional
                },
                namespaces: new[] { "_25dh110430_MyStore.Controllers" }
            );
        }
    }
}
