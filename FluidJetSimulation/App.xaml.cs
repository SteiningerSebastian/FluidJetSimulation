// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using ComputeSharp;
using ComputeSharp.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FluidJetSimulation {
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args) {
            var obj = new SimulationVariables();
            obj.Add("g", "AccelerationDueToGravity", "9.81");
            obj.Add("r", "Radius", "0.03");
            obj.Add("rr", "RenderRadius", "0.5");
            obj.Add("dm", "AmbientDensity", "1.2");
            obj.Add("cw", "DragCoefficient", "0.075");
            obj.Add("m0", "Mass", "0.01");
            obj.Add("dsobj", "FluidDensity", "1000");
            obj.Add("p0", "Pressure", "8");
            obj.Add("alpha", "AngleOfAttack", "35");
            obj.Add("psr", "ParticleSpawnRate", "100");
            obj.Add("step", "SimulationStep", "0.025");
            obj.Add("scale", "Scale", "0.1");
            obj.Add("p0X", "InitPositionX", "1");
            obj.Add("p0Y", "InitPositionY", "2.5");
            obj.Add("stdev", "VelocityStandardDev", "1");
            obj.Add("vw0x", "VelocityWindX", "0");
            obj.Add("vw0y", "VelocityWindY", "0");
            obj.Add("sf", "SlipstreamFactor", "1");
            obj.Add("wsf", "WindSpeedDeltaFactor", "1");
            obj.Add("wer", "WindEffectRadius", "0.5");

            IOCContainer.Instance.Register<ISimulationVariables, SimulationVariables>(obj);
            CancellationTokenSource cts = new CancellationTokenSource();

            var fpsrunner = new FluidParticleShaderRunner(cts.Token);
            IOCContainer.Instance.Register<IShaderRunner, FluidParticleShaderRunner>(fpsrunner);

            var sim = new FluidJetSimulator();
            Task taskSim = sim.StartSimulation(cts.Token);

            IOCContainer.Instance.Register<IFluidJetSimmulator, FluidJetSimulator>(sim);

            m_window = new Window();
            m_window.Content = new MainPage();
            m_window.Title = "Visualization";
            m_window.Activate();

            m_window.Closed += async (_, _) => {
                cts.Cancel();
                await taskSim;
                this.Exit();
                //Because I could not find a better solution. (Background process does not finish)
                foreach (var p in Process.GetProcessesByName("FluidJetSimulation"))
                    p.Kill();
            };

            t_window = new Window();
            t_window.Content = new DataPage();
            t_window.Title = "Data";
            t_window.Activate();
        }

        private Window m_window;
        private Window t_window;
    }
}
