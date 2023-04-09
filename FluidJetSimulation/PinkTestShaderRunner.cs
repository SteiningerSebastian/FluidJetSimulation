using ComputeSharp;
using ComputeSharp.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation;
public class PinkTestShaderRunner : IShaderRunner 
{
    public PinkTestShaderRunner()
    {
    }

    public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object parameter)
    {
        texture.GraphicsDevice.ForEach(texture, new PinkTestShader());
        return true;
    }
}
