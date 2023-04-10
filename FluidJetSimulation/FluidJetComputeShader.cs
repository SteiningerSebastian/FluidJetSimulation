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

        public readonly ReadWriteBuffer<float4> buffer;
        public readonly ReadWriteTexture2D<float4> windTexture;
        public readonly float ambientDensity;
        public readonly float dragCoefficient;
        public readonly float particleSimulationArea;
        public readonly float accelerationDueToGravity;
        public readonly float mass;
        public readonly float simulationStep;

        public void Execute() {
            float4 particle = buffer[ThreadIds.X];
            float pvx = particle.Z + windTexture[ThreadIds.XY].X;
            float pvy = particle.W + windTexture[ThreadIds.XY].Y;
            float velocity2 = pvx * pvx + pvy * pvy;

            float fwres = -0.5f * ambientDensity * dragCoefficient * particleSimulationArea * velocity2;
            
            float alpha = Hlsl.Atan(pvy / pvx);
            float fwy = Hlsl.Sin(alpha) * fwres;
            float fwx = Hlsl.Cos(alpha) * fwres;

            float fy = fwy - accelerationDueToGravity * mass;
            float vy = particle.W + fy / mass * simulationStep;
            float vx = particle.Z + fwx / mass * simulationStep;
            float posX = particle.X + (vx + particle.Z) / 2f * simulationStep;
            float posY = particle.Y + (vy + particle.W) / 2f * simulationStep;
            buffer[ThreadIds.X] = new Float4(posX, posY, vx, vy);
        }
    }
}
