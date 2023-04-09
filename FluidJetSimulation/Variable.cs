using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public partial class Variable : ObservableObject {
        private string id;

        [ObservableProperty]
        public string name;

        [ObservableProperty]
        public string value;

        public Variable(string id, string name, string value) {
            this.id = id;
            this.name = name;
            this.value = value;
        }

        public string GetId() => id;
    }
}
