using CommonTypes;
using ProxyImplement;
using ProxyInvokeMiddleware;
using System.Text.Json.Serialization;

namespace HttpProxyService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            });

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Services.Configure<ProxySettings>(builder.Configuration.GetSection("ProxySettings"));
            builder.Services.AddMemoryCache();

            builder.Services.AddSingleton<IProxyPolicyProvider, ProxyPolicyProvider>();

            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Limits.MaxConcurrentConnections = 1024;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 4096;
                serverOptions.Limits.Http2.MaxStreamsPerConnection = 8;
                serverOptions.Limits.Http2.HeaderTableSize = 4096;
                serverOptions.Limits.Http2.MaxRequestHeaderFieldSize = 8192;
                serverOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(30);
                serverOptions.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromMinutes(1);
            });

            var app = builder.Build();
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseHsts();
            //}
            //app.UseHttpsRedirection();

            //var sampleTodos = new Todo[] {
            //    new(1, "Walk the dog"),
            //    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            //    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            //    new(4, "Clean the bathroom"),
            //    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
            //};

            //var todosApi = app.MapGroup("/todos");
            //todosApi.MapGet("/", () => sampleTodos);
            //todosApi.MapGet("/{id}", (int id) =>
            //    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
            //        ? Results.Ok(todo)
            //        : Results.NotFound());

            //app.UseMiddleware<InvokeMiddleware>();

            //app.UseAuthorization();

            app.UseMiddleware<InvokeMiddleware>();
            app.Run();
        }
    }

    public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

    [JsonSerializable(typeof(Todo[]))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {

    }
}
