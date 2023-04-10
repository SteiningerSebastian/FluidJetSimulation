using ComputeSharp;
using ComputeSharp.__Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    [AutoConstructor]
    public readonly partial struct WindComputeShader : IComputeShader {

        /// <summary>
        /// The texture storing the wind information.
        /// </summary>
        public readonly ReadWriteTexture2D<float4> windTexture;
        /// <summary>
        /// Teh simulation particles.
        /// </summary>
        public readonly ReadWriteBuffer<float4> simulationParticles;
        /// <summary>
        /// The general velocity of the wind
        /// </summary>
        public readonly float2 windVelocity;
        /// <summary>
        /// The scale of the simulation
        /// </summary>
        public readonly float scale;
        /// <summary>
        /// The simulation step.
        /// </summary>
        public readonly float simulationStep;
        /// <summary>
        /// The slipstream factor.
        /// </summary>
        public readonly float slipstreamFactor;
        /// <summary>
        /// The radius of the particle.
        /// </summary>
        public readonly float radius;
        /// <summary>
        /// The radius of the particle.
        /// </summary>
        public readonly float windSpeedFactor;

        private float4 GetPixelFromVelocity(float2 velocity) {
            float4 pixel = new float4();
            pixel.X = velocity.X;
            pixel.Y = velocity.Y;
            return pixel;
        }

        private float2 GetVelocityFromPixel(float4 pixel) {
            float2 velocity = new float2();
            velocity.X = pixel.X;
            velocity.Y = pixel.Y;
            return velocity;
        }


        public void Execute() {
            float2 velocity = GetVelocityFromPixel(windTexture[ThreadIds.XY]);

            float posX = ThreadIds.X * scale;
            float posY = (DispatchSize.Y - ThreadIds.Y) * scale;

            bool hit = false;
            for (int i = 0; i < simulationParticles.Length; i++) {
                float dx = posX - simulationParticles[i].X;
                float dy = posY - simulationParticles[i].Y;
                if (dx * dx + dy * dy < radius * radius) {
                    hit = true;
                    float dvx = velocity.X - simulationParticles[i].Z;
                    float dvy = velocity.Y - simulationParticles[i].W;
                    velocity.X -= dvx * windSpeedFactor;
                    velocity.Y -= dvy * windSpeedFactor;
                }
            }

            if (hit == false) {
                float factor = slipstreamFactor * simulationStep;
                float dvx = velocity.X - windVelocity.X;
                float dvy = velocity.Y - windVelocity.Y;
                velocity.X -= dvx * factor;
                velocity.Y -= dvy * factor;
            }

            windTexture[ThreadIds.XY] = GetPixelFromVelocity(velocity);
        }
    }
}
