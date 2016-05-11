using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Simp {

	public class Table
	{
		private readonly Value[] values;
		
		protected Table()
		{}

		public Table (Bytecode bytecode)
		{
			values = new Value[bytecode.names.Count];
		}

		public Value Get(int ptr)
		{
			return values [ptr];
		}

		public void Set(int ptr, Value value)
		{
			values [ptr] = value;
		}

		public class Dynamic : Table
		{
			public readonly Table super;
			private readonly Dictionary<int, Value> values = new Dictionary<int, Value> ();
			
			public Dynamic (Table super)
			{
				this.super = super;
			}

			public Value Get(int ptr)
			{
				return values [ptr];
			}

			public void Set(int ptr, Value value)
			{
				values [ptr] = value;
			}

		}

	}
	
}