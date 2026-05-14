using Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Controllers
{

    public class AccessControl
    {

        public class UserAccess : AuthorizeAttribute
        {
            private Access RequiredAccess { get; set; }

            public UserAccess(Access Access = Access.Anonymous) : base()
            {
                RequiredAccess = Access;
            }

            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                try
                {
                    if (User.ConnectedUser == null) return false;
                    if (User.ConnectedUser.Blocked) return false;
                    if (User.ConnectedUser.Access < RequiredAccess) return false;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                bool ajaxRequest = filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (ajaxRequest)
                {
                    filterContext.Result = new HttpStatusCodeResult(403);
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Accounts/Login?message=Accès non autorisé!&success=false");
                }
            }
        }
    }
        }
    

