using MDP.Configuration;
using MDP.Hosting;
using MDP.NetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace MDP.AspNetCore
{
    public static class WebApplicationBuilderExtensions
    {
        // Methods
        public static WebApplicationBuilder ConfigureMDP(this WebApplicationBuilder applicationBuilder)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentNullException($"{nameof(applicationBuilder)}=null");

            #endregion

            // ConfigurationBuilder
            var configurationBuilder = applicationBuilder.Configuration as IConfigurationBuilder;
            {
                // ConfigurationRegister
                ConfigurationRegister.RegisterModule(configurationBuilder, new MDP.Configuration.FileConfigurationProvider(applicationBuilder.Environment.EnvironmentName));
            }

            // ContainerBuilder
            var serviceCollection = applicationBuilder.Services;
            {
                // ContainerRegister
                {
                    ServiceFactoryRegister.RegisterModule(applicationBuilder, applicationBuilder.Configuration);
                }
                ContainerRegister.RegisterModule(serviceCollection, applicationBuilder.Configuration);                

                // HttpContext
                serviceCollection.AddHttpContextAccessor();

                // HtmlEncoder
                serviceCollection.AddSingleton<HtmlEncoder>
                (
                    HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs)
                );

                // ProblemDetails
                serviceCollection.AddProblemDetails();

                // LoggerOptions
                serviceCollection.AddLogging(builder =>
                {
                    // Filter
                    builder.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);
                });

                // ForwardedHeaders
                serviceCollection.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });

                // DataProtection
                serviceCollection.AddDataProtection();
            }

            // MvcBuilder
            var mvcBuilder = applicationBuilder.Services.AddMvc();
            {
                // MvcRegister
                MvcRegister.RegisterModule(mvcBuilder);

                // MvcOptions
                mvcBuilder.AddMvcOptions((options) =>
                {
                    // NotAcceptable
                    options.ReturnHttpNotAcceptable = false;

                    // AcceptHeader
                    options.RespectBrowserAcceptHeader = true;

                    // OutputFormatters:Null
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    options.OutputFormatters.Insert(0, new NullContentOutputFormatter());
                });

                // RazorOptions
                mvcBuilder.AddRazorOptions((options) =>
                {
                    // ViewLocation
                    options.ViewLocationFormats.Add("/Views/{0}.cshtml");

                    // AreaViewLocation
                    options.AreaViewLocationFormats.Add("/Views/{2}/{1}/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Views/{2}/Shared/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Views/{2}/{0}.cshtml");
                });

                // JsonOptions
                mvcBuilder.AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
                    options.JsonSerializerOptions.Converters.Add(new DateTimeISO8601Converter());
                });                
            }

            // Return
            return applicationBuilder;
        }


        // Class
        private class NullContentOutputFormatter : IOutputFormatter
        {
            // Methods
            public bool CanWriteResult(OutputFormatterCanWriteContext context)
            {
                #region Contracts

                if (context == null) throw new ArgumentNullException($"{nameof(context)}=null");

                #endregion

                // Void
                if (context.ObjectType == typeof(void) || context.ObjectType == typeof(Task))
                {
                    return true;
                }

                // Null
                if (context.Object == null)
                {
                    return true;
                }

                // Return
                return false;
            }

            public Task WriteAsync(OutputFormatterWriteContext context)
            {
                #region Contracts

                if (context == null) throw new ArgumentNullException($"{nameof(context)}=null");

                #endregion

                // Content
                context.HttpContext.Response.ContentLength = 0;

                // Return
                return Task.CompletedTask;
            }
        }

        private class DateTimeISO8601Converter : JsonConverter<DateTime>
        {
            // Methods
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                #region Contracts

                if (typeToConvert == null) throw new ArgumentNullException($"{nameof(typeToConvert)}=null");
                if (options == null) throw new ArgumentNullException($"{nameof(options)}=null");

                #endregion

                // DataTimeString
                var dataTimeString = reader.GetString();
                if (string.IsNullOrEmpty(dataTimeString) == true) throw new InvalidOperationException($"{nameof(dataTimeString)}=null");

                // DataTime
                var dateTime = DateTime.Parse(dataTimeString);
                if (dateTime.Kind == DateTimeKind.Unspecified)
                {
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                }

                // Read
                return dateTime;
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                #region Contracts

                if (writer == null) throw new ArgumentNullException($"{nameof(writer)}=null");
                if (options == null) throw new ArgumentNullException($"{nameof(options)}=null");

                #endregion

                // DataTime
                var dateTime = value;
                if (dateTime.Kind == DateTimeKind.Unspecified)
                {
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                }

                // DataTimeString
                var dataTimeString = dateTime.ToString("yyyy-MM-dd'T'HH:mm:ssK", CultureInfo.InvariantCulture);
                if (string.IsNullOrEmpty(dataTimeString) == true) throw new InvalidOperationException($"{nameof(dataTimeString)}=null");

                // Write
                writer.WriteStringValue(dataTimeString);
            }
        }
    }
}