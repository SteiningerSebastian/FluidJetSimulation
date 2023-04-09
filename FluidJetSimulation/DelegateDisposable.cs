using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public class DelegateDisposable:IDisposable {
        private readonly Action action;
        public DelegateDisposable(Action a) { 
            this.action = a;
        }

        public void Dispose() {
            this.action.Invoke();
        }
    }
}
