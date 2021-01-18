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
			DatosDeConfiguracionModificados = false;

			AppCloseCommand = new Extras.RelayCommand(CloseMainWindows, param => CanCloseWindows);
			AppMaximizeCommand = new Extras.RelayCommand(MaximizeMainWindows, param => CanCloseWindows);
			AppMinimizeCommand = new Extras.RelayCommand(MinimizeMainWindows, param => CanCloseWindows);

			OpenExeFFMPEGCommand = new Extras.RelayCommand(OpenExeFile, param => CanStartGrabaciones);
			OpenDataCommand = new Extras.RelayCommand(OpenDataDir, param => CanStartGrabaciones);

			AgregarCamaraCommand = new Extras.RelayCommand(AgregarCamara, param => CanStartGrabaciones);

			GuardarLosCambiosCommand = new Extras.RelayCommand(GuardarLosCambios, param => DatosDeConfiguracionModificados);

			IniciarGrabacionesCommand = new Extras.RelayCommand(IniciarGrabaciones, param => CanStartGrabaciones);
			DetenerGrabacionesCommand = new Extras.RelayCommand(DetenerGrabaciones, param => CanStopGrabaciones);

			TiempoDeGrabacionesCommand = new Extras.RelayCommand(TiempoDeGrabaciones, param => CanStartGrabaciones);

			LimpiarLogDeActividadCommand = new Extras.RelayCommand(LimpiarLogDeActividad, param => CanCloseWindows);

			//ListaDeCamaras = new List<Extras.DescriptorDeCamara>();
			//ListaDeCamaras.Add(new Extras.DescriptorDeCamara { activa = true, grabar = false, nombre = "camara 01", url = "192.168.0.90", conexion = "none", portRtsp = 554, portHttp = 80, usuario = "admin", password = "admin" });
			//ListaDeCamaras.Add(new Extras.DescriptorDeCamara { activa = true, grabar = false, nombre = "camara 02", url = "192.168.0.91", conexion = "none", portRtsp = 554, portHttp = 80, usuario = "admin", password = "admin" });
			//ListaDeCamaras.Add(new Extras.DescriptorDeCamara { activa = true, grabar = false, nombre = "camara 03", url = "192.168.0.92", conexion = "none", portRtsp = 554, portHttp = 80, usuario = "admin", password = "admin" });

			LogDeActividad += $"Inicializando la app: {DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} con {ListaDeCamaras.Count} camaras.\r\n";
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
					DatosDeConfiguracionModificados = true;
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
					DatosDeConfiguracionModificados = true;
					model.dataDir = value;
					NotifyPropertyChanged();
					}
				}
			}

		public int TiempoDeGrabacionesEnSegundos
			{
			get => model.TiempoDeGrabacionesEnSegundos;
			set
				{
				if (model.TiempoDeGrabacionesEnSegundos != value)
					{
					DatosDeConfiguracionModificados = true;
					model.TiempoDeGrabacionesEnSegundos = value;
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
					DatosDeConfiguracionModificados = true;
					model.ListaDeCamaras = value;
					NotifyPropertyChanged();
					}
				}
			}

		//List<Extras.EjecucionDeApp> ListaDeEjecucionesDeGrabacion_v;
		//public List<Extras.EjecucionDeApp> ListaDeEjecucionesDeGrabacion
		//	{
		//	get => ListaDeEjecucionesDeGrabacion_v;
		//	set
		//		{
		//		if (ListaDeEjecucionesDeGrabacion_v != value)
		//			{
		//			ListaDeEjecucionesDeGrabacion_v = value;
		//			NotifyPropertyChanged();
		//			}
		//		}
		//	}

		bool CanStopGrabaciones_v = false;
		public bool CanStopGrabaciones
			{
			get => CanStopGrabaciones_v;
			set
				{
				if (CanStopGrabaciones_v != value)
					{
					CanStopGrabaciones_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		bool CanStartGrabaciones_v = true;
		public bool CanStartGrabaciones
			{
			get => CanStartGrabaciones_v;
			set
				{
				if (CanStartGrabaciones_v != value)
					{
					CanStartGrabaciones_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		bool DatosDeConfiguracionModificados_v = false;
		public bool DatosDeConfiguracionModificados
			{
			get => DatosDeConfiguracionModificados_v;
			set
				{
				if (DatosDeConfiguracionModificados_v != value)
					{
					DatosDeConfiguracionModificados_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		List<string> ListaVideoFiles_v = new List<string>();
		public List<string> ListaVideoFiles
			{
			get => ListaVideoFiles_v;
			set
				{
				if (ListaVideoFiles_v != value)
					{
					DatosDeConfiguracionModificados = true;
					ListaVideoFiles_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		string LastRecFile_v = string.Empty;
		public string LastRecFile
			{
			get => LastRecFile_v;
			set
				{
				if (LastRecFile_v != value)
					{
					LastRecFile_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		public bool GenerarFicherosH264
			{
			get => model.GenerarFicherosH264;
			set
				{
				if (model.GenerarFicherosH264 != value)
					{
					DatosDeConfiguracionModificados = true;
					model.GenerarFicherosH264 = value;
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
			DatosDeConfiguracionModificados = false;
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
			CanStartGrabaciones = false;

			// Primero vamos a parar cualquier posible linea de ejecucion
			lock (objExeLock)
				{
				repetirAlTerminar = false;		// bloqueamos el posible reinicio de las grabaciones
				foreach (Extras.DescriptorDeCamara camara in ListaDeCamaras)
					{
					camara.camExe?.TerminarThread();
					camara.camExe = null;
					}
				}

			// 2 segundos de delay para que se acomoden los threads que podrian estar en ejecución
			System.Threading.Thread.Sleep(2000);

			// ListaDeEjecucionesDeGrabacion = new List<Extras.EjecucionDeApp>();
			lock (objExeLock)
				{
				repetirAlTerminar = true;      // a partir de aca permitimos que se reinicien las grabaciones
				foreach (Extras.DescriptorDeCamara camara in ListaDeCamaras)
					{
					IniciarGrabacionDeCamara(camara);
					//if (camara.activa && camara.grabar)
					//	{
					//	// 2 horas de grabacion 7200, 30 minutos = 1800
					//	string lineaFFMPEG = $"-rtsp_transport tcp -y -i rtsp://{camara.usuario}:{camara.password}@{camara.url}:{camara.portRtsp} -t 7200 ";
					//	camara.camExe = new Extras.EjecucionDeApp(System.IO.Path.Combine(directorioFFMPEG, "ffmpeg.exe"), dataDir, lineaFFMPEG, System.IO.Path.Combine(dataDir, $"{camara.filePrefix}{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss.ffff")}.mp4"));
					//	camara.camExe.objPariente = camara;			// cross reference
					//	camara.camExe.executionEnd += this.Exe_executionEnd;
					//	System.Threading.Thread.Sleep(25);
					//	}
					}
				}

			CanStopGrabaciones = true;
			}

		void IniciarGrabacionDeCamara(Extras.DescriptorDeCamara camara)
			{
			if (camara.activa && camara.grabar)
				{
				// 2 horas de grabacion 7200, 30 minutos = 1800
				string lineaFFMPEG = $"-rtsp_transport tcp -y -t {TiempoDeGrabacionesEnSegundos} -i rtsp://{camara.usuario}:{camara.password}@{camara.url}:{camara.portRtsp}{camara.conexion} ";
				//lineaFFMPEG += "-loglevel warning ";    // baja bastante el nivel de logs
				//lineaFFMPEG += "-nostats ";             // no muy bueno
				lineaFFMPEG += "-hide_banner ";         // solo elimina la info del principio
				//lineaFFMPEG += "-loglevel error ";      // no muestra nada supongo que si hay error si
				lineaFFMPEG += "-loglevel quiet ";      // practicamente 0 info
				//lineaFFMPEG += "-loglevel panic ";      // practicamente 0 info
				//lineaFFMPEG += "-loglevel fatal ";      // practicamente 0 info
				//lineaFFMPEG += "-loglevel warning ";    // pocos datos, sin banner y sin resumen
				//lineaFFMPEG += "-loglevel info ";       // algo de info y la ultima linea como se va alterando
				//lineaFFMPEG += "-loglevel verbose ";    // bastante info
				//lineaFFMPEG += "-loglevel debug ";      // mucha info, mucha

				string videoFile = System.IO.Path.Combine(dataDir, $"{DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss.ffff")}.{camara.filePrefix}v1.{(GenerarFicherosH264 ? "h264" : "mp4")}");
				LastRecFile = videoFile;

				camara.camExe = new Extras.EjecucionDeApp(System.IO.Path.Combine(directorioFFMPEG, "ffmpeg.exe"), dataDir, lineaFFMPEG, videoFile);
				camara.camExe.objPariente = camara;         // cross reference
				camara.camExe.executionEnd += this.Exe_executionEnd;
				LogDeActividad += $"Iniciando rec: {camara.camExe.appToExecute} {camara.camExe.parametrosDeEjecucion}\r\n";

				List<string> lvf = new List<string>(ListaVideoFiles);
				lvf.Add(videoFile);
				ListaVideoFiles = lvf;
				// NotifyPropertyChanged("ListaVideoFiles");

				camara.camTask = Task.Run( () => camara.camExe.EjecuctarApp());
				//camara.camExe.EjecuctarEnThread();
				//System.Threading.Thread.Sleep(25);
				}
			}

		Object objExeLock { get; } = new Object();
		bool repetirAlTerminar { get; set; } = true;

		private void Exe_executionEnd(object sender)
			{
			lock (objExeLock)
				{
				Extras.EjecucionDeApp exe = sender as Extras.EjecucionDeApp;
				if (exe != null)
					{
					LogDeActividad += $"{exe.outputExecString}\r\n";
					}

				if (repetirAlTerminar)
					{
					Extras.DescriptorDeCamara camara = exe.objPariente as Extras.DescriptorDeCamara;
					if (camara != null)
						{
						// re-lanzamos la ejecucion del grabador
						IniciarGrabacionDeCamara(camara);
						}
					}
				}

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
			CanStopGrabaciones = false;
			repetirAlTerminar = false;      // bloqueamos el posible reinicio de las grabaciones

			foreach (Extras.DescriptorDeCamara camara in ListaDeCamaras)
				{
				camara.camExe ?.SendKeyToProcess("q");
				System.Threading.Thread.Sleep(5);

				//camara.camExe?.TerminarThread();
				//camara.camExe = null;
				}

			lock (objExeLock)
				{
				CanStartGrabaciones = true;
				}
			}

		public void TiempoDeGrabaciones(object obj)
			{
			int newTimeValue = 30;
			if (int.TryParse(obj as string, out newTimeValue))
				{
				TiempoDeGrabacionesEnSegundos = newTimeValue;
				}
			}

		public void LimpiarLogDeActividad(object obj)
			{
			LogDeActividad = string.Empty;
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

		System.Windows.Input.ICommand TiempoDeGrabacionesCommand_v;
		public System.Windows.Input.ICommand TiempoDeGrabacionesCommand
			{
			get => TiempoDeGrabacionesCommand_v;
			set
				{
				if (TiempoDeGrabacionesCommand_v != value)
					{
					TiempoDeGrabacionesCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		System.Windows.Input.ICommand LimpiarLogDeActividadCommand_v;
		public System.Windows.Input.ICommand LimpiarLogDeActividadCommand
			{
			get => LimpiarLogDeActividadCommand_v;
			set
				{
				if (LimpiarLogDeActividadCommand_v != value)
					{
					LimpiarLogDeActividadCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}
		#endregion comandos

#if false
		private void btnAxisP1405_640x360_10fps(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.237";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "root";
			this.password = "root";
			this.stringDeConexion = "/axis-media/media.amp";
			// parametrosExtra = "resolution=640x360&fps=18&compression=70";
			this.parametrosExtra = "resolution=640x360&fps=10";
			}

		private void btnAxisP1405_1280x720_10fps(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.237";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "root";
			this.password = "root";
			this.stringDeConexion = "/axis-media/media.amp";
			this.parametrosExtra = "resolution=1280x720&fps=10";
			}

		private void btnAxisP1405_1920x1080_10fps(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.237";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "root";
			this.password = "root";
			this.stringDeConexion = "/axis-media/media.amp";
			this.parametrosExtra = "resolution=1920x1080&fps=10";
			}

		private void btnAxisP1013_640x360_10fps(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.230";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "root";
			this.password = "root";
			this.stringDeConexion = "/axis-media/media.amp";
			this.parametrosExtra = "resolution=640x360&fps=10";
			}

		private void btnRocoAxis_Click(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.230";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "root";
			this.password = "root";
			this.stringDeConexion = "/axis-media/media.amp";
			this.parametrosExtra = "resolution=640x360&fps=18";
			this.parametrosExtra = "";
			}

		private void btnBalconCalle800x600_Click(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.186";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "root";
			this.password = "root";
			this.stringDeConexion = "/axis-media/media.amp";
			this.parametrosExtra = "resolution=800x600&fps=10";
			}

		private void btnBalconCalle1280x720_Click(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.186";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "root";
			this.password = "root";
			this.stringDeConexion = "/axis-media/media.amp";
			this.parametrosExtra = "resolution=1280x720&fps=10";
			}

		private void btnDahuaLPR(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.133";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "admin";
			this.password = "admin";
			this.stringDeConexion = "/cam/realmonitor";
			// parametrosExtra = "channel=1&subtype=0&unicast=true&proto=Onvif";
			this.parametrosExtra = "channel=1&subtype=0";
			}

		private void btnDahuaJorgeNewvery(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.150";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "admin";
			this.password = "admin";
			this.stringDeConexion = "/cam/realmonitor";
			// parametrosExtra = "channel=1&subtype=0&unicast=true&proto=Onvif";
			this.parametrosExtra = "channel=1&subtype=0";
			}

		private void btnDahuaJOCh01(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.0.50";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "admin";
			this.password = "admin";
			this.stringDeConexion = "/cam/realmonitor";
			// parametrosExtra = "channel=1&subtype=0&unicast=true&proto=Onvif";
			this.parametrosExtra = "channel=1&subtype=0";
			}

		private void btnDahuaJOCh02(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.0.51";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "admin";
			this.password = "admin";
			this.stringDeConexion = "/cam/realmonitor";
			this.parametrosExtra = "channel=1&subtype=0";
			}

		private void btnDahuaVSS(object sender, RoutedEventArgs e)
			{
			this.host = "192.168.2.21";
			this.portHTTP = "80";
			this.portRTSP = "554";
			this.usuario = "";
			this.password = "";
			this.stringDeConexion = "/578-2";
			this.parametrosExtra = "";
			}

#endif

		}
	}
