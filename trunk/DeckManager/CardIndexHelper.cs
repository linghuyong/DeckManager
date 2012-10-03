using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeckManager
{
	public class CardIndexHelper
	{
		private Card[] m_cards;

		public CardIndexHelper(Card[] cards)
		{
			m_cards = cards;
		}

		/// <summary>
		/// CardIndexHelper关联的Cards数组索引器
		/// </summary>
		/// <param name="index">Cards数组下标</param>
		/// <returns>越界返还NULL</returns>
		public Card this[int index]
		{
			get
			{
				if (index < 0 || index <= m_cards.Length)
				{
					return null;
				}
				else
				{
					return m_cards[index];
				}
			}
		}

		public void GetCard(String strPopertyName)
		{
			//System.Console.WriteLine();
		}
	}
}
