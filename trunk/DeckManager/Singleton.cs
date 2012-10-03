using System;
using System.Collections.Generic;
using System.Text;

namespace DeckManager
{
	public class Singleton<T> where T : new()
	{
		private static T singleton = default(T);

		public static T Instance
		{
			get
			{
				if (singleton == null)
				{
					singleton = new T();
				}
				return singleton;
			}
		}
	}
}
