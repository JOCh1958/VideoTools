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

			ListaDeItemsCamara = new List<itemsCamara>();
			ListaDeItemsCamara.Add(new itemsCamara { activa = true, nombre = "camara 01", ip = "192.168.0.90", usuario = "admin", password = "admin" });
			ListaDeItemsCamara.Add(new itemsCamara { activa = true, nombre = "camara 02", ip = "192.168.0.91", usuario = "admin", password = "admin" });
			ListaDeItemsCamara.Add(new itemsCamara { activa = true, nombre = "camara 03", ip = "192.168.0.92", usuario = "admin", password = "admin" });
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

		public bool CanCloseWindows { get; set; } = true;

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

		List<itemsCamara> ListaDeItemsCamara_v;
		public List<itemsCamara> ListaDeItemsCamara
			{
			get => ListaDeItemsCamara_v;
			set
				{
				if (ListaDeItemsCamara_v != value)
					{
					ListaDeItemsCamara_v = value;
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


		}
	}
