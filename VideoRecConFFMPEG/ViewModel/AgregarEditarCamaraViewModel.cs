using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoRecConFFMPEG.ViewModel
	{
	public class AgregarEditarCamaraViewModel : System.ComponentModel.INotifyPropertyChanged
		{
		#region mvvm_simple
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		// private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
		private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
			{
			if (PropertyChanged != null)
				{
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
				}
			}

		#endregion mvvm_simple

		public AgregarEditarCamaraViewModel()
			{
			GuardarAgregarCamaraCommand = new Extras.RelayCommand(GuardarAgregarCamara, param => CanOperate);
			CancelarCamaraCommand = new Extras.RelayCommand(CancelarCamara, param => CanOperate);
			AgregarStringDeConexionCommand = new Extras.RelayCommand(AgregarStringDeConexion, param => CanOperate);
			}

		string WinTitulo_v = "Agregar camara nueva";
		public string WinTitulo
			{
			get => WinTitulo_v;
			set
				{
				if (WinTitulo_v != value)
					{
					WinTitulo_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		#region propiedades
		Extras.DescriptorDeCamara camara_v;
		public Extras.DescriptorDeCamara camara
			{
			get => camara_v;
			set
				{
				if (camara_v != value)
					{
					camara_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		#endregion propiedades

		public bool CanOperate { get; set; } = true;

		#region comandos
		System.Windows.Input.ICommand GuardarAgregarCamaraCommand_v;
		public System.Windows.Input.ICommand GuardarAgregarCamaraCommand
			{
			get => GuardarAgregarCamaraCommand_v;
			set
				{
				if (GuardarAgregarCamaraCommand_v != value)
					{
					GuardarAgregarCamaraCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand CancelarCamaraCommand_v;
		public System.Windows.Input.ICommand CancelarCamaraCommand
			{
			get => CancelarCamaraCommand_v;
			set
				{
				if (CancelarCamaraCommand_v != value)
					{
					CancelarCamaraCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand AgregarStringDeConexionCommand_v;
		public System.Windows.Input.ICommand AgregarStringDeConexionCommand
			{
			get => AgregarStringDeConexionCommand_v;
			set
				{
				if (AgregarStringDeConexionCommand_v != value)
					{
					AgregarStringDeConexionCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		#endregion comandos

		#region funciones_de_comandos
		void GuardarAgregarCamara(object algo)
			{
			View.AgregarEditarCamaraView.window.DialogResult = true;
			}

		void CancelarCamara(object algo)
			{
			View.AgregarEditarCamaraView.window.DialogResult = false;
			}

		void AgregarStringDeConexion(object parametro)
			{
			string stringDeConexion = parametro as string;
			if (!string.IsNullOrWhiteSpace(stringDeConexion))
				{
				camara.conexion = stringDeConexion;
				NotifyPropertyChanged("camara");
				}
			}

		#endregion funciones_de_comandos


		}
	}
