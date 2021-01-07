using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoRecConFFMPEG
	{
	/// <summary>
	/// Interaction logic for MainWindowVideoRec.xaml
	/// </summary>
	public partial class MainWindowVideoRec : Window
		{
		public static MainWindowVideoRec mainWindows { get; set; }
		public ViewModel.MainViewModel viewModel { get; set; }

		public MainWindowVideoRec()
			{
			mainWindows = this;

			InitializeComponent();

			if ((DataContext != null) && (DataContext is ViewModel.MainViewModel))
				{
				viewModel = DataContext as ViewModel.MainViewModel;
				}
			else
				{
				DataContext = viewModel = new ViewModel.MainViewModel();
				}
			}

		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{
			base.OnMouseLeftButtonDown(e);

			this.DragMove();
			}

		private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
			{

			}

		private void Grid_MouseMove(object sender, MouseEventArgs e)
			{

			}
		}
	}
