using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RepoLite.Common.Enums;
using RepoLite.Common.Options;
using RepoLite.DataAccess;
using RepoLite.DataAccess.Accessors;
using RepoLite.GeneratorEngine;
using RepoLite.GeneratorEngine.Generators;
using RepoLite.GeneratorEngine.Generators.CSharp.MySql;
using RepoLite.GeneratorEngine.Generators.CSharp.SQLServer;
using RepoLite.Models;
using RepoLite.ViewModel;
using RepoLite.ViewModel.Main;
using RepoLite.ViewModel.Settings;
using RepoLite.Views;

namespace RepoLite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        private IConfigurationRoot _configuration;

        public static string ClientDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Pattisoft\CodeGeneration\";

        public static Guid ClientId => Guid.Parse(ConfigurationManager.AppSettings["ClientId"]);

        public static List<CodePreview> CodePreview { get; set; }
        
        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();
            
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                    {
                        ConfigureServices(services);
                    })
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GenerationOptions>(_configuration.GetSection("Generation"));
            services.Configure<SystemOptions>(_configuration.GetSection("System"));
            
            services.AddSingleton<MainWindow>();
            
            services.AddSingleton<LandingViewModel>();
            services.AddSingleton<CreateModelsViewModel>();
            services.AddSingleton<CreateRepositoriesViewModel>();
            services.AddSingleton<AllSettingsViewModel>();

            services.AddTransient<SQLServerAccess>();
            services.AddTransient<MySqlAccess>();
            services.AddTransient<DataSourceResolver>(services => source =>
            {
                switch (source)
                {
                    case DataSourceEnum.SQLServer:
                        return services.GetRequiredService<SQLServerAccess>();
                    case DataSourceEnum.MySql:
                        return services.GetRequiredService<MySqlAccess>();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(source), source, null);
                }
            });
            
            
            services.AddTransient<CSharpSqlServerGenerator>();
            services.AddTransient<CSharpMySqlGenerator>();
            services.AddTransient<GeneratorResolver>(services => (datasource, language) =>
            {
                switch (datasource)
                {
                    case DataSourceEnum.SQLServer:
                        switch (language)
                        {
                            case GenerationLanguage.CSharp:
                                return services.GetRequiredService<CSharpSqlServerGenerator>();
                            default:
                                throw new ArgumentOutOfRangeException(nameof(language), language, null);
                        }
                    case DataSourceEnum.MySql:
                        switch (language)
                        {
                            case GenerationLanguage.CSharp:
                                return services.GetRequiredService<CSharpMySqlGenerator>();
                            default:
                                throw new ArgumentOutOfRangeException(nameof(language), language, null);
                        }

                    default:
                        throw new ArgumentOutOfRangeException(nameof(datasource), datasource, null);
                }
            });
            
            

            IOC.CurrentProvider = services.BuildServiceProvider();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();    
            }
            
            base.OnExit(e);
        }
    }
}