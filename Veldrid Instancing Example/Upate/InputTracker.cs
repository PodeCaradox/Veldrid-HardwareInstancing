using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid_Instancing_Example.Instancing;

namespace Veldrid_Instancing_Example.Upate;

public static class InputTracker
{
    private static HashSet<Key> _currentlyKeysState = new HashSet<Key>();
    private static HashSet<Key> _currentlyPressedKeys = new HashSet<Key>();
    private static HashSet<Key> _newKeysThisFrame = new HashSet<Key>();

    private static HashSet<MouseButton> _currentlyPressedMouseButtons = new HashSet<MouseButton>();
    private static HashSet<MouseButton> _newMouseButtonsThisFrame = new HashSet<MouseButton>();

    public static Vector2 MousePosition;
    private static float _wheelDelta;

    public static InputSnapshot FrameSnapshot { get; private set; }


    public static float WheelDelta()
    {
        return _wheelDelta;
    }
    public static bool GetKey(Key key)
    {
        return _currentlyPressedKeys.Contains(key);
    }

    public static bool GetKeyDown(Key key)
    {
        return _newKeysThisFrame.Contains(key);
    }

    public static bool GetPressedOnce(Key key)
    {
        return _currentlyPressedKeys.Contains(key) && _currentlyKeysState.Contains(key);
    }

    public static bool GetMouseButton(MouseButton button)
    {
        return _currentlyPressedMouseButtons.Contains(button);
    }

    public static bool GetMouseButtonDown(MouseButton button)
    {
        return _newMouseButtonsThisFrame.Contains(button);
    }

    public static void UpdateFrameInput(InputSnapshot snapshot)
    {
        FrameSnapshot = snapshot;
        _newKeysThisFrame.Clear();
        _newMouseButtonsThisFrame.Clear();

        MousePosition = new Vector2(snapshot.MousePosition.X, snapshot.MousePosition.Y);
        for (int i = 0; i < snapshot.KeyEvents.Count; i++)
        {
            KeyEvent ke = snapshot.KeyEvents[i];
            if (ke.Down)
            {
                KeyDown(ke.Key);
            }
            else
            {
                KeyUp(ke.Key);
            }
        }
        _wheelDelta = snapshot.WheelDelta;
        for (int i = 0; i < snapshot.MouseEvents.Count; i++)
        {
            MouseEvent me = snapshot.MouseEvents[i];
         
            if (me.Down)
            {
                MouseDown(me.MouseButton);
            }
            else
            {
                MouseUp(me.MouseButton);
            }
        }
    }

    private static void MouseUp(MouseButton mouseButton)
    {
        _currentlyPressedMouseButtons.Remove(mouseButton);
        _newMouseButtonsThisFrame.Remove(mouseButton);
    }

    private static void MouseDown(MouseButton mouseButton)
    {
        if (_currentlyPressedMouseButtons.Add(mouseButton))
        {
            _newMouseButtonsThisFrame.Add(mouseButton);
        }
    }

    private static void KeyUp(Key key)
    {
        _currentlyKeysState.Add(key);
        _currentlyPressedKeys.Remove(key);
        _newKeysThisFrame.Remove(key);
    }

    private static void KeyDown(Key key)
    {
       
        if (_currentlyPressedKeys.Add(key))
        {
            _currentlyKeysState.Remove(key);
            _newKeysThisFrame.Add(key);
        }
    }
}

