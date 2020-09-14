using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Extensions;
using RepoLite.Common.Settings;
using RepoLite.DataAccess;
using RepoLite.DataAccess.Accessors;
using RepoLite.GeneratorEngine;
using RepoLite.GeneratorEngine.Generators;
using RepoLite.GeneratorEngine.TemplateParsers;
using RepoLite.GeneratorEngine.TemplateParsers.Base;
using RepoLite.ViewModel;
using RepoLite.ViewModel.Main;
using RepoLite.ViewModel.Settings;
using RepoLite.Views;
using System;
using System.IO;
using System.Windows;

namespace RepoLite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string ClientDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Pattisoft\CodeGeneration\";
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfigurationRoot Configuration { get; set; }

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(LandingViewModel));
            services.AddTransient(typeof(CreateModelsViewModel));
            services.AddTransient(typeof(CreateRepositoriesViewModel));
            services.AddTransient(typeof(AllSettingsViewModel));

            //Todo think of a way that we can persist config changes 
            services.ConfigureWritable<GenerationSettings>(Configuration.GetSection("GenerationSettings"));
            services.ConfigureWritable<SystemSettings>(Configuration.GetSection("SystemSettings"));
            //services.ConfigureWritable<GenerationSettings>(Configuration.GetSection("GenerationSettings"), $"{ClientDataPath}appsettings.json");
            //services.ConfigureWritable<SystemSettings>(Configuration.GetSection("SystemSettings"), $"{ClientDataPath}appsettings.json");
            services.AddSingleton(Configuration);



            services.AddTransient<TemplateParserResolver>(provider => () =>
            {
                var generationOptions = provider.GetService<IOptions<GenerationSettings>>();
                var systemSettings = provider.GetService<IOptions<SystemSettings>>();

                if (systemSettings.Value.GenerationLanguage == GenerationLanguage.CSharp)
                    return systemSettings.Value.DataSource switch
                    {
                        DataSourceEnum.SQLServer => new CSharpSqlServersTemplateParser(generationOptions),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                throw new ArgumentOutOfRangeException();
            });

            services.AddTransient<GeneratorResolver>(provider => () =>
            {
                var generationOptions = provider.GetService<IOptions<GenerationSettings>>();
                var systemSettings = provider.GetService<IOptions<SystemSettings>>();

                if (systemSettings.Value.GenerationLanguage == GenerationLanguage.CSharp)
                    return systemSettings.Value.DataSource switch
                    {
                        DataSourceEnum.SQLServer => new CSharpSqlServerGenerator(generationOptions),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                throw new ArgumentOutOfRangeException();
            });

            services.AddTransient<DataSourceResolver>(provider => () =>
            {
                var systemSettings = provider.GetService<IOptions<SystemSettings>>();

                return systemSettings.Value.DataSource switch
                {
                    DataSourceEnum.SQLServer => new SQLServerAccess(systemSettings),
                    _ => throw new ArgumentOutOfRangeException()
                };

            });




            services.AddTransient<C1>();
            services.AddTransient<C2>();
            services.AddTransient<MyInterfaceResolver>(serviceProvider => (int myVal) =>
            {
                return myVal switch
                {
                    1 => serviceProvider.GetService<C1>(),
                    2 => serviceProvider.GetService<C2>(),
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        }
    }
}
