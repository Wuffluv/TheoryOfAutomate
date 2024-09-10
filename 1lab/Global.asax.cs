using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ODN.Utilities.Mvc.Virtual;
using ODN.Utilities.Mvc.Routes;
using ODN.Utilities.Base;

namespace _1lab
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /* Uncomment the two lines below to use the embedded view engine.  This assumes:
				1. You have an assembly that has embedded views (i.e. compiled as embedded resources).
				2. You have the embedded views stored in the traditional Views folder structure. */

            //System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(
            //    new EmbeddedViewPathProvider(AssemblyUtility.ResolveAssembly("EmbeddedModule")));

			/* Uncomment the line below to use the attribute-based routing.  You should
			 make sure you have decorated at least one controller or controller action method
			 with the MapRouteAttribute.*/

            //CustomRoutes.MapCustomRoutes();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
           
        }
    }
}