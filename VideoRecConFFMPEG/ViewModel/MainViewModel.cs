using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoRecConFFMPEG.ViewModel
	{
	public class itemsCamara
		{
		public bool activa { get; set; }
		public string nombre { get; set; }
		public string ip { get; set; }
		public string usuario { get; set; }
		public string password { get; set; }
		}

	public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
		{
		public static Microsoft.Win32.RegistryKey appByJOChKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("byJOChVideoRecConFFMPEG_0100");

		public Model.MainModel model { get; set; }

		#region mvvm
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

		private void StoreBoolChagedValue(bool value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
			{
			if (!String.IsNullOrWhiteSpace(propertyName))
				{
				appByJOChKey.SetValue(propertyName, value);
				}
			}

		private void StoreStringChagedValue(String value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
			{
			if (!String.IsNullOrWhiteSpace(propertyName))
				{
				appByJOChKey.SetValue(propertyName, value);
				}
			}

		private void StoreIntChagedValue(int value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
			{
			if (!String.IsNullOrWhiteSpace(propertyName))
				{
				appByJOChKey.SetValue(propertyName, value);
				}
			}

		private void StoreDoubleChagedValue(double value, [System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
			{
			if (!String.IsNullOrWhiteSpace(propertyName))
				{
				appByJOChKey.SetValue(propertyName, value);
				}
			}

		#endregion mvvm

		public MainViewModel()
			{
			model = new Model.MainModel(appByJOChKey);

			AppCloseCommand = new Extras.RelayCommand(CloseMainWindows, param => CanCloseWindows);
			AppMaximizeCommand = new Extras.RelayCommand(MaximizeMainWindows, param => CanCloseWindows);
			AppMinimizeCommand = new Extras.RelayCommand(MinimizeMainWindows, param => CanCloseWindows);

			OpenExeFFMPEGCommand = new Extras.RelayCommand(OpenExeFile, param => CanCloseWindows);
			OpenDataCommand = new Extras.RelayCommand(OpenDataDir, param => CanCloseWindows);

			AgregarCamaraCommand = new Extras.RelayCommand(AgregarCamara, param => CanCloseWindows);
			GuardarLosCambiosCommand = new Extras.RelayCommand(GuardarLosCambios, param => CanCloseWindows);

			IniciarGrabacionesCommand = new Extras.RelayCommand(IniciarGrabaciones, param => CanCloseWindows);
			DetenerGrabacionesCommand = new Extras.RelayCommand(DetenerGrabaciones, param => CanCloseWindows);

			//ListaDeCamaras = new List<Extras.DescriptorDeCamara>();
			//ListaDeCamaras.Add(new Extras.DescriptorDeCamara { activa = true, grabar = false, nombre = "camara 01", url = "192.168.0.90", conexion = "none", portRtsp = 554, portHttp = 80, usuario = "admin", password = "admin" });
			//ListaDeCamaras.Add(new Extras.DescriptorDeCamara { activa = true, grabar = false, nombre = "camara 02", url = "192.168.0.91", conexion = "none", portRtsp = 554, portHttp = 80, usuario = "admin", password = "admin" });
			//ListaDeCamaras.Add(new Extras.DescriptorDeCamara { activa = true, grabar = false, nombre = "camara 03", url = "192.168.0.92", conexion = "none", portRtsp = 554, portHttp = 80, usuario = "admin", password = "admin" });

			LogDeActividad += $"Inicializando la app: {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}\r\n";
			}

		string AppTitulo_v = "VideoRecConFFMPEG 1.0.0 byJOCh";
		public string AppTitulo
			{
			get => AppTitulo_v;
			set
				{
				if (AppTitulo_v != value)
					{
					AppTitulo_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		string LogDeActividad_v = string.Empty;
		public string LogDeActividad
			{
			get => LogDeActividad_v;
			set
				{
				if (LogDeActividad_v != value)
					{
					LogDeActividad_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		#region propiedades
		public string directorioFFMPEG
			{
			get => model.directorioFFMPEG;
			set
				{
				if (model.directorioFFMPEG != value)
					{
					model.directorioFFMPEG = value;
					NotifyPropertyChanged();
					}
				}
			}

		public string dataDir
			{
			get => model.dataDir;
			set
				{
				if (model.dataDir != value)
					{
					model.dataDir = value;
					NotifyPropertyChanged();
					}
				}
			}

		// List<Extras.DescriptorDeCamara> ListaDeItemsCamara_v;
		public List<Extras.DescriptorDeCamara> ListaDeCamaras
			{
			get => model.ListaDeCamaras;
			set
				{
				if (model.ListaDeCamaras != value)
					{
					model.ListaDeCamaras = value;
					NotifyPropertyChanged();
					}
				}
			}

		List<Extras.EjecucionDeApp> ListaDeEjecucionesDeGrabacion_v;
		public List<Extras.EjecucionDeApp> ListaDeEjecucionesDeGrabacion
			{
			get => ListaDeEjecucionesDeGrabacion_v;
			set
				{
				if (ListaDeEjecucionesDeGrabacion_v != value)
					{
					ListaDeEjecucionesDeGrabacion_v = value;
					NotifyPropertyChanged();
					}
				}
			}
		#endregion propiedades

		#region funciones_de_comandos
		public void AgregarCamara(object obj)
			{
			Extras.DescriptorDeCamara cam = new Extras.DescriptorDeCamara { activa = false, grabar = false, nombre = "nombre", descripcion = "descripcion", url = "192.168.", conexion = "none", portRtsp = 554, portHttp = 80, filePrefix = "algo", usuario = "admin", password = "admin" };
			View.AgregarEditarCamaraView win = new View.AgregarEditarCamaraView(MainWindowVideoRec.mainWindows, cam, "Agregar camara nueva");
			if (win.ShowDialog() == true)
				{
				List<Extras.DescriptorDeCamara> camaras = new List<Extras.DescriptorDeCamara>(ListaDeCamaras);
				camaras.Add(cam);

				ListaDeCamaras = camaras;

				LogDeActividad += $"Se agrego la camara: {cam.nombre}\r\n";
				}
			}

		public void GuardarLosCambios(object obj)
			{
			model.Salvar();
			}

		private void OpenExeFile(object sender)
			{
			var dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.FileName = "ffmpeg.exe";
			dlg.InitialDirectory = directorioFFMPEG;

			dlg.Multiselect = false;
			dlg.CheckFileExists = true;
			dlg.CheckPathExists = true;
			dlg.Title = "Open ffmpeg.exe file...";
			dlg.DefaultExt = ".exe";

			dlg.Filter = "Exe files|*.exe|Todos los files (*.*)|*.*";

			if (dlg.ShowDialog() == true)
				{
				directorioFFMPEG = System.IO.Path.GetDirectoryName(dlg.FileName);
				model.Salvar();
				}
			}

		private void OpenDataDir(object sender)
			{
			var dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.FileName = "data.dat";
			dlg.InitialDirectory = dataDir;

			dlg.Multiselect = false;
			dlg.CheckFileExists = false;
			dlg.CheckPathExists = true;
			dlg.Title = "Open data dir...";
			dlg.DefaultExt = ".dat";

			dlg.Filter = "Data dir|*.dat|Todos los files (*.*)|*.*";

			if (dlg.ShowDialog() == true)
				{
				dataDir = System.IO.Path.GetDirectoryName(dlg.FileName);
				model.Salvar();
				}
			}

		public void CloseMainWindows(object obj)
			{
			MainWindowVideoRec.mainWindows?.Close();
			}

		public void MinimizeMainWindows(object obj)
			{
			if (MainWindowVideoRec.mainWindows != null) MainWindowVideoRec.mainWindows.WindowState = System.Windows.WindowState.Minimized;
			}

		public void MaximizeMainWindows(object obj)
			{
			if (MainWindowVideoRec.mainWindows != null)
				{
				if (MainWindowVideoRec.mainWindows.WindowState == System.Windows.WindowState.Maximized)
					{
					MainWindowVideoRec.mainWindows.WindowState = System.Windows.WindowState.Normal;
					}
				else
					{
					MainWindowVideoRec.mainWindows.WindowState = System.Windows.WindowState.Maximized;
					}
				}
			}

		public void IniciarGrabaciones(object obj)
			{
			ListaDeEjecucionesDeGrabacion = new List<Extras.EjecucionDeApp>();
			foreach (Extras.DescriptorDeCamara camara in ListaDeCamaras)
				{
				if (camara.activa && camara.grabar)
					{
					// 2 horas de grabacion 7200, 30 minutos = 1800
					string lineaFFMPEG = $"-rtsp_transport tcp -y -i rtsp://{camara.usuario}:{camara.password}@{camara.url}:{camara.portRtsp} -t 7200 ";
					Extras.EjecucionDeApp exe = new Extras.EjecucionDeApp(System.IO.Path.Combine(directorioFFMPEG, "ffmpeg.exe"), dataDir, lineaFFMPEG, System.IO.Path.Combine(dataDir, $"{camara.filePrefix}{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss.ffff")}.mp4"));
					exe.executionEnd += this.Exe_executionEnd;
					}
				}
			}

		private void Exe_executionEnd(object sender)
			{
			Extras.EjecucionDeApp exe = sender as Extras.EjecucionDeApp;
			LogDeActividad += $"{exe.outputExecString}\r\n";

			//this.Dispatcher.Invoke(new Action(() =>
			//{
			//	listaDeEjecucionesTerminadas_01.Add(exe01.comandoQueSeEjeuta);
			//	resultFFMpeg01 = exe01.outputExecString;
			//}));

			//if (repetirAlTerminar01)
			//	{
			//	btnTestFFMPeg01(null, null);
			//	}
			}

		public void DetenerGrabaciones(object obj)
			{
			}
		#endregion funciones_de_comandos

		public bool CanCloseWindows { get; set; } = true;

		#region comandos
		System.Windows.Input.ICommand AgregarCamaraCommand_v;
		public System.Windows.Input.ICommand AgregarCamaraCommand
			{
			get => AgregarCamaraCommand_v;
			set
				{
				if (AgregarCamaraCommand_v != value)
					{
					AgregarCamaraCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand GuardarLosCambiosCommand_v;
		public System.Windows.Input.ICommand GuardarLosCambiosCommand
			{
			get => GuardarLosCambiosCommand_v;
			set
				{
				if (GuardarLosCambiosCommand_v != value)
					{
					GuardarLosCambiosCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand OpenExeFFMPEGCommand_v;
		public System.Windows.Input.ICommand OpenExeFFMPEGCommand
			{
			get => OpenExeFFMPEGCommand_v;
			set
				{
				if (OpenExeFFMPEGCommand_v != value)
					{
					OpenExeFFMPEGCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand OpenDataCommand_v;
		public System.Windows.Input.ICommand OpenDataCommand
			{
			get => OpenDataCommand_v;
			set
				{
				if (OpenDataCommand_v != value)
					{
					OpenDataCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand AppCloseCommand_v;
		public System.Windows.Input.ICommand AppCloseCommand
			{
			get => AppCloseCommand_v;
			set
				{
				if (AppCloseCommand_v != value)
					{
					AppCloseCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand AppMaximizeCommand_v;
		public System.Windows.Input.ICommand AppMaximizeCommand
			{
			get => AppMaximizeCommand_v;
			set
				{
				if (AppMaximizeCommand_v != value)
					{
					AppMaximizeCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand AppMinimizeCommand_v;
		public System.Windows.Input.ICommand AppMinimizeCommand
			{
			get => AppMinimizeCommand_v;
			set
				{
				if (AppMinimizeCommand_v != value)
					{
					AppMinimizeCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand DetenerGrabacionesCommand_v;
		public System.Windows.Input.ICommand DetenerGrabacionesCommand
			{
			get => DetenerGrabacionesCommand_v;
			set
				{
				if (DetenerGrabacionesCommand_v != value)
					{
					DetenerGrabacionesCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand IniciarGrabacionesCommand_v;
		public System.Windows.Input.ICommand IniciarGrabacionesCommand
			{
			get => IniciarGrabacionesCommand_v;
			set
				{
				if (IniciarGrabacionesCommand_v != value)
					{
					IniciarGrabacionesCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}
		#endregion comandos
		}
	}
