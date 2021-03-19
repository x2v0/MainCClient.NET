// $Id: DataServers .cs 7760 2020-03-04 05:58:07Z onuchin $
//
// Copyright (C) 2020 Valeriy Onuchin

//==============================================================================
//
// Title:       DataServers.h
//
// Created on:  10.09.2015 at 13:49:12 by Admin.
// Copyright:   Aleksey Shestopalov, Protom. All Rights Reserved.
//
//==============================================================================

//#include "DataServers.h"		// ����������� ������ - �������� ������
//#include "lnklb.h"				// ���������� �������� ������ (�. ����)
//#include "RS-232_RW.h"			// ��������� ������ � �������� � COM-������
//#include "RemReq.h"				// ���������� �������� ������ (�. ����)
//#include "TCPSrv.h"				// ������ �������������� ����������� � ���� �� TCP
//#include "TM_protocol.h"		// �������� ������ ����������� ������� "��������" (�������, ������, �������������)
//#include "Queue.h"				// ������� ������ ������


using System.Collections.Generic;
using System.ComponentModel;
using MainCClient.NET;


// ��� ���� ��������� ���������:
//   GetCntrlTemplaiteByType
//   GetDefaultControlName
//   Termo_ChangeTermometersStatus || XRay_ChangeDevicesStatus || PFSDS_ChangeDevicesStatus || ChangeTmCDevicesStatus || ChangeVacDevicesStatus || ACC_ChangeDevicesStatus || ECSrv_ChangeDevicesStatus
//   SetCurrentServer
//   ShowDevicesStatus
//   GetSelectedControlTypeAvailable


//#define VCCSTATUS_ON_MOVE	1
//#define VCCSTATUS_STABLE	0
/*
static int TMPR_DEVICES_SENSE_COUNT = 8; //���������� �������� �� ������ � ���������� �������������� �������

static int MAX_TIMER_TICKS_FOR_CORRECTDATA = 10; // *10������ ������ �������


static int  DATASERVER_CONNECTED = 0; // ��������, ������������ IsConnected ��� ������� ��������
static int  DATASERVER_DISCONNECTED = 1; // ��������, ������������ IsConnected ��� ���������� ��������

static int  DS_CMD_NOTHING = 0; // ��������, ������ �� ����������
static int DATA_LENGTH_0 = 0; // ����� ������ - 0
static int DATA_STRUCT_NULL = NULL; // �������� ������ ������
*/

// ������������ �������. ����������� ������� ����������� AddVCControl
public class VCControl
{
   public int type; // ��� �������� ������� � CONTROL_TYPE_UNKNOWN

   public int masterCID; // �������� ���������� - ��� ������������� �������� ������

// ��� ��������� ���� CONTROL_TYPE_TEMPSET, CONTROL_TYPE_TEMPSINGLE
   public int CID; // CID
   public int count; // ���������� ������������ � ������ ��������
   public int num; // ����� ������� ������� ( ��� CONTROL_TYPE_TEMPSINGLE )
   public int term_correct; // ��������� ����������
   public byte[] name = new byte[32]; // ����� �����

//	int update;
   public int panelHandle; // ������ ��������, �� ������� ���������� ������
   public int control_id; // id LED ��������, ������������� ������
   public int device_status; // ������ ���������� DEVICE_STATUS_OK DEVICE_STATUS_MOVING
   public int top; // ��������� ��������
   public int left;
   public float control_val; // ��� ���������-�����������
   public int view_mode; // ����� ����������� �������
}

// ��������� ������
// ������ ��� ������� �������

/// <summary>
///    ��������� ������
///    ������ ��� ������� �������
/// </summary>
public class DACADC_data
{
   [Description("������������ ���������� ������� � �������")]
   public static int DACADC_MAX_DAC = 8; //

   [Description("������������ ���������� ������� � �������")]
   public static int DACADC_MAX_ADC = 8;

   public ushort[] dac = new ushort[DACADC_MAX_DAC];
   public double[] adc = new double[DACADC_MAX_ADC];
   public byte addr;
   public byte MAX_DAC; // ������������ ���������� �������� ������� �� ���� ����������
   public byte MAX_ADC; // ������������ ���������� ������� ������� �� ���� ����������
}

// ��������� ����������� � ���������� ����� �������� �������

/*
public class LinkLib 
{
int sid;	// sid
int seg;	// �������
};


// ��������� ��� ������� ����� �������
typedef struct {
SDEV sdev;
void *p_ds;  			// ��������� �� ����-������  DataServer
void *p_dds;  			// ��������� �� ���������-���������� ����������, � �������� ��� ������  DevDescr *
int asknum; 			// ����� �������, ����������� �������
int cmd;				// ������� ����������� ������� (���������� ��� ���������)
//byte log_resp; // ���� - ������� ����� � ���
		
}	DS_LnkLb_ask;
*/

// ��������� � ������������ �������� ��������� ��� ������� ������� � ������
// ���������� ������ ������� ���������� � ����������� ��� ������������� ����-������� XRay_AddServer 		
public class DevDescr
{
   public uint DN; // �������������� ����� ����������

  // public void* p_ds; // ��������� �� ����������
  // public void* p_dev; // ��������� �� ���������� - ��������� (��� ���������� ���������� �����������)
   public int dev_type; // ��� ����������  (DEVICE_TYPE_DIMS etc)
   public int in_wait; // ������ � ������ �������� ������ �� ������
   public byte err_in_wait;

   
   public int configured; // ���������� �����������������, ����� ������ �������
   public int status; // ������ ���������� (�������� TM ���������) ( DEVICE_STATUS_OK	etc )
   public int StatusControl; // ������� �������; ������-��������., �������-�������, �������-��

   private byte[] name = new byte[256]; // �������� ���������� ��� ������ ��������� � �������
}

//������������� ������ - �������� ������ ��� ����������� � ������ ���������� ������:
//TCP �������� �� ��������� TM, ����������� ����� ������� ��� com ����
public class DataServer
{
   public int handle; // ��� ������������. -1 ���� �� ������������ ������, 0+ ���� ����� ���������.
   public byte[] ip = new byte[42]; // = "127.0.0.1";
   public int port; // = 9995;  //for connect to server
   public int connect_type; // ��� �����������: linklib, TMProtocol ��� ���������  SERVER_CONNECT_TYPE_TMPROTOCOL
   public int connect_available; // ��������� (�������������) ������� �����������, ����. SERVER_CONNECT_TYPE_LNKLIB | SERVER_CONNECT_TYPE_DIRECT
   public byte type; // ��� ������� 0-�����������, SERVER_TYPE_TEMPERATURE � �.�
   public byte[] name = new byte[16]; // ��� ������� 
   public int onProceccing; // ���� - ������ ��������� "� ��������", �������� ������� �������

// ��������� ������� ����������� ����� COM ���� - ������ ��� ������� ���� �����������
/*   public ComData COM;

// ������ ��� ����������� linklib:
struct {
int sid;				// sid, seg
unsigned short RQ;		// RQ_ZERO � �.�
TmQueue asks;			// ������� ��������
int asknum; 			// ������� ������ (����������� ������� �� 999), ��� TM ������������ ��� ������� ��������
} lnk;
*/

// ������ ��� ����������� TM
   public byte TMSettings; // ������� ����, ��������� ��������� �������������� - TMSettings_ADDCHECKSUMM

// ��� �������, ����������� �����-�� ���������� ��������
   struct ntm {
      int waitpacketsize; // ������� ��������� ������ ������. ���� 0, �� ����������� ����������� ��������� ���������� ����
   };

   public int panelHandle; // ������, �� ������� ������� ��������� ����������
   public int control_id; // id ��������, ������������� ������� � �������
   public int control_right; // ������ ������� ��������, ��� ������������ �������

   public int NeedAutoReconnect; //�������������� �����������
   public int NeedForReconnect; //������������� �������������� ���������� � ������� ������

   public int TimerTicks; //C������ �����. ������������ ��� ��������� �� ������� ������.

// ������ ��� ��������� ��������:
   public int UpdateTimerID; //������ ������������� �������. ��������� � �������� �� StartUpdateTimer
   public float UpdatePeriod; //�������� ������� � ��������
   public float timeout; //������� �������� ������ �� ������ (������������ � �������������), �������

   public List<VCControl> Controls = new List<VCControl>(); // ������������ ��������
   public int ControlsCount;
   public byte update; // ������� ��������
   public byte dev_status; // ������ ���������� - ����. DEVICE_STATUS_OK
   public byte[] LogPath = new byte[1024];
   public byte traceToLog; // ���� - ��������� ����� �������� � ����������� � ��� LogPath. ������������ � ������ �� ������ ������ - ����������� �������������

   private byte configChanged; // ���� - ���������� ���������� ������������ - ���� ��������� ���. ��������

  // public void* ServerData; // ���������� ������� ������� � ������������ � �����
// ��� type = SERVER_TYPE_TEMPERATURE  ->  ������ ���� TermServerData*
// ��� type = SERVER_TYPE_XRAY  ->  ������ ���� XrayData *xrd = (XrayData *) ds->ServerData;
/*
   void (*F_AfterTryConnect) ( void*pds, int res ); // �������������� ���������� ����� ��������   	���������� � ConnectToServer
   void (*F_AfterDisconnect) ( void*pds ); // �������������� ���������� ����� �����������  ���������� � DisconnectFix 
   int (*F_Connect) ( void*pds ); // ������������������ �������. ���� ����� ��, �� ������������ �� � ConnectToServer
   void (*F_Disconnect) ( void*pds ); // ������������������ �������.  ���������� � DisconnectFromServer 
   int (*F_PostParseData ) (int cmd, int cid); // ��������� �� ������� - �������������� ���������� �������� ������. ����������
// ����� ��������� ������, �������� ��� ���. ������� �� ����������� ������
// ������������ �������������
// ��� PFS - ��������� cmd � cid �� ��������� �������
// ��� ��������� - ��� ������ � ��� ������ 
// ���������� �� �������
// TMCPFS_PostParseData, Kr_PostParseData

   void (*F_FreeServerData) (void*pds); // ���. ������� ������������ ��������� ������.
   void (*F_ParseIncommingPacketPriv) ( void*pds, TMPacket*p, byte[] pdata ); // �������������� ���������� ��������� ������ ��� ClientTCP_ParseIncommingPacketTM
   void (*F_ChangeDevicesStatus) ( void*pds ); // ���������������� �������
   int (*F_SendTMDataPriv) ( void*pds, int cmd, uint sz, byte[] data ); // �������������. ���� ����, �� ���������� ��
   int (*F_FixHardwareError) ( void*pds, int pid, int err_code, byte err_status ); // ������� ������� �������� ���������� ������. �������������� � PFS_ParseResponce   - Kr_FixHardwareError

   int (*F_MakeCommand) (int cmd, byte[] send_data, int n, void*p_dev, short*reqrespdelay
      ); // ������� ������� ��� ������������ ������� � ����������. ����. PFS_MakeCommand Termo_MakeCommand XRAY_MakeCommand TermoL_MakeCommand ACC_MakeCommand

   int (*F_ParseResponce) ( byte[] rx_buff, int n, int cnt, void*p_dev
      ); // ������ ������ �� �����������. ����� ������ �������� ����� ack_parser   ����. PFS_ParseResponce Termo_ParseResponce XRay_ParseResponce ACC_ParseResponce

// ������ � ����� ���������� ������� F_PostParseData
   int (*F_LogResponce ) ( byte[] rx_buff, int n, int cnt, void*p_dev, int cmd, int asknum ); // ����� ������ �� ����������� � ���  ����. PFS_LogResponce, MCSrv_LogResponce, 
   int (*F_LogRequest) ( byte[] rq_buff, int n, void*p_dev, int cmd, int asknum); // ����� ������� � ����������� � ���  ����. PFS_LogRequest MCSrv_LogRequest
   int (*F_FixResponceResult ) ( int res, void*p_dev ); // �������� ���������� ������� � ������, �� ������������. PFSDS_FixResponceResult
   public CtrlCallbackPtr F_update; // ������� �������������� ���������� ������ �� �������
*/
   public byte in_wait; // ������ � ������ �������� ������ �� ������

//	byte err_in_wait;
   public byte[] str_buf = new byte[1024]; // ��������� ����� ��� ���������� �������� ���������

/*
   public static int AddServer(string name, string ip, int port, int type);
   public static VCControl AddVCControl(DataServer ds, int type, int masterCID, int CID, int num, int count);
   public static int Del(int num);
   public static int DelDS(DataServer ds);
   public static int DelAll(void);
   public static int DelVCControl(DataServer ds, int num);
   public static VCControl GetCntlById(int id);
   public static DataServer GetServerByNum(int n);
   public static int GetServerNum(DataServer ds);
   public static DataServer GetServer(int num);
   public static int GetServerCount(void);
   public static void GetStatusString(string buf, int type, int device_status, float control_val);
   public static int GetServerStatus(DataServer ds);
   public static DataServer GetServerByCntlId(int id);
   public static DataServer GetServerByHandle(int handle);
   public static int GetNextAskNum(DataServer ds);

   public static void ShowDevicesStatus(DataServer ds);
   public static void ShowServersDevicesStatus(void);
   public static int StartUpdateTimer(DataServer ds, double period, CtrlCallbackPtr fupdate);
   public static int StopUpdateTimer(DataServer ds);
   public static void UpdateDevicesStatus(void);

// ���������� � ���������� ������
   public static int ConnectToServer(DataServer ds);
   public static void DisconnectFromServer(DataServer ds);
   public static int IsConnected(DataServer ds); // DATASERVER_CONNECTED
   public static int Info2Tree(int panel, int control, int n);
   public static int DataServer_Info2Tree(DataServer ds, int panel, int control, int n);
   public static void ReconnectServers(void);
   public static int SendCommandToDev(DevDescr dds, byte command, double wait);
   public static int SendCommandToHW(DevDescr pdds, int cmd);
   public static int SendCustomToServer(DataServer ds, void* data, uint size);
   public static int SendTMCommand(DataServer ds, int cmd);
   public static int SendTMCommandD(DataServer ds, int cmd, uint sz, byte[] data);
   public static int SendTMData(DataServer ds, int cmd, uint length, byte[] data);
   public static int SendTMInfo(DataServer ds, int cmd, uint sz, byte[] data);
   public static int SetLL_NTF(DevDescr* pdds, int cmd, int (* cbfnq)(PKT*) );
   public static int SetConnectType(DataServer ds, int connect_type);
   public static void ShowLnkLibSts(DataServer ds, byte rq_sts);
   public static int ParseConnectType(string buf);
   public static int ParseServerType(string buf);
   public static int PrintConnectType(DataServer ds, string buf);
   public static int Ping(void* pds);
   */
/*
   public static int CVICALLBACK
   private PingServiceTimerCB(int panel, int control, int event,
   public static void* callbackData,
   private int eventData1,
   private int eventData2);
*/
/*
   public static string GetDeviceTypeName(int type, string buf);
   public static string GetDefaultControlName(VCControl vcc, string buf);
   public static string GetPicControlName(VCControl vcc, string buf);

   public static void ClientTCP_ParseIncommingPacketTM(DataServer ds, int handle);

   public static void ClientTCP_ParseIncommingPacketNTM(DataServer ds, int handle);
//void ParseImcommingPacketDACADCSrv(DataServer ds, suint dataSize, byte[] bytes);

   public static void ChangeVacDevicesStatus(DataServer ds);

   public static int USART_GetErrorMessage(int err_code, int lang, string buf, int buf_sz);
   public static int RS485_GetErrorMessage(int err_code, int lang, string buf, int buf_sz);

   public static int ShowDeviceStateLed(int configured, int status, int panelHandle, int control);
   public static void ShowStatusByteState(int panelHandle, int* StsControls, int off_all, byte STS, byte blueMask);

   public static int SendCurrentConfig_PackInt(byte[] data, int n, int val);
   public static int SendCurrentConfig_PackStr(byte[] data, int n, string str);
   public static int SendCurrentConfig_PackDevAsTag(byte[] data, int n, DevDescr dev);
   public static int SendCurrentConfig_PackDevCfgAsTag(byte[] data, int n, int val);
   public static int SendCurrentConfig_PackConnectAsTag(byte[] data, int n, DataServer ds);
   public static int SendCurrentConfig_PackSoftwareVerAsTag(byte[] data, int n);
   public static int SendCurrentConfig_PackRTLinkAsTag(byte[] data, int n, TCPSrvServer* tcp);
   public static int SendCurrentConfig_PackTxtInfoAsTag(byte[] data, int n, string info);

   public static int ParseCfgPacket_GetInt(byte[] data, int n, int[] val);
   public static int ParseCfgPacket_GetStr(byte[] data, int n, string buf, int buf_size);
   public static int ParseCfgPacket_GetLinkData(byte[] data, int n, DataServer ds);
   public static int ParseCfgPacket_GetDevDescr(byte[] data, int n, DevDescr dds);
   public static int ParseCfgPacket_GetRTLinkData(byte[] data, int n, TCPSrvServer* tcp);
   public static int ParseTagAsString(int srv_type, byte[] pdata, int n, string str, int out_sz);
   */
}
