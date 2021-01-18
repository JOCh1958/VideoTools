using System;
using System.Collections.Generic;
using System.Text;

namespace ParserDeFilesH264.Extras
	{
	public class DataChunks
		{
		public int dataLen { get; set; }
		public long dataOffset { get; set; }

		public override string ToString() => $"{dataLen} at {dataOffset}";
		}
	}
