using System;
using System.IO;
using System.Linq;
using System.Windows;
using RepoLite.Commands;
using RepoLite.ViewModel.Base;
using System.Windows.Input;
using Microsoft.Extensions.Options;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.Common.Options;
using RepoLite.DataAccess;
using RepoLite.GeneratorEngine.Generators;
using RepoLite.GeneratorEngine.Generators.CSharp.SQLServer;
using RepoLite.Views;

namespace RepoLite.ViewModel
{
    public class LandingViewModel : ViewModelBase
    {

        public LandingViewModel()
        {
            var generationSettings = IOC.Resolve<IOptions<GenerationOptions>>();
            var dataSource = IOC.Resolve<DataSourceResolver>().Invoke(DataSourceEnum.SQLServer);

            var tableAndSchemas = dataSource.GetTables();
            var tables = dataSource.LoadTables(tableAndSchemas);
            var monstersTable = tables.First(x => x.DbTableName == "Monsters");
                
            var text = new PkRepository(generationSettings, new RepositoryGenerationObject(monstersTable, tables)).TransformText();
            File.WriteAllText(@"C:\Users\Jimmy\Desktop\temp.txt", text);
            Environment.Exit(0);
        }
        public ICommand NavigateToCreateModels
        {
            get
            {
                return new RelayCommand(o =>
                {
                    DoWork(() =>
                    {
                        var wnd = o as LandingView;
                        Execute("/Views/Generation/CreateModelsView.xaml", wnd);
                    });
                });
            }
        }

        public ICommand NavigateToCreateRepositories
        {
            get
            {
                return new RelayCommand(o =>
                {
                    DoWork(() =>
                    {
                        var wnd = o as LandingView;
                        Execute("/Views/Generation/CreateRepositoriesView.xaml", wnd);
                    });
                });
            }
        }
    }
}
