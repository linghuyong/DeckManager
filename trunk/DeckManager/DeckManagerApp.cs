using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeckManager
{
	class DeckManagerApp
	{
		[STAThread]
		static void Main()
		{
			Singleton<BusinessLogic>.Instance.InitThread(null);


		}
	}
}
