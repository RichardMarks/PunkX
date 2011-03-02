using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunkX.masks
{
    /// <summary>
    /// Uses parent's hitbox to determine collision. This class is used
    /// internally by FlashPunk, you don't need to use this class because
    /// this is the default behaviour of Entities without a Mask object.
    /// </summary>
    public class Hitbox : Mask
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="width">Width of the hitbox.</param>
        /// <param name="height">Height of the hitbox.</param>
        /// <param name="x">X offset of the hitbox.</param>
        /// <param name="y">Y offset of the hitbox.</param>
        public Hitbox(int width = 1, int height = 1, int x = 0, int y = 0)
        {
            _width = width;
			_height = height;
			_x = x;
			_y = y;

            _check["PunkX.Mask"] = collideMask;
            _check["PunkX.masks.Hitbox"] = collideHitbox;
        }

        /// <summary>
        /// Collides against an Entity.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private Boolean collideMask(Mask other)
        {
            return parent.x + _x + _width > other.parent.x - other.parent.originX
				&& parent.y + _y + _height > other.parent.y - other.parent.originY
				&& parent.x + _x < other.parent.x - other.parent.originX + other.parent.width
				&& parent.y + _y < other.parent.y - other.parent.originY + other.parent.height;
        }

        /// <summary>
        /// Collides against a Hitbox.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private Boolean collideHitbox(Mask other)
        {
            Hitbox h = (Hitbox)other;
            return parent.x + _x + _width > other.parent.x + h._x
				&& parent.y + _y + _height > other.parent.y + h._y
				&& parent.x + _x < other.parent.x + h._x + h._width
				&& parent.y + _y < other.parent.y + h._y + h._height;
        }

        /// <summary>
        /// X offset
        /// </summary>
        public int x 
        {
            get { return _x; }
            set 
            {
                if (_x == value) { return; }
                _x = value;
                if (list != null)
                {
                    list.update();
                }
                else if (parent != null)
                {
                    update();
                }
            }
        }

        /// <summary>
        /// Y offset
        /// </summary>
        public int y 
        {
            get { return _y; }
            set 
            {
                if (_y == value) { return; }
                _y = value;
                if (list != null)
                {
                    list.update();
                }
                else if (parent != null)
                {
                    update();
                }
            }
        }

        /// <summary>
        /// Width
        /// </summary>
        public int width 
        {
            get { return _width; }
            set 
            {
                if (_width == value) { return; }
                _width = value;
                if (list != null)
                {
                    list.update();
                }
                else if (parent != null)
                {
                    update();
                }
            }
        }

        /// <summary>
        /// Height
        /// </summary>
        public int height 
        {
            get { return _height; }
            set 
            {
                if (_height == value) { return; }
                _height = value;
                if (list != null)
                {
                    list.update();
                }
                else if (parent != null)
                {
                    update();
                }
            }
        }

        /// <summary>
        /// Updates the parent's bounds for this mask.
        /// </summary>
        override public void update()
        {
            // update parent list
			if (list != null) 
			{
				list.update();
			}
			else
			{
				// update entity bounds
				parent.originX = -_x;
				parent.originY = -_y;
				parent.width = _width;
				parent.height = _height;
			}
        }

		// Hitbox information.
        internal int _width;
        internal int _height;
        internal int _x;
        internal int _y;
    }
}
