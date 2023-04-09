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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            obj.Add("r", "Radius", "0.075");
            obj.Add("rr", "RenderRadius", "0.5");
            obj.Add("dm", "AmbientDensity", "1");
            obj.Add("cw", "DragCoefficient", "0.04");
            obj.Add("dsobj", "FluidDensity", "1000");
            obj.Add("p0", "Pressure", "8");
            obj.Add("alpha", "AngleOfAttack", "30");
            obj.Add("psr", "ParticleSpawnRate", "10");
            obj.Add("step", "SimulationStep", "0.001");
            obj.Add("scale", "Scale", "0.1");
            obj.Add("p0X", "InitPositionX", "0");
            obj.Add("p0Y", "InitPositionY", "2");
            obj.Add("stdev", "VelocityStandardDev", "0.1");

            IOCContainer.Instance.Register<ISimulationVariables, SimulationVariables>(obj);
            IOCContainer.Instance.Register<IShaderRunner, FluidParticleShaderRunner>();
            var sim = new FluidJetSimulator();
            _ = sim.StartSimulation();
            IOCContainer.Instance.Register<IFluidJetSimmulator, FluidJetSimulator>(sim);

            m_window = new Window();
            m_window.Content = new MainPage();
            m_window.Title = "Visualization";
            m_window.Activate();

            t_window = new Window();
            t_window.Content = new DataPage();
            t_window.Title = "Data";
            t_window.Activate();
        }

        private Window m_window;
        private Window t_window;
    }
}
