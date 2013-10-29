using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Common.Base
{
    public interface IController
    {
        bool IsKeyDown(Keys K);
        bool WasKeyPressed(Keys K);
        bool WasMouseClicked();
        bool WasMouseRightClicked();
        bool IsMouseLeftButtonDown();
        bool IsMouseRightButtonDown();
        bool Ctrl();
        bool Shift();
        bool Alt();
        
        Point GetMousePosition();
        Point GetMouseDelta();
        Vector2 GetAngle();
        Vector2 GetAngleDelta();
    }
}
