﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity2D : MonoBehaviour
{
	/**Can this entity move*/
	public bool canMove = true;

	/**The hostility of this entity (Now an integer, 0 - 4, 0 = NONE, 1 = PEACFUL, 2 = NEUTRAL, 3 = HOSTILE, 4 = ALLIED)*/
	public EntityHostility hostility = 0;

	/**The name of this entity*/
	public string entityName = "entity";

	/**The direction of this entity*/
	public ObjectDirection direction = ObjectDirection.EAST;

	/**The animations on this entity*/
	public Animator entityAnimations;

	/**The number of steps per smooth movement, modifiyed by agility*/
	public float moveStep = 10f;
	/**The number of steps per smooth movement*/
	public float moveStepBase = 10f;
	/**The target location of the entity*/
	public IntPosition targetPosition;
	/**Is this entity currently moving*/
	public bool currentlyMoving = false;

	/**The position of this entity*/
	public IntPosition position;

	//TODO: Get rid of the temporary sprite renderer and animation list
	/**The entity's sprite renderer*/
	public SpriteRenderer spriteRenderer;
	/**The sprite animation list*/
	public Sprite[] spriteAnimationList;

	// Use this for initialization
	protected virtual void Start()
	{
		World2D.theWorld.getTile(position).setEntity(this);
		initEntity();
	}
	
	// Update is called once per frame
	protected virtual void Update()
	{
		
	}

	/**Called when this entity is created, creates stats, and checks for entity position*/
	public void initEntity()
	{
		if(World2D.theWorld.getTile(position).getEntity() != null)
		{
			World2D.theWorld.getTile(position).setEntity(this);
		}
		else
		{
			int range = 1;
			for(int x = -range; x <= range; x++)
			{
				for(int y = -range; y <= range; y++)
				{
					if(World2D.theWorld.getTile(new IntPosition(x,y)).getEntity() == null)
					{
						World2D.theWorld.getTile(new IntPosition(x,y)).setEntity(this);
						return;
					}
				}
			}

			this.die();
		}
	}

	/**Teleorts this entity*/
	public void Teleport(IntPosition destination)
	{
		World2D.theWorld.getTile(position).clearEntity();
		targetPosition = destination;
		transform.position = destination;
		position = destination;
		World2D.theWorld.getTile(position).setEntity(this);
	}

	[System.Obsolete("Use MoveSmooth Instead, it is finished and looks much nicer.")]
	/**Moves this entity in the direction supplied*/
	public virtual void Move(IntPosition location)
	{
		if(World2D.theWorld.getTile(position + location).getMovable())
		{
			World2D.theWorld.getTile(position).clearEntity();
			transform.Translate(location);
			position = transform.position;
			World2D.theWorld.getTile(position).setEntity(this);
		}
	}

	/**Moves this entity in the direction specifiyed, and the number of spaces specifiyed*/
	public virtual void Move(ObjectDirection direction, int spaces)
	{
		RotateNoTransform(direction);
		MoveSmooth(getDirection() * spaces);
	}

	/**Smothly moves this entity in the direction supplied, over time*/
	public virtual void MoveSmooth(IntPosition direction)//, int frames)
	{
		if(World2D.theWorld.getTile(position + direction).getMovable() && !currentlyMoving)
		{
			currentlyMoving = true;
			World2D.theWorld.getTile(position).clearEntity();
			OnTileExit(World2D.theWorld.getTile(position)); //Tile exit check
			position = transform.position;
			targetPosition = position + direction;
			World2D.theWorld.getTile(targetPosition).setEntity(this);
			OnTileEnter(World2D.theWorld.getTile(targetPosition)); //Tile Enter check
			ContinueMovement();
		}
	}

	/**Called whenever this entity enters a tile*/
	public virtual void OnTileEnter(Tile2D tile)
	{
		//TODO: Make the call for this better
	}

	/**Called whenever this entity stays on a tile for more than one frame*/
	public virtual void OnTileStay(Tile2D tile)
	{
		//TODO: Make this called in a way that it is not laggy
	}

	/**Called whenever this entity exits a tile*/
	public virtual void OnTileExit(Tile2D tile)
	{
		//TODO: Make the call for this better
	}

	/**Continues the current movement of the player (Smooth Movement)*/
	public virtual void ContinueMovement()
	{
		/*while(transform.position != targetPosition)
		{
			//TODO: Add increment methods for moving
			//transform.position += (Vector3) (getDirection() / 10f);
			yield return null;
		}

		StopCoroutine("ContinueMovement");*/

		switch(direction)
		{
		case ObjectDirection.EAST:
			StartCoroutine(MoveSmoothEast());
			break;
		case ObjectDirection.NORTH:
			StartCoroutine(MoveSmoothNorth());
			break;
		case ObjectDirection.WEST:
			StartCoroutine(MoveSmoothWest());
			break;
		case ObjectDirection.SOUTH:
			StartCoroutine(MoveSmoothSouth());
			break;
		case ObjectDirection.NORTH_EAST:
			StartCoroutine(MoveSmoothNorth());
			StartCoroutine(MoveSmoothEast());
			break;
		case ObjectDirection.NORTH_WEST:
			StartCoroutine(MoveSmoothNorth());
			StartCoroutine(MoveSmoothWest());
			break;
		case ObjectDirection.SOUTH_WEST:
			StartCoroutine(MoveSmoothSouth());
			StartCoroutine(MoveSmoothWest());
			break;
		case ObjectDirection.SOUTH_EAST:
			StartCoroutine(MoveSmoothSouth());
			StartCoroutine(MoveSmoothEast());
			break;
		default:
			Debug.LogWarning("Not Yet Implemented Movable Direction");
			break;
		}
	}

	/**Moves this entity East one frame*/
	public virtual IEnumerator MoveSmoothEast()
	{
		while(transform.position.x < targetPosition.x)
		{
			transform.position += (Vector3) (Vector2.right / moveStep);
			yield return null;
		}
		//Reset the position of the entity to the proper position (Floating point errors)
		setPosition(targetPosition);
		currentlyMoving = false;
		StopCoroutine("MoveSmoothEast");
	}

	/**Moves this entity North one frame*/
	public virtual IEnumerator MoveSmoothNorth()
	{
		while(transform.position.y < targetPosition.y)
		{
			transform.position += (Vector3) (Vector2.up / moveStep);
			yield return null;
		}
		//Reset the position of the entity to the proper position (Floating point errors)
		setPosition(targetPosition);
		currentlyMoving = false;
		StopCoroutine("MoveSmoothNorth");
	}

	/**Moves this entity West one frame*/
	public virtual IEnumerator MoveSmoothWest()
	{
		while(transform.position.x > targetPosition.x)
		{
			transform.position += (Vector3) (Vector2.left / moveStep);
			yield return null;
		}
		//Reset the position of the entity to the proper position (Floating point errors)
		setPosition(targetPosition);
		currentlyMoving = false;
		StopCoroutine("MoveSmoothWest");
	}

	/**Moves this entity South one frame*/
	public virtual IEnumerator MoveSmoothSouth()
	{
		while(transform.position.y > targetPosition.y)
		{
			transform.position += (Vector3) (Vector2.down / moveStep);
			yield return null;
		}
		//Reset the position of the entity to the proper position (Floating point errors)
		setPosition(targetPosition);
		currentlyMoving = false;
		StopCoroutine("MoveSmoothSouth");
	}

	/**Moves this entity in the direction it's facing*/
	public virtual void MoveForward(int distance)
	{
		Move(transform.right * distance);
	}

	/**Move this entity forward in the direction it's facing*/
	public virtual void MoveForwardNoTransform(int distance)
	{
		switch(direction)
		{
		case ObjectDirection.EAST:
			MoveSmooth(transform.right * distance);
			break;
		case ObjectDirection.NORTH:
			MoveSmooth(transform.up * distance);
			break;
		case ObjectDirection.WEST:
			MoveSmooth(-transform.right * distance);
			break;
		case ObjectDirection.SOUTH:
			MoveSmooth(-transform.up * distance);
			break;
		case ObjectDirection.NORTH_EAST:
			MoveSmooth((transform.up + transform.right) * distance);
			break;
		case ObjectDirection.NORTH_WEST:
			MoveSmooth((transform.up + -transform.right) * distance);
			break;
		case ObjectDirection.SOUTH_WEST:
			MoveSmooth((-transform.up + -transform.right) * distance);
			break;
		case ObjectDirection.SOUTH_EAST:
			MoveSmooth((-transform.up + transform.right) * distance);
			break;
		default:
			Debug.Log("Unknown direction supplied!");
			transform.Translate(Vector3.zero);
			break;
		}
	}

	/**Rotates this entity to face a different direction*/
	public virtual void Rotate(ObjectDirection newDirection)
	{
		//Rotate this entity and set it's animation parameters
		direction = newDirection;

		//entityAnimations.SetInteger("Direction", (int) newDirection);

		switch(newDirection)
		{
		case ObjectDirection.EAST:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			break;
		case ObjectDirection.NORTH:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
			break;
		case ObjectDirection.WEST:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
			break;
		case ObjectDirection.SOUTH:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 270f));
			break;
		case ObjectDirection.NORTH_EAST:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 45f));
			break;
		case ObjectDirection.NORTH_WEST:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 135f));
			break;
		case ObjectDirection.SOUTH_WEST:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 225f));
			break;
		case ObjectDirection.SOUTH_EAST:
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 315f));
			break;
		default:
			Debug.Log("Unknown direction supplied!");
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			break;
		}
	}

	/**Rotates this entities direction relateive to the current direction it's facing.*/
	public virtual void RotateLocal(sbyte dirAmount)
	{
		direction += dirAmount;
		if((sbyte) direction < (sbyte) ObjectDirection.EAST)
		{
			direction = ObjectDirection.SOUTH;
		}
		else if((sbyte) direction > (sbyte) ObjectDirection.SOUTH)
		{
			direction = ObjectDirection.EAST;
		}

		Rotate(direction);
	}

	/**Rotates this entities direction relateive to the direction it's facing (without transform).*/
	public virtual void RotateLocalNoTransform(sbyte dirAmount)
	{
		direction += dirAmount;
		if((sbyte) direction < (sbyte) ObjectDirection.EAST)
		{
			direction = ObjectDirection.SOUTH;
		}
		else if((sbyte) direction > (sbyte) ObjectDirection.SOUTH)
		{
			direction = ObjectDirection.EAST;
		}

		RotateNoTransform(direction);
	}

	/**Rotates this entity to face a different direction, happens over time*/
	public virtual void RotateSmooth(ObjectDirection newDirection)
	{
		
	}

	/**Rotates thie entity to face a different direction, doesn't rotate the object*/
	public virtual void RotateNoTransform(ObjectDirection newDirection)
	{
		direction = newDirection;
		//TODO: Finish rotation code
	}

	/**Set's the entities position (And transform)*/
	public virtual void setPosition(IntPosition newPos)
	{
		transform.position = newPos;
		position = newPos;
	}

	/**Called when this entity interacts with another entity*/
	public virtual void onInteract(Entity2D reciver)
	{
		
	}

	/**Called when this entity is interacted with*/
	public virtual void onInteracted(Entity2D sender)
	{
		
	}

	/**Returns the directioon this entity is facing*/
	public virtual Vector2 getDirection()
	{
		switch(direction)
		{
		case ObjectDirection.NONE:
			return Vector2.zero;
		case ObjectDirection.EAST:
			return Vector2.right;
		case ObjectDirection.NORTH:
			return Vector2.up;
		case ObjectDirection.WEST:
			return Vector2.left;
		case ObjectDirection.SOUTH:
			return Vector2.down;
		case ObjectDirection.NORTH_EAST:
			return new Vector2(1,1);
		case ObjectDirection.NORTH_WEST:
			return new Vector2(-1,1);
		case ObjectDirection.SOUTH_WEST:
			return new Vector2(-1,-1);
		case ObjectDirection.SOUTH_EAST:
			return new Vector2(1,-1);
		default:
			return Vector2.zero;
		}
	}

	/**Called when this entity is attacked*/
	public virtual void onAttacked(float damageIn)
	{
		//Do nothing
	}

	/**Called when this entity dies*/
	public virtual void die()
	{
		this.enabled = false;
		World2D.theWorld.getTile(position).clearEntity();
		//TODO: Create a dead clone entity at this position
	}
		
	//TODO: Make animations play from an animator (Sprite changing can be slower, and is less efficent)

	/**Plays this entity's idle animation*/
	public virtual void playIdleAnimation()
	{

	}

	/**Plays this entity's walk animation*/
	public virtual void playWalkAnimation()
	{

	}

	/**Plays this entity's run animation*/
	public virtual void playRunAnimation()
	{
		
	}

	/**Plays this entity's attack animation*/
	public virtual void playAttackAnimation()
	{
		
	}

	/**Plays this entity's block animation*/
	public virtual void playDefendAnimation()
	{

	}

	/**Plays this entity's hurt animation*/
	public virtual void playHurtAnimation()
	{

	}

	/**Plays this entity's heal animation*/
	public virtual void playHealAnimation()
	{

	}

	/**Plays one of the entity's animations*/
	public virtual void playAnimation(string name, float speed)
	{
		
	}

	/**Update any UI displays that this entity has*/
	public virtual void updateEntityUI()
	{
		//Put An entity specific UI updating code here
	}
}

[System.Serializable]
public class EntityAttribute
{
	/**The minimum value of this attribute*/
	public float min;
	/**The maximum value of this attribute*/
	public float max;
	/**The value of this attribute*/
	public float value;
	/**Should integers be used instead of floats*/
	public bool useIntegers = false;

	/**A empty constructor, sets everything to zero*/
	public EntityAttribute()
	{
		min = 0f;
		max = 0f;
		value = 0f;
	}

	/**Creates a new instance of the EntityAttribute class*/
	public EntityAttribute(float minValue, float maxValue, float startValue)
	{
		min = minValue;
		max = maxValue;
		value = startValue;
	}

	/**Creates a new EntityAttribute with infinity min and max values*/
	public EntityAttribute(float startvalue)
	{
		min = float.MinValue;//Mathf.NegativeInfinity;
		max = float.MaxValue;//Mathf.Infinity;
		value = startvalue;
	}

	/**Makes this attriutes min and max true infinity*/
	public EntityAttribute setTrueInfiniteCaps()
	{
		min = Mathf.NegativeInfinity;
		max = Mathf.Infinity;
		return this;
	}

	/**Gets the value of this attribute*/
	public float getValue()
	{
		return value;
	}

	/**Sets the value of this attribute*/
	public void setValue(float newValue)
	{
		value = newValue;
	}

	/**Adds a value to this attribute*/
	public void addValue(float amount)
	{
		value += amount;

		if(value > max)
		{
			value = max;
		}
	}

	/**Subtracts a value from the attribute*/
	public void subtractValue(float amount)
	{
		value -= amount;

		if(value < min)
		{
			value = min;
		}
	}

	public override string ToString()
	{
		if(min == float.MinValue && max == float.MaxValue)
			return value.ToString();
		else if(min == 0)
			return value + "/" + max;
		else
			return "(" + min + ", " + value + ", " + max + ")";
		
	}
}

[System.Serializable]
public class EntityIntAttribute : EntityAttribute
{
	/**The minimum value of this attribute*/
	public int min;
	/**The maximum value of this attribute*/
	public int max;
	/**The value of this attribute*/
	public int value;

	/**A empty constructor, sets everything to zero*/
	public EntityIntAttribute()
	{
		min = 0;
		max = 0;
		value = 0;
	}

	/**Creates a new instance of the EntityAttribute class*/
	public EntityIntAttribute(int minValue, int maxValue, int startValue) : base(minValue, maxValue, startValue)
	{
		min = minValue;
		max = maxValue;
		value = startValue;
	}

	/**Creates a new EntityAttribute with infinity min and max values*/
	public EntityIntAttribute(int startvalue)
	{
		min = int.MinValue;//Mathf.NegativeInfinity;
		max = int.MaxValue;//Mathf.Infinity;
		value = startvalue;
	}
}

public enum EntityHostility
{
	/**None is entities that dont have AI*/
	NONE,
	/**Peaceful entites will never attack you. If you attack them, they will run away*/
	PEACFUL,
	/**Neutral entities will not attack you unless you attack them first or somehow provoke them*/
	NEUTRAL,
	/**Hostile Entities will attack you on sight. They will keep on attacking you until death*/
	HOSTILE,
	/**Allied npc's will follow you around and attack npc's/players/bosses that attack you*/
	ALLIED,
};

[System.Serializable]
public class EntityPosition
{
	/**The x position*/
	public float x;
	/**The y position*/
	public float y;
	/**The z position*/
	public int z;

	public EntityPosition()
	{
		x = 0f;
		y = 0f;
		z = 0;
	}

	public EntityPosition(float xPos, float yPos, int layer)
	{
		x = xPos;
		y = yPos;
		z = layer;
	}

	public EntityPosition(Vector3 position)
	{
		x = position.x;
		y = position.y;
		z = (int) position.z;
	}
}