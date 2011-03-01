using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunkX
{
    /// <summary>
    /// Base class for all graphical types that can be drawn by Entity.
    /// </summary>
    public class Graphic
    {
        /// <summary>
        /// If the graphic should update.
        /// </summary>
        public Boolean active = false;

        /// <summary>
        /// If the graphic should render.
        /// </summary>
        public Boolean visible = true;

        /// <summary>
        ///  X offset.
        /// </summary>
        public float x = 0;

        /// <summary>
        /// Y offset.
        /// </summary>
        public float y = 0;
		
        /// <summary>
        /// X scrollfactor, effects how much the camera offsets the drawn graphic.
        /// Can be used for parallax effect, eg. Set to 0 to follow the camera,
        /// 0.5 to move at half-speed of the camera, or 1 (default) to stay still.
        /// </summary>
        public float scrollX = 1;
        
        /// <summary>
        /// Y scrollfactor, effects how much the camera offsets the drawn graphic.
        /// Can be used for parallax effect, eg. Set to 0 to follow the camera,
        /// 0.5 to move at half-speed of the camera, or 1 (default) to stay still.
        /// </summary>
        public float scrollY = 1;

        /// <summary>
        /// If the graphic should render at its position relative to its parent Entity's position.
        /// </summary>
        public Boolean relative = true;

        /// <summary>
        /// constructor
        /// </summary>
        public Graphic()
        {
        }

        /// <summary>
        /// Updates the graphic.
        /// </summary>
        public virtual void update()
        {
        }

        /// <summary>
        /// Renders the graphic to the screen buffer.
        /// </summary>
        /// <param name="point">The position to draw the graphic.</param>
        /// <param name="camera">The camera offset.</param>
        public virtual void render(Microsoft.Xna.Framework.Vector2 point, Microsoft.Xna.Framework.Vector2 camera)
        {
        }

        public delegate void AssignmentDelegate(); 

        /// <summary>
        /// Callback for when the graphic is assigned to an Entity.
        /// </summary>
        protected AssignmentDelegate assign
        {
            get { return _assign; }
            set { _assign = value; }
        }

        // Graphic information
        internal AssignmentDelegate _assign;
        internal Boolean _scroll = true;
    }
}
