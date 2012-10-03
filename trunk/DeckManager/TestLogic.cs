using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;

namespace DeckManager
{
	// 测试逻辑，把测试逻辑都写在这里吧，免得污染了其他类
	public class TestLogic
	{
		public void TestPrint()
		{
			PrintDocument document = new PrintDocument();
			document.PrintPage += new PrintPageEventHandler(Document_PrintPage);
			PageSetupDialog setup_dialog = new PageSetupDialog();
			setup_dialog.Document = document;
			setup_dialog.ShowDialog();

			PrintPreviewDialog preview_dialog = new PrintPreviewDialog();
			preview_dialog.Document = document;
			//document.DefaultPageSettings.Landscape = true;
			preview_dialog.ShowDialog();
		}

		void Document_PrintPage(object sender, PrintPageEventArgs e)
		{
			e.Graphics.PageUnit = GraphicsUnit.Millimeter;
			int iIndex = 0;
			foreach(Card card in Singleton<CardRepository>.Instance.Cards)
			{
				Image image = Singleton<ImageRepostiry>.Instance.GetImage(card);

				float cardWidth = 63.0f;
				float cardHeight = 88.0f;
				float cmPerInch = 25.4f;

				e.Graphics.DrawImage(image, (iIndex % 3) * (cardWidth), (int)(iIndex / 3.0f) * (cardHeight), cardWidth, cardHeight);

				// 上边
				if (iIndex >= 3)
				{
					e.Graphics.DrawLine(Pens.White,
						(iIndex % 3) * (cardWidth), (int)(iIndex / 3.0f) * (cardHeight),
						(iIndex % 3) * (cardWidth) + cardWidth, (int)(iIndex / 3.0f) * (cardHeight));
				}

				// 左边
				if (iIndex % 3 != 0)
				{
					e.Graphics.DrawLine(Pens.White,
						(iIndex % 3) * (cardWidth), (int)(iIndex / 3.0f) * (cardHeight),
						(iIndex % 3) * (cardWidth), (int)(iIndex / 3.0f) * (cardHeight) + cardHeight);
				}


				iIndex++;

				if (iIndex == 9)
				{
					break;
				}
			}
		}
	}
}
