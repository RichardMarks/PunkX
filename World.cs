using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunkX
{
    public class World : Tweener
    {
        /// <summary>
        /// If the render() loop is performed.
        /// </summary>
        public Boolean visible = true;

        /// <summary>
        /// Point used to determine drawing offset in the render loop.
        /// </summary>
        public Microsoft.Xna.Framework.Point camera = new Microsoft.Xna.Framework.Point();

        // Adding and removal.
        private List<Entity> _add = new List<Entity>();
        private List<Entity> _remove = new List<Entity>();

		// Update information.
		private Entity _updateFirst;
        private uint _count;

		// Render information.
		private List<Entity> _renderFirst = new List<Entity>();
        private List<Entity> _renderLast = new List<Entity>();
        private List<int> _layerList = new List<int>();
        private List<uint> _layerCount = new List<uint>();
        private Boolean _layerSort;
        
        private Dictionary<string, uint> _classCount = new Dictionary<string, uint>();
        internal Dictionary<string, Entity> _typeFirst = new Dictionary<string, Entity>();
        private Dictionary<string, uint> _typeCount = new Dictionary<string, uint>();
        private Dictionary<string, Entity> _recycled = new Dictionary<string, Entity>();
        
        /// <summary>
        /// constructor
        /// </summary>
        public World()
        {
        }

        /// <summary>
        /// Override this; called when World is switch to, and set to the currently active world.
        /// </summary>
        public void begin()
        {
        }

        /// <summary>
        /// Override this; called when World is changed, and the active world is no longer this.
        /// </summary>
        public void end()
        {
        }

        /// <summary>
        /// Performed by the game loop, updates all contained Entities.
        /// If you override this to give your World update code, remember
        /// to call super.update() or your Entities will not be updated.
        /// </summary>
        override public void update()
        {
            // update the entities
            Entity e = _updateFirst;
			while (e != null)
			{
				if (e.active)
				{
                    if (e._tween != null)
                    {
                        e.updateTweens();
                    }
					e.update();
				}
                if (e._graphic != null && e._graphic.active)
                {
                    e._graphic.update();
                }
				e = e._updateNext;
			}
        }

        /// <summary>
        /// Performed by the game loop, renders all contained Entities.
        /// If you override this to give your World render code, remember
        /// to call super.render() or your Entities will not be rendered.
        /// </summary>
        public void render()
        {
            // render the entities in order of depth
            Entity e;
            int i = _layerList.Count;
			while (i-- > 0)
			{
				e = _renderLast[_layerList[i]];
				while (e != null)
				{
                    if (e.visible)
                    {
                        e.render();
                    }
					e = e._renderPrev;
				}
			}
        }

        /// <summary>
        /// X position of the mouse in the World.
        /// </summary>
        public int mouseX
        {
            get { return PX.screen.mouseX + PX.camera.X; }
        }

        /// <summary>
        /// Y position of the mouse in the world.
        /// </summary>
        public int mouseY
        {
            get { return PX.screen.mouseY + PX.camera.Y; }
        }
		
        /// <summary>
        /// Adds the Entity to the World at the end of the frame.
        /// </summary>
        /// <param name="e">Entity object you want to add.</param>
        /// <returns>The added Entity object.</returns>
        public Entity add(Entity e)
        {
            if (e._world != null)
            {
                return e;
            }
			_add[_add.Count] = e;
			e._world = this;
			return e;
        }

        /// <summary>
        /// Removes the Entity from the World at the end of the frame.
        /// </summary>
        /// <param name="e">Entity object you want to remove.</param>
        /// <returns>The removed Entity object.</returns>
        public Entity remove(Entity e)
        {
            if (e._world != this)
            {
                return e;
            }
			_remove[_remove.Count] = e;
			e._world = null;
			return e;
        }
		
        /// <summary>
        /// Removes all Entities from the World at the end of the frame.
        /// </summary>
        public void removeAll()
        {
            Entity e = _updateFirst;
			while (e != null)
			{
				_remove[_remove.Count] = e;
				e._world = null;
				e = e._updateNext;
			}
        }

        /// <summary>
        /// Adds multiple Entities to the world.
        /// </summary>
        /// <param name="list">Several Entities (as arguments) or an Array/Vector of Entities.</param>
        public void addList(params Object[] list)
        {
            if (list[0] is Array)
			{
                Entity[] subList = (Entity[])list[0];
                for (int i = 0; i < subList.Length; i++)
                {
                    add((Entity)subList[i]);
                }
                return;
            }
			else if (list[0] is List<Entity>)
            {
                List<Entity> subList = (List<Entity>)list[0];
                for (int i = 0; i < subList.Count; i++)
                {
                    add((Entity)(subList[i]));
                }
                return;
            }

            for (int i = 0; i < list.Length; i++)
            {
                add((Entity)list[i]);
            }
        }

        /// <summary>
        /// Removes multiple Entities from the world.
        /// </summary>
        /// <param name="list">Several Entities (as arguments) or an Array/Vector of Entities.</param>
        public void removeList(params Object[] list)
        {
            if (list[0] is Array)
			{
                Entity[] subList = (Entity[])list[0];
                for (int i = 0; i < subList.Length; i++)
                {
                    remove((Entity)subList[i]);
                }
                return;
            }
			else if (list[0] is List<Entity>)
            {
                List<Entity> subList = (List<Entity>)list[0];
                for (int i = 0; i < subList.Count; i++)
                {
                    remove((Entity)(subList[i]));
                }
                return;
            }

            for (int i = 0; i < list.Length; i++)
            {
                remove((Entity)list[i]);
            }
        }

        /// <summary>
        /// Adds an Entity to the World with the Graphic object.
        /// </summary>
        /// <param name="graphic">Graphic to assign the Entity.</param>
        /// <param name="layer">Layer of the Entity.</param>
        /// <param name="x">X position of the Entity.</param>
        /// <param name="y">Y position of the Entity.</param>
        /// <returns>The Entity that was added.</returns>
        public Entity addGraphic(Graphic graphic, int layer = 0, float x = 0, float y = 0)
        {
            Entity e = new Entity(x, y, graphic);
			if (layer != 0)
            {
                e.layer = layer;
            }
			e.active = false;
			return add(e);
        }

        /// <summary>
        /// Adds an Entity to the World with the Mask object.
        /// </summary>
        /// <param name="mask">Mask to assign the Entity.</param>
        /// <param name="type">Collision type of the Entity.</param>
        /// <param name="x">X position of the Entity.</param>
        /// <param name="y">Y position of the Entity.</param>
        /// <returns>The Entity that was added.</returns>
        public Entity addMask(Mask mask, string type, float x = 0, float y = 0)
        {
            Entity e = new Entity(x, y, null, mask);
			if (type != null)
            {
                e.type = type;
            }
			e.active = e.visible = false;
			return add(e);
        }

        /// <summary>
        /// Returns a new Entity, or a stored recycled Entity if one exists.
        /// </summary>
        /// <param name="classType">The Class of the Entity you want to add.</param>
        /// <param name="addToWorld">Add it to the World immediately.</param>
        /// <returns>The new Entity object.</returns>
        public Entity create(string classType, Boolean addToWorld = true)
        {
            Entity e = _recycled[classType];
			if (e != null)
			{
				_recycled[classType] = e._recycleNext;
				e._recycleNext = null;
			}
			else 
            {
                e = (Entity)PX.CreateInstance(classType);//new classType;
            }
			if (addToWorld)
            {
                return add(e);
            }
			return e;
        }

        /// <summary>
        /// Removes the Entity from the World at the end of the frame and recycles it.
        /// The recycled Entity can then be fetched again by calling the create() function.
        /// </summary>
        /// <param name="e">The Entity to recycle.</param>
        /// <returns>The recycled Entity.</returns>
        public Entity recycle(Entity e)
        {
            if (e._world != this)
            {
                return e;
            }
			e._recycleNext = _recycled[e._class];
			_recycled[e._class] = e;
			return remove(e);
        }

        /// <summary>
        /// Clears stored reycled Entities of the Class type.
        /// </summary>
        /// <param name="classType">The Class type to clear.</param>
        public void clearRecycled(string classType)
        {
            Entity e = _recycled[classType];
			Entity n;
			while (e != null)
			{
				n = e._recycleNext;
				e._recycleNext = null;
				e = n;
			}
            _recycled.Remove(classType);
			//delete _recycled[classType];
        }

        /// <summary>
        /// Clears stored recycled Entities of all Class types.
        /// </summary>
        public void clearRecycledAll()
        {
            foreach (KeyValuePair<string, Entity> pair in _recycled)
            {
                clearRecycled(pair.Key);
            }
        }

        /// <summary>
        /// Brings the Entity to the front of its contained layer.
        /// </summary>
        /// <param name="e">The Entity to shift.</param>
        /// <returns>If the Entity changed position.</returns>
        public Boolean bringToFront(Entity e)
        {
            if (e._world != this || e._renderPrev == null)
            {
                return false;
            }

			// pull from list
			e._renderPrev._renderNext = e._renderNext;
			if (e._renderNext != null)
            {
                e._renderNext._renderPrev = e._renderPrev;
            }
			else
            {
                _renderLast[e._layer] = e._renderPrev;
            }

			// place at the start
			e._renderNext = _renderFirst[e._layer];
			e._renderNext._renderPrev = e;
			_renderFirst[e._layer] = e;
			e._renderPrev = null;
			return true;
        }

        /// <summary>
        /// Sends the Entity to the back of its contained layer.
        /// </summary>
        /// <param name="e">The Entity to shift.</param>
        /// <returns>If the Entity changed position.</returns>
        public Boolean sendToBack(Entity e)
        {
            if (e._world != this || e._renderNext == null)
            {
                return false;
            }

			// pull from list
			e._renderNext._renderPrev = e._renderPrev;
			if (e._renderPrev != null)
            {
                e._renderPrev._renderNext = e._renderNext;
            }
			else
            {
                _renderFirst[e._layer] = e._renderNext;
            }

			// place at the end
			e._renderPrev = _renderLast[e._layer];
			e._renderPrev._renderNext = e;
			_renderLast[e._layer] = e;
			e._renderNext = null;
			return true;
        }
		
        /// <summary>
        /// Shifts the Entity one place towards the front of its contained layer.
        /// </summary>
        /// <param name="e">The Entity to shift.</param>
        /// <returns>If the Entity changed position.</returns>
        public Boolean bringForward(Entity e)
        {
            if (e._world != this || e._renderPrev == null)
            {
                return false;
            }

			// pull from list
			e._renderPrev._renderNext = e._renderNext;
			if (e._renderNext != null)
            {
                e._renderNext._renderPrev = e._renderPrev;
            }
			else
            {
                _renderLast[e._layer] = e._renderPrev;
            }

			// shift towards the front
			e._renderNext = e._renderPrev;
			e._renderPrev = e._renderPrev._renderPrev;
			e._renderNext._renderPrev = e;
			if (e._renderPrev != null)
            {
                e._renderPrev._renderNext = e;
            }
			else
            {
                _renderFirst[e._layer] = e;
            }
			return true;
        }

        /// <summary>
        /// Shifts the Entity one place towards the back of its contained layer.
        /// </summary>
        /// <param name="e">The Entity to shift.</param>
        /// <returns>If the Entity changed position.</returns>
        public Boolean sendBackward(Entity e)
        {
            if (e._world != this || e._renderNext == null)
            {
                return false;
            }

			// pull from list
			e._renderNext._renderPrev = e._renderPrev;
			if (e._renderPrev != null)
            {
                e._renderPrev._renderNext = e._renderNext;
            }
			else
            {
                _renderFirst[e._layer] = e._renderNext;
            }

			// shift towards the back
			e._renderPrev = e._renderNext;
			e._renderNext = e._renderNext._renderNext;
			e._renderPrev._renderNext = e;
			if (e._renderNext != null)
            {
                e._renderNext._renderPrev = e;
            }
			else
            {
                _renderLast[e._layer] = e;
            }
			return true;
        }

        /// <summary>
        /// If the Entity as at the front of its layer.
        /// </summary>
        /// <param name="e">The Entity to check.</param>
        /// <returns>True or false.</returns>
        public Boolean isAtFront(Entity e)
        {
            return e._renderPrev == null;
        }

        /// <summary>
        /// If the Entity as at the back of its layer.
        /// </summary>
        /// <param name="e">The Entity to check.</param>
        /// <returns>True or false.</returns>
        public Boolean isAtBack(Entity e)
        {
            return e._renderNext == null;
        }

        /// <summary>
        /// Returns the first Entity that collides with the rectangular area.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="rX">X position of the rectangle.</param>
        /// <param name="rY">Y position of the rectangle.</param>
        /// <param name="rWidth">Width of the rectangle.</param>
        /// <param name="rHeight">Height of the rectangle.</param>
        /// <returns>The first Entity to collide, or null if none collide.</returns>
        public Entity collideRect(string type, float rX, float rY, int rWidth, int rHeight)
        {
            Entity e = _typeFirst[type];
			while (e != null)
			{
				if (e.collideRect(e.x, e.y, rX, rY, rWidth, rHeight)) return e;
				e = e._typeNext;
			}
			return null;
        }

        /// <summary>
        /// Returns the first Entity found that collides with the position.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="pX">X position.</param>
        /// <param name="pY">Y position.</param>
        /// <returns>The collided Entity, or null if none collide.</returns>
        public Entity collidePoint(string type, float pX, float pY)
        {
            Entity e = _typeFirst[type];
			while (e != null)
			{
				if (e.collidePoint(e.x, e.y, pX, pY)) return e;
				e = e._typeNext;
			}
			return null;
        }

        /// <summary>
        /// Returns the first Entity found that collides with the line.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="fromX">Start x of the line.</param>
        /// <param name="fromY">Start y of the line.</param>
        /// <param name="toX">End x of the line.</param>
        /// <param name="toY">End y of the line.</param>
        /// <param name="precision">fixme</param>
        /// <param name="p">fixme</param>
        /// <returns></returns>
        public Entity collideLine(string type, int fromX, int fromY, int toX, int toY, int precision = 1, PunkX.glue.PXPoint p = null)
        {
            // If the distance is less than precision, do the short sweep.
            precision = (precision < 1) ? 1 : precision;
			
			if (PX.distance(fromX, fromY, toX, toY) < precision)
			{
				if (p != null)
				{
					if (fromX == toX && fromY == toY)
					{
						p.X = toX; p.Y = toY;
						return collidePoint(type, toX, toY);
					}
					return collideLine(type, fromX, fromY, toX, toY, 1, p);
				}
				else 
                {
                    return collidePoint(type, fromX, toY);
                }
			}
			
			// Get information about the line we're about to raycast.
			int xDelta = Math.Abs(toX - fromX),
				yDelta = Math.Abs(toY - fromY),
				xSign = toX > fromX ? precision : -precision,
				ySign = toY > fromY ? precision : -precision,
                x = fromX, y = fromY; 
            Entity e;
			
			// Do a raycast from the start to the end point.
			if (xDelta > yDelta)
			{
				ySign *= yDelta / xDelta;
				if (xSign > 0)
				{
					while (x < toX)
					{
						if ((e = collidePoint(type, x, y)) != null)
						{
							if (p == null) 
                            {
                                return e;
                            }
							if (precision < 2)
							{
								p.X = x - xSign; p.Y = y - ySign;
								return e;
							}
							return collideLine(type, x - xSign, y - ySign, toX, toY, 1, p);
						}
						x += xSign; y += ySign;
					}
				}
				else
				{
					while (x > toX)
					{
						if ((e = collidePoint(type, x, y)) != null)
						{
							if (p == null)
                            {
                                return e;
                            }
							if (precision < 2)
							{
								p.X = x - xSign; p.Y = y - ySign;
								return e;
							}
							return collideLine(type, x - xSign, y - ySign, toX, toY, 1, p);
						}
						x += xSign; y += ySign;
					}
				}
			}
			else
			{
				xSign *= xDelta / yDelta;
				if (ySign > 0)
				{
					while (y < toY)
					{
						if ((e = collidePoint(type, x, y)) != null)
						{
							if (p == null)
                            {
                                return e;
                            }
							if (precision < 2)
							{
								p.X = x - xSign; p.Y = y - ySign;
								return e;
							}
							return collideLine(type, x - xSign, y - ySign, toX, toY, 1, p);
						}
						x += xSign; y += ySign;
					}
				}
				else
				{
					while (y > toY)
					{
						if ((e = collidePoint(type, x, y)) != null)
						{
							if (p == null)
                            {
                                return e;
                            }
							if (precision < 2)
							{
								p.X = x - xSign; p.Y = y - ySign;
								return e;
							}
							return collideLine(type, x - xSign, y - ySign, toX, toY, 1, p);
						}
						x += xSign; y += ySign;
					}
				}
			}
			
			// Check the last position.
			if (precision > 1)
			{
				if (p == null)
                {
                    return collidePoint(type, toX, toY);
                }
				if (collidePoint(type, toX, toY) != null) 
                {
                    return collideLine(type, x - xSign, y - ySign, toX, toY, 1, p);
                }
			}
			
			// No collision, return the end point.
			if (p != null)
			{
				p.X = toX;
				p.Y = toY;
			}
			return null;
        }

        /// <summary>
        /// Populates an array with all Entities that collide with the rectangle. This
        /// function does not empty the array, that responsibility is left to the user.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="rX">X position of the rectangle.</param>
        /// <param name="rY">Y position of the rectangle.</param>
        /// <param name="rWidth">Width of the rectangle.</param>
        /// <param name="rHeight">Height of the rectangle.</param>
        /// <param name="into">The Array or Vector to populate with collided Entities.</param>
        public void collideRectInto(string type, float rX, float rY, int rWidth, int rHeight, ref Object into)
        {
            uint n = 0;
            if (into is Array)
            {
                Entity[] list = (Entity[])into;
                n = (uint)list.Length;
                Entity e = _typeFirst[type];
                while (e != null)
                {
                    if (e.collideRect(e.x, e.y, rX, rY, rWidth, rHeight))
                    {
                        list[n++] = e;
                    }
				    e = e._typeNext;
                }
                into = list;
            }
            else if (into is List<Entity>)
            {
                List<Entity> list = (List<Entity>)into;
                n = (uint)list.Count;
                Entity e = _typeFirst[type];
                while (e != null)
                {
                    if (e.collideRect(e.x, e.y, rX, rY, rWidth, rHeight))
                    {
                        list.Add(e);// [n++] = e;
                    }
				    e = e._typeNext;
                }
                into = list;
            }
            else
            {
                throw new Exception("World::collideRectInto() Usage Error: \"into\" parameter must be List<Entity> or Array");
            }
        }

        /// <summary>
        /// Populates an array with all Entities that collide with the position. This
        /// function does not empty the array, that responsibility is left to the user.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="pX">X position.</param>
        /// <param name="pY">Y position.</param>
        /// <param name="into">The Array or Vector to populate with collided Entities.</param>
        public void collidePointInto(string type, float pX, float pY, ref Object into)
        {
            uint n = 0;
            if (into is Array)
            {
                Entity[] list = (Entity[])into;
                n = (uint)list.Length;
                Entity e = _typeFirst[type];
                while (e != null)
                {
                    if (e.collidePoint(e.x, e.y, pX, pY))
                    {
                        list[n++] = e;
                    }
				    e = e._typeNext;
                }
                into = list;
            }
            else if (into is List<Entity>)
            {
                List<Entity> list = (List<Entity>)into;
                n = (uint)list.Count;
                Entity e = _typeFirst[type];
                while (e != null)
                {
                    if (e.collidePoint(e.x, e.y, pX, pY))
                    {
                        list.Add(e);// [n++] = e;
                    }
				    e = e._typeNext;
                }
                into = list;
            }
            else
            {
                throw new Exception("World::collidePointInto() Usage Error: \"into\" parameter must be List<Entity> or Array");
            }
        }

        /// <summary>
        /// Finds the Entity nearest to the rectangle.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="x">X position of the rectangle.</param>
        /// <param name="y">Y position of the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        /// <returns>The nearest Entity to the rectangle.</returns>
        public Entity nearestToRect(string type, float x, float y, int width, int height)
        {
            Entity n = _typeFirst[type];
            float nearDist = float.MaxValue;
            Entity near = null;
            float dist;

			while (n != null)
			{
				dist = squareRects(x, y, width, height, n.x - n.originX, n.y - n.originY, n.width, n.height);
				if (dist < nearDist)
				{
					nearDist = dist;
					near = n;
				}
				n = n._typeNext;
			}
			return near;
        }

        /// <summary>
        /// Finds the Entity nearest to another.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="e">The Entity to find the nearest to.</param>
        /// <param name="useHitboxes">If the Entities' hitboxes should be used to determine the distance. If false, their x/y coordinates are used.</param>
        /// <returns>The nearest Entity to e.</returns>
        public Entity nearestToEntity(string type, Entity e, Boolean useHitboxes = false)
        {
            if (useHitboxes)
            {
                return nearestToRect(type, e.x - e.originX, e.y - e.originY, e.width, e.height);
            }

			Entity n = _typeFirst[type];
			float nearDist = float.MaxValue;
			Entity near = null;
            float dist;
			float x = e.x - e.originX;
			float y = e.y - e.originY;
			while (n != null)
			{
				dist = (x - n.x) * (x - n.x) + (y - n.y) * (y - n.y);
				if (dist < nearDist)
				{
					nearDist = dist;
					near = n;
				}
				n = n._typeNext;
			}
			return near;
        }

        /// <summary>
        /// Finds the Entity nearest to the position.
        /// </summary>
        /// <param name="type">The Entity type to check for.</param>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="useHitboxes">If the Entities' hitboxes should be used to determine the distance. If false, their x/y coordinates are used.</param>
        /// <returns>The nearest Entity to the position.</returns>
        public Entity nearestToPoint(string type, float x, float y, Boolean useHitboxes = false)
        {
            Entity n = _typeFirst[type];
            float nearDist = float.MaxValue;
            Entity near = null;
            float dist;
			if (useHitboxes)
			{
				while (n != null)
				{
					dist = squarePointRect(x, y, n.x - n.originX, n.y - n.originY, n.width, n.height);
					if (dist < nearDist)
					{
						nearDist = dist;
						near = n;
					}
					n = n._typeNext;
				}
				return near;
			}
			while (n != null)
			{
				dist = (x - n.x) * (x - n.x) + (y - n.y) * (y - n.y);
				if (dist < nearDist)
				{
					nearDist = dist;
					near = n;
				}
				n = n._typeNext;
			}
			return near;
        }

        /// <summary>
        /// How many Entities are in the World.
        /// </summary>
        public uint count
        {
            get { return _count; }
        }

        /// <summary>
        /// Returns the amount of Entities of the type are in the World.
        /// </summary>
        /// <param name="type">The type (or Class type) to count.</param>
        /// <returns>How many Entities of type exist in the World.</returns>
        public uint typeCount(string type)
        {
            return _typeCount[type];
        }

        /// <summary>
        /// Returns the amount of Entities of the Class are in the World.
        /// </summary>
        /// <param name="name">The Class type to count.</param>
        /// <returns>How many Entities of Class exist in the World.</returns>
        public uint classCount(string name)
        {
            return _classCount[name];
        }

        /// <summary>
        /// Returns the amount of Entities are on the layer in the World.
        /// </summary>
        /// <param name="layer">The layer to count Entities on.</param>
        /// <returns>How many Entities are on the layer.</returns>
        public uint layerCount(int layer)
        {
            return _layerCount[layer];
        }

        /// <summary>
        /// The first Entity in the World.
        /// </summary>
        public Entity first { get { return _updateFirst; } }

        /// <summary>
        /// How many Entity layers the World has.
        /// </summary>
        public uint layers { get { return (uint)_layerList.Count; } }

        /// <summary>
        /// The first Entity of the type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The Entity.</returns>
        public Entity typeFirst(string type)
        {
            if (_updateFirst == null)
            {
                return null;
            }
			return _typeFirst[type];
        }

        /// <summary>
        /// The first Entity of the Class.
        /// </summary>
        /// <param name="name">The Class type to check.</param>
        /// <returns>The Entity.</returns>
        public Entity classFirst(string name)
        {
            if (_updateFirst == null)
            {
                return null;
            }
			Entity e = _updateFirst;
			while (e != null)
			{
				if (e.GetType().FullName.Contains(name))
                {
                    return e;
                }
				e = e._updateNext;
			}
			return null;
        }

        /// <summary>
        /// The first Entity on the Layer.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        /// <returns>The Entity.</returns>
        public Entity layerFirst(int layer)
        {
            if (_updateFirst == null)
            {
                return null;
            }
			return _renderFirst[layer];
        }

        /// <summary>
        /// The last Entity on the Layer.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        /// <returns>The Entity.</returns>
        public Entity layerLast(int layer)
        {
            if (_updateFirst == null)
            {
                return null;
            }
			return _renderLast[layer];
        }

        /// <summary>
        /// The Entity that will be rendered first by the World.
        /// </summary>
        public Entity farthest
        {
            get
            {
                if (_updateFirst == null)
                {
                    return null;
                }
			    return _renderLast[_layerList[_layerList.Count - 1]];
            }
        }

        /// <summary>
        /// The Entity that will be rendered last by the world.
        /// </summary>
        public Entity nearest
        {
            get
            {
                if (_updateFirst == null)
                {
                    return null;
                }
			    return _renderFirst[_layerList[0]];
            }
        }

        /// <summary>
        /// The layer that will be rendered first by the World.
        /// </summary>
        public int layerFarthest
        {
            get
            {
                if (_updateFirst == null)
                {
                    return 0;
                }
			    return _layerList[_layerList.Count - 1];
            }
        }

        /// <summary>
        /// The layer that will be rendered last by the World.
        /// </summary>
        public int layerNearest
        {
            get
            {
                if (_updateFirst == null)
                {
                    return 0;
                }
			    return _layerList[0];
            }
        }

        /// <summary>
        /// How many different types have been added to the World.
        /// </summary>
        public uint uniqueTypes
        {
            get
            {
                return (uint)_typeCount.Keys.Count;
            }
        }

        /// <summary>
        /// Pushes all Entities in the World of the type into the Array or Vector.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="into">The Array or Vector to populate.</param>
        public void getType(string type, ref Object into)
        {
            if (into is Array)
            {
                Entity[] list = (Entity[])into;
                Entity e = _typeFirst[type];
                uint n = (uint)list.Length;
                while (e != null)
                {
                    list[n++] = e;
                    e = e._typeNext;
                }
                into = list;
            }
            else if (into is List<Entity>)
            {
                List<Entity> list = (List<Entity>)into;
                Entity e = _typeFirst[type];
                uint n = (uint)list.Count;
                while (e != null)
                {
                    list.Add(e);
                    e = e._typeNext;
                }
                into = list;
            }
            else
            {
                throw new Exception("World::getType() Usage Error: \"into\" parameter must be Array or List<Entity>");
            }
        }

        /// <summary>
        /// Pushes all Entities in the World of the Class into the Array or Vector.
        /// </summary>
        /// <param name="name">The Class type to check.</param>
        /// <param name="into">The Array or Vector to populate.</param>
        public void getClass(string name, ref Object into)
        {
            if (into is Array)
            {
                Entity[] list = (Entity[])into;
                Entity e = _updateFirst;
                uint n = (uint)list.Length;
                while (e != null)
                {
                    if (e.GetType().FullName.Contains(name))
                    {
                        list[n++] = e;
                    }
                    e = e._updateNext;
                }
                into = list;
            }
            else if (into is List<Entity>)
            {
                List<Entity> list = (List<Entity>)into;
                Entity e = _updateFirst;
                uint n = (uint)list.Count;
                while (e != null)
                {
                    if (e.GetType().FullName.Contains(name))
                    {
                        list.Add(e);
                    }
                    e = e._updateNext;
                }
                into = list;
            }
            else
            {
                throw new Exception("World::getClass() Usage Error: \"into\" parameter must be Array or List<Entity>");
            }
        }

        /// <summary>
        /// Pushes all Entities in the World on the layer into the Array or Vector.
        /// </summary>
        /// <param name="layer">The layer to check.</param>
        /// <param name="into">The Array or Vector to populate.</param>
        public void getLayer(int layer, ref Object into)
        {
            if (into is Array)
            {
                Entity[] list = (Entity[])into;
                Entity e = _renderLast[layer];
                uint n = (uint)list.Length;
                while (e != null)
                {
                    list[n++] = e;
                    e = e._updateNext;
                }
                into = list;
            }
            else if (into is List<Entity>)
            {
                List<Entity> list = (List<Entity>)into;
                Entity e = _renderLast[layer];
                uint n = (uint)list.Count;
                while (e != null)
                {
                    list.Add(e);
                    e = e._updateNext;
                }
                into = list;
            }
            else
            {
                throw new Exception("World::getLayer() Usage Error: \"into\" parameter must be Array or List<Entity>");
            }
        }

        /// <summary>
        /// Pushes all Entities in the World into the array.
        /// </summary>
        /// <param name="into">The Array or Vector to populate.</param>
        public void getAll(ref Object into)
        {
            if (into is Array)
            {
                Entity[] list = (Entity[])into;
                Entity e = _updateFirst;
                uint n = (uint)list.Length;
                while (e != null)
                {
                    list[n++] = e;
                    e = e._updateNext;
                }
                into = list;
            }
            else if (into is List<Entity>)
            {
                List<Entity> list = (List<Entity>)into;
                Entity e = _updateFirst;
                uint n = (uint)list.Count;
                while (e != null)
                {
                    list.Add(e);
                    e = e._updateNext;
                }
                into = list;
            }
            else
            {
                throw new Exception("World::getAll() Usage Error: \"into\" parameter must be Array or List<Entity>");
            }
        }

        /// <summary>
        /// Updates the add/remove lists at the end of the frame.
        /// </summary>
        public void updateLists()
        {
			// remove entities
			if (_remove.Count > 0)
			{
				foreach (Entity e in _remove)
				{
					// PATCH - 10-22-10-1
					if (e._added != true && _add.IndexOf(e) >= 0) 
					{
                        _add.RemoveAt(_add.IndexOf(e));
						//_add.splice(_add.indexOf(e), 1);
						continue;
					}
					// END PATCH
					
					e._added = false;
					e.removed();
					removeUpdate(e);
					removeRender(e);
					if (e._type != null)
                    {
                        removeType(e);
                    }
					if (e.autoClear && e._tween != null)
                    {
                        e.clearTweens();
                    }
				}
                _remove.Clear();
				//_remove.length = 0;
			}
			
			// add entities
			if (_add.Count > 0)
			{
				foreach (Entity e in _add)
				{
					e._added = true;
					addUpdate(e);
					addRender(e);
					if (e._type != null)
                    {
                        addType(e);
                    }
					e.added();
				}
                _add.Clear();
				//_add.length = 0;
			}
			
			// sort the depth list
			if (_layerSort)
			{
				if (_layerList.Count > 1) 
                {
                    PX.sort(_layerList, true);
                }
				_layerSort = false;
			}
        }

        /// <summary>
        /// Adds Entity to the update list.
        /// </summary>
        /// <param name="e">Entity to add</param>
        private void addUpdate(Entity e)
        {
            // add to update list
			if (_updateFirst != null)
			{
				_updateFirst._updatePrev = e;
				e._updateNext = _updateFirst;
			}
			else
            {
                e._updateNext = null;
            }
			e._updatePrev = null;
			_updateFirst = e;
			_count ++;
            
			if (!_classCount.ContainsKey(e._class))
            {
                _classCount[e._class] = 0;
            }
			_classCount[e._class]++;
        }

        /// <summary>
        /// Removes Entity from the update list.
        /// </summary>
        /// <param name="e">Entity to remove</param>
        private void removeUpdate(Entity e)
        {
            // remove from the update list
			if (_updateFirst == e) _updateFirst = e._updateNext;
			if (e._updateNext != null)
            {
                e._updateNext._updatePrev = e._updatePrev;
            }
			if (e._updatePrev != null)
            {
                e._updatePrev._updateNext = e._updateNext;
            }
			e._updateNext = e._updatePrev = null;
			
			_count --;
			_classCount[e._class] --;
        }

        /// <summary>
        /// Adds Entity to the render list.
        /// </summary>
        /// <param name="e">Entity to add</param>
        internal void addRender(Entity e)
        {
            Entity f = _renderFirst[e._layer];
			if (f != null)
			{
				// Append entity to existing layer.
				e._renderNext = f;
				f._renderPrev = e;
				_layerCount[e._layer] ++;
			}
			else
			{
				// Create new layer with entity.
				_renderLast[e._layer] = e;
				_layerList[_layerList.Count] = e._layer;
				_layerSort = true;
				e._renderNext = null;
				_layerCount[e._layer] = 1;
			}
			_renderFirst[e._layer] = e;
			e._renderPrev = null;
        }

        /// <summary>
        /// Removes Entity from the render list.
        /// </summary>
        /// <param name="e">Entity to remove</param>
        internal void removeRender(Entity e)
        {
            if (e._renderNext != null)
            {
                e._renderNext._renderPrev = e._renderPrev;
            }
			else
            {
                _renderLast[e._layer] = e._renderPrev;
            }

			if (e._renderPrev != null)
            {
                e._renderPrev._renderNext = e._renderNext;
            }
			else
			{
				// Remove this entity from the layer.
				_renderFirst[e._layer] = e._renderNext;
				if (e._renderNext == null)
				{
					// Remove the layer from the layer list if this was the last entity.
					if (_layerList.Count > 1)
					{
						_layerList[_layerList.IndexOf(e._layer)] = _layerList[_layerList.Count - 1];
						_layerSort = true;
					}
                    _layerList.RemoveAt(_layerList.Count - 1);
					//_layerList.length --;
				}
			}
			_layerCount[e._layer] --;
			e._renderNext = e._renderPrev = null;
        }

        /// <summary>
        /// Adds Entity to the type list.
        /// </summary>
        /// <param name="e">Entity to add</param>
        internal void addType(Entity e)
        {
            // add to type list
			if (_typeFirst.ContainsKey(e._type))
			{
				_typeFirst[e._type]._typePrev = e;
				e._typeNext = _typeFirst[e._type];
				_typeCount[e._type] ++;
			}
			else
			{
				e._typeNext = null;
				_typeCount[e._type] = 1;
			}
			e._typePrev = null;
			_typeFirst[e._type] = e;
        }

        /// <summary>
        /// Removes Entity from the type list.
        /// </summary>
        /// <param name="e">Entity to remove</param>
        internal void removeType(Entity e)
        {
            // remove from the type list
			if (_typeFirst[e._type] == e) _typeFirst[e._type] = e._typeNext;
			if (e._typeNext != null)
            {
                e._typeNext._typePrev = e._typePrev;
            }
			if (e._typePrev != null)
            {
                e._typePrev._typeNext = e._typeNext;
            }
			e._typeNext = e._typePrev = null;
			_typeCount[e._type] --;
        }

        /// <summary>
        /// Calculates the squared distance between two rectangles.
        /// </summary>
        /// <param name="x1">fixme</param>
        /// <param name="y1">fixme</param>
        /// <param name="w1">fixme</param>
        /// <param name="h1">fixme</param>
        /// <param name="x2">fixme</param>
        /// <param name="y2">fixme</param>
        /// <param name="w2">fixme</param>
        /// <param name="h2">fixme</param>
        /// <returns>fixme</returns>
        static private float squareRects(float x1, float y1, int w1, int h1, float x2, float y2, int w2, int h2)
        {
            if (x1 < x2 + w2 && x2 < x1 + w1)
			{
				if (y1 < y2 + h2 && y2 < y1 + h1) return 0;
				if (y1 > y2) return (y1 - (y2 + h2)) * (y1 - (y2 + h2));
				return (y2 - (y1 + h1)) * (y2 - (y1 + h1));
			}
			if (y1 < y2 + h2 && y2 < y1 + h1)
			{
				if (x1 > x2) return (x1 - (x2 + w2)) * (x1 - (x2 + w2));
				return (x2 - (x1 + w1)) * (x2 - (x1 + w1));
			}
			if (x1 > x2)
			{
				if (y1 > y2) return squarePoints(x1, y1, (x2 + w2), (y2 + h2));
				return squarePoints(x1, y1 + h1, x2 + w2, y2);
			}
            if (y1 > y2) return squarePoints(x1 + w1, y1, x2, y2 + h2);
			return squarePoints(x1 + w1, y1 + h1, x2, y2);
        }

        /// <summary>
        /// Calculates the squared distance between two points.
        /// </summary>
        /// <param name="x1">fixme</param>
        /// <param name="y1">fixme</param>
        /// <param name="x2">fixme</param>
        /// <param name="y2">fixme</param>
        /// <returns>fixme</returns>
        static private float squarePoints(float x1, float y1, float x2, float y2)
        {
            return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
        }

        /// <summary>
        /// Calculates the squared distance between a rectangle and a point.
        /// </summary>
        /// <param name="px">fixme</param>
        /// <param name="py">fixme</param>
        /// <param name="rx">fixme</param>
        /// <param name="ry">fixme</param>
        /// <param name="rw">fixme</param>
        /// <param name="rh">fixme</param>
        /// <returns>fixme</returns>
        static private float squarePointRect(float px, float py, float rx, float ry, int rw, int rh)
        {
            if (px >= rx && px <= rx + rw)
			{
				if (py >= ry && py <= ry + rh) return 0;
				if (py > ry) return (py - (ry + rh)) * (py - (ry + rh));
				return (ry - py) * (ry - py);
			}
			if (py >= ry && py <= ry + rh)
			{
				if (px > rx) return (px - (rx + rw)) * (px - (rx + rw));
				return (rx - px) * (rx - px);
			}
			if (px > rx)
			{
				if (py > ry) return squarePoints(px, py, rx + rw, ry + rh);
				return squarePoints(px, py, rx + rw, ry);
			}
            if (py > ry) return squarePoints(px, py, rx, ry + rh);
			return squarePoints(px, py, rx, ry);
        }
    }
}
