using ComputeSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ABI.System.Windows.Input.ICommand_Delegates;

namespace FluidJetSimulation
{
    [AutoConstructor]
    internal readonly partial struct PinkTestShader : IPixelShader<float4>
    {
        /// <inheritdoc/>
        public float4 Execute()
        {
            float4 pixel = new float4(); 
            pixel.R = 1f;
            pixel.G = 20 / 255f;
            pixel.B = 147 / 255f;
            pixel.A = 1f;
            return pixel;
        }
    }
}
