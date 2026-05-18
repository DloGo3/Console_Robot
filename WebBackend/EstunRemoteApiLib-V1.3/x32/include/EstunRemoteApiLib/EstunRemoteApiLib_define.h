/*****************************************************************//**
 * \file   EstunRemoteApiLib_define.h
 * \brief  
 * 
 * \author wanglei64101
 * \date   May 2022
 *********************************************************************/
#ifndef ESTUNREMOTEAPILIB_DEFINE_H
#define ESTUNREMOTEAPILIB_DEFINE_H

/**
 * 默认服务端口.
 */
#define SETIO_SERVER_PORT 23456
#define GETIO_SERVER_PORT 23457
#define STATUS_SERVER_PORT 6700

/**
 * 位置坐标值数量.
 */
#define ROBOT_AXLE_NUM 16
/**
 * 空间坐标系的cfg配置个数.
 */
#define POS_CFG_NUM 7
/**
 * 负载参数个数.
 */
#define PAYLOAD_CFG_NUM 10
/**
 * 工具参数个数.
 */
#define TOOL_CFG_NUM 16
/**
 * 坐标参数个数.
 */
#define COORD_CFG_NUM 6
/**
 * 运动状态包中的指令个数.
 */
#define MOV_STATUS_PACKAGE_NUM 300
/**
 * 机器人在线指定值.
 */
#define ROBOT_ONLINE 12345
/**
 * 延时时长（毫秒）.
 */
#define THREAD_SLEEP_MILESECONDS 5000

/**
 * 运动指令状态.
 * -2-error -1-notFound 0-default 1-finished 2-pause 3-error/stop 4-running
 */
#define CMD_EXE_SUCSSES 1
#define CMD_EXE_STOP 3
#define CMD_EXE_ERR -2

/**
 * 权限限定值.
 */
#define HAS_CMD_PERMIT 1

/**
 * 限定工作模式.
 */
#define API_SYS_MODE 5//API模式

/**
 * APIStatus Err.
 */
#define API_STATUS_ERR 1
#endif // !ESTUNREMOTEAPILIB_DEFINE_H




