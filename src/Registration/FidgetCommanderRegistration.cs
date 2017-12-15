/*  Copyright 2017 Sean Terry

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License. 
*/

using Fidget.Commander;
using Fidget.Commander.Internal;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for registering Fidget.Commander with the stock dependency injection.
    /// </summary>

    public static class FidgetCommanderRegistration
    {
        /// <summary>
        /// Adds Fidget.Commander to the services collection.
        /// You will need to register your handlers and decorators independently.
        /// </summary>
        /// <param name="services">Services collection.</param>
        /// <remarks>
        /// When using Fidget.Commander with the standard dependency injection container, it is recommended
        /// that you also use Scrutor to add all your handlers and decorators by scanning the assembly.
        /// </remarks>
        
        public static IServiceCollection AddFidgetCommander( this IServiceCollection services )
        {
            if ( services == null ) throw new ArgumentNullException( nameof( services ) );

            services.AddTransient( typeof(CommandAdapter<,>), typeof(CommandAdapter<,>) );
            services.AddTransient( typeof(CommandAdapter<>), typeof(CommandAdapter<>) );
            services.AddTransient<ICommandAdapterFactory,CommandAdapterFactory>();
            services.AddTransient<ICommandDispatcher,CommandDispatcher>();

            return services;
        }
    }
}