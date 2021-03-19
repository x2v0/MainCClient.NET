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

//#include "DataServers.h"		// абстрактный сервер - источник данных
//#include "lnklb.h"				// библиотека сетевого обмена (П. Лунёв)
//#include "RS-232_RW.h"			// структуры данных и операции с COM-портом
//#include "RemReq.h"				// библиотека сетевого обмена (П. Лунёв)
//#include "TCPSrv.h"				// сервер обрабадывающий подключение к нему по TCP
//#include "TM_protocol.h"		// протокол обмена сообщениями проекта "томограф" (рентген, кресло, термоконтроль)
//#include "Queue.h"				// очереди блоков данных


using System.Collections.Generic;
using System.ComponentModel;
using MainCClient.NET;


// для всех контролов заполнить:
//   GetCntrlTemplaiteByType
//   GetDefaultControlName
//   Termo_ChangeTermometersStatus || XRay_ChangeDevicesStatus || PFSDS_ChangeDevicesStatus || ChangeTmCDevicesStatus || ChangeVacDevicesStatus || ACC_ChangeDevicesStatus || ECSrv_ChangeDevicesStatus
//   SetCurrentServer
//   ShowDevicesStatus
//   GetSelectedControlTypeAvailable


//#define VCCSTATUS_ON_MOVE	1
//#define VCCSTATUS_STABLE	0
/*
static int TMPR_DEVICES_SENSE_COUNT = 8; //количество датчиков на слэйве в интерфейсе температурного сервера

static int MAX_TIMER_TICKS_FOR_CORRECTDATA = 10; // *10секунд запуск таймера


static int  DATASERVER_CONNECTED = 0; // значение, возвращаемое IsConnected при наличии коннекта
static int  DATASERVER_DISCONNECTED = 1; // значение, возвращаемое IsConnected при отсутствии коннекта

static int  DS_CMD_NOTHING = 0; // пустышка, ничего не отправлять
static int DATA_LENGTH_0 = 0; // длина данных - 0
static int DATA_STRUCT_NULL = NULL; // пустышка вместо данных
*/

// визуализация датчика. Добавляется серверу посредством AddVCControl
public class VCControl
{
   public int type; // тип контрола начиная с CONTROL_TYPE_UNKNOWN

   public int masterCID; // основное устройство - для температурных датчиков только

// для контролов вида CONTROL_TYPE_TEMPSET, CONTROL_TYPE_TEMPSINGLE
   public int CID; // CID
   public int count; // количество подключенных к слэйву датчиков
   public int num; // номер данного датчика ( для CONTROL_TYPE_TEMPSINGLE )
   public int term_correct; // коррекция температур
   public byte[] name = new byte[32]; // текст метки

//	int update;
   public int panelHandle; // панель контрола, на которой расположен датчик
   public int control_id; // id LED контрола, отображающего статус
   public int device_status; // статус устройства DEVICE_STATUS_OK DEVICE_STATUS_MOVING
   public int top; // положение контрола
   public int left;
   public float control_val; // для контролов-индикаторов
   public int view_mode; // режим отображения датчика
}

// серверные данные
// только для сервера вакуума

/// <summary>
///    серверные данные
///    только для сервера вакуума
/// </summary>
public class DACADC_data
{
   [Description("максимальное количество каналов в сервере")]
   public static int DACADC_MAX_DAC = 8; //

   [Description("максимальное количество каналов в сервере")]
   public static int DACADC_MAX_ADC = 8;

   public ushort[] dac = new ushort[DACADC_MAX_DAC];
   public double[] adc = new double[DACADC_MAX_ADC];
   public byte addr;
   public byte MAX_DAC; // максимальное количество выходных каналов на этом устройстве
   public byte MAX_ADC; // максимальное количество входных каналов на этом устройстве
}

// параметры подключения к устройству через протокол линклиб

/*
public class LinkLib 
{
int sid;	// sid
int seg;	// сегмент
};


// структура для запроса через линклиб
typedef struct {
SDEV sdev;
void *p_ds;  			// указатель на дата-сервер  DataServer
void *p_dds;  			// указатель на структуру-дескриптор устройства, к которому был запрос  DevDescr *
int asknum; 			// номер запроса, циклический счетчик
int cmd;				// текущая выполняемая команда (внутренний код программы)
//byte log_resp; // флаг - вывести ответ в лог
		
}	DS_LnkLb_ask;
*/

// структура с необходимыми адресами устройств для колбека запроса к железу
// содержится внутри каждого устройства и заполняется при инициализации дата-сервера XRay_AddServer 		
public class DevDescr
{
   public uint DN; // индивидуальный адрес устройства

  // public void* p_ds; // указатель на датасервер
  // public void* p_dev; // указатель на устройство - владельца (как внутреннее устройство датасервера)
   public int dev_type; // тип устройства  (DEVICE_TYPE_DIMS etc)
   public int in_wait; // сервер в режиме ожидания ответа от железа
   public byte err_in_wait;

   
   public int configured; // устройство отконфигурировано, можно делать запросы
   public int status; // статус устройства (согласно TM протоколу) ( DEVICE_STATUS_OK	etc )
   public int StatusControl; // контрол статуса; черный-неотконф., красный-поломка, зеленый-ОК

   private byte[] name = new byte[256]; // название устройства для вывода сообщений в консоль
}

//универсальный сервер - источник данных для подключения к разным источникам данных:
//TCP серверам по протоколу TM, устройствам через линклиб или com порт
public class DataServer
{
   public int handle; // для коммуникации. -1 если не используется сейчас, 0+ если канал действует.
   public byte[] ip = new byte[42]; // = "127.0.0.1";
   public int port; // = 9995;  //for connect to server
   public int connect_type; // тип подключения: linklib, TMProtocol или локальное  SERVER_CONNECT_TYPE_TMPROTOCOL
   public int connect_available; // возможные (реализованные) способы подключения, напр. SERVER_CONNECT_TYPE_LNKLIB | SERVER_CONNECT_TYPE_DIRECT
   public byte type; // тип сервера 0-неизвестный, SERVER_TYPE_TEMPERATURE и т.д
   public byte[] name = new byte[16]; // имя сервера 
   public int onProceccing; // флаг - сервер находится "в процессе", удаление требует ожиданя

// параметры прямого подключения через COM порт - только для данного типа подключения
/*   public ComData COM;

// только для подключения linklib:
struct {
int sid;				// sid, seg
unsigned short RQ;		// RQ_ZERO и т.д
TmQueue asks;			// очередь запросов
int asknum; 			// текущий запрос (циклический счетчик до 999), для TM используется как счетчик запросов
} lnk;
*/

// только для подключения TM
   public byte TMSettings; // битовое поле, настройки протокола взаимодействия - TMSettings_ADDCHECKSUMM

// для пакетов, реализующих какой-то уникальный протокол
   struct ntm {
      int waitpacketsize; // текущий ожидаемый размер пакета. Если 0, то вычитвается максимально возможное количество байт
   };

   public int panelHandle; // панель, на которую выведен индикатор соединения
   public int control_id; // id контрола, отображающего коннект к серверу
   public int control_right; // правая граница контрола, для выстраивания цепочки

   public int NeedAutoReconnect; //автоматическое подключение
   public int NeedForReconnect; //необходимость восстановления соединения в текущем сеансе

   public int TimerTicks; //Cчетчик тиков. Сбрасывается при пришедших от сервера данных.

// только для пассивных серверов:
   public int UpdateTimerID; //таймер периодических опросов. заводится и стартует из StartUpdateTimer
   public float UpdatePeriod; //интервал таймера в секундах
   public float timeout; //таймаут ожидания ответа от железа (задействован в термоконтроле), секунды

   public List<VCControl> Controls = new List<VCControl>(); // интерфейсные контролы
   public int ControlsCount;
   public byte update; // счетчик апдейтов
   public byte dev_status; // статус устройства - напр. DEVICE_STATUS_OK
   public byte[] LogPath = new byte[1024];
   public byte traceToLog; // флаг - сохранять обмен пакетами с устройством в лог LogPath. Используется в кресле по желтой кнопке - вычитывание конфигурациии

   private byte configChanged; // флаг - изменилась аппаратная конфигурация - надо разослать инф. клиентам

  // public void* ServerData; // уникальная начинка сервера в соответствии с типом
// для type = SERVER_TYPE_TEMPERATURE  ->  данные типа TermServerData*
// для type = SERVER_TYPE_XRAY  ->  данные типа XrayData *xrd = (XrayData *) ds->ServerData;
/*
   void (*F_AfterTryConnect) ( void*pds, int res ); // дополнительный обработчик после коннекта   	вызывается в ConnectToServer
   void (*F_AfterDisconnect) ( void*pds ); // дополнительный обработчик после дисконнекта  вызывается в DisconnectFix 
   int (*F_Connect) ( void*pds ); // специализированный коннект. Если задан он, то используется он в ConnectToServer
   void (*F_Disconnect) ( void*pds ); // специализированный коннект.  вызывается в DisconnectFromServer 
   int (*F_PostParseData ) (int cmd, int cid); // указатель на функцию - дополнительный обработчик входящих команд. Вызывается
// после обработки данных, например для доп. реакции по отображению данных
// использовать необязательно
// для PFS - параметры cmd и cid из пришедшей команды
// для остальных - тип пакета и вид данных 
// вызывается из парсера
// TMCPFS_PostParseData, Kr_PostParseData

   void (*F_FreeServerData) (void*pds); // доп. функция освобождения серверных данных.
   void (*F_ParseIncommingPacketPriv) ( void*pds, TMPacket*p, byte[] pdata ); // дополнительный обработчик входящего пакета для ClientTCP_ParseIncommingPacketTM
   void (*F_ChangeDevicesStatus) ( void*pds ); // переопределенная функция
   int (*F_SendTMDataPriv) ( void*pds, int cmd, uint sz, byte[] data ); // переопределен. Если есть, то вызывается он
   int (*F_FixHardwareError) ( void*pds, int pid, int err_code, byte err_status ); // внешняя функция фиксации аппаратной ошибки. Поддерживается в PFS_ParseResponce   - Kr_FixHardwareError

   int (*F_MakeCommand) (int cmd, byte[] send_data, int n, void*p_dev, short*reqrespdelay
      ); // внешняя функция для формирования команды к устройству. Напр. PFS_MakeCommand Termo_MakeCommand XRAY_MakeCommand TermoL_MakeCommand ACC_MakeCommand

   int (*F_ParseResponce) ( byte[] rx_buff, int n, int cnt, void*p_dev
      ); // разбор ответа от контроллера. Может придти напрямую через ack_parser   Напр. PFS_ParseResponce Termo_ParseResponce XRay_ParseResponce ACC_ParseResponce

// должна в конце попытаться вызвать F_PostParseData
   int (*F_LogResponce ) ( byte[] rx_buff, int n, int cnt, void*p_dev, int cmd, int asknum ); // сброс ответа от контроллера в лог  Напр. PFS_LogResponce, MCSrv_LogResponce, 
   int (*F_LogRequest) ( byte[] rq_buff, int n, void*p_dev, int cmd, int asknum); // сброс запроса к контроллеру в лог  Напр. PFS_LogRequest MCSrv_LogRequest
   int (*F_FixResponceResult ) ( int res, void*p_dev ); // фиксация результата запроса к железу, не обязательная. PFSDS_FixResponceResult
   public CtrlCallbackPtr F_update; // функция периодического обновления данных по таймеру
*/
   public byte in_wait; // сервер в режиме ожидания ответа от железа

//	byte err_in_wait;
   public byte[] str_buf = new byte[1024]; // строковый буфер для временного хранения сообщений

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

// соединение с источником данных
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
