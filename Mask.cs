using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PunkX.masks;

namespace PunkX
{
    /// <summary>
    /// Base class for Entity collision masks.
    /// </summary>
    public class Mask
    {
        /// <summary>
        /// The parent Entity of this mask.
        /// </summary>
        public Entity parent;

        /// <summary>
        /// The parent Masklist of the mask.
        /// </summary>
        public Masklist list;

        /// <summary>
        /// constructor
        /// </summary>
        public Mask()
        {
            _class = this.GetType().FullName;
			_check["PunkX.Mask"] = collideMask;
			_check["PunkX.masks.Masklist"] = collideMasklist;
        }

        /// <summary>
        /// Checks for collision with another Mask.
        /// </summary>
        /// <param name="mask">The other Mask to check against.</param>
        /// <returns>If the Masks overlap.</returns>
        public Boolean collide(Mask mask)
        {
            if (_check[mask._class] != null) return _check[mask._class](mask);
			if (mask._check[_class] != null) return mask._check[_class](this);
			return false;
        }

        /// <summary>
        /// Collide against an Entity.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private Boolean collideMask(Mask other)
        {
            return parent.x - parent.originX + parent.width > other.parent.x - other.parent.originX
				&& parent.y - parent.originY + parent.height > other.parent.y - other.parent.originY
				&& parent.x - parent.originX < other.parent.x - other.parent.originX + other.parent.width
				&& parent.y - parent.originY < other.parent.y - other.parent.originY + other.parent.height;
        }

        /// <summary>
        /// Collide against a Masklist.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected Boolean collideMasklist(Mask other)
        {
            Masklist m = (Masklist)other;
            return other.collide(this);
        }

        /// <summary>
        /// Assigns the mask to the parent.
        /// </summary>
        /// <param name="parent">Entity parent for the mask</param>
        public void assignTo(Entity parent)
        {
            this.parent = parent;
            if (list == null && parent != null)
            {
                update();
            }
        }

        /// <summary>
        /// Updates the parent's bounds for this mask.
        /// </summary>
        public virtual void update()
        {
        }

		// Mask information.
		private string _class; // need a fix for this
        public delegate Boolean CollisionDelegate(Mask m);

        protected Dictionary<string, CollisionDelegate> _check = new Dictionary<string, CollisionDelegate>();
    }
}
