using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunkX
{
    /// <summary>
    /// Main game Entity class updated by World.
    /// </summary>
    public class Entity : Tweener
    {
        /// <summary>
        /// If the Entity should render.
        /// </summary>
        public Boolean visible = true;

        /// <summary>
        /// lets the user store additional data in their entities without needing to create inherited classes
        /// </summary>
        public Object userData = new Object();

        /// <summary>
        /// If the Entity should respond to collision checks.
        /// </summary>
        public Boolean collidable = true;

        /// <summary>
        /// X position of the Entity in the World.
        /// </summary>
        public float x = 0;

        /// <summary>
        /// Y position of the Entity in the World.
        /// </summary>
        public float y = 0;

        /// <summary>
        /// Width of the Entity's hitbox.
        /// </summary>
        public int width;

        /// <summary>
        /// Height of the Entity's hitbox.
        /// </summary>
        public int height;

        /// <summary>
        /// X origin of the Entity's hitbox.
        /// </summary>
        public int originX;

        /// <summary>
        /// Y origin of the Entity's hitbox.
        /// </summary>
        public int originY;

        // Entity information.

        internal string _class; // need to find a solution here
        internal World _world;
        internal Boolean _added;
        internal string _type;
        internal int _layer;
		internal Entity _updatePrev;
        internal Entity _updateNext;
        internal Entity _renderPrev;
        internal Entity _renderNext;
        internal Entity _typePrev;
        internal Entity _typeNext;
        internal Entity _recycleNext;

		// Collision information.
		private /*const*/ Mask HITBOX = new Mask();
        private Mask _mask;
        private float _x;
        private float _y;

		// Rendering information.
		internal Graphic _graphic;
        private Microsoft.Xna.Framework.Vector2 _point = PX.point;
        private Microsoft.Xna.Framework.Vector2 _camera = PX.point2;

        /// <summary>
        /// Constructor. Can be usd to place the Entity and assign a graphic and mask.
        /// </summary>
        /// <param name="x">X position to place the Entity.</param>
        /// <param name="y">Y position to place the Entity.</param>
        /// <param name="graphic">Graphic to assign to the Entity.</param>
        /// <param name="mask">Mask to assign to the Entity.</param>
        public Entity(float x = 0, float y = 0, Graphic graphic = null, Mask mask = null)
        {
            this.x = x;
			this.y = y;
			if (graphic != null)
            {
                this.graphic = graphic;
            }
			if (mask != null)
            {
                this.mask = mask;
            }
			HITBOX.assignTo(this);
            _class = this.GetType().FullName;
        }

        /// <summary>
        /// Override this, called when the Entity is added to a World.
        /// </summary>
        public virtual void added()
        {
        }

        /// <summary>
        /// Override this, called when the Entity is removed from a World.
        /// </summary>
        public virtual void removed()
        {
        }

        override public void update()
        {
        }

        /// <summary>
        /// Renders the Entity. If you override this for special behaviour,
        /// remember to call super.render() to render the Entity's graphic.
        /// </summary>
        public virtual void render()
        {
            if (_graphic != null && _graphic.visible)
			{
                if (_graphic.relative)
                {
                    _point.X = x;
                    _point.Y = y;
                }
                else
                {
                    _point.X = _point.Y = 0;
                }
				_camera.X = PX.camera.X;
				_camera.Y = PX.camera.Y;
				_graphic.render(_point, _camera);
			}
        }

        /// <summary>
        /// Checks for a collision against an Entity type.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="x">Virtual x position to place this Entity.</param>
        /// <param name="y">Virtual y position to place this Entity.</param>
        /// <returns>The first Entity collided with, or null if none were collided.</returns>
        public Entity collide(String type, float x, float y)
        {
            Entity e = PX._world._typeFirst[type];

			if (!collidable || e == null)
            {
                return null;
            }
			
			_x = this.x; _y = this.y;
			this.x = x; this.y = y;
			
			if (_mask == null)
			{
				while (e != null)
				{
					if (x - originX + width > e.x - e.originX
					&& y - originY + height > e.y - e.originY
					&& x - originX < e.x - e.originX + e.width
					&& y - originY < e.y - e.originY + e.height
					&& e.collidable && e != this)
					{
						if (e._mask == null || e._mask.collide(HITBOX))
						{
							this.x = _x; this.y = _y;
							return e;
						}
					}
					e = e._typeNext;
				}
				this.x = _x; this.y = _y;
				return null;
			}
			
			while (e != null)
			{
				if (x - originX + width > e.x - e.originX
				&& y - originY + height > e.y - e.originY
				&& x - originX < e.x - e.originX + e.width
				&& y - originY < e.y - e.originY + e.height
				&& e.collidable && e != this)
				{
					if (_mask.collide(e._mask != null ? e._mask : e.HITBOX))
					{
						this.x = _x; this.y = _y;
						return e;
					}
				}
				e = e._typeNext;
			}
			this.x = _x; this.y = _y;
			return null;
        }

        /// <summary>
        /// Checks for collision against multiple Entity types.
        /// </summary>
        /// <param name="types">An Array or Vector of Entity types to check for.</param>
        /// <param name="x">Virtual x position to place this Entity.</param>
        /// <param name="y">Virtual y position to place this Entity.</param>
        /// <returns>The first Entity collided with, or null if none were collided.</returns>
        public Entity collideTypes(List<string> types, float x, float y)
        {
            Entity e;
			foreach (string type in types)
			{
                e = collide(type, x, y);
                if (e != null)
                {
                    return e;
                }
			}
			return null;
        }

        /// <summary>
        /// Checks if this Entity collides with a specific Entity.
        /// </summary>
        /// <param name="e">The Entity to collide against.</param>
        /// <param name="x">Virtual x position to place this Entity.</param>
        /// <param name="y">Virtual y position to place this Entity.</param>
        /// <returns>The Entity if they overlap, or null if they don't.</returns>
        public Entity collideWith(Entity e, float x, float y)
        {
            _x = this.x; _y = this.y;
			this.x = x; this.y = y;
			
			if (x - originX + width > e.x - e.originX
			&& y - originY + height > e.y - e.originY
			&& x - originX < e.x - e.originX + e.width
			&& y - originY < e.y - e.originY + e.height
			&& collidable && e.collidable)
			{
				if (_mask == null)
				{
					if (e._mask == null || e._mask.collide(HITBOX))
					{
						this.x = _x; this.y = _y;
						return e;
					}
					this.x = _x; this.y = _y;
					return null;
				}
				if (_mask.collide(e._mask != null ? e._mask : e.HITBOX))
				{
					this.x = _x; this.y = _y;
					return e;
				}
			}
			this.x = _x; this.y = _y;
			return null;
        }

        /// <summary>
        /// Checks if this Entity overlaps the specified rectangle.
        /// </summary>
        /// <param name="x">Virtual x position to place this Entity.</param>
        /// <param name="y">Virtual y position to place this Entity.</param>
        /// <param name="rX">X position of the rectangle.</param>
        /// <param name="rY">Y position of the rectangle.</param>
        /// <param name="rWidth">Width of the rectangle.</param>
        /// <param name="rHeight">Height of the rectangle.</param>
        /// <returns>If they overlap.</returns>
        public Boolean collideRect(float x, float y, float rX, float rY, int rWidth, int rHeight)
        {
            if (x - originX + width >= rX && y - originY + height >= rY
			&& x - originX <= rX + rWidth && y - originY <= rY + rHeight)
			{
                if (_mask == null)
                {
                    return true;
                }
				_x = this.x; _y = this.y;
				this.x = x; this.y = y;
				PX.entity.x = rX;
				PX.entity.y = rY;
				PX.entity.width = rWidth;
				PX.entity.height = rHeight;
				if (_mask.collide(PX.entity.HITBOX))
				{
					this.x = _x; this.y = _y;
					return true;
				}
				this.x = _x; this.y = _y;
				return false;
			}
			return false;
        }

        /// <summary>
        /// Checks if this Entity overlaps the specified position.
        /// </summary>
        /// <param name="x">Virtual x position to place this Entity.</param>
        /// <param name="y">Virtual y position to place this Entity.</param>
        /// <param name="pX">X position.</param>
        /// <param name="pY">Y position.</param>
        /// <returns>If the Entity intersects with the position.</returns>
        public Boolean collidePoint(float x, float y, float pX, float pY)
        {
            if (pX >= x - originX && pY >= y - originY
			&& pX < x - originX + width && pY < y - originY + height)
			{
                if (_mask == null)
                {
                    return true;
                }
				_x = this.x; _y = this.y;
				this.x = x; this.y = y;
				PX.entity.x = pX;
				PX.entity.y = pY;
				PX.entity.width = 1;
				PX.entity.height = 1;
				if (_mask.collide(PX.entity.HITBOX))
				{
					this.x = _x; this.y = _y;
					return true;
				}
				this.x = _x; this.y = _y;
				return false;
			}
			return false;
        }

        /// <summary>
        /// Populates an array with all collided Entities of a type.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="x">Virtual x position to place this Entity.</param>
        /// <param name="y">Virtual y position to place this Entity.</param>
        /// <param name="array">The Array or Vector object to populate.</param>
        public void collideInto(string type, float x, float y, List<Entity> array)
        {
            Entity e = PX._world._typeFirst[type];
			if (!collidable || e == null)
            {
                return;
            }
			
			_x = this.x; _y = this.y;
			this.x = x; this.y = y;
			int n = array.Count;
			
			if (_mask == null)
			{
				while (e != null)
				{
					if (x - originX + width > e.x - e.originX
					&& y - originY + height > e.y - e.originY
					&& x - originX < e.x - e.originX + e.width
					&& y - originY < e.y - e.originY + e.height
					&& e.collidable && e != this)
					{
						if (e._mask == null || e._mask.collide(HITBOX))
                        {
                            array[n ++] = e;
                        }
					}
					e = e._typeNext;
				}
				this.x = _x; this.y = _y;
				return;
			}
			
			while (e != null)
			{
				if (x - originX + width > e.x - e.originX
				&& y - originY + height > e.y - e.originY
				&& x - originX < e.x - e.originX + e.width
				&& y - originY < e.y - e.originY + e.height
				&& e.collidable && e != this)
				{
                    if (_mask.collide(e._mask != null ? e._mask : e.HITBOX))
                    {
                        array[n++] = e;
                    }
				}
				e = e._typeNext;
			}
			this.x = _x; this.y = _y;
			return;
        }

        /// <summary>
        /// Populates an array with all collided Entities of multiple types.
        /// </summary>
        /// <param name="types">An array of Entity types to check for.</param>
        /// <param name="x">Virtual x position to place this Entity.</param>
        /// <param name="y">Virtual y position to place this Entity.</param>
        /// <param name="array">The Array or Vector object to populate.</param>
        public void collideTypesInto(List<string> types, float x, float y, List<Entity> array)
        {
            foreach (string type in types)
            {
                collideInto(type, x, y, array);
            }
        }

        /// <summary>
        /// If the Entity collides with the camera rectangle.
        /// </summary>
        public Boolean onCamera
        {
            get { return collideRect(x, y, PX.camera.X, PX.camera.Y, PX.width, PX.height); }
        }

        /// <summary>
        /// The World object this Entity has been added to.
        /// </summary>
        public World world
        {
            get { return _world; }
        }

        /// <summary>
        /// The rendering layer of this Entity. Higher layers are rendered first.
        /// </summary>
        public int layer
        {
            get { return _layer; }
            set
            {
                if (_layer == value)
                {
                    return;
                }
			    if (!_added)
			    {
				    _layer = value;
				    return;
			    }
			    _world.removeRender(this);
			    _layer = value;
			    _world.addRender(this);
            }
        }

        /// <summary>
        /// The collision type, used for collision checking.
        /// </summary>
        public string type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;
		        if (!_added)
		        {
			        _type = value;
			        return;
		        }
                if (_type != null)
                {
                    _world.removeType(this);
                }
		        _type = value;
                if (value != null)
                {
                    _world.addType(this);
                }
            }
        }

        /// <summary>
        /// An optional Mask component, used for specialized collision. If this is
        /// not assigned, collision checks will use the Entity's hitbox by default.
        /// </summary>
        public Mask mask
        {
            get { return _mask; }
            set
            {
                if (_mask == value)
                {
                    return;
                }
                if (_mask != null)
                {
                    _mask.assignTo(null);
                }
                _mask = value;
                if (value != null)
                {
                    _mask.assignTo(this);
                }
            }
        }

        /// <summary>
        /// Graphical component to render to the screen.
        /// </summary>
        public Graphic graphic
        {
            get { return _graphic; }
            set
            {
                if (_graphic == value)
                {
                    return;
                }
			    _graphic = value;
			    if (value != null && value._assign != null)
                {
                    value._assign();
                }
            }
        }

        /// <summary>
        /// Sets the Entity's hitbox properties.
        /// </summary>
        /// <param name="width">Width of the hitbox.</param>
        /// <param name="height">Height of the hitbox.</param>
        /// <param name="originX">X origin of the hitbox.</param>
        /// <param name="originY">Y origin of the hitbox.</param>
        public void setHitbox(int width = 0, int height = 0, int originX = 0, int originY = 0)
        {
            this.width = width;
			this.height = height;
			this.originX = originX;
			this.originY = originY;
        }

        /// <summary>
        /// Center's the Entity's origin (half width & height).
        /// </summary>
        public void centerOrigin()
        {
            originX = width / 2;
			originY = height / 2;
        }
        
        /// <summary>
        /// Calculates the distance from another Entity.
        /// </summary>
        /// <param name="e">The other Entity.</param>
        /// <param name="useHitboxes">If hitboxes should be used to determine the distance. If not, the Entities' x/y positions are used.</param>
        /// <returns>The distance.</returns>
        public float distanceFrom(Entity e, Boolean useHitboxes = false)
        {
            if (!useHitboxes) 
            {
                return (float)Math.Sqrt((x - e.x) * (x - e.x) + (y - e.y) * (y - e.y));
            }
			return PX.distanceRects(x - originX, y - originY, width, height, e.x - e.originX, e.y - e.originY, e.width, e.height);
        }

        /// <summary>
        /// Calculates the distance from this Entity to the point.
        /// </summary>
        /// <param name="px">X position.</param>
        /// <param name="py">Y position.</param>
        /// <param name="useHitbox">If hitboxes should be used to determine the distance. If not, the Entities' x/y positions are used.</param>
        /// <returns>The distance.</returns>
        public float distanceToPoint(float px, float py, Boolean useHitbox = false)
        {
            if (!useHitbox)
            {
                return (float)Math.Sqrt((x - px) * (x - px) + (y - py) * (y - py));
            }
			return PX.distanceRectPoint(px, py, x - originX, y - originY, width, height);
        }

        /// <summary>
        /// Calculates the distance from this Entity to the rectangle.
        /// </summary>
        /// <param name="rx">X position of the rectangle.</param>
        /// <param name="ry">Y position of the rectangle.</param>
        /// <param name="rwidth">Width of the rectangle.</param>
        /// <param name="rheight">Height of the rectangle.</param>
        /// <returns>The distance.</returns>
        public float distanceToRect(float rx, float ry, float rwidth, float rheight)
        {
            return PX.distanceRects(rx, ry, rwidth, rheight, x - originX, y - originY, width, height);
        }
		
        /// <summary>
        /// Gets the class name as a string.
        /// </summary>
        /// <returns>A string representing the class name.</returns>
        public string toString()
        {
            return this.GetType().FullName;
        }


    }
}
