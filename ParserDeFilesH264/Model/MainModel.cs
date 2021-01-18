using System;
using System.Collections.Generic;
using System.Text;

namespace ParserDeFilesH264.Model
	{
	public class MainModel
		{
		internal Microsoft.Win32.RegistryKey appByJOChKey { get; set; }

		public MainModel(Microsoft.Win32.RegistryKey _appByJOChKey)
			{
			appByJOChKey = _appByJOChKey;
			Load();
			}

		public string directorioFFMPEG { get; set; }
		public string dataDir { get; set; }
		public string lastBinPairFile { get; set; }
		public int TiempoDeGrabacionesEnSegundos { get; set; }

		public bool GenerarFicherosH264 { get; set; }

		public bool Salvar()
			{
			if (appByJOChKey == null) return false;

			try
				{
				appByJOChKey.SetValue("directorioFFMPEG", directorioFFMPEG);
				appByJOChKey.SetValue("dataDir", dataDir);
				appByJOChKey.SetValue("lastBinPairFile", lastBinPairFile);
				appByJOChKey.SetValue("TiempoDeGrabacionesEnSegundos", TiempoDeGrabacionesEnSegundos.ToString());

				appByJOChKey.SetValue("GenerarFicherosH264", GenerarFicherosH264);
				}
			catch (Exception ex)
				{
				return false;
				}

			return false;
			}

		public bool Load()
			{
			if (appByJOChKey == null) return false;

			// appDir = System.IO.Path.GetDirectoryName(System.Environment.GetCommandLineArgs()[0]);
			try
				{
				directorioFFMPEG = appByJOChKey.GetValue("directorioFFMPEG", System.IO.Path.GetDirectoryName(System.Environment.GetCommandLineArgs()[0])).ToString();
				dataDir = appByJOChKey.GetValue("dataDir", System.IO.Path.GetDirectoryName(System.Environment.GetCommandLineArgs()[0])).ToString();
				lastBinPairFile = appByJOChKey.GetValue("lastBinPairFile", @"d:\").ToString();
				TiempoDeGrabacionesEnSegundos = int.Parse(appByJOChKey.GetValue("TiempoDeGrabacionesEnSegundos", "30").ToString());

				GenerarFicherosH264 = bool.Parse(appByJOChKey.GetValue("GenerarFicherosH264", "False").ToString());
				}
			catch (Exception ex)
				{
				}

			return true;
			}
		}
	}
