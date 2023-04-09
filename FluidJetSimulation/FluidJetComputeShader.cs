using ComputeSharp;
using ComputeSharp.__Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    [AutoConstructor]
    public readonly partial struct FluidJetComputeShader : IComputeShader {

        public readonly ReadOnlyBuffer<SimulationParticle> buffer;
        public readonly ReadWriteBuffer<float4> writeBuffer;
        public readonly float ambientDensity;
        public readonly float dragCoefficient;
        public readonly float particleSimulationArea;
        public readonly float accelerationDueToGravity;
        public readonly float mass;
        public readonly float simulationStep;

        public void Execute() {
            SimulationParticle particle = buffer[ThreadIds.X];
            float velocity2 = particle.velocityY * particle.velocityY + particle.velocityX * particle.velocityX;
            float fwres = -0.5f * ambientDensity * dragCoefficient * particleSimulationArea * velocity2;
            float alpha = Hlsl.Atan(particle.velocityY / particle.velocityX);
            float fwy = Hlsl.Sin(alpha) * fwres;
            float fwx = Hlsl.Cos(alpha) * fwres;
            float fy = fwy - accelerationDueToGravity * mass;
            float vy = particle.velocityY + fy / mass * simulationStep;
            float vx = particle.velocityX + fwx / mass * simulationStep;
            float posX = particle.positionX + (vx + particle.velocityX) / 2f * simulationStep;
            float posY = particle.positionY + (vy + particle.velocityY) / 2f * simulationStep;
            writeBuffer[ThreadIds.X] = new Float4(posX, posY, vx, vy);
        }
    }
}
