/*****************************************************************//**
 * \file   EstunRemoteApiLib.h
 * \brief  
 * 
 * \author wanglei64101
 * \date   May 2022
 *********************************************************************/

#ifndef ESTUNREMOTEAPILIB_H
#define ESTUNREMOTEAPILIB_H

#include "EstunRemoteApiLib_global.h"
#include "EstunRemoteApiLib_struct.h"

using namespace EstunRobot;

class ESTUNREMOTEAPILIBAPI EstunRemoteApiLib
{
public:
	/**
	 * 类的默认构造接口，默认为当前目录下的Config文件夹
	 * 
	 */
	EstunRemoteApiLib();

	/**
	 * 类的构造接口
	 *
	 * \param configDir 配置文件以及错误信息文件所在文件夹
	 */
	EstunRemoteApiLib(ESString configDir);
	~EstunRemoteApiLib();

	/**
	 * 设置本地日志配置.
	 *
	 * \param logDir 日志文件目录，不指定则在当前目录生成
	 * \param size 日志文件大小
	 */
	void setLogConfig(ESString logDir, unsigned int size);

	/**
	 * 是否输出本地日志.
	 * 
	 * \param enable 控制writeLog是否往文件写内容
	 */
	void setLogEnable(bool enable);

	/**
	 * 写本地日志.
	 *
	 * \param str 日志信息
	 */
	void writeLog(ESString str);

	/**
	 * 获取最近一次接口调用错误的信息.
	 * 
	 * \return 错误信息字符串
	 */
	ESString getLastErr() const;

	/**
	 * 连接服务器.
	 * 
	 * \param ipAddr 服务IP地址，IPV4格式
	 * \param isAutoReConnect 是否自动重连，如果为True，客户端在异常掉线时会自动重连，否则不会
	 * \return 成功返回true，失败返回false
	 */
	bool connectToServer(ESString ipAddr, bool isAutoReConnect = false);

	/**
	 * 断开与服务的连接.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool disConnect();

	/**
	 * 检测是否已连接服务.
	 * 
	 * \return 连接返回true，断开返回false
	 */
	bool isConnected();

	/**
	 * 强制开启API模式.(仅部分特殊授权身份可用)
	 * 
	 * \param ident 特殊标识
	 * \param enableStatus 0表示关闭，1表示开启
	 * \return 成功返回true
	 */
	bool E_ForceEnableApi(ESString ident, int enableStatus);

	/**
	 * 获取控制器时间.
	 * 
	 * \param time_str 控制器时间，格式"YYYY-MM-DD hh:mm:ss"
	 * \return 获取成功返回true，失败返回false
	 */
	bool E_GetControlTime(ESString& time_str);

	/**
	 * 获取当前模式的工作状态.
	 * 
	 * \return -1表示失败，0表示正常，1表示机器人错误，2表示机器人处于停止状态，需要调用start接口启动
	 */
	int E_GetAPIStatus();

	/**
	 * 申请机器人运动操作权限.
	 * 
	 * \return 权限信息
	 */
	E_ROB_PERMIT E_AcquirePermit();

	/**
	 * 查询当前机器人运动操作权限.
	 *
	 * \return 权限信息
	 */
	E_ROB_PERMIT E_CurrentPermit();

	/**
	 * 释放机器人运动操作权限.
	 * 
	 * \param permit 之前获取的权限
	 * \return 成功返回true，失败返回false
	 */
	bool E_ReleasePermit(E_ROB_PERMIT permit);

	/**
	 * 唤醒机器人运动，从stop模式恢复.
	 *
	 * \return 成功返回true，失败返回false
	 */
	bool E_MotionStart();

	/**
	 * 暂停机器人运动，不会清空运动队列.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_MotionPause();

	/**
	 * 恢复机器人运动，继续执行运动队列.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_MotionContinue();

	/**
	 * 停止机器人运动，同时清空运动队列.
	 *
	 * \return 成功返回true，失败返回false
	 */
	bool E_MotionStop();

	/**
	 * 获取机器人运动状态.
	 * 
	 * \return 运动返回1，不运动返回0
	 */
	int E_GetRobotRunStatus();

	/**
	 * 执行MovJ指令.
	 * 
	 * \param Pname 目标位置点变量名， 例："t_g.p1"
	 * \param velName 运动速度变量名， 例："t_g.SPEED0"
	 * \param blendType 过渡类型，"", "Rel", "Abs"，三选一
	 * \param zname 过渡参数变量名， 例："t_g.ZONE0"
	 * \param tname 工具变量名， 例："t_g.TOOL1"
	 * \param coorname 坐标系变量名， 例："t_g.USER0"
	 * \param payloadName 负载变量名， 例："t_g. PAYLOAD0"
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovJ(ESString Pname, ESString velName = "", ESString blendType = "", ESString zname = "", ESString tname = "", ESString coorname = "", ESString payloadName = "", const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);
	
	/**
	 * 执行MovJ2指令.
	 * 
	 * \param dstPoint 目标位置点
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovJ2(const E_ROB_POS& dstPoint, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);

	/**
	 * 执行MovL指令.
	 * 
	 * \param Pname 目标位置点变量名， 例："t_g.p1"
	 * \param velName 运动速度变量名， 例："t_g.SPEED0"
	 * \param blendType 过渡类型，"", "Rel", "Abs"，三选一
	 * \param zname 过渡参数变量名， 例："t_g.ZONE0"
	 * \param tname 工具变量名， 例："t_g.TOOL1"
	 * \param coorname 坐标系变量名， 例："t_g.USER0"
	 * \param payloadName 负载变量名， 例："t_g. PAYLOAD0"
	 * \param overrideType 速率类型，"", "GOFF", "GON"，三选一
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovL(ESString Pname, ESString velName = "", ESString blendType = "", ESString zname = "", ESString tname = "", ESString coorname = "", ESString payloadName = "", ESString overrideType = "GON", const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);
	
	/**
	 * 执行MovL2指令.
	 * 
	 * \param dstPoint 目标位置点
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param overrideType 速率类型, 0:GOVROFF;1:GOVRON
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovL2(const E_ROB_POS& dstPoint, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, int overrideType = 1, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);

	/**
	 * 执行MovC指令.
	 * 
	 * \param PAuxname 辅助位置点变量名， 例："t_g.p0"
	 * \param PDstname 目标位置点变量名， 例："t_g.p1"
	 * \param velName 运动速度变量名， 例："t_g.SPEED0"
	 * \param blendType 过渡类型，"", "Rel", "Abs"，三选一
	 * \param zname 过渡参数变量名， 例："t_g.ZONE0"
	 * \param tname 工具变量名， 例："t_g.TOOL1"
	 * \param coorname 坐标系变量名， 例："t_g.USER0"
	 * \param payloadName 负载变量名， 例："t_g. PAYLOAD0"
	 * \param overrideType 速率类型，"", "GOFF", "GON"，三选一
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovC(ESString PAuxname, ESString PDstname, ESString velName = "", ESString blendType = "", ESString zname = "", ESString tname = "", ESString coorname = "", ESString payloadName = "", ESString overrideType = "GON", const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);
	
	/**
	 * 执行MovC2指令.
	 * 
	 * \param auxP 辅助位置点
	 * \param dstPoint 目标位置点
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param overrideType 速率类型, 0:GOVROFF;1:GOVRON
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovC2(const E_ROB_POS& auxP, const E_ROB_POS& dstPoint, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, int overrideType = 1, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);

	/**
	 * 执行MovArch指令.
	 * 
	 * \param Pname 目标位置点变量名， 例："t_g.p1"
	 * \param velName 运动速度变量名， 例："t_g.SPEED0"
	 * \param blendType 过渡类型，"", "Rel", "Abs"，三选一
	 * \param zname 过渡参数变量名， 例："t_g.ZONE0"
	 * \param tname 工具变量名， 例："t_g.TOOL1"
	 * \param coorname 坐标系变量名， 例："t_g.USER0"
	 * \param payloadName 负载变量名， 例："t_g. PAYLOAD0"
	 * \param cornerName 拱形运动转角变量名， 例："t_g.ZONE1"
	 * \param height 抬升高度，单位毫米
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovArch(ESString Pname, ESString velName = "", ESString blendType = "", ESString zname = "", ESString tname = "", ESString coorname = "", ESString payloadName = "", ESString cornerName = "", double height = 0.0, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);
	
	/**
	 * 执行MovArch2指令.
	 * 
	 * \param dstPoint 目标位置点
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param connerZone 转角参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param heigt 抬升高度，单位毫米
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovArch2(const E_ROB_POS& dstPoint, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), const E_ROB_ZONE& connerZone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, double heigt = 0.0, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);
	/**
	 * 执行E_MovJArch2指令.
	 * 
	 * \param dstPoint 目标位置点
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param connerZone 转角参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param Upheight 抬升高度，单位毫米
	 * \param Downheight 抬升高度，单位毫米
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovJArch2(const E_ROB_POS& dstPoint, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), const E_ROB_ZONE& connerZone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, double Upheight = 0.0, double Downheight = 0.0, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);
	
	/**
	 * 执行E_MovLArch2指令.
	 *
	 * \param dstPoint 目标位置点
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param connerZone 转角参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param Upheight 抬升高度，单位毫米
	 * \param Downheight 抬升高度，单位毫米
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovLArch2(const E_ROB_POS& dstPoint, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), const E_ROB_ZONE& connerZone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, double Upheight = 0.0, double Downheight = 0.0, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);

	/**
	 * 获取所有工具坐标系ID.
	 * 
	 * \param num 获取到的个数
	 * \param idArray ID数组指针，由接口初始化
	 * \return 成功获取返回true
	 */
	bool E_GetToolsID(int& num, int*& idArray);

	/**
	 * 创建工具坐标系.
	 * 
	 * \param toolID 新的工具号，必须大于0
	 * \param toolCoord 工具坐标参数
	 * \param payloadvalue 工具负载参数
	 * \return 成功返回true，失败返回false
	 */
	bool E_CreateTool(int toolID, const E_ROB_COORD & toolCoord, const E_ROB_PAYLOAD& payloadvalue);

	/**
	 * 修改工具坐标系.
	 * 
	 * \param toolID 要修改的工具id，必须大于0
	 * \param toolCoord 工具坐标参数
	 * \param payloadvalue 工具负载参数
	 * \return 成功返回true，失败返回false
	 */
	bool E_ModifyTool(int toolID, const E_ROB_COORD & toolCoord, const E_ROB_PAYLOAD& payloadvalue);

	/**
	 * 加载工具坐标系.
	 * 
	 * \param toolID 工具id，如果为0表示不加载
	 * \return 成功返回true，失败返回false
	 */
	bool E_LoadTool(int toolID);

	/**
	 * 获取当前工具id.
	 * 
	 * \return 工具id
	 */
	int E_GetCurToolID();

	/**
	 * 获取工具坐标变量名称.
	 * 
	 * \param toolID 工具id
	 * \return 名称，为空则表示获取失败
	 */
	ESString E_GetToolName(int toolID);

	/**
	 * 获取工具坐标系变量.
	 * 
	 * \param toolID 工具id
	 * \param toolCoord 工具参数，引用
	 * \param toolPayload 负载参数，引用
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetTool(int toolID, E_ROB_COORD& toolCoord, E_ROB_PAYLOAD& toolPayload);

	/**
	 * 获取所有用户坐标系ID.（ID大于10000为外部坐标系）
	 *
	 * \param num 获取到的个数
	 * \param idArray ID数组指针，由接口初始化
	 * \return 成功获取返回true
	 */
	bool E_GetUserCoordsID(int& num, int*& idArray);

	/**
	 * 创建用户坐标.
	 * 
	 * \param usrID 新的坐标id，必须大于0（ID大于10000为外部坐标系）
	 * \param usrData 坐标参数
	 * \return 成功返回true，失败返回false
	 */
	bool E_CreateUserCoord(int usrID, const E_ROB_COORD& usrData);

	/**
	 * 修改用户坐标.
	 * 
	 * \param usrID 要修改的用户坐标id，必须大于0
	 * \param usrData 坐标参数
	 * \return 成功返回true，失败返回false
	 */
	bool E_ModifyUserCoord(int usrID, const E_ROB_COORD& usrData);

	/**
	 * 加载用户坐标.
	 * 
	 * \param usrID 用户坐标id，如果为0表示不加载
	 * \return 成功返回true，失败返回false
	 */
	bool E_LoadUserCoord(int usrID);

	/**
	 * 获取当前用户坐标id.
	 *
	 * \param coordtype 用户坐标类型，引用
	 * \return 用户坐标号
	 */
	int E_GetCurUserCoordID(E_UserCoordType& coordtype);

	/**
	 * 获取用户坐标变量名称.
	 *
	 * \param coordID 用户坐标id
	 * \param coordtype 用户坐标类型
	 * \return 名称，为空则表示获取失败
	 */
	ESString E_GetUserCoordName(int coordID, E_UserCoordType coordtype);

	/**
	 * 获取用户坐标信息.
	 * 
	 * \param coordID 用户坐标id
	 * \param coordtype 用户坐标类型
	 * \param coord 用户坐标参数，引用
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetUserCoord(int coordID, E_UserCoordType coordtype, E_ROB_COORD& coord);

	/**
	 * 获取所有负载ID.
	 *
	 * \param num 获取到的个数
	 * \param idArray ID数组指针，由接口初始化
	 * \return 成功获取返回true
	 */
	bool E_GetPayLoadsID(int& num, int*& idArray);

	/**
	 * 创建负载.
	 * 
	 * \param payloadID 新的负载id，必须大于0
	 * \param payloadData 负载参数
	 * \return 成功返回true，失败返回false
	 */
	bool E_CreatePayLoad(int payloadID, const E_ROB_PAYLOAD& payloadData);

	/**
	 * 修改负载.
	 * 
	 * \param payloadID 要修改的负载id，必须大于0
	 * \param payloadData 负载参数
	 * \return 成功返回true，失败返回false
	 */
	bool E_ModifyPayLoad(int payloadID, const E_ROB_PAYLOAD& payloadData);

	/**
	 * 加载负载.
	 * 
	 * \param payloadID 负载id，如果为0表示不加载
	 * \return 成功返回true，失败返回false
	 */
	bool E_LoadPayLoad(int payloadID);

	/**
	 * 获取当前负载变量id.
	 * 
	 * \return 负载号
	 */
	int E_GetCurPayLoadID();

	/**
	 * 获取负载变量名称.
	 *
	 * \param payloadID 负载变量id
	 * \return 名称，为空则表示获取失败
	 */
	ESString E_GetPayLoadName(int payloadID);

	/**
	 * 获取负载变量.
	 * 
	 * \param payloadID 负载变量id
	 * \param payload 负载参数，引用
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetPayLoad(int payloadID, E_ROB_PAYLOAD& payload);

	/**
	 * 获取默认加载的负载参数.
	 * 
	 * \param payloadID 负载id，引用
	 * \param name 负载名称，引用
	 * \param payload 负载参数，引用
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetDefaultPayload(int& payloadID, ESString& name, E_ROB_PAYLOAD& payload);

	/**
	 * 设置IO的值.
	 * 
	 * \param IoIndex IoIndex 开始的IO下标，从1开始
	 * \param IoType IO类型
	 * \param dataIn 要修改的IO值数组指针
	 * \param dataNum 要修改的IO值数组元素个数
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetIOValue(int IoIndex, E_IOType IoType, double* dataIn, int dataNum);

	/**
	 * 获取IO的值.
	 * 
	 * \param IoIndex 开始的IO下标，从1开始
	 * \param IoType IO类型
	 * \param IoNum 查询IO数量，和数组元素个数一致
	 * \param dataOut 从【IoIndex】开始的【IoNum】个的IO的值数组，数组指针，需要预先申请内存
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetIOValue(int IoIndex, E_IOType IoType, int IoNum, double* dataOut);

	/**
	 * 设置IO备注.
	 * 
	 * \param IoType IO类型
	 * \param IoIndex IO下标，从1开始
	 * \param label 备注内容（utf格式）
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetIONote(E_IOType IoType, int IoIndex, ESString label);

	/**
	 * 获取IO备注.
	 * 
	 * \param IoType IO类型
	 * \param IoIndex IO下标，从1开始，为-1时表示获取该类所有IO备注
	 * \param label 备注内容返回值（utf格式，多个以';'隔开）
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetIONote(E_IOType IoType, int IoIndex, ESString& label);

	/**
	 * 刷新全局变量.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_RefreshGlobalVarFromFile();

	/**
	 * 设置变量是否掉电保存.
	 * 
	 * \param varType 变量类型
	 * \param varName 变量名
	 * \param varScope 作用域
	 * \param isSave 是否掉电保存
	 * \return 成功返回true
	 */
	bool E_SetVarIsSaveFile(E_VarType varType, ESString varName, E_ScopeType varScope, bool isSave);

	/**
	 * 设置变量.
	 * 
	 * \param varType 变量类型
	 * \param varName 变量名
	 * \param varValue 变量值，字符串类型，需要从其他类型转换
	 * \param varScope 变量域
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetVarValue(E_VarType varType, ESString varName, ESString varValue, E_ScopeType varScope);

	/**
	 * 获取变量.
	 * 
	 * \param varType 变量类型
	 * \param varName 变量名
	 * \param varScope 变量域
	 * \param valueBack 变量值，字符串类型，引用类型，需要转换为其他类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetVarValue(E_VarType varType, ESString varName, E_ScopeType varScope, ESString& valueBack);

	/**
	 * 设置默认到位精度值，掉电保存.
	 * 
	 * \param value 到位精度值
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetFine(double value);

	/**
	 * 获取默认到位精度值.
	 * 
	 * \param value 到位精度值，引用类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetFine(double& value);

	/**
	 * 设置点动坐标系类型.
	 * 
	 * \param coordType 坐标系类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetJogCoord(E_CoordType coordType);

	/**
	 * 获取点动坐标系类型.
	 * 
	 * \return 坐标系类型
	 */
	E_CoordType E_GetJogCoord();

	/**
	 * 设置jog是否可用.
	 * 
	 * \param isEnable 为true，jog可用，为false，jog不可用
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetJogEnable(bool isEnable);

	/**
	 * 获取jog是否可用.
	 *
	 * \return 可用返回true，不可用返回false
	 */
	bool E_GetJogEnable();

	/**
	 * 点动.
	 * 
	 * \param axisId 值[0,5]，对应关节坐标系下为轴1-6，世界坐标系下为xyzabc
	 * \param distention 运动方向
	 * \param mov 为true运动，为false停止
	 * \return 成功返回true，失败返回false
	 */
	bool E_JogMotion(int axisId, E_DistentionType distention, bool mov);

	/**
	 * 寸动.
	 * 
	 * \param axisId 值[0,5]，对应关节坐标系下为轴1-6，世界坐标系下为xyzabc
	 * \param distention 运动方向
	 * \param dist 寸动距离，单位毫米
	 * \param mov 为true运动，为false停止
	 * \return 成功返回true，失败返回false
	 */
	bool E_JogMicroMotion(int axisId, E_DistentionType distention, double dist, bool mov);

	/**
	 * 示教点.
	 * 
	 * \param pointName 位置点变量名
	 * \param scope 变量域，不能是系统域
	 * \return 成功返回true，失败返回false
	 */
	bool E_TeachPoint(ESString pointName, E_ScopeType scope);

	/**
	 * 获取机器人类型.
	 * 
	 * \param value 机器人型号
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetRobotType(ESString& value);

	/**
	 * 获取机器人信息.
	 * 
	 * \param value 机器人信息
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetRobotInfo(ESString& value);

	/**
	 * 获取本体轴数.
	 *
	 * \return 轴数，小于0表示获取失败
	 */
	int E_GetRobotAxisNum();

	/**
	 * 获取附加轴数.
	 *
	 * \return 轴数，小于0表示获取失败
	 */
	int E_GetAuxAxisNum();

	/**
	 * 获取机器人状态.
	 * 
	 * \return 12345表示机器人在线，其余为机器人不在线
	 */
	int E_GetRobotStatus();

	/**
	 * 设置全局速度.
	 * 
	 * \param value 速度值[0-100]
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetGlobalSpeed(int value);

	/**
	 * 获取全局速度.
	 * 
	 * \return 速度值,如果为0,可能是全局速度为0,也可能是出错,调用getLastErr获得错误信息
	 */
	int E_GetGlobalSpeed();

	/**
	 * 设置系统模式.
	 * 
	 * \param sysMode 系统模式，目前支支持手动和API，其余设置无效
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetSysMode(E_SysModeType sysMode);
	bool E_SetSysMode_ALL(E_SysModeType sysMode);

	/**
	 * 获取系统模式.
	 * 
	 * \return 系统模式
	 */
	E_SysModeType E_GetSysMode();

	/**
	 * 获取急停按钮状态.
	 * 
	 * \return true为急停按下
	 */
	bool E_GetEmergencyBtnStatus();

	/**
	 * 设置运动模式.
	 * 
	 * \param mode 运动模式
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetRunMode(E_RunModeType mode);

	/**
	 * 获取运动模式.
	 * 
	 * \return 运动模式
	 */
	E_RunModeType E_GetCurRunMode();

	/**
	 * 设置机器人伺服使能.
	 * 
	 * \param isOn 为true上使能，为false掉使能
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetServoOn(bool isOn);

	/**
	 * 获取机器人伺服是否使能.
	 * 
	 * \return 使能标志
	 */
	E_ServoStatusType E_GetServoOn();

	/**
	 * 获取机器人待机励磁状态.
	 *
	 * \return 使能标志
	 */
	E_ServoStatusType E_GetStandbyMotState();

	/**
	 * 设置震动抑制使能.
	 * 
	 * \param enable true为使能，false为不使能
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetMotionVibraEnable(bool enable);

	/**
	 * 获取震动抑制使能状态.
	 * 
	 * \param enable 获取的结果 true为使能，false为不使能
	 * \return  成功获取返回true，否则返回false
	 */
	bool E_GetMotionVibraEnable(bool& enable);

	/**
	 * 设置震动抑制等级.
	 * 
	 * \param axis 轴号1-6
	 * \param enumValue 震动抑制等级
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetMotionVibraParam(int axis, E_SuppresLevel enumValue);

	/**
	 * 获取震动抑制等级.
	 * 
	 * \param axis 轴号1-6
	 * \return 震动抑制等级
	 */
	E_SuppresLevel E_GetMotionVibraParam(int axis);

	/**
	 * 清除伺服报警.
	 * 
	 * \param name 配置名称
	 * \param index 配置序号（轴号）
	 * \return 成功返回true，失败返回false
	 */
	bool E_ResetServoError(ESString name, int index);

	/**
	 * 设置伺服参数.
	 * 
	 * \param name 配置名称
	 * \param index 配置序号（轴号）
	 * \param value 参数值
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetServoParam(ESString name, int index, int value);

	/**
	 * 获取伺服参数.
	 * 
	 * \param name 配置名称
	 * \param index 配置序号（轴号）
	 * \param value 获取的参数值
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetServoParam(ESString name, int index, int& value);

	/**
	 * 设置碰撞检测使能.
	 * 
	 * \param enable true为使能，false为不使能
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetCollisionEnable(bool enable);

	/**
	 * 获取碰撞检测使能状态.
	 * 
	 * \param enable 获取的结果 true为使能，false为不使能
	 * \return  成功获取返回true，否则返回false
	 */
	bool E_GetCollisionEnable(bool& enable);

	/**
	 * 设置碰撞检测参数.
	 * 
	 * \param axis 轴号（1~6）
	 * \param perValue 检测灵敏度，数值越小越灵敏（最大100%）
	 * \return 成功获取返回true，否则返回false
	 */
	bool E_SetCollisionParam(int axis, int perValue);

	/**
	 * 获取碰撞检测参数.
	 * 
	 * \param axis  轴号（1~6）
	 * \return 小于0表示获取失败，其他值为获取的检测灵敏度，数值越小越灵敏（最大100%）
	 */
	int E_GetCollisionParam(int axis);

	/**
	 * 单轴回零.
	 * 
	 * \param axis 轴号1-6
	 * \param value 单圈值
	 * \return 成功返回true，失败返回false
	 */
	bool E_HomeSingleJoint(int axis, unsigned int value);

	/**
	 * 整体回零.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_HomeAllJoint();

	/**
	 * 获取单圈值.
	 * 
	 * \param axis 轴号1-6
	 * \return 单圈值，0为获取失败
	 */
	unsigned int E_GetEncoderValueOfZore(int axis);

	/**
	 * 设置机器人安装方式.
	 * 
	 * \param InstallMode 
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetRobotInstallMode(E_RobotInstallType InstallMode);

	/**
	 * 获取机器人的安装方式.
	 * 
	 * \return 安装类型
	 */
	E_RobotInstallType E_GetRobotInstallMode();

	/**
	 * 获取是否支持吊装.
	 * 
	 * \return 支持返回true，不支持返回false
	 */
	bool E_GetRobotSupportCeiling();

	/**
	 * 向机器人推送告警信息.
	 * 
	 * \param ErrInfo
	 * \param ErrNum 90000以上的自定义值
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetRtToError(ESString ErrInfo, int ErrNum);

	/**
	 * 获取当前错误个数.
	 * 
	 * \return 小于0表示获取失败
	 */
	int E_GetErrorNum();

	/**
	 * 获取指定的错误信息.
	 * 
	 * \param index 指定的序号（从0开始，配合E_GetErrorNum获取的个数使用）
	 * \param code 获取的告警编号
	 * \param level 获取的告警等级
	 * \param text 获取的告警内容
	 * \param time 告警时间
	 * \return 成功获取放回true
	 */
	bool E_GetErrorInfo(int index, int& code, E_ErrorLevel& level, ESString& text, ESString& time);

	/**
	 * 清除所有错误报警.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_ClearError();

	/**
	 * 清除所有错误报警后获取是否还有错误.
	 *
	 * \return 没有错误返回true，有错误返回false
	 */
	bool E_CheckClearError();

	/**
	 * 清除错误报警.
	 *
	 * \param errorId 指定清楚的错误号
	 * \return 成功返回true，失败返回false
	 */
	bool E_ClearError(int errorId);

	/**
	 * 获取机器人当前错误号（目前最近触发的）.
	 * 
	 * \return 错误号
	 */
	int E_GetErrorId();

	/**
	 * 获取机器人错误内容.
	 * 
	 * \param errorId 错误号
	 * \return 错误详情
	 */
	ESString E_GetErrorString(int errorId);

	/**
	 * 获取机器人错误内容.
	 *
	 * \param errorId 错误号
	 * \param language 语言，目前支持ch-中文，en-英文
	 * \return 错误详情
	 */
	ESString E_GetErrorString(int errorId, ESString language/* = "ch"*/);

	/**
	 * 获取当前位置信息，关节坐标系.
	 * 
	 * \return 位置信息，APos
	 */
	E_ROB_POS E_GetCurJPos();

	/**
	 * 获取当前位置信息，世界坐标系.
	 * 
	 * \return 位置信息，CPos
	 */
	E_ROB_POS E_GetCurWPos();

	/**
	 * 设置当前外部坐标系.
	 * 
	 * \param ecpName 外部坐标系变量名，例：t_g.EXTTCP1
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetExternalTCPByName(ESString ecpName);

	/**
	 * 设置当前外部坐标系.
	 *
	 * \param ecpid 外部坐标系变量id，如果为0表示不加载
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetExternalTCPByID(int ecpid);

	/**
	 * 获取当前加载的程序名.
	 * 
	 * \param value 工程程序名，引用类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetCurProgramName(E_ROB_PROGNAME& value);

	/**
	 * 获取当前程序运行状态.
	 * 
	 * \return 程序运行状态
	 */
	E_RunStatusType E_GetCurProgramRunStatus();

	/**
	 * 卸载工程.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_UnLoadProject();

	/**
	 * 卸载程序.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_UnLoadProgram();

	/**
	 * 只加载工程.
	 * 
	 * \param prjname 工程名
	 * \return 成功返回true，失败返回false
	 */
	bool E_LoadProject(ESString prjname);

	/**
	 * 加载程序.
	 * 
	 * \param prjname 工程名
	 * \param programname 程序名
	 * \return 成功返回true，失败返回false
	 */
	bool E_LoadProgame(ESString prjname, ESString programname);

	/**
	 * 刷新变量依赖.
	 * 
	 * \return 成功返回true
	 */
	bool E_RefrushVarCE();

	/**
	 * 设置自启动程序.
	 * 
	 * \param prjname 工程名称
	 * \param programname 程序名称
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetProgAutoload(ESString prjname, ESString programname);

	/**
	 * 加载自动加载程序（需要在示教器上设置自动加载）.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_LoadAutoloadProg();

	/**
	 * 设置程序运行指针位置.
	 * 
	 * \param varScope
	 * \return 
	 */
	bool E_SetProgPC(int lineNum);

	/**
	 * 获取程序指针.
	 * 
	 * \return <0表示获取失败
	 */
	int E_GetCurrPC();

	/**
	 * 运行程序.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_ProgRun();

	/**
	 * 暂停程序.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_ProgPause();

	/**
	 * 停止程序.
	 * 
	 * \return 成功返回true，失败返回false
	 */
	bool E_ProgStop();

	/**
	 * 读取Int型变量值.
	 * 
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param valueBack 变量值，引用类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_ReadInt(ESString varName, E_ScopeType varScope, int& valueBack);

	/**
	 * 写入Int型变量值.
	 * 
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param value 变量值
	 * \return 成功返回true，失败返回false
	 */
	bool E_WriteInt(ESString varName, E_ScopeType varScope, int value);

	/**
	 * 读取Double型变量值.
	 * 
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param valueBack 变量值，引用类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_ReadReal(ESString varName, E_ScopeType varScope, double& valueBack);

	/**
	 * 写入Double型变量值.
	 * 
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param value 变量值
	 * \return 成功返回true，失败返回false
	 */
	bool E_WriteReal(ESString varName, E_ScopeType varScope, double value);

	/**
	 * 读取String型变量值.
	 *
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param valueBack 变量值，引用类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_ReadStr(ESString varName, E_ScopeType varScope, ESString& valueBack);

	/**
	 * 写入String型变量值.
	 *
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param value 变量值
	 * \return 成功返回true，失败返回false
	 */
	bool E_WriteStr(ESString varName, E_ScopeType varScope, ESString value);

	/**
	 * 读取位置类型变量值.
	 *
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param valueBack 变量值，引用类型
	 * \return 成功返回true，失败返回false
	 */
	bool E_ReadPos(ESString varName, E_ScopeType varScope, E_ROB_POS& valueBack);

	/**
	 * 写入位置类型变量值.
	 *
	 * \param varName 变量名称
	 * \param varScope 变量域
	 * \param value 变量值
	 * \return 成功返回true，失败返回false
	 */
	bool E_WritePos(ESString varName, E_ScopeType varScope, E_ROB_POS value);

	/**
	 * 设置DO的值.
	 * 
	 * \param IoValues IoValues IO值数组指针，+/-代表1/0，数值代表DO下标，例：{-1,2,6,-8}，表示将DO1、DO8设置为0，DO2、DO6设置为1
	 * \param number IoValues IO值数组个数
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetMultiDO(int* IoValues, int number);

	/**
	 * 获取最近一次下发成功的运动指令的信息.
	 * 
	 * \return 运动指令信息
	 */
	E_ROB_JOBID E_GetCurMovCmdJobId();

	/**
	 * 获取最近一次下发成功的运动指令的执行状态.
	 * 
	 * \return -2-错误 -1-未找到，0-运动未完成，1-运动完成,2-暂停，3-结束/运动错误，4-(预留类型)
	 */
	int E_GetCurMovCmdSts();

	/**
	 * 获取指定运动指令执行状态.
	 * 
	 * \param jobID 指定的运动指令信息
	 * \return -2-错误 -1-未找到，0-运动未完成，1-运动完成,2-暂停，3-结束/运动错误，4-(预留类型)
	 */
	int E_GetMovCmdSts(E_ROB_JOBID jobID);

	/**
	 * 接口耗时.
	 *
	 * \return 接口耗时
	 */
	double getLastFuncExecTime();

	/**
	 * 设置点位数组.
	 *
	 * \param varType 变量类型 只可以是CposArray或AposArray,其他类型无效
	 * \param varName 变量名
	 * \param varScope 变量域
	 * \param startIndex 指定存储的起始下标
	 * \param nums 指定存储的个数
	 * \param dataIn 存储的点位数组
	 * \return 成功返回true，失败返回false
	 */
	bool E_SetPosArrayValue(E_VarType varType, ESString varName, E_ScopeType varScope, int startIndex, int nums, E_ROB_POS* dataIn);

	/**
	 * 获取点位数组.
	 *
	 * \param varType 变量类型 只可以是CposArray或AposArray,其他类型无效
	 * \param varName 变量名
	 * \param varScope 变量域
	 * \param startIndex 指定存储的起始下标
	 * \param nums 指定存储的个数
	 * \param dataOut 返回的点位数组
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetPosArrayValue(E_VarType varType, ESString varName, E_ScopeType varScope, int startIndex, int nums, E_ROB_POS* dataOut);

	/**
	 * 获取力矩的值.
	 *
	 * \param dataOut 从力矩值的存储位置，需要预先申请内存，16个double的内存。
	 * \return 成功返回true，失败返回false
	 */
	bool E_GetAxeTorqueValue(double* dataOut);

	/**
	 * 设置控制器网络配置.
	 * 
	 * \param sRtosLan3IP lan3口IP
	 * \param sRtosLan3IPNetmask lan3口掩码
	 * \param sRtosLan3IPGateway lan3口网关
	 * \param sContrLan4IP lan4口IP
	 * \param sContrLan4IPNetmask lan4口掩码
	 * \param sContrLan4IPGateway lan4口网关
	 * \return 
	 */
	bool E_SetControllerExtIP(ESString sRtosLan3IP, ESString sRtosLan3IPNetmask, ESString sRtosLan3IPGateway, ESString sContrLan4IP, ESString sContrLan4IPNetmask, ESString sContrLan4IPGateway);
	
	/**
	 * 获取控制器网络配置.
	 * 
	 * \param sRtosLan3IP lan3口IP，引用
	 * \param sRtosLan3IPNetmask lan3口掩码，引用
	 * \param sRtosLan3IPGateway lan3口网关，引用
	 * \param sContrLan4IP lan4口IP，引用
	 * \param sContrLan4IPNetmask lan4口掩码，引用
	 * \param sContrLan4IPGateway lan4口网关，引用
	 * \return 
	 */
	bool E_GetControllerExtIP(ESString& sRtosLan3IP, ESString& sRtosLan3IPNetmask, ESString& sRtosLan3IPGateway, ESString& sContrLan4IP, ESString& sContrLan4IPNetmask, ESString& sContrLan4IPGateway);

	/**
	 * 获取IO个数.
	 * 
	 * \param IoType IO类型
	 * \return 个数，<0表示获取失败
	 */
	int E_GetIOCount(E_IOType IoType);
	
	/**
	 * 计算用户坐标系.
	 * 
	 * \param coord 计算结果，坐标值，引用
	 * \param UserCorrdinateParam 点位
	 * \param AxisType 轴类型
	 * \param AxisDir 轴方向
	 * \param PlaneType 面类型
	 * \param PlaneDir 面方向
	 * \return 成功返回true
	 */
	bool E_CalculateUserCorrdinate(E_ROB_COORD& coord, E_ROB_POS CorrdinateParam[3], E_CoordAxisType AxisType, E_DistentionType AxisDir, E_CoordPlaneType PlaneType, E_DistentionType PlaneDir);
	
	/**
	 * 计算工具坐标系_4点法.
	 * 
	 * \param coord 计算结果，坐标值，引用
	 * \param ToolCorrdinateParam 点位数组，4个元素
	 * \return 成功返回true
	 */
	bool E_CalculateToolCorrdinate_4Pnt(E_ROB_COORD& coord, E_ROB_POS ToolCorrdinateParam[4]);
	
	/**
	 * 计算工具坐标系_1点法.
	 * 
	 * \param coord 计算结果，坐标值，引用
	 * \param ToolCorrdinateParam 点位，1个
	 * \return 成功返回true
	 */
	bool E_CalculateToolCorrdinate_1Pnt(E_ROB_COORD& coord, const E_ROB_POS& ToolCorrdinateParam);

	/**
	 * 执行MovJ2数组指令.
	 *
	 * \param dstPointArray 目标位置点,轨迹点数组
	 * \param posNums 轨迹点个数，最大支持50个
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovJ2_Trajectory(E_ROB_POS* dstPointArray, int posNums, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);


	/**
	 * 执行MovL2指令数组.
	 *
	 * \param dstPointArray 目标位置点,轨迹点数组
	 * \param posNums 轨迹点的个数，最大支持50个
	 * \param velo 运行速度
	 * \param zone 过度参数
	 * \param payloadId 负载号
	 * \param toolId 工具号
	 * \param coordId 用户坐标号
	 * \param overrideType 速率类型, 0:GOVROFF;1:GOVRON
	 * \param addDo 附加指令，内容由其他结构体填充，需要调用AddDoTypeConvert的相关接口完成转换，目前仅支持E_ROB_ADDDO_IO类型
	 * \param valfine 到位精度值
	 * \param IsWaitFinish 是否等运动执行结束，为true则接口在运动执行到位后再返回，为false则立即返回
	 * \return 指令标识信息，id小于0为失败
	 */
	E_ROB_JOBID E_MovL2_Trajectory(E_ROB_POS* dstPointArray, int posNums, const E_ROB_SPEED& velo = E_ROB_SPEED(), const E_ROB_ZONE& zone = E_ROB_ZONE(), int payloadId = 0, int toolId = 0, int coordId = 0, int overrideType = 1, const E_ROB_APIBUFF& addDo = E_ROB_APIBUFF(), double valfine = 0.0, bool IsWaitFinish = false);

	/**
	 * 获取指定运动数组指令执行状态.
	 *
	 * \param posArrayJobID 指定的轨迹id信息
	 * \return -2-错误 -1-未找到，0-运动未完成，1-运动完成,2-暂停，3-结束/运动错误，4-(预留类型)
	 */
	int E_GetTrajectoryCmdSts(E_ROB_JOBID posArrayJobID);

	/**
	 * 获取指定运动数组指令执行状态.
	 *
	 * \param posArrayJobID 指定的轨迹id信息
	 * \param posAllJobIDBuff 接收获取到的运动信息的数组空间
	 * \param buffNums 数组容量，最大50个
	 * \return true-成功,false-失败
	 */
	bool E_GetTrajectoryPointsJobId(E_ROB_JOBID posArrayJobID, E_ROB_JOBID* posAllJobIDBuff,int buffNums);

	
	/**
	 * 笛卡尔坐标转关节坐标.
	 * 
	 * \param Cpos 笛卡尔坐标 mode,cf1,cf2,cf3,cf4,cf5,cf6,x,y,z,a,b,c
	 * \param Apos 关节坐标 a1,a2,a3,a4,a5,a6
	 * \return 转换成功返回true
	 */
	bool E_CposToAPos(double Cpos[13], double Apos[6]);

	/**
	 * 关节坐标转笛卡尔坐标.
	 * 
	 * \param Apos 关节坐标 a1,a2,a3,a4,a5,a6
	 * \param Cpos 笛卡尔坐标 mode,cf1,cf2,cf3,cf4,cf5,cf6,x,y,z,a,b,c
	 * \return 转换成功返回true
	 */
	bool E_AposToCPos(double Apos[6], double Cpos[13]);

	/**
	 * 获得机器人当前线速度
	 *
	 * \return 返回机器人线速度
	 */
	int E_GetCurTcpVelo();

	/**
	 * 弧焊通用协议设置指令.
	 * 
	 * \param procotol 弧焊自定义协议内容
	 * \return 成功返回true
	 */
	bool E_SetChannelForWeld(ESString procotol);

	/**
	 * 弧焊通用协议获取指令.
	 * 
	 * \param procotol 弧焊自定义协议内容
	 * \return 成功返回true
	 */
	bool E_GetChannelForWeld(ESString& procotol);

	/**
	 * 弧焊状态获取指令.
	 *
	 * \param procotol 弧焊自定义状态内容
	 * \return 成功返回true
	 */
	bool E_GetWeldStatus(ESString& procotol);

private:
	/**
	 * 内部接口.
	 * 
	 */
	void initConfig(std::string configDir);
	void sendRequest();
	void recvData(const char* data, size_t bytes_recv);
	void dealRecvMsg(char * data, size_t data_length);
	bool getIOValue(int IoIndex, E_IOType IoType, int IoNum, double* dataOut);
	void waitJobid();

public:
	typedef void(*recvCallBak)(const char*, size_t);
	/**
	 * 设置状态消息回调.
	 * 
	 * \param func 回调接口
	 */
	void setStatusDealFunc(recvCallBak func);

private:
	struct EstunRemoteApiLibPrivate;
	EstunRemoteApiLibPrivate* _p;
};

#endif //ESTUNREMOTEAPILIB_H