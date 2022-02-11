using Newtonsoft.Json;
using RepoLite.Commands;
using RepoLite.Common.Models;
using RepoLite.ViewModel.Base;
using RepoLite.Views.Generation.Procedures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RepoLite.ViewModel.Generation.Procedures
{
    internal class ConfigureProceduresViewModel : ViewModelBase
    {
        public ObservableCollection<ProcedureGenerationObject> Procedures { get; internal set; }

        public Dictionary<string, List<string>> PreviousGenerationSettings { get; internal set; }

        public ConfigureProceduresViewModel(List<ProcedureGenerationObject> procedures, Dictionary<string, List<string>> genSettings)
        {
            Procedures = new ObservableCollection<ProcedureGenerationObject>(procedures);
            foreach (var procedure in Procedures)
            {
                if (genSettings.ContainsKey(procedure.Name))
                {
                    for (int i = 0; i < procedure.ResultSets.Count; i++)
                    {
                        var item = procedure.ResultSets[i];
                        item.Name = genSettings[procedure.Name][i];
                    }
                }
            }

            OnPropertyChanged(nameof(Procedures));
        }

        public ICommand Save
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var wnd = o as ConfigureProcedures;

                    var genSettings = new Dictionary<string, List<string>>();
                    foreach (var procedure in Procedures)
                    {
                        genSettings[procedure.Name] = procedure.ResultSets.Select(x => x.Name).ToList();                        
                    }
                    Properties.Settings.Default["PreviousProcedureResultSetNames"] = JsonConvert.SerializeObject(genSettings);
                    Properties.Settings.Default.Save();

                    wnd.Close();
                });
            }
        }
    }
}
