using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PunkX.utils;
namespace PunkX
{
    /// <summary>
    /// Main game class, Manages the game loop.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// If the game should stop updating/rendering.
        /// </summary>
        public Boolean paused;

		// Timing information.
		private uint _delta = 0;
        private uint _time;
        private uint _last;
        private Timer _timer;
        private uint _rate;
        private uint _skip;
        private uint _prev;

		// Debug timing information.
        private uint _updateTime;
        private uint _renderTime;
        private uint _gameTime;
        private uint _flashTime;

		// Game constants.
		private const float MAX_ELAPSED = 0.0333f;
        private const uint MAX_FRAMESKIP = 5;
        private const uint TICK_RATE = 4;

		// FrameRate tracking.
		private uint _frameLast = 0;
        private uint _frameListSum = 0;
        private List<uint> _frameList = new List<uint>();
        
        /// <summary>
        /// Constructor. Defines startup information about your game.
        /// </summary>
        /// <param name="width">The width of your game.</param>
        /// <param name="height">The height of your game.</param>
        /// <param name="frameRate">The game framerate, in frames per second.</param>
        /// <param name="fixedFrameRate">If a fixed-framerate should be used.</param>
        public Engine(int width, int height, int frameRate = 60, Boolean fixedFrameRate = false)
        {
            // global game properties
			PX.width = width;
			PX.height = height;
			PX.assignedFrameRate = frameRate;
			PX.fixedFrameRate = fixedFrameRate;
			
			// global game objects
			PX.engine = this;
			PX.screen = new Screen();
			PX.bounds = new Microsoft.Xna.Framework.Rectangle(0, 0, width, height);
            PX._world = new World();
			
			// miscellaneous startup stuff
            /*
			if (PX.randomSeed == 0) 
            {
                PX.randomizeSeed();
            }
             * */
			PX.entity = new Entity();
            PX._time = 0;//getTimer();
			
			// on-stage event listener
			//addEventListener(Event.ADDED_TO_STAGE, onStage);
            onStage();
        }

        /// <summary>
        /// override this method - called by Engine before main loop starts
        /// </summary>
        public virtual void init()
        {
        }

        /// <summary>
        /// updates the game, updating the World and Entities
        /// </summary>
        public void update()
        {
            if (PX._world.active)
            {
                if (PX._world._tween != null)
                {
                    PX._world.updateTweens();
                }
                PX._world.update();
            }
            PX._world.updateLists();
            if (PX._goto != null)
            {
                checkWorld();
            }
        }

        /// <summary>
        /// Renders the game, rendering the World and Entities.
        /// </summary>
        public void render()
        {
            // timing stuff
            long t = DateTime.Now.Ticks;
            
			if (0 == _frameLast)
            {
                _frameLast = (uint)t;
            }
			
			// render loop
			PX.screen.swap();
			//Draw.resetTarget();
			PX.screen.refresh();
            if (PX._world.visible)
            {
                PX._world.render();
            }
			PX.screen.redraw();
			
			// more timing stuff
            t = DateTime.Now.Ticks;
			_frameListSum += (_frameList[_frameList.Count] = (uint)t - _frameLast);

            if (_frameList.Count > 10)
            {
                uint firstFrame = _frameList[0];
                _frameList.RemoveAt(0);
                _frameListSum -= firstFrame;
            }

			PX.frameRate = (int)(1000 / (_frameListSum / (uint)_frameList.Count));
			_frameLast = (uint)t;
        }

        /// <summary>
        /// Sets the game's stage properties. Override this to set them differently.
        /// </summary>
        public void setStageProperties()
        {
            /*
             stage.frameRate = FP.assignedFrameRate;
			stage.align = StageAlign.TOP_LEFT;
			stage.quality = StageQuality.HIGH;
			stage.scaleMode = StageScaleMode.NO_SCALE;
			stage.displayState = StageDisplayState.NORMAL;*/
        }

        /// <summary>
        /// Event handler for stage entry.
        /// </summary>
        private void onStage()
        {
            // remove event listener
			//removeEventListener(Event.ADDED_TO_STAGE, onStage);
			
			// set stage properties
			//PX.stage = stage;
			setStageProperties();
			
			// enable input
			Input.enable();
			
			// switch worlds
			if (PX._goto != null) 
            {
                checkWorld();
            }
			
			// game start
			init();
			
			// start game loop
			_rate = (uint)(1000 / PX.assignedFrameRate);
			if (PX.fixedFrameRate)
			{
				// fixed framerate
				_skip = _rate * MAX_FRAMESKIP;
                _last = _prev = (uint)DateTime.Now.Ticks;//getTimer();
				//_timer = new Timer(TICK_RATE);
				//_timer.addEventListener(TimerEvent.TIMER, onTimer);
				//_timer.start();
			}
			else
			{
				// nonfixed framerate
                _last = (uint)DateTime.Now.Ticks;//getTimer();
				//addEventListener(Event.ENTER_FRAME, onEnterFrame);
			}
        }

        /// <summary>
        /// Framerate independent game loop.
        /// </summary>
        private void onEnterFrame()
        {
            // update timer
            _time = _gameTime = (uint)DateTime.Now.Ticks;//getTimer();
			PX._flashTime = _time - _flashTime;
			_updateTime = _time;
			PX.elapsed = (_time - _last) / 1000;
            if (PX.elapsed > MAX_ELAPSED)
            {
                PX.elapsed = MAX_ELAPSED;
            }
			PX.elapsed *= PX.rate;
			_last = _time;
			
			// update console
            if (PX._console != null)
            {
                PX._console.update();
            }
			
			// update loop
			if (!paused) update();
			
			// update input
			Input.update();
			
			// update timer
            _time = _renderTime = (uint)DateTime.Now.Ticks;//getTimer();
			PX._updateTime = _time - _updateTime;
			
			// render loop
            if (!paused)
            {
                render();
            }
			
			// update timer
            _time = _flashTime = (uint)DateTime.Now.Ticks;//getTimer();
			PX._renderTime = _time - _renderTime;
			PX._gameTime = _time - _gameTime;
        }

        /// <summary>
        /// Fixed framerate game loop.
        /// </summary>
        private void onTimer()
        {
            // update timer
            _time = (uint)DateTime.Now.Ticks;//getTimer();
			_delta += (_time - _last);
			_last = _time;
			
			// quit if a frame hasn't passed
            if (_delta < _rate)
            {
                return;
            }
			
			// update timer
			_gameTime = _time;
			PX._flashTime = _time - _flashTime;
			
			// update console
            if (PX._console != null)
            {
                PX._console.update();
            }
			
			// update loop
			if (_delta > _skip) _delta = _skip;
			while (_delta > _rate)
			{
				// update timer
				_updateTime = _time;
				_delta -= _rate;
				PX.elapsed = (_time - _prev) / 1000;
                if (PX.elapsed > MAX_ELAPSED)
                {
                    PX.elapsed = MAX_ELAPSED;
                }
				PX.elapsed *= PX.rate;
				_prev = _time;
				
				// update loop
                if (!paused)
                {
                    update();
                }
				
				// update input
				Input.update();
				
				// update timer
                _time = (uint)DateTime.Now.Ticks;//getTimer();
				PX._updateTime = _time - _updateTime;
			}
			
			// update timer
			_renderTime = _time;
			
			// render loop
            if (!paused)
            {
                render();
            }
			
			// update timer
            _time = _flashTime = (uint)DateTime.Now.Ticks;//getTimer();
			PX._renderTime = _time - _renderTime;
			PX._gameTime =  _time - _gameTime;
        }

        /// <summary>
        /// Switch Worlds if they've changed.
        /// </summary>
        private void checkWorld()
        {
            if (PX._goto == null)
            {
                return;
            }

			PX._world.end();
			PX._world.updateLists();
            if (PX._world != null && PX._world.autoClear && PX._world._tween != null)
            {
                PX._world.clearTweens();
            }
			PX._world = PX._goto;
			PX._goto = null;
			PX.camera = PX._world.camera;
			PX._world.updateLists();
			PX._world.begin();
			PX._world.updateLists();
        }


    }
}
