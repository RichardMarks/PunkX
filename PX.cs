using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PunkX.debug;
namespace PunkX
{
    public class PX
    {
        /// <summary>
        /// major version of PunkX
        /// </summary>
        static public const string VERSION = "0.1";
        
        /// <summary>
        /// width of the game
        /// </summary>
        static public int width;

        /// <summary>
        /// height of the game
        /// </summary>
        static public int height;

        /// <summary>
        /// if the game is running at a fixed framerate
        /// </summary>
        static public Boolean fixedFrameRate;

        /// <summary>
        /// the current framerate
        /// </summary>
        static public int frameRate;

        /// <summary>
        /// the framerate assigned to the stage
        /// </summary>
        static public int assignedFrameRate;

        /// <summary>
        /// milliseconds elapsed since the last frame (non-fixed framerate only)
        /// </summary>
        static public float elapsed;

        /// <summary>
        /// time scale applied to PX.elapsed (non-fixed framerate only)
        /// </summary>
        static public int rate;

        /// <summary>
        /// the screen object, use to transform or offset the screen
        /// </summary>
        static public Screen screen;

        /// <summary>
        /// the current screen buffer that the scene is being rendered to
        /// </summary>
        static public Microsoft.Xna.Framework.Graphics.Texture2D buffer;

        /// <summary>
        /// rectangle representing the size of the screen
        /// </summary>
        static public Microsoft.Xna.Framework.Rectangle bounds;

        /// <summary>
        /// points used to determine drawing offset in the render loop
        /// </summary>
        static public Microsoft.Xna.Framework.Point camera;

        /// <summary>
        /// reference to the game engine
        /// </summary>
        static public Engine engine;


        // World information.
        static internal World _world;
        static internal World _goto;

		// Console information.
		static internal PXConsole _console;

		// Time information.
		static internal uint _time;
        static public uint _updateTime;
        static public uint _renderTime;
        static public uint _gameTime;
        static public uint _flashTime;

		// Bitmap storage.
        static private List<Microsoft.Xna.Framework.Graphics.Texture> _bitmap;

		// Pseudo-random number generation (the seed is set in Engine's contructor).
		static private uint _seed = 0;
        static private uint _getSeed;

		// Volume control.
		static private int _volume;
        static private int _pan;
        //static private SoundTransform _soundTransform;
        
		// Used for rad-to-deg and deg-to-rad conversion.
		static public const double DEG = 180 / Math.PI;
        static public const double RAD = Math.PI / 180;

		// Global Flash objects.
		//static public Stage stage;

		// Global objects used for rendering, collision, etc.
        static public Microsoft.Xna.Framework.Vector2 point = new Microsoft.Xna.Framework.Vector2();
        static public Microsoft.Xna.Framework.Vector2 point2 = new Microsoft.Xna.Framework.Vector2();
        static public Microsoft.Xna.Framework.Vector2 zero = new Microsoft.Xna.Framework.Vector2();
        static public Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle();
        static public Microsoft.Xna.Framework.Matrix matrix = new Microsoft.Xna.Framework.Matrix();
        //static public Sprite sprite = new Sprite();
        static public Entity entity;


        static public Object CreateInstance(string name)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            string path = assembly.GetName().ToString();
            path = path.Substring(0, path.IndexOf(","));
            return Activator.CreateInstance(assembly.GetType(path + name));
        }

    }
}
