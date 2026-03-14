namespace API.partonair_v01.GlobalManager
{
    public static class ApplicationBuilderManager
    {
        public static WebApplication ConfigureDevelopmentMiddleware(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }

        public static WebApplication ConfigureExceptionHandling(this WebApplication app)
        {
            //app.UseExceptionHandler();
            app.UseStatusCodePages();

            return app;
        }

        public static WebApplication ConfigureHttpPipeline(this WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }

        public static WebApplication ConfigureRouting(this WebApplication app)
        {
            app.MapControllers();

            return app;
        }
    }
}