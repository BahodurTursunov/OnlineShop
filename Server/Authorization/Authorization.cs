namespace Server.Authorization
{
    public static class Authorization
    {
        public static void AddMyAuth(this IServiceCollection service)
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
