using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunkX
{
    /// <summary>
    /// Base class for all Tween objects, can be added to any Core-extended classes.
    /// </summary>
    public class Tween
    {
        // delegate definitions
        public delegate void CompletionDelegate();

        public delegate float EaserDelegate(float n);

        /// <summary>
        /// persistent tween type - stops when the tween is finished
        /// </summary>
        static public const uint PERSIST = 0;

        /// <summary>
        /// looping tween type - restarts immediate when the tween is finished
        /// </summary>
        static public const uint LOOPING = 1;

        /// <summary>
        /// single-use tween type - stops and removes itself from core container when the tween is finished
        /// </summary>
        static public const uint ONESHOT = 2;

        /// <summary>
        /// if the tween should update
        /// </summary>
        public Boolean active;

        /// <summary>
        /// callback function to be called when the tween has finished
        /// </summary>
        public CompletionDelegate complete;

        private uint _type;
        protected EaserDelegate _ease;
        protected float _t;
        protected float _time;
        protected uint _target;

        internal Boolean _finish;
        internal Tweener _parent;
        internal Tween _prev;
        internal Tween _next;

        /// <summary>
        /// Constructor. Specify basic information about the Tween.
        /// </summary>
        /// <param name="duration">Duration of the tween (in seconds or frames).</param>
        /// <param name="type">Tween type, one of Tween.PERSIST (default), Tween.LOOPING, or Tween.ONESHOT.</param>
        /// <param name="onCompletion">Optional callback for when the Tween completes.</param>
        /// <param name="ease">Optional easer function to apply to the Tweened value.</param>
        public Tween(uint duration, uint type = 0, CompletionDelegate onCompletion = null, EaserDelegate ease = null)
        {
            _target = duration;
            _type = type;
            complete = onCompletion;
            _ease = ease;
        }

        /// <summary>
        /// Updates the Tween, called by World.
        /// </summary>
        public void update()
        {
            _time += PX.fixedFrameRate ? 1 : PX.elapsed;
			_t = _time / _target;
			if (_ease != null && _t > 0 && _t < 1)
            {
                _t = _ease(_t);
            }
			if (_time >= _target)
			{
				_t = 1;
				_finish = true;
			}
        }

        /// <summary>
        /// Starts the Tween, or restarts it if it's currently running.
        /// </summary>
        public void start()
        {
            _time = 0;
			if (_target == 0)
			{
				active = false;
				return;
			}
			active = true;
        }

        /// <summary>
        /// Called when the Tween completes.
        /// </summary>
        internal void finish()
        {
            switch (_type)
			{
				case 0:
                {
				    _time = _target;
				    active = false;
				} break;

				case 1:
                {
				    _time %= _target;
				    _t = _time / _target;
				    if (_ease != null && _t > 0 && _t < 1)
                    {
                        _t = _ease(_t);
                    }
				    start();
				} break;

				case 2:
                {
				    _time = _target;
				    active = false;
				    _parent.removeTween(this);
				} break;
			}
			
            _finish = false;

			if (complete != null)
            {
                complete();
            }
        }

        /// <summary>
        /// The completion percentage of the Tween.
        /// </summary>
        public float percent
        {
            get { return (float)(_time / _target); }
            set { _time = (_target * value); }
        }

        /// <summary>
        /// The current time scale of the Tween (after easer has been applied).
        /// </summary>
        public float scale
        {
            get { return _t; }
        }

    }
}
