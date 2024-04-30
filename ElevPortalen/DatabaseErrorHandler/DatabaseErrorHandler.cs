using Azure.Core;
using ElevPortalen.Data;

namespace ElevPortalen.DatabaseErrorHandler
{
    public class DatabaseErrorHandler
    {
        private readonly RequestDelegate _request;

        public DatabaseErrorHandler(RequestDelegate request)
        {
            _request = request;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext AppDbContext,
            ElevPortalenDataDbContext EpDbContext, JobOfferDbContext JobDbContext, DataRecoveryDbContext RecoveryContext)
        {
            try
            {
                // Attempt to query the database to check the connection
                await AppDbContext.Database.CanConnectAsync();
                await EpDbContext.Database.CanConnectAsync();
                await JobDbContext.Database.CanConnectAsync();
                await RecoveryContext.Database.CanConnectAsync();

                // If the connection is successful 
                await _request(context);
            }
            catch (Exception)
            {
                // If an exception occurs (no connection for example)
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "text/plain";

                // Throw an error message
                await context.Response.WriteAsync("Error: Failed to connect to the database. Please try again later.");
            }
        }
    }
}

