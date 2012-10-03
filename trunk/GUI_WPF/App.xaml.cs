using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace GUI_WPF
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private MainWnd m_mainWnd = new MainWnd();

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			this.MainWindow = m_mainWnd;

			m_mainWnd.Show();
		}
	}
}
