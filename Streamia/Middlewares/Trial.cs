using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streamia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Streamia.Middlewares
{
    public class Trial
    {
        private readonly RequestDelegate next;

        public Trial(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            var user = await userManager.GetUserAsync(httpContext.User);

            if (user != null)
            {
                if (((ClaimsIdentity)httpContext.User.Identity).HasClaim("IsAdmin", "true"))
                {
                    await next(httpContext);
                    return;
                }

                if (user.TrialAccount)
                {
                    if ((DateTime.Now - user.CreationDateTime).TotalDays > user.TrialDays)
                    {
                        await signInManager.SignOutAsync();
                        httpContext.Response.Redirect("/account/login");
                        return;
                    }
                }
            } 

            await next(httpContext);
        }
    }
}
