using UnityEngine;
using System.Collections;


namespace Simp {

	public sealed class Value
	{
		
		public enum Type
		{
			UNDEFINED, NUMBER, OBJECT
		}
		
		public readonly Type type;
		public readonly int Int;
		public readonly float Float;
		public readonly object Object;
		
		public Value (int asInt)
		{
			type = Type.NUMBER;
			this.Float = asInt;
			this.Int = asInt;
		}
		
		public Value (float asFloat)
		{
			type = Type.NUMBER;
			this.Float = asFloat;
			this.Int = Mathf.RoundToInt(asFloat);
		}
		
		public Value (object asObject)
		{
			type = Type.OBJECT;
			this.Object = asObject;
		}
		
		public Value () 
		{
			type = Type.UNDEFINED;
		}
		
		public static Value UNDEFINED = new Value();
		public static Value TRUE = new Value(1);
		public static Value FALSE = new Value(0);
		
		public static Value Create(object value) 
		{
			if (value is int) 
			{
				return new Value((int) value);
			}
			if (value is float) 
			{
				return new Value((float) value);
			}
			return new Value (value);
		}
		
		public T Cast<T>() 
		{
			if (type == Type.NUMBER)
			{
				return (T) ((object)Float);
			}
			return (T)Object;
		}
	}

}