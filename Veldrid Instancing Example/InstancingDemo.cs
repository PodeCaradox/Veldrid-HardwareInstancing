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
            X = 100,
            Y = 100,
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = "Veldrid Tutorial"
        };
        _window = VeldridStartup.CreateWindow(ref windowCI);


        GraphicsDeviceOptions options = new GraphicsDeviceOptions
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            ResourceBindingModel= ResourceBindingModel.Improved,
            Debug = true
        };
        var graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options, GraphicsBackend.OpenGL);
        _map = new Map(graphicsDevice, 175, 175);
    }

    internal void Run()
    {
        
        while (_window.Exists)
        {
            
            InputSnapshot inputSnapshot = _window.PumpEvents();
            InputTracker.UpdateFrameInput(inputSnapshot);
            Draw();
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

