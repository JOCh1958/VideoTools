using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoRecConFFMPEG.Extras
	{
	public class EjecucionDeApp : System.ComponentModel.INotifyPropertyChanged
		{
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
		#endregion mvvm

		#region bindings
		internal bool onWork_v = true;
		public bool onWork
			{
			get
				{
				return onWork_v;
				}
			set
				{
				onWork_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string comandoQueSeEjeuta_v = string.Empty;
		public string comandoQueSeEjeuta
			{
			get
				{
				return comandoQueSeEjeuta_v;
				}
			set
				{
				comandoQueSeEjeuta_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string appToExecute_v = string.Empty;
		public string appToExecute
			{
			get
				{
				return appToExecute_v;
				}
			set
				{
				appToExecute_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string parametrosDeEjecucion_v = string.Empty;
		public string parametrosDeEjecucion
			{
			get
				{
				return parametrosDeEjecucion_v;
				}
			set
				{
				parametrosDeEjecucion_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outFileOrLastParameter_v = string.Empty;
		public string outFileOrLastParameter
			{
			get
				{
				return outFileOrLastParameter_v;
				}
			set
				{
				outFileOrLastParameter_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string directorioDeTrabajo_v = string.Empty;
		public string directorioDeTrabajo
			{
			get
				{
				return directorioDeTrabajo_v;
				}
			set
				{
				directorioDeTrabajo_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outputString_v = string.Empty;
		public string outputString
			{
			get
				{
				return outputString_v;
				}
			set
				{
				outputString_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outputExecString_v = string.Empty;
		public string outputExecString
			{
			get
				{
				return outputExecString_v;
				}
			set
				{
				outputExecString_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outputStringLineaAnteultima_v = string.Empty;
		public string outputStringLineaAnteultima
			{
			get
				{
				return outputStringLineaAnteultima_v;
				}
			set
				{
				outputStringLineaAnteultima_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outputStringLineaUltima_v = string.Empty;
		public string outputStringLineaUltima
			{
			get
				{
				return outputStringLineaUltima_v;
				}
			set
				{
				outputStringLineaUltima_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outputErrorString_v = string.Empty;
		public string outputErrorString
			{
			get
				{
				return outputErrorString_v;
				}
			set
				{
				outputErrorString_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outputErrorStringLineaAnteultima_v = string.Empty;
		public string outputErrorStringLineaAnteultima
			{
			get
				{
				return outputErrorStringLineaAnteultima_v;
				}
			set
				{
				outputErrorStringLineaAnteultima_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string outputErrorStringLineaUltima_v = string.Empty;
		public string outputErrorStringLineaUltima
			{
			get
				{
				return outputErrorStringLineaUltima_v;
				}
			set
				{
				outputErrorStringLineaUltima_v = value;
				NotifyPropertyChanged();
				}
			}

		internal System.Diagnostics.Process exe_process_v = null;
		public System.Diagnostics.Process exe_process
			{
			get
				{
				return exe_process_v;
				}
			set
				{
				exe_process_v = value;
				NotifyPropertyChanged();
				}
			}

		internal System.Threading.Thread workThread_v = null;
		public System.Threading.Thread workThread
			{
			get
				{
				return workThread_v;
				}
			set
				{
				workThread_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string startTime_v = string.Empty;
		public string startTime
			{
			get
				{
				return startTime_v;
				}
			set
				{
				startTime_v = value;
				NotifyPropertyChanged();
				}
			}

		internal string endTime_v = string.Empty;
		public string endTime
			{
			get
				{
				return endTime_v;
				}
			set
				{
				endTime_v = value;
				NotifyPropertyChanged();
				}
			}

		internal System.Diagnostics.Stopwatch timeOn_v = new System.Diagnostics.Stopwatch();
		public System.Diagnostics.Stopwatch timeOn
			{
			get
				{
				return timeOn_v;
				}
			set
				{
				timeOn_v = value;
				NotifyPropertyChanged();
				}
			}

		#endregion bindings

		public delegate void ReporteDesdeObjetoExeApp(object sender);
		public event ReporteDesdeObjetoExeApp executionEnd;

		public EjecucionDeApp(string _appToExecute, string _directorioDeTrabajo, string _parametrosDeEjecucion)
			{
			appToExecute = _appToExecute;
			directorioDeTrabajo = _directorioDeTrabajo;
			parametrosDeEjecucion = _parametrosDeEjecucion;
			}

		public EjecucionDeApp(string _appToExecute, string _directorioDeTrabajo, string _parametrosDeEjecucion, string _outFileOrLastParameter)
			{
			appToExecute = _appToExecute;
			directorioDeTrabajo = _directorioDeTrabajo;
			parametrosDeEjecucion = _parametrosDeEjecucion;
			outFileOrLastParameter = _outFileOrLastParameter;

			if (!string.IsNullOrWhiteSpace(outFileOrLastParameter)) parametrosDeEjecucion += $" {outFileOrLastParameter}";
			}

		public string EjecuctarApp(out string result)
			{
			// string resultString = EjecutarAplicacionAndGetOutput_1procesador(appToExecute, directorioDeTrabajo, parametrosDeEjecucion, out result, System.Diagnostics.ProcessWindowStyle.Normal);
			// string resultString = EjecutarAplicacionAndGetOutput(appToExecute, directorioDeTrabajo, parametrosDeEjecucion, out result, System.Diagnostics.ProcessWindowStyle.Normal);
			string resultString = EjecutarAplicacionAndGetOutputWithLetInput(appToExecute, directorioDeTrabajo, parametrosDeEjecucion, out result, out actualExeProcess, System.Diagnostics.ProcessWindowStyle.Normal);
			onWork = false;
			outputExecString = resultString;
			executionEnd?.Invoke(this);
			return resultString;
			}

		public string EjecuctarApp()
			{
			string result = "";
			// string resultString = EjecutarAplicacionAndGetOutput_1procesador(appToExecute, directorioDeTrabajo, parametrosDeEjecucion, out result, System.Diagnostics.ProcessWindowStyle.Normal);
			// string resultString = EjecutarAplicacionAndGetOutput(appToExecute, directorioDeTrabajo, parametrosDeEjecucion, out result, System.Diagnostics.ProcessWindowStyle.Normal);
			string resultString = EjecutarAplicacionAndGetOutputWithLetInput(appToExecute, directorioDeTrabajo, parametrosDeEjecucion, out result, out actualExeProcess, System.Diagnostics.ProcessWindowStyle.Normal);
			onWork = false;
			outputExecString = resultString;
			executionEnd?.Invoke(this);
			return resultString;
			}

		public void EjecuctarEnThread()
			{
			workThread = new System.Threading.Thread(ThreadDeEjecutarApp);
			workThread.Priority = System.Threading.ThreadPriority.Normal;
			workThread.Name = $"EjecucionDeApp-ThreadDeEjecutarApp-{DateTime.Now.ToString("yyyyMMddHHmmssffff")}";
			workThread.Start(this);
			// System.Threading.Thread.Sleep(100);
			}

		public void TerminarThread()
			{
			workThread?.Abort();
			workThread = null;
			onWork = false;
			}

		public System.Diagnostics.Process actualExeProcess = null;

		internal void ThreadDeEjecutarApp(Object exeObj)
			{
			EjecucionDeApp ex = exeObj as EjecucionDeApp;
			string result = ex?.EjecuctarApp();
			workThread = null;
			}

		public void SendKeyToProcess(string textToSend)
			{
			try
				{
				exe_process?.StandardInput.WriteAsync(textToSend);
				}
			catch (Exception ex)
				{
				}
			}

		~EjecucionDeApp()
			{

			}

		private void ExtP_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
			{
			// throw new NotImplementedException();
			outputErrorString += e.Data + "\r\n";

			if (!string.IsNullOrWhiteSpace(e.Data))
				{
				outputErrorStringLineaAnteultima = outputErrorStringLineaUltima;
				outputErrorStringLineaUltima = e.Data;
				}
			}

		private void ExtP_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
			{
			// throw new NotImplementedException();
			outputString += e.Data + "\r\n";

			if (!string.IsNullOrWhiteSpace(e.Data))
				{
				outputStringLineaAnteultima = outputStringLineaUltima;
				outputStringLineaUltima = e.Data;
				}
			}

		private string EjecutarAplicacionAndGetOutput(string app, string workingDir, string parametros, out string comandoEjecutado, out System.Diagnostics.Process actualExeProcess, System.Diagnostics.ProcessWindowStyle win_style = System.Diagnostics.ProcessWindowStyle.Hidden)
			{
			comandoEjecutado = "";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			actualExeProcess = process;

			System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

			process.StartInfo.FileName = app;
			process.StartInfo.WorkingDirectory = workingDir;   // directorio de trabajo
															   // extP.StartInfo.Arguments = parametros + " -y -stats";
															   // extP.StartInfo.Arguments = parametros + " -y";
			process.StartInfo.Arguments = parametros;

			process.StartInfo.UseShellExecute = false;

			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;

			// process.StartInfo.RedirectStandardInput = false;
			process.StartInfo.RedirectStandardInput = true;

			process.StartInfo.ErrorDialog = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WindowStyle = win_style;
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

			comandoQueSeEjeuta = comandoEjecutado = process.StartInfo.FileName + " " + process.StartInfo.Arguments;

			outputErrorString += "exe line:\r\n" + comandoEjecutado + "\r\n\r\n";

			process.OutputDataReceived += ExtP_OutputDataReceived;
			process.ErrorDataReceived += ExtP_ErrorDataReceived;

			startTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss.ffff");
			timeOn.Restart();
			process.Start();

			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			//int coresTotales = System.Environment.ProcessorCount;
			//extP.ProcessorAffinity = (IntPtr)(1 /*+ 2 + 4 + 8 + 16 + 32 + 64 + 128*/);
			process.WaitForExit();
			timeOn.Stop();
			endTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss.ffff");

			process.Close();

			comandoQueSeEjeuta += $"elapsed time: ({timeOn.ElapsedMilliseconds:N0} ms)";
			return outputString + "\r\n" + outputErrorString + "\r\n";
			}

		private string EjecutarAplicacionAndGetOutput_1procesador(string app, string workingDir, string parametros, out string comandoEjecutado, System.Diagnostics.ProcessWindowStyle win_style = System.Diagnostics.ProcessWindowStyle.Hidden)
			{
			comandoEjecutado = "";
			exe_process = new System.Diagnostics.Process();
			System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

			exe_process.StartInfo.FileName = app;
			exe_process.StartInfo.WorkingDirectory = workingDir;   // directorio de trabajo
																   // extP.StartInfo.Arguments = parametros + " -y -stats";
																   // extP.StartInfo.Arguments = parametros + " -y";
			exe_process.StartInfo.Arguments = parametros;

			exe_process.StartInfo.UseShellExecute = false;

			exe_process.StartInfo.RedirectStandardOutput = true;
			exe_process.StartInfo.RedirectStandardError = true;

			// exe_process.StartInfo.RedirectStandardInput = false;
			exe_process.StartInfo.RedirectStandardInput = true;

			exe_process.StartInfo.ErrorDialog = false;
			exe_process.StartInfo.CreateNoWindow = true;
			exe_process.StartInfo.WindowStyle = win_style;
			exe_process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

			comandoQueSeEjeuta = comandoEjecutado = exe_process.StartInfo.FileName + " " + exe_process.StartInfo.Arguments;

			outputErrorString += "exe line:\r\n" + comandoEjecutado + "\r\n\r\n";

			exe_process.OutputDataReceived += ExtP_OutputDataReceived;
			exe_process.ErrorDataReceived += ExtP_ErrorDataReceived;

			startTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss.ffff");
			timeOn.Restart();
			exe_process.Start();

			exe_process.BeginOutputReadLine();
			exe_process.BeginErrorReadLine();

			int coresTotales = System.Environment.ProcessorCount;
			exe_process.ProcessorAffinity = (IntPtr)(1 /*+ 2 + 4 + 8 + 16 + 32 + 64 + 128*/);
			exe_process.WaitForExit();

			exe_process.Close();
			exe_process.Dispose();
			exe_process = null;

			timeOn.Stop();
			endTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss.ffff");

			return outputString + "\r\n" + outputErrorString + "\r\n";
			}

		private string EjecutarAplicacionAndGetOutputWithLetInput(string app, string workingDir, string parametros, out string comandoEjecutado, out System.Diagnostics.Process actualExeProcess, System.Diagnostics.ProcessWindowStyle win_style = System.Diagnostics.ProcessWindowStyle.Hidden)
			{
			comandoEjecutado = "";
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			actualExeProcess = exe_process = process;

			System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

			process.StartInfo.FileName = app;
			process.StartInfo.WorkingDirectory = workingDir;   // directorio de trabajo
															   // extP.StartInfo.Arguments = parametros + " -y -stats";
															   // extP.StartInfo.Arguments = parametros + " -y";
			process.StartInfo.Arguments = parametros;

			process.StartInfo.UseShellExecute = false;

			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;

			// process.StartInfo.RedirectStandardInput = false;
			process.StartInfo.RedirectStandardInput = true;

			process.StartInfo.ErrorDialog = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WindowStyle = win_style;
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

			comandoQueSeEjeuta = comandoEjecutado = process.StartInfo.FileName + " " + process.StartInfo.Arguments;

			outputErrorString += "exe line:\r\n" + comandoEjecutado + "\r\n\r\n";

			process.OutputDataReceived += ExtP_OutputDataReceived;
			process.ErrorDataReceived += ExtP_ErrorDataReceived;

			startTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss.ffff");
			timeOn.Restart();
			process.Start();

			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			//int coresTotales = System.Environment.ProcessorCount;
			//extP.ProcessorAffinity = (IntPtr)(1 /*+ 2 + 4 + 8 + 16 + 32 + 64 + 128*/);
			try
				{
				process.WaitForExit();
				}
			catch (Exception ex)
				{
				}
			timeOn.Stop();
			endTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss.ffff");

			process.Close();

			comandoQueSeEjeuta += $"elapsed time: ({timeOn.ElapsedMilliseconds:N0} ms)";
			return outputString + "\r\n" + outputErrorString + "\r\n";
			}

		//public override string ToString()
		//	{
		//	return $"{(onWork ? "* " : "  ")} {outputErrorStringLineaUltima}";
		//	}

		}
	}
