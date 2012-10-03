using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using DeckManager;

namespace GUI_WPF
{
	/// <summary>
	/// Interaction logic for MainWnd.xaml
	/// </summary>
	public partial class MainWnd : Window, DeckManager.IGetFromWebNotify
	{
		private string m_strMsg;
		private DispatcherTimer m_timer;
		private bool m_bWaitThread;

		public MainWnd()
		{
			InitializeComponent();
		}

		public void Notity(string strMsg, float fProcess)
		{
			m_strMsg = strMsg.ToString() + "(" + fProcess.ToString() + ")";
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//WPF中的UI控件不能被其他线程操作，这里使用一个计时器来更新调试信息
			m_timer = new DispatcherTimer();
			m_timer.Interval = TimeSpan.FromMilliseconds(10);
			m_timer.Tick += UpdateDebugInfo;
			m_timer.Start();

			m_bWaitThread = true;

			Singleton<BusinessLogic>.Instance.InitThread(this);
		}

		private void UpdateDebugInfo(object sender, EventArgs e)
		{
			if (m_bWaitThread && Singleton<BusinessLogic>.Instance.ThreadHasFinished())
			{
				m_bWaitThread = false;
				Info.Content = "完成";

				m_timer.Stop();
				m_timer = null;

				// 随便画画
				DrawingGroup imageDrawings = new DrawingGroup();

				Card[] cards = Singleton<CardRepository>.Instance.Cards;
				for (int i = 0; i < cards.Length; ++i)
				{
					ImageDrawing image = new ImageDrawing();
					image.Rect = new Rect(i * 50, 50, 150, 200);
					image.ImageSource = new BitmapImage(new Uri(cards[i].ImagePath, UriKind.Relative));

					imageDrawings.Children.Add(image);
				}

				DrawingImage drawingImageSource = new DrawingImage(imageDrawings);
				drawingImageSource.Freeze();

				Image imageControl = new Image();
				imageControl.Stretch = Stretch.None;
				imageControl.Source = drawingImageSource;

				Border imageBorder = new Border();
				imageBorder.BorderBrush = Brushes.Gray;
				imageBorder.BorderThickness = new Thickness(1);
				imageBorder.HorizontalAlignment = HorizontalAlignment.Left;
				imageBorder.VerticalAlignment = VerticalAlignment.Top;
				imageBorder.Margin = new Thickness(20);
				imageBorder.Child = imageControl;

				CardsBrowser.Content = imageBorder;
			}
			else
			{
				Info.Content = m_strMsg;
			}
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			Singleton<TestLogic>.Instance.TestPrint();
		}
	}
}
