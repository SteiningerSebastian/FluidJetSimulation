using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ComputeSharp.WinUI;

namespace FluidJetSimulation {
    public partial class MainViewmodel: ObservableObject {
        /// <summary>
        /// Creates a new <see cref="MainViewModel"/> instance.
        /// </summary>
        public MainViewmodel() {
            this.isVerticalSyncEnabled = false;
            this.isDynamicResolutionEnabled = true;
            this.isRenderingPaused = false;
            this.selectedResolutionScale = 11;
            this.shaderRunner = IOCContainer.Instance.GetInstance<IShaderRunner>();
        }

        /// <summary>
        /// Gets or sets whether the vertical sync is enabled.
        /// </summary>
        [ObservableProperty]
        private bool isVerticalSyncEnabled;

        /// <summary>
        /// Gets or sets whether the dynamic resolution is enabled.
        /// </summary>
        [ObservableProperty]
        private bool isDynamicResolutionEnabled;

        /// <summary>
        /// Gets the currently selected resolution scale setting (as percentage value).
        /// </summary>
        [ObservableProperty]
        private int selectedResolutionScale;

        /// <summary>
        /// Gets or sets the currently selected compute shader.
        /// </summary>
        [ObservableProperty]
        private IShaderRunner shaderRunner;

        /// <summary>
        /// Gets or sets whether the rendering is currently paused.
        /// </summary>
        [ObservableProperty]
        private bool isRenderingPaused;
    }
}
