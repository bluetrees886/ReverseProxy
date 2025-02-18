
using CommonTypes;
using HttpProxyAuthentication;
using HttpProxyAuthentication.Types;
using Microsoft.Extensions.Options;
using ProxyImplement;
using ProxyInvokeMiddleware;
using PermissionMiddleware;

namespace HttpProxyService.Jit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Services.Configure<ProxySettings>(builder.Configuration.GetSection("ProxySettings"));
            builder.Services.AddMemoryCache();

            builder.Services.AddSingleton<ISignerProviderStorage, SignerProviderStorage>();
            builder.Services.AddSingleton<IProxyPolicyProvider, ProxyPolicyProvider>();
            //builder.Services.AddSingleton<IDnsResolver, LocalDnsResolver>();
            //builder.Services.AddSingleton<ProxyHttpClientFactoryProvider>();
            //builder.Services.AddSingleton<HttpProxyPolicyFactory>();
            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();


            //app.MapControllers();
            app
                .UseVerifyMiddleware(storage => storage.UseSm2().UseRsa())
                .UseInvokeMiddleware();

            app.Run();
        }
    }
}
