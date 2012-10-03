using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DeckManager
{
	public class ImageRepostiry
	{
		private List<Image> m_images = new List<Image>();
		private Dictionary<Card, Image> m_indexMap = new Dictionary<Card, Image>();

		public void LoadImages(Card[] cards)
		{
			lock(this)
			{
				foreach (Card card in cards)
				{
					Image image = Image.FromFile(card.ImagePath);
					m_images.Add(image);
					m_indexMap.Add(card, image);
				}
			}
		}

		public Image GetImage(Card card)
		{
			Image ret = null;
			lock(this)
			{
				m_indexMap.TryGetValue(card, out ret);
			}
			return ret;
		}
	}
}
