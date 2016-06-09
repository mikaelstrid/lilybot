﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lily.ShoppingList.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }
    }
}
