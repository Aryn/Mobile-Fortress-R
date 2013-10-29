#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;
using Mobile_Fortress_R.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Nuclex.UserInterface.Visuals.Flat.Renderers
{

    /// <summary>Renders button controls in a traditional flat style</summary>
    public class GraphicalButtonRenderer :
      IFlatControlRenderer<GraphicalButtonControl>
    {
        /// <summary>
        ///   Renders the specified control using the provided graphics interface
        /// </summary>
        /// <param name="control">Control that will be rendered</param>
        /// <param name="graphics">
        ///   Graphics interface that will be used to draw the control
        /// </param>
        public void Render(
          GraphicalButtonControl control, IFlatGuiGraphics graphics
        )
        {
            RectangleF controlBounds = control.GetAbsoluteBounds();
            Rectangle source = control.Source;

            // Determine the style to use for the button
            if (control.Enabled)
            {
                if (control.Depressed)
                {
                    source.Y += source.Height * 2;
                }
                else if (control.MouseHovering)
                {
                    source.Y += source.Height;
                }
                else if (control.Selected)
                {
                    source.Y += source.Height * 2;
                }
            }
            else
            {
                source.Y += source.Height * 3;
            }

            SpriteEffects effect = SpriteEffects.None;

            if (control.VerticalFlip) effect |= SpriteEffects.FlipVertically;
            if (control.HorizontalFlip) effect |= SpriteEffects.FlipHorizontally;


            RectangleF destinationF = control.GetAbsoluteBounds();
            Rectangle destination = new Rectangle((int)destinationF.X, (int)destinationF.Y, (int)destinationF.Width, (int)destinationF.Height);

            if (control.Batch == null)
            {
                //Accessing a private member in this way would probably be frowned upon, but the alternative is to write
                //a whole custom graphics processor and I do not want to deal with Nuclex's shit rn.
                Type graphicsType = graphics.GetType();
                FieldInfo field = graphicsType.GetField("spriteBatch", BindingFlags.Instance | BindingFlags.NonPublic);
                control.Batch = (SpriteBatch)field.GetValue(graphics);
            }

            control.Batch.Draw(control.Sheet, destination, source, Color.White, 0.0f, Vector2.Zero, effect, 0.0f);
        }
    }

} // namespace Nuclex.UserInterface.Visuals.Flat.Renderers
