using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ParserDeFilesH264.ViewModel
	{
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

			OpenExeFFMPEGCommand = new Extras.RelayCommand(OpenExeFile, param => CanCloseWindows);

			OpenPairsBinFileCommand = new Extras.RelayCommand(OpenPairsBinFile, param => CanCloseWindows);
			}

		string AppTitulo_v = "ParseDeFilesH264 1.0.0 byJOCh";
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

		List<Extras.DataChunks> listaDeLengts_v = new List<Extras.DataChunks>();
		public List<Extras.DataChunks> listaDeLengts
			{
			get => listaDeLengts_v;
			set
				{
				if (listaDeLengts_v != value)
					{
					listaDeLengts_v = value;
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

		public string lastBinPairFile
			{
			get => model.lastBinPairFile;
			set
				{
				if (model.lastBinPairFile != value)
					{
					DatosDeConfiguracionModificados = true;
					model.lastBinPairFile = value;
					NotifyPropertyChanged();
					}
				}
			}

		Extras.DataChunks offsetInBinFile_v = null;
		public Extras.DataChunks offsetInBinFile
			{
			get => offsetInBinFile_v;
			set
				{
				if (offsetInBinFile_v != value)
					{
					offsetInBinFile_v = value;
					NotifyPropertyChanged();

					if (offsetInBinFile_v != null)
						{
						if (brBinPairFile != null)
							{
							long lastPos = brBinPairFile.BaseStream.Seek(offsetInBinFile.dataOffset, SeekOrigin.Begin);
							byte[] data = brBinPairFile.ReadBytes(offsetInBinFile.dataLen);
							ShowData(data, offsetInBinFile.dataOffset);
							}
						}
					}
				}
			}

		BinaryReader brBinPairFile_v = null;
		public BinaryReader brBinPairFile
			{
			get => brBinPairFile_v;
			set
				{
				if (brBinPairFile_v != value)
					{
					brBinPairFile_v = value;
					NotifyPropertyChanged();
					}
				}
			}

		void ShowData(byte[] data, long dataOffset)
			{
			ChunkToRich(MainWindow.mainWindows.rtbFirstChunk, data, dataOffset);
			}

		public void ChunkToRich(System.Windows.Controls.RichTextBox rich, byte[] chunkData, long dataOffset)
			{
			if (rich == null) return;
			if (chunkData == null) return;

			string sTmp = "";

			System.Windows.Documents.Run r = new System.Windows.Documents.Run("byJOCh");
			System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();
			p.FontFamily = new System.Windows.Media.FontFamily("Courier new");
			p.FontSize = 10.5;

			if (chunkData[0] == '$')
				{
				byte rb = 0, id = 0, hi = 0, lo = 0;
				rb = chunkData[0];    // brOpenFile.ReadByte();
				id = chunkData[1];    // brOpenFile.ReadByte();
				hi = chunkData[2];    // brOpenFile.ReadByte();
				lo = chunkData[3];    // brOpenFile.ReadByte();

				byte byFlagV = (byte)(chunkData[4] >> 6);
				byte byCSRCCount = (byte)(chunkData[4] & 0xf);
				bool bPFlag = (chunkData[4] & 0x20) == 0 ? false : true;
				bool bXFlag = (chunkData[4] & 0x10) == 0 ? false : true;
				bool bMmarker = (chunkData[5] & 0x80) == 0 ? false : true;    // marca de ultimo paquete de frame
				byte byPayloadType = (byte)(chunkData[5] & 0x7f);

				ulong ulNumeroDeSequencia = ((uint)chunkData[6] << 8) + chunkData[7];
				UInt32 ulTimeStamp = ((UInt32)chunkData[8] << 24) + ((UInt32)chunkData[9] << 16) + ((UInt32)chunkData[10] << 8) + chunkData[11];
				ulong ulSSRC = ((ulong)chunkData[12] << 24) + ((ulong)chunkData[13] << 16) + ((ulong)chunkData[14] << 8) + chunkData[15];

				r = new System.Windows.Documents.Run("Reporte RTP:\r\nFlagV: " + byFlagV.ToString());
				p.Inlines.Add(r);
				r = new System.Windows.Documents.Run(", CSRCCount: " + byCSRCCount.ToString() + ", PFlag: " + bPFlag.ToString() + ", XFlag: " + bXFlag.ToString() + ", Marker (last chunk): ");
				p.Inlines.Add(r);
				r = new System.Windows.Documents.Run(bMmarker.ToString());
				r.Foreground = bMmarker ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.DarkGray;
				p.Inlines.Add(r);
				p.Inlines.Add(new System.Windows.Documents.LineBreak());

				r = new System.Windows.Documents.Run("Sequencia: ");
				p.Inlines.Add(r);
				r = new System.Windows.Documents.Run(ulNumeroDeSequencia.ToString() + " (0x" + ulNumeroDeSequencia.ToString("X4") + ")");
				r.Background = System.Windows.Media.Brushes.Violet;
				p.Inlines.Add(r);
				p.Inlines.Add(new System.Windows.Documents.LineBreak());

				r = new System.Windows.Documents.Run("TimeStamp: ");
				p.Inlines.Add(r);

				sTmp = $"{ulTimeStamp} (0x{ulTimeStamp.ToString("X8")})";

				r = new System.Windows.Documents.Run(sTmp);
				r.Background = System.Windows.Media.Brushes.GreenYellow;
				p.Inlines.Add(r);
				p.Inlines.Add(new System.Windows.Documents.LineBreak());

				r = new System.Windows.Documents.Run("SSRC: ");
				p.Inlines.Add(r);
				r = new System.Windows.Documents.Run("0x" + ulSSRC.ToString("X8"));
				r.Background = System.Windows.Media.Brushes.Turquoise;
				p.Inlines.Add(r);
				p.Inlines.Add(new System.Windows.Documents.LineBreak());
				}

			r = new System.Windows.Documents.Run($"Reporte de data bytes num bytes: {chunkData.Length} at file offset {dataOffset}");
			p.Inlines.Add(r);
			p.Inlines.Add(new System.Windows.Documents.LineBreak());
			dataToParagraph(dataOffset, p, chunkData);

			System.Windows.Documents.FlowDocument fd = new System.Windows.Documents.FlowDocument(p);
			rich.Document = fd;
			}

		static void dataToParagraph(long displayOffset, System.Windows.Documents.Paragraph p, byte[] chunk)
			{
			System.Windows.Documents.Run r = new System.Windows.Documents.Run($"offset: {displayOffset:N0} ({displayOffset:X8})\r\n");

			p.Inlines.Add(new System.Windows.Documents.LineBreak());

			p.Inlines.Add(r);
			p.Inlines.Add(new System.Windows.Documents.LineBreak());

			string charLine = "";
			string byteLine = "";

			char spaceChar = ' ';

			System.Windows.Media.Brush fondoRTD = System.Windows.Media.Brushes.Cyan;

			ushort redCount = 0;
			for (int x = 0; x < chunk.Length; x++)
				{
				if ((x & 0xf) == 0)
					{
					//if (byteLine.Length > 0)
					//	{
					//	r = new System.Windows.Documents.Run(byteLine);
					//	r.Foreground = System.Windows.Media.Brushes.DarkBlue;
					//	p.Inlines.Add(r);
					//	byteLine = "";
					//	}

					if (charLine.Length > 0)
						{
						r = new System.Windows.Documents.Run(" " + charLine);
						r.Foreground = System.Windows.Media.Brushes.DarkGreen;
						p.Inlines.Add(r);
						p.Inlines.Add(new System.Windows.Documents.LineBreak());
						charLine = "";
						}

					r.Background = System.Windows.Media.Brushes.White;
					r = new System.Windows.Documents.Run((x + displayOffset).ToString("X8") + "  ");
					p.Inlines.Add(r);
					}

				if (redCount <= 0) if (chunk[x] == '$')
					{
					redCount = 4;

					// chunk len in network format
					ushort len = chunk[x + 3];
					len += (ushort)((ushort)chunk[x + 2] * (ushort)256);

					redCount += len;

					if (fondoRTD == System.Windows.Media.Brushes.Cyan) fondoRTD = System.Windows.Media.Brushes.Orange; else fondoRTD = System.Windows.Media.Brushes.Cyan;
					}

				r = new System.Windows.Documents.Run(chunk[x].ToString("X2") + " ");
				if (redCount > 0) if (redCount-- > 0) r.Background = fondoRTD;
				p.Inlines.Add(r);

				//byteLine += chunk[x].ToString("X2") + (redCount-- > 0 ? '*' : ' ');
				charLine += RepresentacionDeByte(chunk[x]);
				}

			if (byteLine.Length > 0)
				{
				r = new System.Windows.Documents.Run(byteLine);
				r.Foreground = System.Windows.Media.Brushes.DarkBlue;
				p.Inlines.Add(r);
				byteLine = "";
				}

			if (charLine.Length > 0)
				{
				r = new System.Windows.Documents.Run(" " + charLine);
				r.Foreground = System.Windows.Media.Brushes.DarkGreen;
				p.Inlines.Add(r);
				p.Inlines.Add(new System.Windows.Documents.LineBreak());
				charLine = "";
				}
			}

		static char RepresentacionDeByte(byte val)
			{
			if (Char.IsControl((char)val))
				{
				return '.';
				}
			else if (Char.IsWhiteSpace((char)val))
				{
				return ' ';
				}
			else if (Char.IsSeparator((char)val))
				{
				return '.';
				}
			else if (Char.IsSymbol((char)val))
				{
				return (char)val;
				}
			else if (Char.IsPunctuation((char)val))
				{
				return (char)val;
				}
			else if (Char.IsLetterOrDigit((char)val))
				{
				return (char)val;
				}
			else
				{
				return '.';
				}
			}

		#region funciones_de_comandos
		public void GuardarLosCambios(object obj)
			{
			model.Salvar();
			DatosDeConfiguracionModificados = false;
			}

		private void OpenPairsBinFile(object sender)
			{
			var dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.FileName = lastBinPairFile;
			dlg.InitialDirectory = System.IO.Path.GetDirectoryName(lastBinPairFile);

			dlg.Multiselect = false;
			dlg.CheckFileExists = true;
			dlg.CheckPathExists = true;
			dlg.Title = "Open bin file...";
			dlg.DefaultExt = ".bin";

			dlg.Filter = "Bin files|*.bin|Todos los files (*.*)|*.*";

			if (dlg.ShowDialog() == true)
				{
				lastBinPairFile = dlg.FileName;
				model.Salvar();

				// D:\Data\video\rtsp\captura.h264.rx.raw.bin
				// captura.h264.rx.lengts.bin
				// captura.h264.rx.raw.bin
				string dir = System.IO.Path.GetDirectoryName(lastBinPairFile);
				string fname = System.IO.Path.GetFileName(lastBinPairFile);

				if (fname.Contains(".raw.bin"))
					{
					string companionFile = fname.Substring(0, fname.Length - 8);
					string companionFileName = System.IO.Path.Combine(dir, companionFile + ".lengts.bin");
					if (System.IO.File.Exists(companionFileName))
						{
						List<Extras.DataChunks> listaDeLens = new List<Extras.DataChunks>();
						using (BinaryReader br = new BinaryReader(File.Open(companionFileName, FileMode.Open)))
							{
							try
								{
								long dataOffset = 0;
								while (true)
									{
									int chunkLength = br.ReadInt32();
									listaDeLens.Add(new Extras.DataChunks { dataLen= chunkLength, dataOffset= dataOffset });
									dataOffset += (long)chunkLength;
									}
								}
							catch (Exception)
								{
								}

							listaDeLengts = listaDeLens;
							brBinPairFile = new BinaryReader(File.Open(lastBinPairFile, FileMode.Open));
							}
						}
					}
				}
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
			MainWindow.mainWindows?.Close();
			}

		public void MinimizeMainWindows(object obj)
			{
			if (MainWindow.mainWindows != null) MainWindow.mainWindows.WindowState = System.Windows.WindowState.Minimized;
			}

		public void MaximizeMainWindows(object obj)
			{
			if (MainWindow.mainWindows != null)
				{
				if (MainWindow.mainWindows.WindowState == System.Windows.WindowState.Maximized)
					{
					MainWindow.mainWindows.WindowState = System.Windows.WindowState.Normal;
					}
				else
					{
					MainWindow.mainWindows.WindowState = System.Windows.WindowState.Maximized;
					}
				}
			}

		public void LimpiarLogDeActividad(object obj)
			{
			LogDeActividad = string.Empty;
			}
		#endregion funciones_de_comandos

		public bool CanCloseWindows { get; set; } = true;

		#region comandos
		System.Windows.Input.ICommand OpenPairsBinFileCommand_v;
		public System.Windows.Input.ICommand OpenPairsBinFileCommand
			{
			get => OpenPairsBinFileCommand_v;
			set
				{
				if (OpenPairsBinFileCommand_v != value)
					{
					OpenPairsBinFileCommand_v = value;
					NotifyPropertyChanged();
					}
				}
			}

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

		}
	}
