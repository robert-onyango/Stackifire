﻿using Serilog;
using Serilog.Exceptions;
using SerilogWeb.Classic;
using SerilogWeb.Classic.Enrichers;

namespace StackifyExample4
{
   public class LoggerFactory
   {
      bool initialized;

      public ILogger Create()
      {
         if (!initialized)
         {
            Init();
         }
         return Log.Logger;
      }

      public void Init()
      {
         if (initialized)
         {
            return;
         }
         initialized = true;
         Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.With<UserNameEnricher>()
            .Enrich.With<HttpRequestUrlEnricher>()
            .Enrich.With<HttpRequestUserAgentEnricher>()
            .Enrich.WithExceptionDetails()
            .WriteTo.Trace(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{UserName}] [{HttpRequestUrl}] {Message}{NewLine}{Exception}")
            .WriteTo.Stackify()
            .CreateLogger();

         // Do not need the logging of all request
         SerilogWebClassic.Configure(cfg => cfg.Disable());

         // Only needed for debugging stackify
         //StackifyAPILogger.LogEnabled = true;
         //StackifyAPILogger.OnLogMessage += data => Debug.WriteLine(data);

         Log.Logger.ForContext<LoggerFactory>().Information("App Restarted");
      }
   }
}
