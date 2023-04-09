using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ABI.System.Windows.Input.ICommand_Delegates;

namespace FluidJetSimulation {
    [AutoConstructor]
    internal readonly partial struct FluidParticleShader : IPixelShader<float4> {
        /// <summary>
        /// Teh simulation particles.
        /// </summary>
        public readonly ReadOnlyBuffer<SimulationParticle> simulationParticles;

        /// <summary>
        /// The scale to render at.
        /// </summary>
        public readonly float scale;

        /// <summary>
        /// The radius of the particle rendered.
        /// </summary>
        public readonly float renderRadius;

        /// <summary>
        /// The radius of the particle rendered.
        /// </summary>
        public readonly float minVelocity;

        /// <summary>
        /// The radius of the particle rendered.
        /// </summary>
        public readonly float maxVelocity;

        /// <inheritdoc/>
        public float4 Execute() {
            float4 pixel = new float4();
            pixel.A = 1f;
            pixel.R = 0f;
            pixel.B = 0f;
            pixel.G = 0f;

            float posX = ThreadIds.X * scale;
            float posY = (DispatchSize.Y - ThreadIds.Y) * scale;

            for (int i = 0; i < simulationParticles.Length; i++) {
                float dx = posX - simulationParticles[i].positionX;
                float dy = posY - simulationParticles[i].positionY;
                if (dx * dx + dy * dy < renderRadius * renderRadius) {
                    float v = simulationParticles[i].velocityX * simulationParticles[i].velocityX + simulationParticles[i].velocityY * simulationParticles[i].velocityY;
                    v -= minVelocity;
                    pixel.R = v / (maxVelocity-minVelocity);
                    pixel.B = 1f - pixel.R;
                }
            }

            return pixel;
        }
    }
}
