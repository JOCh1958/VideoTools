using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VideoRecConFFMPEG.Extras
	{
	public class DescriptorDeCamara
		{
		public bool activa { get; set; }
		public bool grabar { get; set; }
		public string nombre { get; set; }
		public string descripcion { get; set; }
		public string url { get; set; }
		public string conexion { get; set; }
		public int portRtsp { get; set; }
		public int portHttp { get; set; }
		public string filePrefix { get; set; }
		public string usuario { get; set; }
		public string password { get; set; }

		[JsonIgnore]
		public EjecucionDeApp camExe { get; set; }
		}
	}
