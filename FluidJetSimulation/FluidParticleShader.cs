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
        /// The simulation particles.
        /// </summary>
        public readonly ReadOnlyBuffer<float4> simulationParticles;

        /// <summary>
        /// The wind texture.
        /// </summary>
        public readonly ReadWriteTexture2D<float4> windTexture;

        /// <summary>
        /// The scale to render at.
        /// </summary>
        public readonly float scale;

        /// <summary>
        /// The radius of the particle rendered.
        /// </summary>
        public readonly float renderRadius;

        /// <summary>
        /// The max velocity
        /// </summary>
        public readonly float minVelocity;

        /// <summary>
        /// The min velocity
        /// </summary>
        public readonly float maxVelocity;

        /// <summary>
        /// The max wind velocity
        /// </summary>
        public readonly float minWindVelocity;

        /// <summary>
        /// The min wind velocity
        /// </summary>
        public readonly float maxWindVelocity;

        /// <inheritdoc/>
        public float4 Execute() {
            float4 pixel = new float4();

            float4 pixelWind = windTexture[ThreadIds.XY];
            var vwind = pixelWind.X * pixelWind.X + pixelWind.Y * pixelWind.Y;

            pixel.A = 1f;
            pixel.R = 0f;
            pixel.B = 0f;
            if ((maxWindVelocity - minWindVelocity) == 0) {
                pixel.G = vwind / maxWindVelocity / 2;
            }
            else {
                pixel.G = vwind / (maxWindVelocity - minWindVelocity);
            }

            float posX = ThreadIds.X * scale;
            float posY = (DispatchSize.Y - ThreadIds.Y) * scale;

            for (int i = 0; i < simulationParticles.Length; i++) {
                float dx = posX - simulationParticles[i].X;
                float dy = posY - simulationParticles[i].Y;
                if (dx * dx + dy * dy < renderRadius * renderRadius) {
                    float v = simulationParticles[i].Z * simulationParticles[i].Z + simulationParticles[i].W * simulationParticles[i].W;
                    v -= minVelocity;
                    pixel.R = v / (maxVelocity - minVelocity);
                    pixel.B = 1f - pixel.R;
                    pixel.G = 0f;
                    return pixel;
                }
            }

            return pixel;
        }
    }
}
