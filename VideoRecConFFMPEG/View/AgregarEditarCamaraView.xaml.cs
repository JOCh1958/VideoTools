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
using System.Windows.Shapes;

namespace VideoRecConFFMPEG.View
	{
	/// <summary>
	/// Interaction logic for AgregarEditarCamaraView.xaml
	/// </summary>
	public partial class AgregarEditarCamaraView : Window
		{
		public static AgregarEditarCamaraView window { get; set; }
		public ViewModel.AgregarEditarCamaraViewModel viewModel { get; set; }

		public AgregarEditarCamaraView(Window _owner, Extras.DescriptorDeCamara _camara, string _WinTitulo)
			{
			this.Owner = _owner;

			window = this;

			InitializeComponent();
			DataContext = viewModel = new ViewModel.AgregarEditarCamaraViewModel();

			viewModel.camara = _camara;
			viewModel.WinTitulo = _WinTitulo;
			}
		}
	}
