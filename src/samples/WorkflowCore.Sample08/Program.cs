﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample08
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<HumanWorkflow>();
            host.Start();


            string workflowId = host.StartWorkflow("HumanWorkflow").Result;

            Thread.Sleep(3000);

            Console.WriteLine("Open user actions are");
            var openItems = host.GetOpenUserActions(workflowId);
            foreach (var item in openItems)
            {
                Console.WriteLine("Prompt = " + item.Prompt + ", Assigned to " + item.AssignedPrincipal);
            }


            //string value = Console.ReadLine();
            host.PublishUserAction(openItems.First().Key, "MYDOMAIN\\me", null).Wait();

            Console.ReadLine();
            host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddWorkflow();
            //services.AddWorkflow(x => x.UseMongoDB(@"mongodb://localhost:27017", "workflow"));
            

            var serviceProvider = services.BuildServiceProvider();

            //config logging
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddDebug(LogLevel.Debug);
            return serviceProvider;
        }

        
    }
}