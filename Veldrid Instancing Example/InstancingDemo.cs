using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;
using SharpText;
using System.Runtime.CompilerServices;
using Veldrid_Instancing_Example.Instancing;
using Veldrid_Instancing_Example.Upate;

namespace Veldrid_Instancing_Example;
internal class InstancingDemo
{
    private Sdl2Window _window;
    private Map _map;

    public InstancingDemo()
    {
        WindowCreateInfo windowCI = new WindowCreateInfo()
        {
            X = 0,
            Y = 0,
            WindowWidth = 2560,
            WindowHeight = 1440,
            WindowTitle = "Veldrid Tutorial"
        };
        _window = VeldridStartup.CreateWindow(ref windowCI);


        GraphicsDeviceOptions options = new GraphicsDeviceOptions
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            ResourceBindingModel= ResourceBindingModel.Improved,
            SwapchainSrgbFormat = true,
            //Debug = true
        };
        var graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options, GraphicsBackend.Direct3D11);
        _map = new Map(graphicsDevice, 128, 128);
    }

    internal void Run()
    {
        
        while (_window.Exists)
        {
            
            InputSnapshot inputSnapshot = _window.PumpEvents();
            InputTracker.UpdateFrameInput(inputSnapshot);
            Draw();
            if (InputTracker.GetKeyDown(Key.Escape))
            {
                _window.Close();
            }
        }

        DisposeResources();
    }

    private void DisposeResources()
    {
        _map.DisposeResources();
    }

    private void Draw()
    {
        _map.Draw();
    }




   

}

