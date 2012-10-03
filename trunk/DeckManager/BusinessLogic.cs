using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DeckManager
{
	public class BusinessLogic
	{
		private Thread m_thread;

		public void InitThread(IGetFromWebNotify notifier)
		{
			m_thread = new Thread
				(
					delegate()
					{
						if (!Singleton<CardRepository>.Instance.LoadCards())
						{
							Singleton<CardRepository>.Instance.GetFromWeb(notifier);
						}

						Singleton<ImageRepostiry>.Instance.LoadImages(Singleton<CardRepository>.Instance.Cards);
					}
				);
			m_thread.Start();
		}

		public bool ThreadHasFinished()
		{
			return m_thread.ThreadState == ThreadState.Stopped;
		}

		/// <summary>
		/// ѡ���Ƶ�������ӿڣ���ӡ
		/// </summary>
		/// <param name="cards">ѡ�õ��ƣ���Card���鴫��</param>
		public void PrintCards(Card[] cards)
		{
		}


	}
}
