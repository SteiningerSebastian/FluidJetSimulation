using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public struct SimulationParticle {
        public SimulationParticle(float positionX, float positionY, float velocityX, float velocityY) {
            this.positionX = positionX;
            this.positionY = positionY;
            this.velocityX = velocityX;
            this.velocityY = velocityY;
        }

        public float positionX;
        public float positionY;
        public float velocityX;
        public float velocityY;

        public override string ToString() {
            return $@"positionX: {positionX}, 
positionY: {positionY}
velocityX: {velocityX}
velocityY: {velocityY}
";
        }
    }
}
