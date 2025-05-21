using System;
using UnityEngine;

namespace QuickChat
{
	public struct Bind
	{
		public Bind(KeyCode k, string m)
		{
			this.key = k;
			this.msg = m;
		}
		public KeyCode key;
		public string msg;
	}
}
