﻿using Microsoft.Extensions.DependencyInjection;

namespace ServerLibrary.Authentication.Claim
{
    public static class Claims
    {
        public static void AddMyClaims(this IServiceCollection service)
        {
            service.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",
                    policy =>
                    {
                        policy.RequireRole("Admin");
                    });
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });
        }
    }
}
