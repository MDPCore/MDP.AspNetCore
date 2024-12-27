using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;

namespace MDP.AspNetCore
{
    public static class WebApplicationExtensions
    {
        // Methods
        public static WebApplication ConfigureMDP(this WebApplication application)
        {
            #region Contracts

            if (application == null) throw new ArgumentNullException($"{nameof(application)}=null");

            #endregion

            // ExceptionHandler
            if (application.Environment.IsDevelopment() == false)
            {
                application.UseProblemDetails();
            }
            application.UseHook("ExceptionHandler");

            // Network 
            application.UsePathBase();
            application.UsePathDefault();
            application.UseForwardedHeaders();

            // Security
            application.UseHttpsRedirection();
            application.UseHsts();         

            // StaticFile
            application.UseDefaultFiles();
            application.UseStaticFiles();

            // Routing
            application.UseRouting();
            {
                // Auth
                application.UseAuthentication();
                application.UseAuthorization();
                application.UseAntiforgery();

                // Route
                application.UseHook("Routing");
                {
                    // ControllerRoute
                    application.MapControllers();
                    application.MapDefaultControllerRoute();
                }
                application.UseHook("Routed");
            }            

            // Return
            return application;
        }
    }
}
