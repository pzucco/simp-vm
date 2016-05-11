using UnityEngine;
using System.Collections;


namespace Simp {

	public class OP {

		public enum Type 
		{
			IDENT, VALUE, RCALL, CALL
		}

		public readonly Type type;
		public readonly Value value;

		public OP (Type type, object value)
		{
			this.type = type;
			this.value = Value.Create(value);
		}

	}

}
