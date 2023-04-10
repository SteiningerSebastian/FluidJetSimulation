using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public interface ISimulationVariables {
        public System.Collections.ObjectModel.ObservableCollection<Variable> Data { get; }
        public Variable GetVar(string id);
        public string this[string id] {
            get;
        }
    }
}
