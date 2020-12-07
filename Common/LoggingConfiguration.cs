using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Logging.Logging
{
   public static class LoggingConfiguration
   {
       public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger => (context, configuration) =>
       {
           #region Enriching Logger Context

           var env = context.HostingEnvironment;


           configuration.Enrich.FromLogContext()
               .Enrich.WithProperty("ApplicationName", env.ApplicationName)
               .Enrich.WithProperty("Environment", env.EnvironmentName)
               .Enrich.WithExceptionDetails();

           #endregion

           configuration.WriteTo.Console().MinimumLevel.Information();



           #region ElasticSearch Configuration. Comment if Not Needed 


           var elasticUrl = context.Configuration.GetValue<string>("Logging:ElasticUrl");

           if (!string.IsNullOrEmpty(elasticUrl))
           {
               configuration.WriteTo.Elasticsearch(
                   new ElasticsearchSinkOptions(new Uri(elasticUrl))
                   {
                       AutoRegisterTemplate = true,
                       AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                       IndexFormat = "mywebapilog-logs-{0:yyyy.MM.dd}",
                       MinimumLogEventLevel = LogEventLevel.Debug
                   });
           }

           #endregion
       };
   }
}
