using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public class DataViewModel {
        public ObservableCollection<Variable> Data;

        public DataViewModel() {
        }

        public DataViewModel(ObservableCollection<Variable> data) {
            Data = data;
            Data.CollectionChanged += Variable_Changed;
        }

        private void Variable_Changed(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            Console.WriteLine("A new variablef was added.");
        }
    }
}
