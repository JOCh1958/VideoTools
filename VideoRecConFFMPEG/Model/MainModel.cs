using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoRecConFFMPEG.Model
	{
	public class MainModel
		{
		internal Microsoft.Win32.RegistryKey appByJOChKey { get; set; }

		public string directorioFFMPEG { get; set; }
		public string dataDir { get; set; }

		public MainModel(Microsoft.Win32.RegistryKey _appByJOChKey)
			{
			appByJOChKey = _appByJOChKey;
			Load();
			}

		public bool Salvar()
			{
			if (appByJOChKey == null) return false;

			try
				{
				appByJOChKey.SetValue("directorioFFMPEG", directorioFFMPEG);
				appByJOChKey.SetValue("dataDir", dataDir);
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
				}
			catch (Exception ex)
				{
				return false;
				}

			return false;
			}
		}
	}
