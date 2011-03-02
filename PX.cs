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
        public const string VERSION = "0.1";
        
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
        static public Microsoft.Xna.Framework.Vector2 camera;

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
		public const double DEG = 180 / Math.PI;
        public const double RAD = Math.PI / 180;

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

        // ported functions below

        /// <summary>
        /// returns a string of the specified number padded with zeroes
        /// </summary>
        /// <param name="number">returns a string of the specified number padded with zeroes</param>
        /// <param name="width">how many zeroes</param>
        /// <returns>a string of the specified number padded with zeroes</returns>
        static public string zeroPad(int number, int width)
        {
            string s = "" + number.ToString();
            while(s.Length < width)
            {
                s = "0" + s;
            }
            return s;
        }

        /// <summary>
        /// The currently active World object. When you set this, the World is flagged
        /// to switch, but won't actually do so until the end of the current frame.
        /// </summary>
        static public World world
        {
            get { return _world; }
            set 
            {
                if (_world == value)
                {
                    return;
                }
			    _goto = value;
            }
        }

        /// <summary>
        /// Resets the camera position.
        /// </summary>
        static public void resetCamera()
        {
            camera.X = camera.Y = 0;
        }

        /// <summary>
        /// Global volume factor for all sounds, a value from 0 to 1.
        /// </summary>
        static public float volume
        {
            get { return _volume; }
            set
            {
                if (value < 0) 
                {
                    value = 0;
                }
			    if (_volume == value)
                {
                    return;
                }
			    //_soundTransform.volume = _volume = value;
			    //SoundMixer.soundTransform = _soundTransform;
            }
        }

        /// <summary>
        /// Global panning factor for all sounds, a value from -1 to 1.
        /// </summary>
        static public float pan
        {
            get { return _pan; }
            set
            {
                if (value < -1)
                {
                    value = -1;
                }
			    if (value > 1) 
                {
                    value = 1;
                }
			    if (_pan == value)
                {
                    return;
                }
			    //_soundTransform.pan = _pan = value;
			    //SoundMixer.soundTransform = _soundTransform;
            }
        }

        /// <summary>
        /// Randomly chooses and returns one of the provided values.
        /// </summary>
        /// <param name="objs">The Objects you want to randomly choose from. Can be ints, Numbers, Points, etc.</param>
        /// <returns>The Objects you want to randomly choose from. Can be ints, Numbers, Points, etc.</returns>
        static public Object choose(params Object[] objs)
        {
            Random r = new Random();
            List<Object> list = null;
            if (objs.Length == 1)
            {
                if (objs[1] is Array)
                {
                    list = new List<Object>((Object[])objs[0]);
                }
                else if (objs[0] is List<Object>)
                {
                    list = new List<Object>((List<Object>)objs[0]);
                }
            }

            if (list != null)
            {
                return list[r.Next(list.Count)];
            }

            return objs[r.Next(objs.Length)];
        }

        /// <summary>
        /// Finds the sign of the provided value.
        /// </summary>
        /// <param name="value">The Number to evaluate.</param>
        /// <returns>1 if value greater than 0, -1 if value less than 0, and 0 when value equals 0.</returns>
        static public int sign(int value)
        {
            return value < 0 ? -1 : (value > 0 ? 1 : 0);
        }

        /// <summary>
        /// Approaches the value towards the target, by the specified amount, without overshooting the target.
        /// </summary>
        /// <param name="value">The starting value.</param>
        /// <param name="target">The target that you want value to approach.</param>
        /// <param name="amount">How much you want the value to approach target by.</param>
        /// <returns>The new value.</returns>
        static public float approach(float value, float target, float amount)
        {
            return value < target ? (target < value + amount ? target : value + amount) : (target > value - amount ? target : value - amount);
        }

        /// <summary>
        /// Linear interpolation between two values.
        /// </summary>
        /// <param name="a">First value.</param>
        /// <param name="b">Second value.</param>
        /// <param name="t">Interpolation factor.</param>
        /// <returns>When t=0, returns a. When t=1, returns b. When t=0.5, will return halfway between a and b. Etc.</returns>
        static public float lerp(float a, float b, float t = 1)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// Linear interpolation between two colors.
        /// </summary>
        /// <param name="fromColor">First color.</param>
        /// <param name="toColor">Second color.</param>
        /// <param name="t">Interpolation value. Clamped to the range [0, 1].</param>
        /// <returns>ARGB component-interpolated color value.</returns>
        static public uint colorLerp(uint fromColor, uint toColor, float t = 1)
        {
            if (t <= 0) { return fromColor; }
			if (t >= 1) { return toColor; }

            uint 
                a = fromColor >> 24 & 0xFF,
                r = fromColor >> 16 & 0xFF,
                g = fromColor >> 8 & 0xFF,
                b = fromColor & 0xFF,
                dA = (toColor >> 24 & 0xFF) - a,
                dR = (toColor >> 16 & 0xFF) - r,
                dG = (toColor >> 8 & 0xFF) - g,
                dB = (toColor & 0xFF) - b;

            a += (uint)(float)(dA * t);
			r += (uint)(float)(dR * t);
			g += (uint)(float)(dG * t);
			b += (uint)(float)(dB * t);
			return a << 24 | r << 16 | g << 8 | b;
        }

        /// <summary>
        /// Steps the object towards a point.
        /// </summary>
        /// <param name="positionableObject">Object to move must implement the IPosition interface</param>
        /// <param name="x">X position to step towards.</param>
        /// <param name="y">Y position to step towards.</param>
        /// <param name="distance">The distance to step (will not overshoot target).</param>
        static public void stepTowards(glue.IPosition positionableObject, float x, float y, float distance = 1)
        {
            point.X = x - positionableObject.x;
			point.Y = y - positionableObject.y;
			if (point.Length() <= distance)
			{
				positionableObject.x = x;
				positionableObject.y = y;
				return;
			}
            
			point.Normalize();
			point *= distance;
            positionableObject.x += point.X;
			positionableObject.y += point.Y;
        }

        /// <summary>
        /// Finds the angle (in degrees) from point 1 to point 2.
        /// </summary>
        /// <param name="x1">The first x-position.</param>
        /// <param name="y1">The first y-position.</param>
        /// <param name="x2">The second x-position.</param>
        /// <param name="y2">The second y-position.</param>
        /// <returns>The second y-position.</returns>
        static public float angle(float x1, float y1, float x2, float y2)
        {
            float a = (float)(Math.Atan2(y2 - y1, x2 - x1) * DEG);
			return a < 0 ? a + 360 : a;
        }

        /// <summary>
        /// Sets the x/y values of the provided object to a vector of the specified angle and length.
        /// </summary>
        /// <param name="positionableObject">The object whose x/y properties should be set.</param>
        /// <param name="angle">The angle of the vector, in degrees.</param>
        /// <param name="length">The distance to the vector from (0, 0).</param>
        static public void angleXY(glue.IPosition positionableObject, float angle, float length = 1)
        {
            angle *= (float)RAD;
			positionableObject.x = (float)(Math.Cos(angle) * length);
			positionableObject.y = (float)(Math.Sin(angle) * length);
        }

        /// <summary>
        /// Find the distance between two points.
        /// </summary>
        /// <param name="x1">The first x-position.</param>
        /// <param name="y1">The first y-position.</param>
        /// <param name="x2">The second x-position.</param>
        /// <param name="y2">The second y-position.</param>
        /// <returns>The distance.</returns>
        static public float distance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }

        /// <summary>
        /// Find the distance between two rectangles. Will return 0 if the rectangles overlap.
        /// </summary>
        /// <param name="x1">The x-position of the first rect.</param>
        /// <param name="y1">The y-position of the first rect.</param>
        /// <param name="w1">The width of the first rect.</param>
        /// <param name="h1">The height of the first rect.</param>
        /// <param name="x2">The x-position of the second rect.</param>
        /// <param name="y2">The y-position of the second rect.</param>
        /// <param name="w2">The width of the second rect.</param>
        /// <param name="h2">The height of the second rect.</param>
        /// <returns>The distance.</returns>
        static public float distanceRects(float x1, float y1, int w1, int h1, float x2, float y2, int w2, int h2)
        {
            if (x1 < x2 + w2 && x2 < x1 + w1)
			{
				if (y1 < y2 + h2 && y2 < y1 + h1) return 0;
				if (y1 > y2) return y1 - (y2 + h2);
				return y2 - (y1 + h1);
			}
			if (y1 < y2 + h2 && y2 < y1 + h1)
			{
				if (x1 > x2) return x1 - (x2 + w2);
				return x2 - (x1 + w1);
			}
			if (x1 > x2)
			{
				if (y1 > y2) return distance(x1, y1, (x2 + w2), (y2 + h2));
				return distance(x1, y1 + h1, x2 + w2, y2);
			}
			if (y1 > y2) return distance(x1 + w1, y1, x2, y2 + h2);
			return distance(x1 + w1, y1 + h1, x2, y2);
        }

        /// <summary>
        /// Find the distance between a point and a rectangle. Returns 0 if the point is within the rectangle.
        /// </summary>
        /// <param name="px">The x-position of the point.</param>
        /// <param name="py">The y-position of the point.</param>
        /// <param name="rx">The x-position of the rect.</param>
        /// <param name="ry">The y-position of the rect.</param>
        /// <param name="rw">The width of the rect.</param>
        /// <param name="rh">The height of the rect.</param>
        /// <returns>The distance.</returns>
        static public float distanceRectPoint(float px, float py, float rx, float ry, int rw, int rh)
        {
            if (px >= rx && px <= rx + rw)
			{
				if (py >= ry && py <= ry + rh) return 0;
				if (py > ry) return py - (ry + rh);
				return ry - py;
			}
			if (py >= ry && py <= ry + rh)
			{
				if (px > rx) return px - (rx + rw);
				return rx - px;
			}
			if (px > rx)
			{
				if (py > ry) return distance(px, py, rx + rw, ry + rh);
				return distance(px, py, rx + rw, ry);
			}
			if (py > ry) return distance(px, py, rx, ry + rh);
			return distance(px, py, rx, ry);
        }

        /// <summary>
        /// Clamps the value within the minimum and maximum values.
        /// </summary>
        /// <param name="value">The Number to evaluate.</param>
        /// <param name="min">The minimum range.</param>
        /// <param name="max">The maximum range.</param>
        /// <returns>The clamped value.</returns>
        static public float clamp(float value, float min, float max)
        {
            if (max > min)
			{
				value = value < max ? value : max;
				return value > min ? value : min;
			}
			value = value < min ? value : min;
			return value > max ? value : max;
        }

        /// <summary>
        /// Transfers a value from one scale to another scale. For example, scale(.5, 0, 1, 10, 20) == 15, and scale(3, 0, 5, 100, 0) == 40.
        /// </summary>
        /// <param name="value">The value on the first scale.</param>
        /// <param name="min">The minimum range of the first scale.</param>
        /// <param name="max">The maximum range of the first scale.</param>
        /// <param name="min2">The minimum range of the second scale.</param>
        /// <param name="max2">The maximum range of the second scale.</param>
        /// <returns></returns>
        static public float scale(float value, float min, float max, float min2, float max2)
        {
            return min2 + ((value - min) / (max - min)) * (max2 - min2);
        }

        /// <summary>
        /// Transfers a value from one scale to another scale, but clamps the return value within the second scale.
        /// </summary>
        /// <param name="value">The value on the first scale.</param>
        /// <param name="min">The minimum range of the first scale.</param>
        /// <param name="max">The maximum range of the first scale.</param>
        /// <param name="min2">The minimum range of the second scale.</param>
        /// <param name="max2">The maximum range of the second scale.</param>
        /// <returns>The scaled and clamped value.</returns>
        static public float scaleClamp(float value, float min, float max, float min2, float max2)
        {
            value = min2 + ((value - min) / (max - min)) * (max2 - min2);
			if (max2 > min2)
			{
				value = value < max2 ? value : max2;
				return value > min2 ? value : min2;
			}
			value = value < min2 ? value : min2;
			return value > max2 ? value : max2;
        }

        /// <summary>
        /// The random seed used by FP's random functions.
        /// </summary>
        static public uint randomSeed
        {
            get { return _getSeed; }
            set 
            {
                _seed = (uint)clamp(value, 1, 2147483646);
			    _getSeed = _seed;
            }
        }

        /// <summary>
        /// Randomizes the random seed using Flash's Math.random() function.
        /// </summary>
        static public void randomizeSeed()
        {
            Random r = new Random();
            randomSeed = (uint)(2147483647 * r.NextDouble());
        }

        /// <summary>
        /// A pseudo-random Number produced using FP's random seed, where 0 less than or equal to Number less than 1.
        /// </summary>
        static public double random
        {
            get 
            {
                _seed = (_seed * 16807) % 2147483647;
			    return _seed / 2147483647;
            }
        }

        /// <summary>
        /// Returns a pseudo-random uint.
        /// </summary>
        /// <param name="amount">The returned value will always be within 0 and amount</param>
        /// <returns>pseudo-random uint</returns>
        static public uint rand(uint amount)
        {
            _seed = (_seed * 16807) % 2147483647;
			return (_seed / 2147483647) * amount;
        }

        /// <summary>
        /// Returns the next item after current in the list of options.
        /// </summary>
        /// <param name="current">The currently selected item (must be one of the options).</param>
        /// <param name="options">An array of all the items to cycle through.</param>
        /// <param name="loop">If true, will jump to the first item after the last item is reached.</param>
        /// <returns>The next item in the list.</returns>
        static public Object next(Object current, List<Object> options, Boolean loop = true)
        {
            if (loop)
            {
                return options[(options.IndexOf(current) + 1) % options.Count];
            }

			return options[Math.Max(options.IndexOf(current) + 1, options.Count - 1)];
        }

        /// <summary>
        /// Returns the item previous to the current in the list of options.
        /// </summary>
        /// <param name="current">The currently selected item (must be one of the options).</param>
        /// <param name="options">An array of all the items to cycle through.</param>
        /// <param name="loop">If true, will jump to the last item after the first is reached.</param>
        /// <returns>The previous item in the list.</returns>
        static public Object prev(Object current, List<Object> options, Boolean loop = true)
        {
            if (loop)
            {
                return options[((options.IndexOf(current) - 1) + options.Count) % options.Count];
            }
			return options[Math.Max(options.IndexOf(current) - 1, 0)];
        }

        /// <summary>
        /// Swaps the current item between a and b. Useful for quick state/string/value swapping.
        /// </summary>
        /// <param name="current">The currently selected item.</param>
        /// <param name="a">Item a.</param>
        /// <param name="b">Item b.</param>
        /// <returns>Returns a if current is b, and b if current is a.</returns>
        static public Object swap(Object current, Object a, Object b)
        {
            return current == a ? b : a;
        }

        /// <summary>
        /// Creates a color value by combining the chosen RGB values.
        /// </summary>
        /// <param name="r">The red value of the color, from 0 to 255.</param>
        /// <param name="g">The green value of the color, from 0 to 255.</param>
        /// <param name="b">The blue value of the color, from 0 to 255.</param>
        /// <returns>The color uint.</returns>
        static public uint getColorRGB(uint r = 0, uint g = 0, uint b = 0)
        {
            return r << 16 | g << 8 | b;
        }

        /// <summary>
        /// Creates a color value with the chosen HSV values.
        /// </summary>
        /// <param name="h">The hue of the color (from 0 to 1).</param>
        /// <param name="s">The saturation of the color (from 0 to 1).</param>
        /// <param name="v">The value of the color (from 0 to 1).</param>
        /// <returns></returns>
        static public uint getColorHSV(float h, float s, float v)
        {
            h = (uint)(h * 360);
			uint hi = (uint)Math.Floor(h / 60) % 6;
			float f = (float)(h / 60 - Math.Floor(h / 60));
			float p = (v * (1 - s));
			float q = (v * (1 - f * s));
			float t = (v * (1 - (1 - f) * s));
			switch (hi)
			{
				case 0: return (uint)(v * 255) << 16 | (uint)(t * 255) << 8 | (uint)(p * 255);
				case 1: return (uint)(q * 255) << 16 | (uint)(v * 255) << 8 | (uint)(p * 255);
				case 2: return (uint)(p * 255) << 16 | (uint)(v * 255) << 8 | (uint)(t * 255);
				case 3: return (uint)(p * 255) << 16 | (uint)(q * 255) << 8 | (uint)(v * 255);
				case 4: return (uint)(t * 255) << 16 | (uint)(p * 255) << 8 | (uint)(v * 255);
				case 5: return (uint)(v * 255) << 16 | (uint)(p * 255) << 8 | (uint)(q * 255);
				default: return 0;
			}
			//return 0;
        }

        /// <summary>
        /// Finds the red factor of a color.
        /// </summary>
        /// <param name="color">The color to evaluate.</param>
        /// <returns>A uint from 0 to 255.</returns>
        static public uint getRed(uint color)
        {
            return color >> 16 & 0xFF;
        }

        /// <summary>
        /// Finds the green factor of a color.
        /// </summary>
        /// <param name="color">The color to evaluate.</param>
        /// <returns>A uint from 0 to 255.</returns>
        static public uint getGreen(uint color)
        {
            return color >> 8 & 0xFF;
        }

        /// <summary>
        /// Finds the blue factor of a color.
        /// </summary>
        /// <param name="color">The color to evaluate.</param>
        /// <returns>A uint from 0 to 255.</returns>
        static public uint getBlue(uint color)
        {
            return color & 0xFF;
        }

        static public Object getBitmap(string source)
        {
            throw new Exception("deprecated - no embedded resources");
        }

        /// <summary>
        /// Sets a time flag.
        /// </summary>
        /// <returns>Time elapsed (in milliseconds) since the last time flag was set.</returns>
        static public uint timeFlag()
        {
            uint t = (uint)DateTime.Now.Ticks;
            uint e = t - _time;
            _time = t;
            return e;
        }

        /// <summary>
        /// The global Console object.
        /// </summary>
        static public PXConsole console
        {
            get 
            {
                if (_console == null)
                {
                    _console = new PXConsole();
                }
                return _console;
            }
        }
		
        /// <summary>
        /// Logs data to the console.
        /// </summary>
        /// <param name="data">The data parameters to log, can be variables, objects, etc. Parameters will be separated by a space (" ").</param>
        static public void log(params Object[] data)
        {
            if (_console == null)
            {
                return;
            }
            if (data.Length > 1)
            {
                int i = 0;
                string s = "";
                while(i < data.Length)
                {
                    if (i > 0)
                    {
                        s += " ";
                    }
                    s += data[i++].ToString();
                }
                _console.log(s);
            }
            else
            {
                _console.log(data[0].ToString());
            }
        }

        /// <summary>
        /// Adds properties to watch in the console's debug panel.
        /// </summary>
        /// <param name="properties">The properties (strings) to watch.</param>
        static public void watch(params string[] properties)
        {
            if (_console == null)
            {
                return;
            }
            if (properties.Length > 1)
            {
                _console.watchMany(properties);
            }
            else
            {
                _console.watch(properties[0]);
            }
        }

        static public Object getXML(string file)
        {
            throw new Exception("deprecated - see PunkX.xml.PXML class");
        }

        /// <summary>
        /// Gets an array of frame indices.
        /// </summary>
        /// <param name="from">Starting frame.</param>
        /// <param name="to">Ending frame.</param>
        /// <param name="skip">Skip amount every frame (eg. use 1 for every 2nd frame).</param>
        /// <returns>array of frame indices.</returns>
        static public List<uint> frames(uint from, uint to, uint skip = 0)
        {
            List<uint> a = new List<uint>();
            skip++;
            if (from < to)
			{
				while (from <= to)
				{
					a.Add(from);
					from += skip;
				}
			}
			else
			{
				while (from >= to)
				{
					a.Add(from);
					from -= skip;
				}
			}
			return a;
        }

        /// <summary>
        /// Shuffles the elements in the array.
        /// </summary>
        /// <param name="a">The Object to shuffle (an Array or Vector).</param>
        static public void shuffle(ref Object a)
        {
            if (a is Array)
            {
                Object[] list = (Object[])a;
                int i = list.Length;
                int j = 0;
                Object t;
                while (--i > 0)
                {
                    t = list[i];
                    list[i] = list[j = (int)PX.rand((uint)i + 1)];
                    list[j] = t;
                }
                a = list;
            }
            else if (a is List<Object>)
            {
                List<Object> list = (List<Object>)a;
                int i = list.Count;
                int j = 0;
                Object t;
                while (--i > 0)
                {
                    t = list[i];
                    list[i] = list[j = (int)PX.rand((uint)i + 1)];
                    list[j] = t;
                }
                a = list;
            }
        }

    }
}
