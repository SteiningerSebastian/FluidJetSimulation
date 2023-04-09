using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public class SimulationVariables: ISimulationVariables {
        public System.Collections.ObjectModel.ObservableCollection<Variable> Data { get; } = new System.Collections.ObjectModel.ObservableCollection<Variable>();
        public string this[string id] {
            get => Data.FirstOrDefault(v => v.GetId().Equals(id))?.Value??throw new NullReferenceException(id);
        }

        public void Add(string id, string name, string value) {
            Data.Add(new Variable(id, name, value));
        }
    }
}
