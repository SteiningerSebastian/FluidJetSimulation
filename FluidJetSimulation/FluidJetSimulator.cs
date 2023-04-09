using ComputeSharp;
using ComputeSharp.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// The mass of the particle
        /// </summary>
        public float Mass => 4f / 3f * (float)Math.PI * ParticleSimulationRadius * ParticleSimulationRadius * ParticleSimulationRadius * FluidDensity;
        /// <inheritdoc/>
        public List<SimulationParticle> SimulationParticles { get; protected set; } = new List<SimulationParticle>();

        public FluidJetSimulator() {
            Variables = IOCContainer.Instance.GetInstance<ISimulationVariables>(InstanceType.Singleton);
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

        private SimulationParticle GetInitParticle() {            
            var velocity = Math.Sqrt(2 * Pressure / FluidDensity);
            var vx = GetRandomVelocity((float)(Math.Cos(AngleOfAttack) * velocity));
            var vy = GetRandomVelocity((float)(Math.Sin(AngleOfAttack) * velocity));

            return new SimulationParticle(InitPositionX, InitPositionY, vx, vy);
        }

        /// <summary>
        /// Execute the simulation-step.
        /// </summary>
        private void Execute() {
            var _temp = SimulationParticles;

            //Spawn particles to simulate.
            if (_simulationTime - lastSpawn > 1 / ParticleSpawnRate * 1000000) {
                lastSpawn = _simulationTime;
                _temp.Add(GetInitParticle());
            }

            if (_temp.Count > 0)
                using (ReadOnlyBuffer<SimulationParticle> buffer = GraphicsDevice.GetDefault().AllocateReadOnlyBuffer(_temp.ToArray())) {
                    using (ReadWriteBuffer<float4> writeBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer<float4>(_temp.Count)) {
                        GraphicsDevice.GetDefault().For(buffer.Length, new FluidJetComputeShader(buffer, writeBuffer, AmbientDensity, DragCoefficient, ParticleSimulationArea, AccelerationDueToGravity, Mass, this.SimulationStep));
                        var _par = writeBuffer.ToArray();
                        SimulationParticles = _par.Select(p => new SimulationParticle(p.X, p.Y, p.Z, p.W)).ToList();
                    }
                }

            //Remove particles not visible
            for (int i = 0; i < SimulationParticles.Count;) {
                var particle = SimulationParticles[i];
                if (particle.positionY < 0)
                    SimulationParticles.RemoveAt(i);
                else
                    i++;
            }

            //Make the step in simulated time.
            _simulationTime += (long)(SimulationStep * 1000000);
        }
    }
}
