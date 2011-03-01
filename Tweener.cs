using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunkX
{
    /// <summary>
    /// Updateable Tween container.
    /// </summary>
    public class Tweener
    {
        /// <summary>
        /// If the Tweener should update.
        /// </summary>
        public Boolean active;

        /// <summary>
        /// If the Tweener should clear on removal. For Entities, this is when they are
        /// removed from a World, and for World this is when the active World is switched.
        /// </summary>
        public Boolean autoClear = false;

        /// <summary>
        /// List information.
        /// </summary>
        internal Tween _tween;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Tweener()
        {
        }

        /// <summary>
        /// Updates the Tween container.
        /// </summary>
        public void update()
        {
        }

        /// <summary>
        /// Adds a new Tween.
        /// </summary>
        /// <param name="t">The Tween to add.</param>
        /// <param name="start">If the Tween should call start() immediately.</param>
        /// <returns>The added Tween.</returns>
        public Tween addTween(Tween t, Boolean start = false)
        {
            //if (t._parent) throw new Error("Cannot add a Tween object more than once.");
			if (t._parent != null)
			{
				return null;
			}

			t._parent = this;
			t._next = _tween;
            if (_tween != null)
            {
                _tween._prev = t;
            }
			_tween = t;
            if (start)
            {
                _tween.start();
            }
			return t;
        }

        /// <summary>
        /// Removes a Tween.
        /// </summary>
        /// <param name="t">The Tween to remove.</param>
        /// <returns>The removed Tween.</returns>
        public Tween removeTween(Tween t)
        {
            if (t._parent != this)
            {
                throw new Exception("Core object does not contain Tween.");
            }

            if (t._next != null)
            {
                t._next._prev = t._prev;
            }

            if (t._prev != null)
            {
                t._prev._next = t._next;
            }
            else
            {
                _tween = t._next;
            }

			t._next = t._prev = null;
			t._parent = null;
			t.active = false;
			return t;
        }
       
        /// <summary>
        /// Removes all Tweens.
        /// </summary>
        public void clearTweens()
        {
            Tween t = _tween;
            Tween n;
            while(t != null)
            {
                n = t._next;
                removeTween(t);
                t = n;
            }

        }

        /// <summary>
        /// Updates all contained tweens.
        /// </summary>
        public void updateTweens()
        {
            Tween t = _tween;
			while (t != null)
			{
				if (t.active)
				{
					t.update();
                    if (t._finish)
                    {
                        t.finish();
                    }
				}
				t = t._next;
			}
        }
    }
}
