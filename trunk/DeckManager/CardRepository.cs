using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Tags;
using System.Text.RegularExpressions;
using Winista.Text.HtmlParser.Nodes;
using System.Xml.Serialization;

namespace DeckManager
{
	public interface IGetFromWebNotify
	{
		void Notity(String strMsg, float fProcess);
	}

	public class Card
	{
		public int ID;
		public String Name;
		public String Type;
		public List<String> Jobs = new List<String>();
		public int Attack;
		public int Defend;
		public int Loyalty;
		public String ManaCost;
		public String Rare;
		public String ImagePath;
	}

	class StringHelper
	{
		public static void FillCardLongInfo(String strText, Card card)
		{
			String[] strTmp = strText.Split(new char[2] { '～', ' ' });
			card.Type = strTmp[0];
			if (strTmp.Length == 4)
			{
				card.Loyalty = Convert.ToInt32(strTmp[3].TrimEnd(')'));
			}
			else if (strTmp.Length == 3)
			{
				card.Jobs.AddRange(strTmp[1].Split('／'));
				if (strTmp[2].Contains("*"))
				{
					card.Attack = -1;
					card.Defend = -1;
				}
				else
				{
					int iOffSet = strTmp[2].IndexOf('/');
					card.Attack = Convert.ToInt32(strTmp[2].Substring(0, iOffSet));
					card.Defend = Convert.ToInt32(strTmp[2].Substring(iOffSet + 1));
				}
			}

		}
	}

	public class CardRepository
	{
		private List<Card> m_Cards = null;

		public Card[] Cards
		{
			get
			{
				lock (this)
				{
					return m_Cards.ToArray();
				}
			}
		}

		public bool LoadCards()
		{
			if (!File.Exists(Config.CardsXml))
			{
				return false;
			}

			XmlSerializer s = new XmlSerializer(typeof(List<Card>));
			FileStream fstream = new FileStream(Config.CardsXml, FileMode.Open);
			m_Cards = s.Deserialize(fstream) as List<Card>;
			fstream.Close();

			if (m_Cards == null)
			{
				return false;
			}

			return true;
		}

		public void GetFromWeb(IGetFromWebNotify notifier)
		{
			Directory.CreateDirectory(Config.ImagePath);

			if (notifier != null)
				notifier.Notity(String.Format("下载 {0}", Config.Uri), 0.0f);
			WebClient webClient = new WebClient();
			webClient.Encoding = Encoding.UTF8;
			String strHtml = webClient.DownloadString(Config.Uri);

			if (notifier != null)
				notifier.Notity("解析html文档", 0.0f);
			Lexer lexer = new Lexer(strHtml);
			Parser parser = new Parser(lexer);
			AndFilter andFilter = new AndFilter(new NodeClassFilter(typeof(TableRow)), new OrFilter(new HasAttributeFilter("class", "even"), new HasAttributeFilter("class", "odd")));
			NodeList htmlNodes = parser.ExtractAllNodesThatMatch(andFilter);
			lock (this)
			{
				m_Cards = new List<Card>();
				foreach (INode node in htmlNodes.ToNodeArray())
				{
					int iFiledIndex = 0;
					Card card = new Card();
					foreach (INode subNode in node.Children.ToNodeArray())
					{
						if (subNode is TextNode)
						{
							continue;
						}

						switch (iFiledIndex)
						{
							case 0:
								card.ID = Convert.ToInt32(subNode.FirstChild.GetText());
								card.ImagePath = Path.Combine(Config.ImagePath, card.ID.ToString() + ".jpg");
								break;
							case 1:
								card.Name = subNode.FirstChild.FirstChild.GetText();
								break;
							case 2:
								StringHelper.FillCardLongInfo(subNode.FirstChild.GetText(), card);
								break;
							case 3:
								if (subNode.FirstChild != null)
								{
									card.ManaCost = subNode.FirstChild.GetText();
								}
								else
								{
									card.ManaCost = String.Empty;
								}
								break;
							case 4:
								card.Rare = subNode.FirstChild.GetText();
								break;
						}

						iFiledIndex++;
					}
					m_Cards.Add(card);
				}
			}

			XmlSerializer s = new XmlSerializer(typeof(List<Card>));
			FileStream fstream = new FileStream(Config.CardsXml, FileMode.CreateNew);
			s.Serialize(fstream, m_Cards);
			fstream.Close();

			foreach (Card card in m_Cards)
			{
				if (notifier != null)
					notifier.Notity(String.Format("获取卡片\"{0}\"信息", card.Name), 1.0f / m_Cards.Count);
				webClient.DownloadFile(Path.Combine(Config.BaseImageUri, card.ID.ToString() + ".jpg"), card.ImagePath);
			}
		}
	}
}
