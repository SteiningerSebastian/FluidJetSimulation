﻿using ComputeSharp;
using ComputeSharp.Resources;
using ComputeSharp.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation {
    public interface IFluidJetSimmulator {
        /// <summary>
        /// The simulation variables.
        /// </summary>
        public ISimulationVariables Variables { get; init; }

        /// <summary>
        /// The simulation particles.
        /// </summary>
        public List<Float4> SimulationParticles { get; }

        /// <summary>
        /// The texture to simulate the wind.
        /// </summary>
        public ReadWriteTexture2D<float4> WindTexture { get; }

        /// <summary>
        /// The acceleration due to gravity, 9.81 m/s^2 for earth.
        /// </summary>
        public float AccelerationDueToGravity { get; }

        /// <summary>
        /// The radius of the particle simulated.
        /// </summary>
        public float ParticleSimulationRadius { get; }

        /// <summary>
        /// The radius of the particle when rendered.
        /// </summary>
        public float ParticleRenderRadius { get; }

        /// <summary>
        /// The density of the simulated fluid.
        /// </summary>
        public float FluidDensity { get; }

        /// <summary>
        /// The radius of the particle when rendered.
        /// </summary>
        public float Mass { get; }

        /// <summary>
        /// Density of the ambient fluid/gas e.g. air.
        /// </summary>
        public float AmbientDensity { get; }

        /// <summary>
        /// The drag coefficient of the particle.
        /// </summary>
        public float DragCoefficient { get; }

        /// <summary>
        /// The angel f attack of the fluid jet.
        /// </summary>
        public float AngleOfAttack { get; }

        /// <summary>
        /// The pressure of the jet at the beginning to calculate the particle velocity
        /// </summary>
        public float Pressure { get; }

        /// <summary>
        /// The amount of particles spawnd per second.
        /// </summary>
        public float ParticleSpawnRate { get; }

        /// <summary>
        /// The step in seconds to make.
        /// </summary>
        public float SimulationStep { get; }

        /// <summary>
        /// The scale of the simulation.
        /// </summary>
        public float Scale { get; }

        /// <summary>
        /// The initial X position
        /// </summary>
        public float InitPositionX { get; }

        /// <summary>
        /// Teh initial Y position
        /// </summary>
        public float InitPositionY { get; }

        /// <summary>
        /// Standard deviation for velocity
        /// </summary>
        public float VelocityStandardDev { get; }

        /// <summary>
        /// The resolution of the wind.
        /// </summary>
        public int WindResolution { get; }

        /// <summary>
        /// The Velocity of the wind.
        /// </summary>
        public float VelocityWindX { get; }

        /// <summary>
        /// The Velocity of the wind.
        /// </summary>
        public float VelocityWindY { get; }

        /// <summary>
        /// The amout of slipstream in the wind.
        /// </summary>
        public float SlipstreamFactor { get; }

        /// <summary>
        /// The amout of slipstream in the wind.
        /// </summary>
        public float WindSpeedFactor { get; }

        /// <summary>
        /// The radius for the slipstream.
        /// </summary>
        public float WindEffectRadius { get; }
    }
}
