using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public class DelegateObserver<T> : IObserver<T> {

        Action<T> onNext;
        Action onCompleted;
        Action<Exception> onError;

        public DelegateObserver(Action<T> onNext, Action onCompleted,Action<Exception> onError) { 
            this.onNext = onNext;
            this.onCompleted = onCompleted;
            this.onError = onError;
        }

        public void OnCompleted() => onCompleted();

        public void OnError(Exception error) => onError(error);

        public void OnNext(T value) => onNext(value);
    }
}
