using System;

namespace MDP.AspNetCore
{
    public static class Host
    {
        // Methods
        public static void Run(string[] args)
        {
            #region Contracts

            if (args == null) throw new ArgumentException($"{nameof(args)}=null");

            #endregion

            // ApplicationBuilder
            var applicationBuilder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args).ConfigureMDP();
            if (applicationBuilder == null) throw new InvalidOperationException($"{nameof(applicationBuilder)}=null");

            // Application
            var application = applicationBuilder.Build().ConfigureMDP();
            if (application == null) throw new InvalidOperationException($"{nameof(application)}=null");

            // Run
            application.Run();
        }
    }
}
