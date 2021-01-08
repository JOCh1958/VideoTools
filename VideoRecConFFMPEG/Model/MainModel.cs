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

		public MainModel(Microsoft.Win32.RegistryKey _appByJOChKey)
			{
			appByJOChKey = _appByJOChKey;
			Load();
			}

		public string directorioFFMPEG { get; set; }
		public string dataDir { get; set; }

		public List<Extras.DescriptorDeCamara> ListaDeCamaras { get; set; }

		public bool Salvar()
			{
			if (appByJOChKey == null) return false;

			var json = System.Text.Json.JsonSerializer.Serialize(ListaDeCamaras);

			try
				{
				appByJOChKey.SetValue("directorioFFMPEG", directorioFFMPEG);
				appByJOChKey.SetValue("dataDir", dataDir);
				appByJOChKey.SetValue("listaDeCamaras", json);
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
				}

			try
				{
				ListaDeCamaras = System.Text.Json.JsonSerializer.Deserialize<List<Extras.DescriptorDeCamara>>(appByJOChKey.GetValue("listaDeCamaras", "").ToString());
				if (ListaDeCamaras == null)
					{
					ListaDeCamaras = new List<Extras.DescriptorDeCamara>();
					}
				}
			catch (Exception ex)
				{
				if (ex.Source?.Contains("System.Text.Json") == true)
					{
					ListaDeCamaras = new List<Extras.DescriptorDeCamara>();
					}
				}

			return true;
			}
		}
	}
