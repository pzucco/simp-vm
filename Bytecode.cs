using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Simp {

	public class Bytecode {

		readonly Stack<List<OP>> branchStack = new Stack<List<OP>> ();
		readonly Stack<int> argsCounter = new Stack<int> ();
		readonly Stack<int> returnCounter = new Stack<int> ();

		public readonly Dictionary<string, int> names = new Dictionary<string, int>();
		public readonly List<List<OP>> branches = new List<List<OP>> ();
		
		string acc = "";

		public Bytecode (string source)
		{
			argsCounter.Push (0);
			returnCounter.Push (0);
			
			branchStack.Push (new List<OP> ());
			branches.Add(branchStack.Peek());
			
			for (int i = 0; i < source.Length; i++) {
				char c = source[i];
				
				switch(c) {
					
				case '(':
					
					AccSeparator();
					BeginCall();
					
					break;
					
				case ')':
					
					AccSeparator();
					EndCall();
					
					break;
					
				case '{':
					
					AccSeparator();
					BeginBranch();
					
					break;
					
				case '}':
					
					AccSeparator();
					EndBranch();
					
					break;
					
				case ' ':
					
					AccSeparator();
					
					break;
					
				case '$':
					
					acc += c;
					if (source[i+1] == '<') {
						i += 2;
						while(true) {
							char cc = source[i];
							if (cc == '>' && source[i+1] == '$') {
								i += 1;
								break;
							}
							acc += cc.ToString();
							i++;
						}
						AccSeparator();
					}
					
					break;
					
				default:
					
					acc += c.ToString().Trim ();
					
					break;
				}
			}

			branchStack.Clear ();
			argsCounter.Clear ();
			returnCounter.Clear ();
		}
		
		void BeginCall()
		{
			argsCounter.Push (0);
			returnCounter.Push (returnCounter.Pop() + 1);
		}
		
		void EndCall()
		{
			if (returnCounter.Peek () > 1) {
				branchStack.Peek ().Add (new OP (OP.Type.RCALL, argsCounter.Pop ()));			
			} else {
				branchStack.Peek ().Add (new OP (OP.Type.CALL, argsCounter.Pop ()));
			}
			
			argsCounter.Push (argsCounter.Pop () +1);
			returnCounter.Push (returnCounter.Pop () -1);
		}
		
		void BeginBranch()
		{
			branchStack.Peek ().Add(new OP(OP.Type.VALUE, branches.Count));
			branchStack.Push (new List<OP> ());
			branches.Add(branchStack.Peek ());
			argsCounter.Push (0);
			returnCounter.Push (0);
		}
		
		void EndBranch()
		{
			branchStack.Pop();
			argsCounter.Pop ();
			argsCounter.Push (argsCounter.Pop() + 1);
			returnCounter.Pop ();
		}
		
		void AccSeparator() 
		{
			if(acc.Length > 0)
			{
				int asInt = 0;
				float asFloat = 0f;
				
				if (int.TryParse(acc, out asInt))
				{
					branchStack.Peek ().Add(new OP(OP.Type.VALUE, asInt));
				} 
				else if (float.TryParse(acc, out asFloat)) 
				{
					branchStack.Peek().Add(new OP(OP.Type.VALUE, asFloat));
				} 
				else 
				{
					if (acc.Substring(0, 1) == ":") 
					{
						string ident = acc.Substring(1, acc.Length-1);
						branchStack.Peek ().Add(new OP(OP.Type.VALUE, Lookup(ident)));
					} 
					else if (acc.Substring(0, 1) == "$")
					{
						string value = acc.Substring(1, acc.Length-1);
						branchStack.Peek ().Add(new OP(OP.Type.VALUE, value));
					} 
					else
					{
						branchStack.Peek ().Add(new OP(OP.Type.IDENT, Lookup(acc)));
					}
				}
				
				acc = "";
				argsCounter.Push (argsCounter.Pop () + 1);
			}
		}
		
		public int Lookup(string ident) {
			if (!names.ContainsKey (ident)) {
				int index = names.Count;
				names [ident] = index;
				return index;
			}
			return names [ident];
		}

	}

}