using System;
using System.Web;

namespace Demo
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e) { }

        public static bool IsAccessGranted(string[] userRoles, string operation, string resource)
        {
            var state = HttpContext.Current.Application;
            if (state["AccessControlList"] == null)
                state["AccessControlList"] = GetAclTableFromSourceSystem();

            var acl = (IAccessControlList)state["AccessControlList"];
            return acl.IsGranted(userRoles, operation, resource);
        }

        private static IAccessControlList GetAclTableFromSourceSystem()
        {
            // You'll need to look after this for your own source system.
            return new AccessControlList();
        }
    }
}