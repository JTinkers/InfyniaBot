using Engine.GameStructure.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Engine.Libraries.GUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class GUIWindow : Window
	{
		public ProxyModel ViewModel { get; set; } = new ProxyModel();

		public GUIWindow()
		{
			DataContext = ViewModel;

			InitializeComponent();

			var consoleWriter = new ConsoleRedirector();

			consoleWriter.TextWritten += (o, e) =>
			{
				ConsoleGUI.Dispatcher.Invoke(() =>
				{
					ConsoleGUI.AppendText(e.Text);

					if(!ConsoleGUI.IsFocused)
						ConsoleGUI.ScrollToEnd();
				});
			};

			Console.SetOut(consoleWriter);

			var refreshTimer = new Timer();
			refreshTimer.Interval = 500;
			refreshTimer.Enabled = true;
			refreshTimer.Elapsed += (t, args) =>
			{
				ViewModel.OnPropertyChanged("Level");
				ViewModel.OnPropertyChanged("Health");
				ViewModel.OnPropertyChanged("Mana");
				ViewModel.OnPropertyChanged("Position");
				ViewModel.OnPropertyChanged("IsPaused");
				ViewModel.OnPropertyChanged("Target");
				ViewModel.OnPropertyChanged("TargetDistance");
			};
		}

		private readonly Regex pattern = new Regex("[^0-9]+");
		public bool IsTextNumeric(string text) 
			=> !pattern.IsMatch(text);

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (ConsoleGUI.LineCount > ConsoleGUI.MaxLines)
				ConsoleGUI.Text = string.Empty;

			e.Handled = !IsTextNumeric(e.Text);
		}

		private void Button_Click(object sender, RoutedEventArgs e) 
			=> Core.IsBreakEnabled = !Core.IsBreakEnabled;

		private void Reload_Click(object sender, RoutedEventArgs e) 
			=> Game.LocalPlayer.Base = IntPtr.Zero;
	}
}
