// ConexionesRTSP.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#define DEFAULT_PORT 554
#define DEFAULT_BUFLEN (65536 * 2)

#include <iostream>
#include <fstream>
#include <strstream>
#include <sstream>
#include <vector>

#include <winsock2.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <locale>
#include <algorithm>

#include <conio.h>

#pragma comment(lib, "Ws2_32.lib")

class SequenciaRequestRTSP
	{
	public:
		bool respuestaOK;
		int contentLength;

		int iStrSeq;
		std::string sStrSeq;

		std::vector<std::string> lineasDeRespuesta;
		std::string strRespuesta;

		std::vector<std::string> lineasDeDescribe;
		std::string strDescribe;

		std::string strComando;

		std::string strTrackVideo;
		std::string strTrackAudio;

		std::string strSession;

		SequenciaRequestRTSP()
			: respuestaOK(false), contentLength(-1), iStrSeq(-1)
			{

			}
	};

class MemoryBuffer
	{
	public:
		MemoryBuffer(int bufferLength)
			: iBufLen(0), pBuffer(nullptr), iTailPos(0)
			{
			iBufLen = bufferLength;
			pBuffer = (byte*)std::malloc(iBufLen);

			errorReport << "Inicio de MemoryBuffer con " << iBufLen << "bytes\r\n";
			}

		~MemoryBuffer()
			{
			std:free(pBuffer);
			iBufLen = 0;
			pBuffer = nullptr;
			}

		bool AddData(byte* pData, int iDataLen)
			{
			if (iDataLen <= 0)
				{
				errorReport << "Error el data length no es mayor que cero " << iDataLen << "bytes mas\r\n";
				return false;
				}

			if ((iTailPos + iDataLen) >= iBufLen)
				{
				errorReport << "Error buffer no alcansa para " << iDataLen << "bytes mas\r\n";
				return false;
				}

			MoveMemory(&pBuffer[iTailPos], pData, iDataLen);
			iTailPos += iDataLen;
			return true;
			}

		std::ofstream h264File;

		std::ofstream h264FileLengts;
		bool WriteDataToFile(std::ofstream& destinoFile, std::ofstream& lengtsFile)
			{
			if (destinoFile.is_open())
				{
				destinoFile.write((const char*)pBuffer, iTailPos);
				}

			if (lengtsFile.is_open())
				{
				lengtsFile.write((char*)&iTailPos, sizeof(int));
				}

			// after save reset counters
			iTailPos = 0;

			return true;
			}

		std::stringstream errorReport;

		int iTailPos;

		int iBufLen;
		byte* pBuffer;
	};

// retorna la cantidad de bytes/caracteres consumidos del buffer de datos
int ProcesarRecepcionDeComandosRTSP(bool &comandoCompletado, bool& partialWork, std::vector<std::string>& lineasRespuesta, std::vector<std::string>& lineasExtraData, std::string &strRxRTSP, std::string &strRxRTSPExtraData, int dataOffset, int dataFirst, int dataEnd, char* recvbuf)
	{
	comandoCompletado = partialWork = false;

	std::string strTemporal;
	std::string strTemporalTodo;

	//std::string strRxRTSP;
	//std::string strRxRTSPExtraData;

	int iExtraLength = 0;
	int actual_line_count = 0;
	int total_count = 0;		// total de bytes/caracteres consumidos del buffer de datos

	int contentLength = 0;

	bool bAgregarAExtraData = false;

	// int orig_num_bytes_in_buffer = numBytesInBuffer;

	// byte[] local_buff = new byte[16];

	char char_rx = (char)0;
	while (true)
		{
		if (dataFirst < dataEnd)
			{
			// hay datos
			if (recvbuf[dataFirst] == '$')
				{
				// esto llego a su fin, esto ya corresponde a un bloque de los streams
				// hay datos para procesar. puede o no que se hallan completados los datos del comando

				return total_count;
				}

			char_rx = recvbuf[dataFirst++];
			strTemporal += char_rx;
			strTemporalTodo += char_rx;
			}
		else
			{
			// nos quedamos sin datos, hay que pedir mas
			// o terminamos el proceso y esperamos que la capa superior pida mas datos

			// nos quedamos sin datos, terminamos la tarea, que la capa superior pida mas datos
			partialWork = true;
			return total_count;
			}

		//if (numBytesInBuffer > 0)
		//	{
		//	// tenemos info en el buffer, consumimos de ahi...
		//	char_rx = (char)sckt_rx_buffer[offset++];
		//	strRxRTSP += char_rx;
		//	numBytesInBuffer--;
		//	}
		//else
		//	{
		//	// ya no hay info en el buffer, tenemos que recibir del canal
		//	// y en un buffer local, poco eficaz pero no ocurre mucho
		//	int rxb = sckt_conexion_con_camara.Receive(local_buff, 0, 1, SocketFlags.None);
		//	lastRX = DateTime.Now;
		//	char_rx = (char)local_buff[0];
		//	strRxRTSP += char_rx;

		//	if (fsAllRxData != null)
		//		{
		//		fsAllRxData.WriteByte(local_buff[0]);
		//		fsAllRxData.Flush();
		//		}

		//	if (fsRtdTxRxData != null)
		//		{
		//		fsRtdTxRxData.WriteByte(local_buff[0]);
		//		}
		//	}


		// sin importar cual es el caracter, lo clasificamos en respuesta y extra data
		if (bAgregarAExtraData)
			{
			strRxRTSP += char_rx;
			}
		else
			{
			strRxRTSPExtraData += char_rx;
			}

		if (char_rx == '\r')
			{
			}
		else if (char_rx == '\n')
			{
			// final de linea
			// --------------

			if (bAgregarAExtraData)
				{
				lineasExtraData.push_back(strTemporal);
				if (actual_line_count == 0)
					{
					// linea vacia en extra data, esto es el final del comando
					comandoCompletado = true;
					break;
					}
				}
			else
				{
				lineasRespuesta.push_back(strTemporal);

				// to lower, para chequeos
				std::string line(strTemporal);
				std::for_each(line.begin(), line.end(), [](char& c) { c = ::tolower(c); });

				if (line.find("content-length: ") == 0)
					{
					// Content-Length line
					contentLength = atoi(line.substr(16, 8).c_str());
					}

				if (actual_line_count == 0)
					{
					// linea vacia, fin de la primer parte
					if (contentLength > 0)
						{
						// Tenemos contenido extra, como en DESCRIBE, a partir de ahora se agregan a lineasExtraData
						bAgregarAExtraData = true;
						}
					else
						{
						// linea vacia sin extra data
						comandoCompletado = true;
						break;
						}
					}
				}

			strTemporal.clear();
			actual_line_count = 0;
			}
		else
			{
			actual_line_count++;
			total_count++;
			}
		}

	// llegamos al final
	return total_count;
	// ProcesarRespuestaRTSP(strRxRTSP, strRxRTSPExtraData);
	}

bool SplitResponseInLines(std::string strRecv, std::vector<std::string>& lines, int& lineaVacia)
	{
	lineaVacia = -1;
	lines.clear();
	std::stringstream input_buffer(strRecv);

	std::string line;
	while (std::getline(input_buffer, line))
		{
		line.erase(std::remove(line.begin(), line.end(), '\r'), line.end());

		// ubicamos la primer linea vacia
		if (line.size() == 0) if (lineaVacia < 0) lineaVacia = lines.size();

		lines.push_back(line);
		}

	return true;
	}

bool ArmarDescribe(std::stringstream& os, const char* szIP, const char* szPORT, const char* szUsuario, const char* szPassword, const char* szStringDeConexion)
	{
	os.clear();
	os << "DESCRIBE rtsp://" << szUsuario << ":" << szPassword << "@" << szIP << ":" << szPORT << szStringDeConexion << " RTSP/1.0\r\n";
	os << "Accept: application/sdp\r\n";
	os << "CSeq: 1\r\n";
	os << "Authorization: Basic YWRtaW46YWRtaW4=\r\n";
	os << "User-Agent: JOCh2021\r\n";
	os << "\r\n";			// Linea final, gatillo
	// os << "\0";

	return true;
	}

/*
SETUP rtsp://192.168.0.90:554/cam/realmonitor/trackID=0?channel=1&subtype=0 RTSP/1.0
Transport: RTP/AVP/TCP;unicast;interleaved=0-1
CSeq: 2
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017
*/
bool ArmarVideoSetup(SequenciaRequestRTSP& sequenciaDESCRIBE, std::stringstream& os, const char* szIP, const char* szPORT, const char* szUsuario, const char* szPassword, const char* szStringDeConexion)
	{
	os.clear();
	os << "SETUP rtsp://" << szIP << ":" << szPORT << szStringDeConexion << "/" << sequenciaDESCRIBE.strTrackVideo << " RTSP/1.0\r\n";
	os << "Transport: RTP/AVP/TCP;unicast;interleaved=0-1\r\n";
	os << "CSeq: " << sequenciaDESCRIBE.iStrSeq + 1 << "\r\n";
	os << "Authorization: Basic YWRtaW46YWRtaW4=\r\n";
	os << "User-Agent: JOCh2021\r\n";
	os << "\r\n";			// Linea final, gatillo

	return true;
	}

/*
PLAY rtsp://192.168.0.90:554/cam/realmonitor?channel=1&subtype=0 RTSP/1.0
Session: 80518407122
CSeq: 3
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017
*/
bool ArmarVideoPlay(SequenciaRequestRTSP& sequenciaSETUP, std::stringstream& os, const char* szIP, const char* szPORT, const char* szUsuario, const char* szPassword, const char* szStringDeConexion)
	{
	os.clear();
	os << "PLAY rtsp://" << szIP << ":" << szPORT << szStringDeConexion << " RTSP/1.0\r\n";
	os << "Session: " << sequenciaSETUP.strSession << "\r\n";
	os << "CSeq: " << sequenciaSETUP.iStrSeq + 1 << "\r\n";
	os << "Authorization: Basic YWRtaW46YWRtaW4=\r\n";
	os << "User-Agent: JOCh2021\r\n";
	os << "\r\n";			// Linea final, gatillo

	return true;
	}

bool ParseDescribeResponse(std::string strRecv, std::vector<std::string>& lines, SequenciaRequestRTSP& seqRTSP)
	{
	lines.clear();
	int divideLine = -1;

	if (SplitResponseInLines(strRecv, lines, divideLine))
		{
		// buscamos la linea en 0, que en describe divide la parte de respuesta del protocolo de la parte de describe propiamente dicha
		for (int x = 0; x < lines.size(); x++)
			{
			if (lines[x].size() == 0)
				{
				divideLine = x;
				}
			}

		// el DESCRIBE si o si tiene dos partes separadas por una linea vacia
		if (divideLine < 0) return false;

		//bool respuestaOK = false;
		//int contentLength = -1;

		bool videoON = false;
		bool audioON = false;

		//int iStrSeq = -1;
		//std::string sStrSeq;

		// primero parseamos la primer parte de la respuesta, que es la respuesta del protocolo
		// ------------------------------------------------------------------------------------
		// RTSP/1.0 200 OK
		// Server: Rtsp Server/2.0
		// CSeq: 1
		// Content-Base: rtsp://admin:admin@192.168.0.90:554/cam/realmonitor?channel=1&subtype=0
		// Content-Type: application/sdp
		// Content-Length: 358
		// Cache-Control: must-revalidate
		// x-Accept-Dynamic-Rate: 1
		if (strstr(lines[0].c_str(), "200 OK"))
			{
			seqRTSP.respuestaOK = true;
			seqRTSP.lineasDeRespuesta.push_back(lines[0]);
			}
		else
			{
			return false;
			}

		seqRTSP.strComando = "DESCRIBE";

		for (int x = 1; x < divideLine; x++)
			{
			seqRTSP.lineasDeRespuesta.push_back(lines[x]);

			std::string line(lines[x]);
			std::for_each(line.begin(), line.end(), [](char& c) { c = ::tolower(c); });

			if (line.find("server: ") == 0)
				{
				}
			else if (line.find("cseq: ") == 0)
				{
				// CSeq: 1
				seqRTSP.sStrSeq = line.substr(6, 8);
				seqRTSP.iStrSeq = atoi(seqRTSP.sStrSeq.c_str());
				}
			else if (line.find("content-base: ") == 0)
				{
				// Content-Base
				}
			else if (line.find("content-type: ") == 0)
				{
				// Content-Type
				}
			else if (line.find("content-length: ") == 0)
				{
				// Content-Length
				seqRTSP.contentLength = atoi(line.substr(16, 8).c_str());
				}
			}

		// ahora parseamos la parte del describe propiamente dicha
		// -------------------------------------------------------
		// v=0
		// o=- 2251978910 2251978910 IN IP4 0.0.0.0
		// s=RTSP Session/2.0
		// c=IN IP4 0.0.0.0
		// t=0 0
		// a=control:*
		// a=range:npt=now-
		// a=packetization-supported:DH
		// m=video 0 RTP/AVP 96
		// a=control:trackID=0
		// a=framerate:20.000000
		// a=rtpmap:96 H264/90000
		// a=fmtp:96 packetization-mode=1;profile-level-id=4D001F;sprop-parameter-sets=Z00AH5WoFAFuQA==,aO48gA==
		// a=recvonly
		for (int x = divideLine + 1; x < lines.size(); x++)
			{
			seqRTSP.lineasDeDescribe.push_back(lines[x]);

			std::string line(lines[x]);
			std::for_each(line.begin(), line.end(), [](char& c) { c = ::tolower(c); });

			if (line.find("v=") == 0)
				{
				// v=0
				}
			else if (line.find("o=") == 0)
				{
				// o=- 2251978910 2251978910 IN IP4 0.0.0.0
				// Origin
				// owner/creator and session identifier
				// o=<username> <session id> <version> <network type> <address type> <address>
				// o=- 15085671366927581184 15085671366927581184 IN IP4 posip-laptop
				// o=- 533369188 533369188 IN IP4 127.0.0.1
				// o=- 1275849093732432 1275849093732432 IN IP4 190.221.15.2
				}
			else if (line.find("s=") == 0)
				{
				// s=RTSP Session/2.0
				}
			else if (line.find("c=") == 0)
				{
				// c=IN IP4 0.0.0.0
				}
			else if (line.find("t=") == 0)
				{
				// t=0 0
				}
			else if (line.find("a=control:*") == 0)
				{
				// a=control:*
				}
			else if (line.find("a=range") == 0)
				{
				// a=range:npt=now-
				}
			else if (line.find("a=packetization-supported") == 0)
				{
				// a=packetization-supported:DH
				}
			else if (line.find("m=video") == 0)
				{
				// m=video 0 RTP/AVP 96
				videoON = true;
				audioON = false;
				}
			else if (line.find("m=audio") == 0)
				{
				// m=audio 0 RTP/AVP 96
				videoON = false;
				audioON = true;
				}
			else if (line.find("a=control:t") == 0)
				{
				// a=control:trackID=0
				if (videoON)
					{
					seqRTSP.strTrackVideo = line.substr(10, 32);
					}
				else if (audioON)
					{
					seqRTSP.strTrackAudio = line.substr(10, 32);
					}
				}
			else if (line.find("a=framerate:") == 0)
				{
				// a=framerate:20.000000
				}
			else if (line.find("a=rtpmap:") == 0)
				{
				// a=rtpmap:96 H264/90000
				if (videoON)
					{
					}
				else if (audioON)
					{
					}
				}
			else if (line.find("a=fmtp:") == 0)
				{
				// a=fmtp:96 packetization-mode=1;profile-level-id=4D001F;sprop-parameter-sets=Z00AH5WoFAFuQA==,aO48gA==
				}
			else if (line.find("a=recvonly") == 0)
				{
				// a=recvonly
				}
			else if (line.find("") == 0)
				{
				// 
				}
			else if (line.find("") == 0)
				{
				// 
				}
			else if (line.find("") == 0)
				{
				// 
				}
			}

		return true;
		}

	return false;
	}

bool ParseSetupResponse(std::string strRecv, std::vector<std::string>& lines, SequenciaRequestRTSP& seqDescribeRTSP, SequenciaRequestRTSP& seqRTSP)
	{
	int lineaVacia = -1;
	lines.clear();
	if (SplitResponseInLines(strRecv, lines, lineaVacia))
		{
		// parseamos la respuesta
		// ----------------------
		//RTSP/1.0 200 OK
		//Server: Rtsp Server/2.0
		//CSeq: 2
		//Session: 178590884951;timeout=60
		//Transport: RTP/AVP/TCP;unicast;interleaved=0-1;ssrc=CC800354
		//x-dynamic-rate: 1
		if (strstr(lines[0].c_str(), "200 OK"))
			{
			seqRTSP.respuestaOK = true;
			seqRTSP.lineasDeRespuesta.push_back(lines[0]);
			}
		else
			{
			return false;
			}

		seqRTSP.strComando = "SETUP";

		for (int x = 1; x < lines.size(); x++)
			{
			seqRTSP.lineasDeRespuesta.push_back(lines[x]);

			std::string line(lines[x]);
			std::for_each(line.begin(), line.end(), [](char& c) { c = ::tolower(c); });

			if (line.find("server: ") == 0)
				{
				}
			else if (line.find("cseq: ") == 0)
				{
				// CSeq: 2
				seqRTSP.sStrSeq = line.substr(6, 8);
				seqRTSP.iStrSeq = atoi(seqRTSP.sStrSeq.c_str());
				}
			else if (line.find("session: ") == 0)
				{
				// Session: 178590884951;timeout=60
				seqRTSP.strSession = line.substr(9, 12);
				}
			else if (line.find("transport: ") == 0)
				{
				// Transport: RTP/AVP/TCP;unicast;interleaved=0-1;ssrc=CC800354
				}
			else if (line.find("x-dynamic-rate: ") == 0)
				{
				// x-dynamic-rate: 1
				}
			}

		return true;
		}

	return false;
	}

/*
RTSP/1.0 200 OK
Server: Rtsp Server/2.0
CSeq: 3
Session: 80518407122
RTP-Info: url=trackID=0;seq=14941;rtptime=135580723;ssrc=4294965765
Range: npt=0.000000-
*/
bool ParsePlayResponse(std::string strRecv, std::vector<std::string>& lines, SequenciaRequestRTSP& seqSetupRTSP, SequenciaRequestRTSP& seqRTSP)
	{
	int lineaVacia = -1;
	lines.clear();
	if (SplitResponseInLines(strRecv, lines, lineaVacia))
		{
		// parseamos la respuesta
		if (strstr(lines[0].c_str(), "200 OK"))
			{
			seqRTSP.respuestaOK = true;
			seqRTSP.lineasDeRespuesta.push_back(lines[0]);
			}
		else
			{
			return false;
			}

		seqRTSP.strComando = "PLAY";

		for (int x = 1; x < lines.size(); x++)
			{
			seqRTSP.lineasDeRespuesta.push_back(lines[x]);

			std::string line(lines[x]);
			std::for_each(line.begin(), line.end(), [](char& c) { c = ::tolower(c); });

			if (line.find("server: ") == 0)
				{
				}
			else if (line.find("cseq: ") == 0)
				{
				// CSeq: 2
				seqRTSP.sStrSeq = line.substr(6, 8);
				seqRTSP.iStrSeq = atoi(seqRTSP.sStrSeq.c_str());
				}
			else if (line.find("session: ") == 0)
				{
				// Session: 80518407122
				seqRTSP.strSession = line.substr(9, 12);
				}
			else if (line.find("rtp-info: ") == 0)
				{
				// RTP-Info: url=trackID=0;seq=14941;rtptime=135580723;ssrc=4294965765
				}
			else if (line.find("range: ") == 0)
				{
				// Range: npt=0.000000-
				}
			}

		return true;
		}

	return false;
	}

// retorna true si el comando se procesa, en todos los casos se limpia la informacion despues del proceso
bool ParseCommandoResponseGenerico(std::vector<std::string>& lineasRespuesta, std::vector<std::string>& lineasExtraData, std::string& strRxRTSP, std::string& strRxRTSPExtraData, int &iStrSeq, std::string &strSession)
	{
	bool respuestaOK = false;
	std::string sStrSeq;
	// int iStrSeq = -1;
	// std::string strSession;

	if (strstr(lineasRespuesta[0].c_str(), "200 OK"))
		{
		respuestaOK = true;
		}
	else
		{
		return false;
		}

	for (int x = 1; x < lineasRespuesta.size(); x++)
		{
		std::string line(lineasRespuesta[x]);
		std::for_each(line.begin(), line.end(), [](char& c) { c = ::tolower(c); });

		if (line.find("server: ") == 0)
			{
			}
		else if (line.find("cseq: ") == 0)
			{
			// CSeq: 2
			sStrSeq = line.substr(6, 8);
			iStrSeq = atoi(sStrSeq.c_str());
			}
		else if (line.find("session: ") == 0)
			{
			// Session: 80518407122
			strSession = line.substr(9, 12);
			}
		else if (line.find("rtp-info: ") == 0)
			{
			// RTP-Info: url=trackID=0;seq=14941;rtptime=135580723;ssrc=4294965765
			}
		else if (line.find("range: ") == 0)
			{
			// Range: npt=0.000000-
			}
		}

	return true;

	return false;
	}

#if (false)
int old_main(int argc, char* argv[])
	{
	if (argc < 3)
		{
		std::cout << "Numero de parametros no validos: " << argc << "\n ";
		std::cout << "El modo de uso es: ConexionRTSP <ip> <port> <usuario> <password> <stringDeConexion>" << argc << "\n ";
		return -1;
		}

	int iResult;

	WSADATA wsaData;

	SOCKET ConnectSocket = INVALID_SOCKET;
	struct sockaddr_in clientService;

	int recvbuflen = DEFAULT_BUFLEN;
	char recvbuf[DEFAULT_BUFLEN] = "";
	// char* sendbuf = "Client: sending data test";

	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0)
		{
		std::cout << "WSAStartup failed: " << iResult << "\n ";
		printf("WSAStartup failed: %d\n", iResult);
		return 1;
		}

	struct addrinfo* result = NULL, * ptr = NULL, hints;

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	// Resolve the server address and port
	iResult = getaddrinfo(argv[1], argv[2], &hints, &result);
	if (iResult != 0)
		{
		printf("getaddrinfo failed: %d\n", iResult);
		WSACleanup();
		return 1;
		}

	SOCKET ConnectSocket = INVALID_SOCKET;

	Attempt to connect to the first address returned by
		the call to getaddrinfo
		ptr = result;

	   // Create a SOCKET for connecting to server
	ConnectSocket = socket(ptr->ai_family, ptr->ai_socktype, ptr->ai_protocol);

	if (ConnectSocket == INVALID_SOCKET)
		{
		printf("Error at socket(): %ld\n", WSAGetLastError());
		freeaddrinfo(result);
		WSACleanup();
		return 1;
		}

	// Connect to server.
	iResult = connect(ConnectSocket, ptr->ai_addr, (int)ptr->ai_addrlen);
	if (iResult == SOCKET_ERROR)
		{
		closesocket(ConnectSocket);
		ConnectSocket = INVALID_SOCKET;
		}

	// Should really try the next address returned by getaddrinfo
	// if the connect call failed
	// But for this simple example we just free the resources
	// returned by getaddrinfo and print an error message
	freeaddrinfo(result);

	if (ConnectSocket == INVALID_SOCKET)
		{
		printf("Unable to connect to server!\n");
		WSACleanup();
		return 1;
		}

	int recvbuflen = DEFAULT_BUFLEN;

	// const char* sendbuf = "this is a test";
	char recvbuf[DEFAULT_BUFLEN];

	std::stringstream os;

	if (!ArmarDescribe(os, argv[1], argv[2], "admin", "admin", "/cam/realmonitor?channel=1&subtype=0"))
		{
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	std::string str = os.str();

	// Send an initial buffer
	// iResult = send(ConnectSocket, sendbuf, (int)strlen(sendbuf), 0);
	iResult = send(ConnectSocket, os.str().c_str(), (int)os.str().size(), 0);
	if (iResult == SOCKET_ERROR)
		{
		printf("send failed: %d\n", WSAGetLastError());
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	printf("Bytes Sent: %ld\n", iResult);

	// shutdown the connection for sending since no more data will be sent
	// the client can still use the ConnectSocket for receiving data
	//iResult = shutdown(ConnectSocket, SD_SEND);
	//if (iResult == SOCKET_ERROR)
	//	{
	//	printf("shutdown failed: %d\n", WSAGetLastError());
	//	closesocket(ConnectSocket);
	//	WSACleanup();
	//	return 1;
	//	}

	// Receive data until the server closes the connection
	do
		{
		iResult = recv(ConnectSocket, recvbuf, recvbuflen, 0);

		if (iResult > 0)
			printf("Bytes received: %d\n", iResult);
		else if (iResult == 0)
			printf("Connection closed\n");
		else
			printf("recv failed: %d\n", WSAGetLastError());
		} while (iResult > 0);

		closesocket(ConnectSocket);
		WSACleanup();
		if (argc > 1)
			{
			std::cout << argv[0] << "\n";
			std::cout << argv[1] << "\n";
			// std::cout << "Hello World!\n";
	}
	}
#endif

int main(int argc, char* argv[])
	{
	if (argc < 5)
		{
		std::cout << "Numero de parametros no validos: " << argc << "\r\n";
		std::cout << "El modo de uso es: ConexionRTSP <ip> <port> <usuario> <password> <stringDeConexion>" << argc << "\r\n";
		return -1;
		}

	std::cout << "ConexionesRTSP args: " << argc << "\r\n";

	//----------------------
	// Declare and initialize variables.
	int iResult;
	WSADATA wsaData;

	SOCKET ConnectSocket = INVALID_SOCKET;
	struct sockaddr_in clientService;

	int recvbuflen = DEFAULT_BUFLEN;
	// char* sendbuf = "Client: sending data test";
	byte recvbuf[DEFAULT_BUFLEN] = "";

	//----------------------
	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != NO_ERROR)
		{
		wprintf(L"WSAStartup failed with error: %d\n", iResult);
		return 1;
		}

	//----------------------
	// Create a SOCKET for connecting to server
	ConnectSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (ConnectSocket == INVALID_SOCKET)
		{
		wprintf(L"socket failed with error: %ld\n", WSAGetLastError());
		WSACleanup();
		return 1;
		}

	//----------------------
	// The sockaddr_in structure specifies the address family,
	// IP address, and port of the server to be connected to.
	clientService.sin_family = AF_INET;
	// clientService.sin_addr.s_addr = inet_addr("127.0.0.1");
	// clientService.sin_addr.s_addr = inet_addr(argv[1]);
	clientService.sin_port = htons(DEFAULT_PORT);

	InetPtonA(AF_INET, argv[1], &clientService.sin_addr.s_addr);

	//----------------------
	// Connect to server.
	iResult = connect(ConnectSocket, (SOCKADDR*)&clientService, sizeof(clientService));
	if (iResult == SOCKET_ERROR)
		{
		wprintf(L"connect failed with error: %d\n", WSAGetLastError());
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	std::stringstream os;

	if (!ArmarDescribe(os, argv[1], argv[2], "admin", "admin", "/cam/realmonitor?channel=1&subtype=0"))
		{
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	std::string strSend = os.str();

	std::ofstream h264FramesFile;
	h264FramesFile.open("d:\\Data\\video\\rtsp\\a.captura.raw.h264", std::ios::binary);

	std::ofstream h264FramesFileLengts;
	h264FramesFileLengts.open("d:\\Data\\video\\rtsp\\a.captura.h264.lengts.bin", std::ios::binary);

	// files para salvar lo que se recibe de modo crudo
	std::ofstream rxFileRaws;
	rxFileRaws.open("d:\\Data\\video\\rtsp\\b.captura.rx.raw.bin", std::ios::binary);

	std::ofstream rxFileRawsLengts;
	rxFileRawsLengts.open("d:\\Data\\video\\rtsp\\b.captura.rx.lengts.bin", std::ios::binary);

	// files para salvar lo que se recibe de modo crudo
	std::ofstream rxFileRawsRTD;
	rxFileRawsRTD.open("d:\\Data\\video\\rtsp\\c.captura.rtd.rx.raw.bin", std::ios::binary);

	std::ofstream rxFileRawsLengtsRTD;
	rxFileRawsLengtsRTD.open("d:\\Data\\video\\rtsp\\c.captura.rtd.rx.lengts.bin", std::ios::binary);

	//----------------------
	// Send DESCRIBE request
	// iResult = send(ConnectSocket, sendbuf, (int)strlen(sendbuf), 0);
	iResult = send(ConnectSocket, strSend.c_str(), (int)strlen(strSend.c_str()), 0);
	if (iResult == SOCKET_ERROR)
		{
		wprintf(L"send failed with error: %d\n", WSAGetLastError());
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	// rec send data
	rxFileRaws.write(strSend.c_str(), iResult);
	rxFileRaws.flush();
	rxFileRawsLengts.write((char*)&iResult, sizeof(int));
	rxFileRawsLengts.flush();

	printf("DESCRIBE Bytes Sent: %d\n", iResult);

	SequenciaRequestRTSP seqDescribeRTSP;

	iResult = recv(ConnectSocket, (char*)recvbuf, recvbuflen, 0);
	if (iResult > 0)
		{
		rxFileRaws.write((const char *)recvbuf, iResult);				// write raw data, solo lo que se acaba de leer
		rxFileRaws.flush();
		rxFileRawsLengts.write((char *)&iResult, sizeof(int));				// write raw data, solo lo que se acaba de leer
		rxFileRawsLengts.flush();

		recvbuf[iResult] = 0;
		std::string strRecv = (const char*)recvbuf;
		recvbuf[0] = 0;

		wprintf(L"Bytes received: %d\n", iResult);

		std::vector<std::string> lines;
		if (!ParseDescribeResponse(strRecv, lines, seqDescribeRTSP))
			{
			iResult = closesocket(ConnectSocket);
			WSACleanup();
			return 1;
			}
		}
	else if (iResult == 0)
		{
		wprintf(L"Connection closed\n");
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}
	else
		{
		wprintf(L"recv failed with error: %d\n", WSAGetLastError());
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	if (!seqDescribeRTSP.respuestaOK)
		{
		// no fue valida la respuesta del describe, aca termina todo
		printf("DESCRIBE sin OK: %s\n", seqDescribeRTSP.strComando);
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	// Hasta aqui ok la recepcion del DESCRIBE
	// ---------------------------------------
	wprintf(L"OK rx DESCRIBE: %d\n", iResult);

	// Send video SETUP
	// ----------------
	std::stringstream stup;

	if (!ArmarVideoSetup(seqDescribeRTSP, stup, argv[1], argv[2], "admin", "admin", "/cam/realmonitor?channel=1&subtype=0"))
		{
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	std::string strToSend = stup.str();

	// send SETUP
	iResult = send(ConnectSocket, stup.str().c_str(), (int)strlen(stup.str().c_str()), 0);
	if (iResult == SOCKET_ERROR)
		{
		wprintf(L"send failed with error: %d\n", WSAGetLastError());
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	// rec send data
	rxFileRaws.write(stup.str().c_str(), iResult);
	rxFileRaws.flush();
	rxFileRawsLengts.write((char*)&iResult, sizeof(int));
	rxFileRawsLengts.flush();

	printf("SETUP Bytes Sent: %d\n", iResult);

	SequenciaRequestRTSP seqSetupRTSP;

	iResult = recv(ConnectSocket, (char*)recvbuf, recvbuflen, 0);
	if (iResult > 0)
		{
		rxFileRaws.write((const char*)recvbuf, iResult);				// write raw data, solo lo que se acaba de leer
		rxFileRaws.flush();
		rxFileRawsLengts.write((char*)&iResult, sizeof(int));				// write raw data, solo lo que se acaba de leer
		rxFileRawsLengts.flush();

		recvbuf[iResult] = 0;
		std::string strRecv = (const char*)recvbuf;
		recvbuf[0] = 0;

		wprintf(L"Bytes received: %d\n", iResult);

		std::vector<std::string> lines;
		if (!ParseSetupResponse(strRecv, lines, seqDescribeRTSP, seqSetupRTSP))
			{
			iResult = closesocket(ConnectSocket);
			WSACleanup();
			return 1;
			}
		}
	else if (iResult == 0)
		{
		wprintf(L"Connection closed\n");
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}
	else
		{
		wprintf(L"recv failed with error: %d\n", WSAGetLastError());
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	if (!seqSetupRTSP.respuestaOK)
		{
		// no fue valida la respuesta del setup, aca termina todo
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	// Send video PLAY
	// ----------------
	std::stringstream ply;

	if (!ArmarVideoPlay(seqSetupRTSP, ply, argv[1], argv[2], "admin", "admin", "/cam/realmonitor?channel=1&subtype=0"))
		{
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	// solo para debug
	strToSend = ply.str();

	// send PLAY
	iResult = send(ConnectSocket, ply.str().c_str(), (int)strlen(ply.str().c_str()), 0);
	if (iResult == SOCKET_ERROR)
		{
		wprintf(L"send play failed with error: %d\n", WSAGetLastError());
		closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	// rec send data
	rxFileRaws.write(ply.str().c_str(), iResult);
	rxFileRaws.flush();
	rxFileRawsLengts.write((char*)&iResult, sizeof(int));
	rxFileRawsLengts.flush();

	printf("PLAY Bytes Sent: %d\n", iResult);

	SequenciaRequestRTSP seqPlayRTSP;

	iResult = recv(ConnectSocket, (char*)recvbuf, recvbuflen, 0);
	if (iResult > 0)
		{
		rxFileRaws.write((const char*)recvbuf, iResult);				// write raw data, solo lo que se acaba de leer
		rxFileRaws.flush();
		rxFileRawsLengts.write((char*)&iResult, sizeof(int));				// write raw data, solo lo que se acaba de leer
		rxFileRawsLengts.flush();

		recvbuf[iResult] = 0;
		std::string strRecv = (const char*)recvbuf;
		recvbuf[0] = 0;

		wprintf(L"Bytes received: %d\n", iResult);

		std::vector<std::string> lines;
		if (!ParsePlayResponse(strRecv, lines, seqSetupRTSP, seqPlayRTSP))
			{
			iResult = closesocket(ConnectSocket);
			WSACleanup();
			return 1;
			}
		}
	else if (iResult == 0)
		{
		wprintf(L"Connection closed\n");
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}
	else
		{
		wprintf(L"recv failed with error: %d\n", WSAGetLastError());
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	if (!seqPlayRTSP.respuestaOK)
		{
		// no fue valida la respuesta del setup, aca termina todo
		iResult = closesocket(ConnectSocket);
		WSACleanup();
		return 1;
		}

	// Receive until the peer closes the connection
	int ch, dataOffset = 0, dataFirst = 0, dataEnd = 0, frameCount = 0;
	MemoryBuffer frameHolder(DEFAULT_BUFLEN);

	std::vector<std::string> lineasRespuesta;
	std::vector<std::string> lineasExtraData;
	std::string strRxRTSP;
	std::string strRxRTSPExtraData;

	 int iStrSeq = -1;
	 std::string strSession;
	 bool partialWork = false;
	 bool partialRTD = false;
	 bool continueRX = true;

	do
		{
readMoreData:
		if (dataFirst > 0)
			{
			// hay que ajustar el buffer antes de proseguir
			if (dataFirst < dataEnd)
				{
				MoveMemory(recvbuf, &recvbuf[dataFirst], dataEnd - dataFirst);
				dataEnd -= dataFirst;
				}
			else
				{
				dataEnd = 0;
				}
			dataFirst = 0;
			}
		iResult = recv(ConnectSocket, (char*)&recvbuf[dataEnd], recvbuflen - dataEnd, 0);
		if (iResult > 0)
			{
			rxFileRaws.write((const char*)&recvbuf[dataEnd], iResult);				// write raw data, solo lo que se acaba de leer
			rxFileRaws.flush();
			rxFileRawsLengts.write((char*)&iResult, sizeof(int));				// write raw data, solo lo que se acaba de leer
			rxFileRawsLengts.flush();

			wprintf(L"Bytes received: %d\n", iResult);

			dataEnd += iResult;
			if (dataEnd < recvbuflen) recvbuf[dataEnd] = 0;			// ponemos a cero el caracter de fin del buffer, para posibles string

			if (partialWork)
				{
				// nos habiamos quedado sin datos, vamos a continuar procesando el parseo del comando
				bool bComandoCompleto = false;
				int datosConsumidos = ProcesarRecepcionDeComandosRTSP(bComandoCompleto, partialWork, lineasRespuesta, lineasExtraData, strRxRTSP, strRxRTSPExtraData, dataOffset, dataFirst, dataEnd, (char*)recvbuf);
				dataFirst += datosConsumidos;

				if (bComandoCompleto)
					{
					// se completo la info de al menos un comando, analizamos y procesamos el comando
					if (ParseCommandoResponseGenerico(lineasRespuesta, lineasExtraData, strRxRTSP, strRxRTSPExtraData, iStrSeq, strSession))
						{
						// el problema es de donde venimos, por ahora no hacemos nada mas
						}
					}

				if (dataFirst < dataEnd)
					{
					goto procesarData;
					}
				else
					{
					// aun hay datos en el buffer, vamos a consumirlos
					goto readMoreData;
					}
				}

procesarData:
			// ahora procesamos el buffer de recepcion completo
			if (recvbuf[dataFirst] == '$')
				{
				// el buffer comienza con $
				while (recvbuf[dataFirst] == '$')
					{
					if ((dataFirst + 4) >= dataEnd)
						{
						// faltan datos hay que ir a leer
						// minimo 4 bytes tiene que haber
						goto readMoreData;
						}

					// Capa RTD
					byte streamID = recvbuf[dataFirst + 1];
					int len = ((int)recvbuf[dataFirst + 2] * 256) + recvbuf[dataFirst + 3];
					int lenRTD = len + 4;

					if (len < 0)
						{
						// error en la logica de recepcion, abortamos todo
						continueRX = false;
						break;
						}

					if ((dataFirst + 4 + len) >= dataEnd)
						{
						// faltan datos hay que ir a leer
						goto readMoreData;
						}

					WORD wFlags = ((WORD)recvbuf[dataFirst + 4] << 8) + (WORD)recvbuf[dataFirst + 5];
					bool bLastFrameMark = (recvbuf[dataFirst + 5] & 0x80) == 0 ? false : true;  // marca de ultimo paquete de frame en chunks
					byte byPayloadType = (byte)(recvbuf[dataFirst + 5] & 0x7f);

					WORD wVideoSequencia = ((WORD)recvbuf[dataFirst + 6] << 8) + (WORD)recvbuf[dataFirst + 7];
					DWORD dwTimeStamp = ((DWORD)recvbuf[dataFirst + 8] << 24) + ((DWORD)recvbuf[dataFirst + 9] << 16) + ((DWORD)recvbuf[dataFirst + 10] << 8) + (DWORD)recvbuf[dataFirst + 11];
					DWORD dwSSRC = ((DWORD)recvbuf[dataFirst + 12] << 24) + ((DWORD)recvbuf[dataFirst + 13] << 16) + ((DWORD)recvbuf[dataFirst + 14] << 8) + (DWORD)recvbuf[dataFirst + 15];

					// escribimos la parte h264 del chunk de video
					if (!frameHolder.AddData((byte*)&recvbuf[dataFirst + 16], len - 12))
						{
						// algo salio mal, abortamos todo lo que sigue
						// si hay algo para salvar se salva
						frameHolder.WriteDataToFile(h264FramesFile, h264FramesFileLengts);
						continueRX = false;
						break;
						}

					if (bLastFrameMark)
						{
						// es el ultimo chunk de un frame, grabamos en el file h264, automaticamente pone a cero el buffer
						frameHolder.WriteDataToFile(h264FramesFile, h264FramesFileLengts);
						std::cout << "Write frame: " << ++frameCount << "\n ";
						}

					rxFileRawsRTD.write((const char*)&recvbuf[dataFirst], len + 4);				// write raw data, solo lo que se acaba de leer
					rxFileRawsRTD.flush();
					rxFileRawsLengtsRTD.write((char*)&lenRTD, sizeof(int));				// write raw data, solo lo que se acaba de leer
					rxFileRawsLengtsRTD.flush();
					
					// nos preparamos para el siguiente chunk
					dataFirst += len + 4;

					if (dataFirst == dataEnd)
						{
						// agotamos el buffer, se pone todo a 0
						dataOffset = dataFirst = dataEnd = 0;
						dataFirst = 0;
						}

					if (_kbhit())
						{
						ch = _getch();
						if ((ch == (int)'q') || (ch == (int)'Q'))
							{
							continueRX = false;
							break;
							}
						}
					}

				// luego de leer los chunks pone a 0 el buffer
				}
			else if ((recvbuf[dataFirst] == 'R') || (recvbuf[dataFirst] == 'A') || (recvbuf[dataFirst] == 'G') || (recvbuf[dataFirst] == 'O') || (recvbuf[dataFirst] == 'S'))
				{
				bool bComandoCompleto = false;
				int datosConsumidos = ProcesarRecepcionDeComandosRTSP(bComandoCompleto, partialWork, lineasRespuesta, lineasExtraData, strRxRTSP, strRxRTSPExtraData, dataOffset, dataFirst, dataEnd, (char*)recvbuf);
				dataFirst += datosConsumidos;
				if (bComandoCompleto)
					{
					// se completo la info de al menos un comando, analizamos y procesamos el comando
					if (ParseCommandoResponseGenerico(lineasRespuesta, lineasExtraData, strRxRTSP, strRxRTSPExtraData, iStrSeq, strSession))
						{
						// el problema es de donde venimos, por ahora no hacemos nada mas
						}
					}

				if (dataFirst < dataEnd)
					{
					goto procesarData;
					}
				else
					{
					// aun hay datos en el buffer, vamos a consumirlos
					goto readMoreData;
					}

				}
			else if ((recvbuf[dataFirst] == 'X') && (recvbuf[dataFirst + 1] == 'X'))
				{
				// JOCh tx out of bands
				// ProcesarRecepcionDeComandosRTSP_out_of_band_data_rx(ref rb, ref offset, sckt_rx_buffer);
				}
			else
				{
				// esto es una condicion de error
				std::string strRecv = (const char*)&recvbuf[dataOffset];
				printf("Error Datos: %s\n", strRecv.c_str());

				continueRX = false;
				break;
				}

			}
		else if (iResult == 0)
			wprintf(L"Connection closed\n");
		else
			wprintf(L"recv failed with error: %d\n", WSAGetLastError());

		if (_kbhit())
			{
			ch = _getch();
			if ((ch == (int)'q') || (ch == (int)'Q'))
				{
				continueRX = false;
				break;
				}
			}
		} while ((iResult > 0) && (continueRX));

	h264FramesFile.close();
	h264FramesFileLengts.close();

	rxFileRaws.close();
	rxFileRawsLengts.close();

	rxFileRawsRTD.close();
	rxFileRawsLengtsRTD.close();

		// close the socket
		iResult = closesocket(ConnectSocket);
		if (iResult == SOCKET_ERROR)
			{
			wprintf(L"close failed with error: %d\n", WSAGetLastError());
			WSACleanup();
			return 1;
			}

		WSACleanup();
		return 0;
	}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file

/*

rx from camera
RTSP/1.0 200 OK
Server: Rtsp Server/2.0
CSeq: 1
Content-Base: rtsp://admin:admin@192.168.0.90:554/cam/realmonitor?channel=1&subtype=0
Content-Type: application/sdp
Content-Length: 358
Cache-Control: must-revalidate
x-Accept-Dynamic-Rate: 1

v=0
o=- 2251978910 2251978910 IN IP4 0.0.0.0
s=RTSP Session/2.0
c=IN IP4 0.0.0.0
t=0 0
a=control:*
a=range:npt=now-
a=packetization-supported:DH
m=video 0 RTP/AVP 96
a=control:trackID=0
a=framerate:20.000000
a=rtpmap:96 H264/90000
a=fmtp:96 packetization-mode=1;profile-level-id=4D001F;sprop-parameter-sets=Z00AH5WoFAFuQA==,aO48gA==
a=recvonly


*/

/*
DESCRIBE rtsp://192.168.0.90:554/cam/realmonitor?channel=1&subtype=0 RTSP/1.0
Accept: application/sdp
CSeq: 1
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017


RTSP/1.0 200 OK
Server: Rtsp Server/2.0
CSeq: 1
Content-Base: rtsp://192.168.0.90:554/cam/realmonitor?channel=1&subtype=0
Content-Type: application/sdp
Content-Length: 358
Cache-Control: must-revalidate
x-Accept-Dynamic-Rate: 1


v=0
o=- 2252115307 2252115307 IN IP4 0.0.0.0
s=RTSP Session/2.0
c=IN IP4 0.0.0.0
t=0 0
a=control:*
a=range:npt=now-
a=packetization-supported:DH
m=video 0 RTP/AVP 96
a=control:trackID=0
a=framerate:20.000000
a=rtpmap:96 H264/90000
a=fmtp:96 packetization-mode=1;profile-level-id=4D001F;sprop-parameter-sets=Z00AH5WoFAFuQA==,aO48gA==
a=recvonly


SETUP rtsp://192.168.0.90:554/cam/realmonitor/trackID=0?channel=1&subtype=0 RTSP/1.0
Transport: RTP/AVP/TCP;unicast;interleaved=0-1
CSeq: 2
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017


RTSP/1.0 200 OK
Server: Rtsp Server/2.0
CSeq: 2
Session: 178590884951;timeout=60
Transport: RTP/AVP/TCP;unicast;interleaved=0-1;ssrc=CC800354
x-dynamic-rate: 1




PLAY rtsp://192.168.0.90:554/cam/realmonitor?channel=1&subtype=0 RTSP/1.0
Session: 178590884951
CSeq: 3
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017


RTSP/1.0 200 OK
Server: Rtsp Server/2.0
CSeq: 3
Session: 178590884951
RTP-Info: url=trackID=0;seq=9148;rtptime=3827255265;ssrc=3430941524
Range: npt=0.000000-




TEARDOWN rtsp://192.168.0.90:554/cam/realmonitor?channel=1&subtype=0 RTSP/1.0
Session: 178590884951
Motivo: RTSP Dispose ShutDown
CSeq: 4
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017

SETUP rtsp://192.168.0.90:554/cam/realmonitor/trackID=0?channel=1&subtype=0 RTSP/1.0
Transport: RTP/AVP/TCP;unicast;interleaved=0-1
CSeq: 2
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017


RTSP/1.0 200 OK
Server: Rtsp Server/2.0
CSeq: 2
Session: 80518407122;timeout=60
Transport: RTP/AVP/TCP;unicast;interleaved=0-1;ssrc=FFFFFA05
x-dynamic-rate: 1




PLAY rtsp://192.168.0.90:554/cam/realmonitor?channel=1&subtype=0 RTSP/1.0
Session: 80518407122
CSeq: 3
Authorization: Basic YWRtaW46YWRtaW4=
User-Agent: NetCamara&JOCh2017


RTSP/1.0 200 OK
Server: Rtsp Server/2.0
CSeq: 3
Session: 80518407122
RTP-Info: url=trackID=0;seq=14941;rtptime=135580723;ssrc=4294965765
Range: npt=0.000000-


*/
