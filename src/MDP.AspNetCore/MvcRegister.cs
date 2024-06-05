using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore
{
    public class MvcRegister
    {
        // Methods
        public static void RegisterModule(IMvcBuilder mvcBuilder)
        {
            #region Contracts

            if (mvcBuilder == null) throw new ArgumentNullException($"{nameof(mvcBuilder)}=null");

            #endregion

            // ApplicationAssemblyList
            var applicationAssemblyList = MDP.Reflection.Assembly.FindAllApplicationAssembly();
            if (applicationAssemblyList == null) throw new InvalidOperationException($"{nameof(applicationAssemblyList)}=null");

            // ApplicationPart
            {
                // RegisteredAssembly
                var registeredAssemblyList = new List<Assembly>();
                registeredAssemblyList.AddRange(mvcBuilder.PartManager.ApplicationParts.OfType<AssemblyPart>().Select(assemblyPart => assemblyPart.Assembly));
                registeredAssemblyList.AddRange(mvcBuilder.PartManager.ApplicationParts.OfType<CompiledRazorAssemblyPart>().Select(assemblyPart => assemblyPart.Assembly));

                // Register
                foreach (var applicationAssembly in applicationAssemblyList)
                {
                    if (registeredAssemblyList.Contains(applicationAssembly) == false)
                    {
                        mvcBuilder.AddApplicationPart(applicationAssembly);
                    }
                }
            }

            // ApplicationAsset
            {
                // AssetProvider
                var assetProviderList = new List<IFileProvider>();
                foreach (var applicationAssembly in applicationAssemblyList)
                {
                    // Require
                    if (applicationAssembly.GetManifestResourceNames().Length <= 0) continue;

                    // AssetProvider
                    IFileProvider assetProvider = null;
                    try
                    {
                        assetProvider = new ManifestEmbeddedFileProvider(applicationAssembly, @"wwwroot");
                    }
                    catch
                    {
                        assetProvider = null;
                    }

                    // Add
                    if (assetProvider != null)
                    {
                        assetProviderList.Add(assetProvider);
                    }
                }

                // Register
                mvcBuilder.Services.AddOptions<StaticFileOptions>().Configure<IWebHostEnvironment>((options, hostEnvironment) =>
                {
                    // RootFileProvider
                    if (hostEnvironment.WebRootFileProvider != null)
                    {
                        assetProviderList.Insert(0, hostEnvironment.WebRootFileProvider);
                    }

                    // CompositeFileProvider
                    options.FileProvider = new CompositeFileProvider
                    (
                        assetProviderList
                    );
                });
            }
        }
    }
}
