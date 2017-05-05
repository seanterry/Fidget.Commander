using Fidget.Commander.Dispatch;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using StructureMap.AspNetCore;

namespace Fidget.Commander.Samples.StructureMap
{
    /// <summary>
    /// Application startup object.
    /// </summary>

    public class Startup
    {
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>

        public static void Main( string[] args )
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseStructureMap()
                .Build();

            host.Run();
        }

        /// <summary>
        /// Configures application services.
        /// </summary>
        /// <param name="services">Services collection.</param>
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        /// <summary>
        /// Configures application dependency injection.
        /// </summary>
        /// <param name="registry">StructureMap registry.</param>

        public void ConfigureContainer( Registry registry )
        {
            registry.Scan( scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.ConnectImplementationsToTypesClosing( typeof( ICommandHandler<,> ) );
                scanner.ConnectImplementationsToTypesClosing( typeof( ICommandDecorator<,> ) );
            });
            
            registry.For( typeof(ICommandAdapter<,>) ).Use( typeof(CommandAdapter<,>) );
            registry.For<ICommandAdapterFactory>().Use<CommandAdapterFactory>();
            registry.For<ICommandDispatcher>().Use<CommandDispatcher>();
        }

        /// <summary>
        /// Configures the HTTP pipeline.
        /// </summary>

        public void Configure( IApplicationBuilder app )
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
}