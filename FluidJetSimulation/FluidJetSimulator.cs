using ComputeSharp;
using ComputeSharp.Resources;
using ComputeSharp.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;

namespace FluidJetSimulation {
    public class FluidJetSimulator : IFluidJetSimmulator {
        ///<inheritdoc/>
        public ISimulationVariables Variables { get; init; }
        ///<inheritdoc/>
        public float AccelerationDueToGravity => float.Parse(Variables["g"]);
        ///<inheritdoc/>
        public float ParticleSimulationRadius => float.Parse(Variables["r"]);
        ///<inheritdoc/>
        public float ParticleSimulationArea => ParticleSimulationRadius * ParticleSimulationRadius * (float)Math.PI;
        ///<inheritdoc/>
        public float ParticleRenderRadius => float.Parse(Variables["rr"]);
        ///<inheritdoc/>
        public float FluidDensity => float.Parse(Variables["dsobj"]);
        ///<inheritdoc/>
        public float AmbientDensity => float.Parse(Variables["dm"]);
        ///<inheritdoc/>
        public float DragCoefficient => float.Parse(Variables["cw"]);
        ///<inheritdoc/>
        public float AngleOfAttack => float.Parse(Variables["alpha"]) / 360f * 2f * (float)Math.PI;
        ///<inheritdoc/>
        public float Pressure => float.Parse(Variables["p0"]) * 100000;
        ///<inheritdoc/>
        public float ParticleSpawnRate => float.Parse(Variables["psr"]);
        ///<inheritdoc/>
        public float SimulationStep => float.Parse(Variables["step"]);
        ///<inheritdoc/>
        public float Scale => float.Parse(Variables["scale"]);
        ///<inheritdoc/>
        public float InitPositionX => float.Parse(Variables["p0X"]);
        ///<inheritdoc/>
        public float InitPositionY => float.Parse(Variables["p0Y"]);
        ///<inheritdoc/>
        public float VelocityStandardDev => float.Parse(Variables["stdev"]);
        ///<inheritdoc/>
        public int WindResolution => 1000;
        ///<inheritdoc/>
        public float VelocityWindX => float.Parse(Variables["vw0x"]);
        ///<inheritdoc/>
        public float VelocityWindY => float.Parse(Variables["vw0y"]);
        ///<inheritdoc/>
        public float SlipstreamFactor => float.Parse(Variables["sf"]);
        ///<inheritdoc/>
        public float WindSpeedFactor => float.Parse(Variables["wsf"]);
        ///<inheritdoc/>
        public ReadWriteTexture2D<float4> WindTexture { get; protected set; }
        ///<inheritdoc/>
        public float WindEffectRadius => float.Parse(Variables["wer"]);


        /// <summary>
        /// The mass of the particle
        /// </summary>
        public float Mass => float.Parse(Variables["m0"]);/*4f / 3f * (float)Math.PI * ParticleSimulationRadius * ParticleSimulationRadius * ParticleSimulationRadius * FluidDensity;*/
        /// <inheritdoc/>
        public List<Float4> SimulationParticles { get; protected set; } = new List<Float4>(100);


        public FluidJetSimulator() {
            Variables = IOCContainer.Instance.GetInstance<ISimulationVariables>(InstanceType.Singleton);

            WindTexture = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D<float4>(WindResolution, WindResolution);
        }

        /// <summary>
        /// The simulated time in ns.
        /// </summary>
        private long _simulationTime = 0;

        /// <summary>
        /// The time the last particle was spawned.
        /// </summary>
        private long lastSpawn = -1;


        /// <summary>
        /// Start the simulation.
        /// </summary>
        /// <param name="ct">The cancellation token to cancel the simulation.</param>
        /// <returns>Returns a running task.</returns>
        public Task StartSimulation(CancellationToken ct = default) => Task.Run(async () => {
            while (!ct.IsCancellationRequested) {
                Execute();
            }
        }, ct);

        private float GetRandomVelocity(float v) {
            Random rand = Random.Shared; //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            float randNormal = (float)(VelocityStandardDev * randStdNormal);
            return v + randNormal;
        }

        private Float4 GetInitParticle() {
            var velocity = Math.Sqrt(2 * Pressure / FluidDensity);
            var vx = GetRandomVelocity((float)(Math.Cos(AngleOfAttack) * velocity));
            var vy = GetRandomVelocity((float)(Math.Sin(AngleOfAttack) * velocity));

            return new Float4(InitPositionX, InitPositionY, vx, vy);
        }

        /// <summary>
        /// Execute the simulation-step.
        /// </summary>
        private void Execute() {
            //Spawn particles to simulate.
            if (_simulationTime - lastSpawn > 1 / ParticleSpawnRate * 1000000) {
                long particlesToSpawn = (long)((_simulationTime - lastSpawn) / (1 / ParticleSpawnRate * 1000000));
                for (int i = 0; i < particlesToSpawn; i++)
                    SimulationParticles.Add(GetInitParticle());
                lastSpawn = _simulationTime;
            }

            if (SimulationParticles.Count > 0) {
                Span<float4> particles = CollectionsMarshal.AsSpan(SimulationParticles);
                using (ReadWriteBuffer<Float4> buffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<float4>(particles)) {
                    //Simulate wind.
                    WindTexture.GraphicsDevice.For(WindResolution, WindResolution, new WindComputeShader(WindTexture, buffer, new Float2(VelocityWindX, VelocityWindY), Scale, SimulationStep, SlipstreamFactor, WindEffectRadius, WindSpeedFactor));

                    //Simulate particle movement
                    GraphicsDevice.GetDefault().For(buffer.Length, new FluidJetComputeShader(buffer, WindTexture, AmbientDensity, DragCoefficient, ParticleSimulationArea, AccelerationDueToGravity, Mass, this.SimulationStep));
                    SimulationParticles = new List<float4>(buffer.ToArray());
                }
            }
        
            //Remove particles not visible
            SimulationParticles.RemoveAll((p) => p.Y < 0);

            //Make the step in simulated time.
            _simulationTime += (long)(SimulationStep * 1000000);
        }
    }
}
