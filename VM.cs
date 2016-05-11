using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Simp {

	public class VM {

		//==================================================

		public Table table;

		readonly Value[] args = new Value[100];
		readonly Value[] stack = new Value[100];
		readonly OP[][] branches;

		int argsIdx;
		int stackIdx;
		int argsCount;

		int drop = 0;


		//==================================================

		public VM(Bytecode bytecode, params Dictionary<string, object>[] libs) 
		{
			this.table = new Table (bytecode);
			
			foreach (KeyValuePair<string, int> seek in bytecode.names)
			{
				table.Set (seek.Value, Value.UNDEFINED);

				foreach (Dictionary<string, object> lib in libs) 
				{
					if (lib.ContainsKey(seek.Key)) 
					{
						table.Set (seek.Value, Value.Create(lib[seek.Key]));
						break;
					}
				}
			}

			branches = new OP[bytecode.branches.Count][];
			int i = 0;

			foreach (List<OP> branchAsList in bytecode.branches)
			{
				branches[i] = branchAsList.ToArray();
				i++;
			}
		}


		//==================================================
		
		public void BeginScope() 
		{
			table = new Table.Dynamic (table);
		}
		
		public void EndScope()
		{
			table = ((Table.Dynamic)table).super;
		}
		
		public Value Pop() 
		{
			int curr = argsIdx;
			argsIdx++;
			argsCount--;
			return args[curr];
		}
		
		public int ArgsCount() 
		{
			return argsCount;
		}

		public void Drop(int i) 
		{
			drop = i;
		}
		
		//==================================================



		public Value Execute(int branch = 0) 
		{
			Value result = Value.UNDEFINED;

			OP[] ops = branches [branch];
			int idx = -1;

			while (true) 
			{
				idx++;
				if (idx >= ops.Length || drop > 0) 
				{
					if (drop > 0) 
					{
						drop--;
					}
					break;
				}

				OP op = ops[idx];

				switch(op.type)
				{

				case OP.Type.IDENT:

					result = table.Get(op.value.Int);
					stack[stackIdx] = result;
					stackIdx++;

					break;
				
				case OP.Type.VALUE:

					result = op.value;
					stack[stackIdx] = result;
					stackIdx++;
					
					break;
						
				case OP.Type.CALL:
				case OP.Type.RCALL:

					argsCount = op.value.Int;

					for(int i = argsCount - 1; i >= 0; i--) 
					{
						args[i] = stack[stackIdx-1];
						stackIdx--;
					}

					argsIdx = 1;
					result = args[0].Cast<System.Func<VM, Value>>().Invoke (this);
					
					if (op.type == OP.Type.RCALL)
					{
						stack[stackIdx] = result;
						stackIdx++;
					}

					break;
				}
			}

			return result;
		}

	}

}
