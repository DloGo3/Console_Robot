/*****************************************************************//**
 * \file   EstunRemoteApiLib_struct.h
 * \brief  
 * 
 * \author wanglei64101
 * \date   May 2022
 *********************************************************************/
#ifndef ESTUNREMOTEAPILIB_STRUCT_H
#define ESTUNREMOTEAPILIB_STRUCT_H

#include "EstunRemoteApiLib_global.h"

#include <vector>
#include <string>
/**
 * AddDo结构内容大小.
 */
#define APIBUFFLENGTH 512
namespace EstunRobot
{
	/**
	 * IO类型.
	 * 0-DI，1-DO，2-AO，3-AI，11-SIM_DI，12-SIM_DO，13-SIM_AI，14-SIM_AO
	 */
	enum E_IOType 
	{
		DI = 0,
		DO = 1,
		AO = 2,
		AI = 3,
		SIM_DI = 11,
		SIM_DO = 12,
		SIM_AI = 13,
		SIM_AO = 14,
	};

	/**
	 * 数据类型.
	 * 0-其他类型，1-整型，2-实数，3-关节位置，4-空间位置，5-字符串，6-实数数组，7-工具参数，8-用户坐标参数，11-空间坐标数组，12-轴关节坐标数组
	 */
	enum E_VarType 
	{
		OtherType = 0,
		IntType = 1,
		RealType = 2,
		AposType = 3,
		CposType = 4,
		StrType = 5,
		RealIntArray = 6,
		ToolType = 7,
		UserType = 8,
		CposArray = 11,
		AposArray = 12,
		BoolType = 13,
		PayloadType = 14
	};

	/**
	 * 变量域类型.
	 * 0-系统，1-全局，2-工程，3-程序本地
	 */
	enum E_ScopeType 
	{
		system = 0,
		global,
		project,
		local,
	};

	/**
	*坐标类型
	*0-关节坐标，1-世界坐标，2-工具坐标，3-用户坐标，4-错误
	*/
	enum E_CoordType  
	{
		axisCoord = 0,
		worldCoord,
		toolCoord,
		usrCoord,
		errCoord,
	};

	/**
	*用户坐标系类型
	*-1-类型错误，0-用户坐标，1-外部坐标系，
	*/
	enum E_UserCoordType
	{
		ErrorUserCoord = -1,
		UsrCoord,
		ExtCoord,
	};

	/**
	 * 描述坐标的哪一轴.
	 * 1-x轴，2-y轴，3-z轴
	 */
	enum E_CoordAxisType
	{
		E_X = 1,
		E_Y,
		E_Z
	};

	/**
	 * 描述坐标平面.
	 * 1-xy平面，2-zx平面，3-yz平面
	 */
	enum E_CoordPlaneType
	{
		E_XY = 1,
		E_ZX,
		E_YZ
	};

	/**
	*方向类型
	*1-为正方向，0-负方向
	*/
	enum E_DistentionType 
	{
		negative = 0,
		forward,
	};

	/**
	*系统模式类型
	*0-手动，1-自动，2-远程，3-禁止，4-多工位，5-API，6-错误
	*/
	enum E_SysModeType 
	{
		manualMode = 0,
		autoMode,
		remoteMode,
		banMode,
		mutil,
		API,
		errSysMode,
	};

	/**
	 * 告警等级.
	 * 1-严重错误，2-普通错误，3-警告，4-提示
	 */
	enum E_ErrorLevel
	{
		fault_error = 1,
		error,
		warning,
		info,
	};

	/**
	*运动模式类型
	* 1-单步，2-连续，3-错误
	*/
	enum E_RunModeType 
	{
		step = 1,
		sustain,
		errRunMode,
	};

	/**
	 * 运动状态类型.
	 * 1-运行，2-暂停，3-停止，4-错误
	 */
	enum E_RunStatusType 
	{
		running = 1,
		suspend,
		stop,
		errRunStatus,
	};

	/**
	 * mot使能状态.
	 * 0-未使能, 1-使能, 3-错误
	 */
	enum E_ServoStatusType
	{
		ServoOff = 0,
		ServoOn,
		errStatus,
	};

	/**
	 * 安装方式.
	 * 1-标准按照，2-吊装
	 */
	enum E_RobotInstallType
	{
		Normal = 0,
		Ceiling,
		ErrType,
	};

	/**
	 * 震动抑制等级.
	 */
	enum E_SuppresLevel
	{
		Standard = 0,
		High,
		MidHigh,
		MidLow,
		Low,
		ErrLevel
	};

	/**
	 * 权限信息.
	 */
	struct E_ROB_PERMIT 
	{
		/**
		 * 权限唯一码，大于1的整数表示有运动操作权限，否则没有.
		 */
		uint16_t 	m_mainctrlcode;
		/**
		 * 时间戳，时间戳大于0表示正确获取到权限，否则获取权限失败.
		 */
		uint16_t	m_timestamp;
	};

	/**
	 * 指令标识信息.
	 */
	struct E_ROB_JOBID
	{
		/**
		 * 构造函数.
		 * 
		 */
		E_ROB_JOBID()
		{
			m_jobID = -1;
			m_timeStamp = 0;
		}

		/**
		 * 大于0表示指令下发成功，否则表示指令下发失败.
		 */
		int m_jobID;
		/**
		 * 运动指令下发成功的当前时间戳.
		 */
		uint32_t m_timeStamp;
	};

	/**
	 * 内部字符串类型.
	 */
	class ESTUNREMOTEAPILIBAPI ESString
	{
	public:
		/**
		 * 默认构造函数，构造一个空字符串.
		 *
		 */
		ESString();

		/**
		 * 构造函数，构造一个内容为c_str的字符串.
		 *
		 * \param c_str 字符串内容
		 */
		ESString(const char* c_str);

		/**
		 * 拷贝构造函数.
		 *
		 * \param t 内部字符串类型
		 */
		ESString(const ESString& t);

		/**
		 * 析构函数.
		 *
		 */
		~ESString();

		/**
		 * 赋值运算符.
		 *
		 * \param t 内部字符串类型
		 * \return 内部字符串类型
		 */
		ESString& operator=(const ESString& t);
		ESString& operator=(const char* t);

		/**
		 * 获取字符串大小.
		 *
		 * \return 字符串大小
		 */
		size_t size();

		/**
		 * 获取字符串内容.
		 *
		 * \return 字符串内容
		 */
		const char* c_str() const;

	private:
		struct ESStringPrivate;
		ESStringPrivate *_p;
	};

	/**
	 * 工程程序名.
	 */
	struct E_ROB_PROGNAME
	{
		/**
		 * 当前加载的工程名.
		 */
		ESString prjname;
		/**
		 * 当前加载的程序名.
		 */
		ESString programname;
		/**
		 * 当前运行的程序名.如果运行的工程不是当前加载工程，格式为"工程名;程序名"
		 */
		ESString exeprogramName;
	};

	/**
	 * 附加指令，具体内容由其他结构参数转换得到，需要调用AddDoTypeConvert相关接口.
	 */
	struct E_ROB_APIBUFF
	{
		/**
		 * 构造函数，将内存置0.
		 * 
		 */
		E_ROB_APIBUFF()
			:m_type(0)
		{
			memset(m_data, 0, APIBUFFLENGTH);
		}

		/**
		 * 附加指令类型.
		 */
		uint8_t m_type;
		/**
		 * 指令内容二进制.
		 */
		uint8_t m_data[APIBUFFLENGTH];
	};

	/**
	 * 附加指令结构一.
	 */
	struct E_ROB_ADDDO_IO
	{
		/**
		 * 构造函数.
		 * 
		 */
		E_ROB_ADDDO_IO()
		{
			ioType = DI;
			ioindex = 0;
			iovalue = 0;
			trigType = 0;
			exTimes = 0;
			exPercent = 0;
			exDistance = 0;
		}

		/**
		 * IO类型.
		 */
		E_IOType ioType;
		/**
		 * IO序号.
		 */
		int ioindex;
		/**
		 * IO值.
		 */
		double iovalue;
		/**
		 * 触发类型（1:路径百分比；2：距离）.
		 */
		int trigType;
		/**
		 * 触发延时,单位毫秒，负数为提前触发.
		 */
		int exTimes;
		/**
		 * 当前运动执行触发百分比.
		 */
		int exPercent;
		/**
		 * 当前运动执行触发距离.
		 */
		double exDistance;
	};

	/**
	 * 数据类型基类.
	 */
	class ESTUNREMOTEAPILIBAPI IObject
	{
	public:
		IObject();
		virtual ~IObject();

		/**
		 * 获取值是否有效.
		 * 
		 * \return 有效返回true，无效返回false
		 */
		bool getIsValid() const;

		/**
		 * 设置值是否有效.
		 * 
		 * \param val 有效为true，无效为false
		 */
		void setIsValid(bool val);

	private:
		struct IObjectPrivate;
		IObjectPrivate* _p;
	};

	/**
	 * 空间位置形态配置数据.
	 */
	class ESTUNREMOTEAPILIBAPI E_ROB_POSCFG : public IObject
	{
	public:
		/**
		 * 默认构造，值无效.
		 * 
		 */
		E_ROB_POSCFG();

		/**
		 * 基本构造函数.
		 * 
		 * \param mode 关节位置模式，[0,7]
		 * \param cf1 关节 1 号轴角度所在的象限值
		 * \param cf2 关节 2 号轴角度所在的象限值
		 * \param cf3 关节 3 号轴角度所在的象限值
		 * \param cf4 关节 4 号轴角度所在的象限值
		 * \param cf5 关节 5 号轴角度所在的象限值
		 * \param cf6 关节 6 号轴角度所在的象限值
		 */
		E_ROB_POSCFG(int mode, int cf1 = 0, int cf2 = 0, int cf3 = 0, int cf4 = 0, int cf5 = 0, int cf6 = 0);

		/**
		 * 数组参数构造函数，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param c_array 形态配置数据，{mode,cf1,cf2,cf3,cf4,cf5,cf6}
		 */
		E_ROB_POSCFG(const std::vector<int>& c_array);
		E_ROB_POSCFG(int c_array[7]);

		/**
		 * 拷贝构造函数.
		 * 
		 * \param t 空间位置形态配置数据源
		 */
		E_ROB_POSCFG(const E_ROB_POSCFG& t);

		/**
		 * 析构函数.
		 * 
		 */
		~E_ROB_POSCFG();

		/**
		 * 设置形态配置数据，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 形态配置数据，{mode,cf1,cf2,cf3,cf4,cf5,cf6}
		 */
		void setCfgByArray(const std::vector<int>& array);
		void setCfgByArray(int array[7]);

		/**
		 * 获取关节位置模式.
		 * 
		 * \return 关节位置模式[0,7]
		 */
		int getMode() const;

		/**
		 * 设置关节位置模式.
		 * 
		 * \param val 关节位置模式[0,7]
		 */
		void setMode(int val);

		/**
		 * 获取轴角度所在的象限值.
		 * 
		 * \param index 轴号，mode,cf1,cf2,cf3,cf4,cf5,cf6
		 * \return 轴角度所在的象限值
		 */
		int getCF_ValByIndex(int index) const;

		/**
		 * 设置轴角度所在的象限取值.
		 * 
		 * \param index 轴号，mode,cf1,cf2,cf3,cf4,cf5,cf6
		 * \param val 轴角度所在的象限值
		 */
		void setCF_ValByIndex(int index, int val);

		/**
		 * 获取空间位置形态配置数据数组，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \return 数组，{mode,cf1,cf2,cf3,cf4,cf5,cf6}
		 */
		std::vector<int> getValueList() const;
		void getValueList(int res[7]);

	private:
		struct RobotPos_POSCFGPrivate;
		RobotPos_POSCFGPrivate* _p;
	};

	/**
	 * 机器人位置类.
	 */
	class ESTUNREMOTEAPILIBAPI E_ROB_POS : public IObject
	{
	public:
		/**
		 * 默认构造，值无效.
		 *
		 */
		E_ROB_POS();
		
		/**
		 * 轴关节坐标构造函数.
		 * 
		 * \param j1 关节 1 号轴的坐标
		 * \param j2 关节 2 号轴的坐标
		 * \param j3 关节 3 号轴的坐标
		 * \param j4 关节 4 号轴的坐标
		 * \param j5 关节 5 号轴的坐标
		 * \param j6 关节 6 号轴的坐标
		 * \param j7 关节 7 号轴的坐标，选填，默认为0
		 * \param j8 关节 8 号轴的坐标，选填，默认为0
		 * \param j9 关节 9 号轴的坐标，选填，默认为0
		 * \param j10 关节 10 号轴的坐标，选填，默认为0
		 * \param j11 关节 11 号轴的坐标，选填，默认为0
		 * \param j12 关节 12 号轴的坐标，选填，默认为0
		 * \param j13 关节 13 号轴的坐标，选填，默认为0
		 * \param j14 关节 14 号轴的坐标，选填，默认为0
		 * \param j15 关节 15 号轴的坐标，选填，默认为0
		 * \param j16 关节 16 号轴的坐标，选填，默认为0
		 */
		E_ROB_POS(double j1, double j2, double j3, double j4, double j5, double j6,
			double j7 = 0, double j8 = 0, double j9 = 0, double j10 = 0, double j11 = 0, double j12 = 0,
			double j13 = 0, double j14 = 0, double j15 = 0, double j16 = 0);

		/**
		 * 轴关节坐标构造函数，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param apos_Array 坐标数组，{j1,j2,j3...j16}
		 */
		E_ROB_POS(const std::vector<double>& apos_Array);
		E_ROB_POS(double apos_Array[16]);
		
		/**
		 * 空间坐标构造函数.
		 * 
		 * \param cfg 空间位置形态配置数据
		 * \param x TCP 点在参考坐标系上 x 方向的坐标
		 * \param y TCP 点在参考坐标系上 y 方向的坐标
		 * \param z TCP 点在参考坐标系上 z 方向的坐标
		 * \param a TCP 点相对于参考坐标系 z 轴旋转的欧拉角
		 * \param b TCP 点相对于参考坐标系 y 轴旋转的欧拉角
		 * \param c TCP 点相对于参考坐标系 x 轴旋转的欧拉角
		 * \param a7 附加轴关节 7 号轴的坐标，选填，默认为0
		 * \param a8 附加轴关节 8 号轴的坐标，选填，默认为0
		 * \param a9 附加轴关节 9 号轴的坐标，选填，默认为0
		 * \param a10 附加轴关节 10 号轴的坐标，选填，默认为0
		 * \param a11 附加轴关节 11 号轴的坐标，选填，默认为0
		 * \param a12 附加轴关节 12 号轴的坐标，选填，默认为0
		 * \param a13 附加轴关节 13 号轴的坐标，选填，默认为0
		 * \param a14 附加轴关节 14 号轴的坐标，选填，默认为0
		 * \param a15 附加轴关节 15 号轴的坐标，选填，默认为0
		 * \param a16 附加轴关节 16 号轴的坐标，选填，默认为0
		 */
		E_ROB_POS(const E_ROB_POSCFG& cfg, double x, double y, double z, double a, double b, double c,
			double a7 = 0, double a8 = 0, double a9 = 0, double a10 = 0, double a11 = 0, double a12 = 0,
			double a13 = 0, double a14 = 0, double a15 = 0, double a16 = 0);

		/**
		 * 空间坐标构造函数，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param cfg_Array 形态配置数据，{mode,cf1,cf2,cf3,cf4,cf5,cf6}
		 * \param cpos_Array 空间坐标数组，{x,y,z,a,b,c,a7...a16}
		 */
		E_ROB_POS(const std::vector<int>& cfg_Array, const std::vector<double>& cpos_Array);
		E_ROB_POS(int cfg_Array[7], double cpos_Array[16]);

		/**
		 * 拷贝构造函数.
		 * 
		 * \param t 轴关节坐标/空间坐标
		 */
		E_ROB_POS(const E_ROB_POS& t);

		/**
		 * 析构函数.
		 * 
		 */
		~E_ROB_POS();

		/**
		 * 赋值运算符.
		 * 
		 * \param t 轴关节坐标/空间坐标
		 * \return 轴关节坐标/空间坐标
		 */
		E_ROB_POS& operator=(const E_ROB_POS& t);

		/**
		 * 坐标类型.
		 * 0-轴关节坐标，1-空间坐标
		 */
		enum PosType
		{
			PT_APos = 0,
			PT_CPos
		};

		/**
		 * 获取坐标类型.
		 * 
		 * \return 坐标类型
		 */
		PosType getPosType() const;

		/**
		 * 设置坐标类型.
		 * 
		 * \param pt 坐标类型
		 */
		void setPosType(PosType pt);

		/**
		 * 设置轴关节坐标数组，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 轴关节坐标数组，{j1,j2,j3...j16}
		 */
		void setAPosByArray(const std::vector<double>& array);
		void setAPosByArray(double array[16]);

		/**
		 * 获取某个轴的坐标.
		 * 
		 * \param index 轴号，[1,16]
		 * \return 轴关节坐标
		 */
		double getApos_ValByIndex(int index) const;

		/**
		 * 设置某个的坐标.
		 * 
		 * \param index 轴号，[1,16]
		 * \param val 轴关节坐标
		 */
		void setApos_ValByIndex(int index, double val);

		/**
		 * 设置空间位置形态配置数据，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 形态配置数据，{mode,cf1,cf2,cf3,cf4,cf5,cf6}
		 */
		void setCfgByArray(const std::vector<double>& array);
		void setCfgByArray(int array[7]);

		/**
		 * 设置空间坐标数组，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 空间坐标数组，{x,y,z,a,b,c,a7...a16}
		 */
		void setCPosByArray(const std::vector<double>& array);
		void setCPosByArray(double array[16]);

		/**
		 * 获取Cfg值.
		 *
		 * \param outData 获得cfg数据
		 * \return true成功, false失败
		 */
		bool GetCfgValue(E_ROB_POSCFG& outData);

		/**
		 * 获取Cfg值.
		 *
		 * \param dataIn cfg值
		 */
		void SetCfgValue(E_ROB_POSCFG dataIn);

		/**
		 * 获取TCP 点在参考坐标系上 x 方向的坐标.
		 * 
		 * \return 坐标值
		 */
		double getX() const;

		/**
		 * 设置TCP 点在参考坐标系上 x 方向的坐标.
		 * 
		 * \param val 坐标值
		 */
		void setX(double val);

		/**
		 * 获取TCP 点在参考坐标系上 y 方向的坐标.
		 *
		 * \return 坐标值
		 */
		double getY() const;

		/**
		 * 设置TCP 点在参考坐标系上 y 方向的坐标.
		 *
		 * \param val 坐标值
		 */
		void setY(double val);

		/**
		 * 获取TCP 点在参考坐标系上 z 方向的坐标.
		 *
		 * \return 坐标值
		 */
		double getZ() const;

		/**
		 * 设置TCP 点在参考坐标系上 z 方向的坐标.
		 *
		 * \param val 坐标值
		 */
		void setZ(double val);

		/**
		 * 获取TCP 点相对于参考坐标系 z 轴旋转的欧拉角.
		 * 
		 * \return 角度值
		 */
		double getA() const;

		/**
		 * 设置TCP 点相对于参考坐标系 z 轴旋转的欧拉角.
		 * 
		 * \param val 角度值
		 */
		void setA(double val);

		/**
		 * 获取TCP 点相对于参考坐标系 y 轴旋转的欧拉角.
		 *
		 * \return 角度值
		 */
		double getB() const;

		/**
		 * 设置TCP 点相对于参考坐标系 y 轴旋转的欧拉角.
		 *
		 * \param val 角度值
		 */
		void setB(double val);

		/**
		 * 获取TCP 点相对于参考坐标系 x 轴旋转的欧拉角.
		 *
		 * \return 角度值
		 */
		double getC() const;

		/**
		 * 设置TCP 点相对于参考坐标系 x 轴旋转的欧拉角.
		 *
		 * \param val 角度值
		 */
		void setC(double val);

		/**
		 * 获取空间坐标的附加轴关节坐标.
		 * 
		 * \param index 轴号 [7,16]
		 * \return 轴关节坐标
		 */
		double getExValByIndex(int index) const;

		/**
		 * 设置空间坐标的附加轴关节坐标..
		 * 
		 * \param index 轴号 [7,16]
		 * \param val 轴关节坐标
		 */
		void setExValByIndex(int index, double val);

		/**
		 * 获取位置的值数组，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \return 类型为轴关节坐标时获取{j1,j2,j3...j16}，类型为空间坐标时获取{mode,cf1,cf2,cf3,cf4,cf5,cf6,}
		 */
		std::vector<double> getValueList() const;
		void getValueList(double value[23]);

	private:
		struct RobotPosPrivate;
		RobotPosPrivate* _p;
	};

	/**
	 * 机器人运动速度类.
	 */
	class ESTUNREMOTEAPILIBAPI E_ROB_SPEED : public IObject
	{
	public:
		/**
		 * 默认构造，值无效.
		 *
		 */
		E_ROB_SPEED();

		/**
		 * 基本构造函数.
		 * 
		 * \param m_Per 关节速度百分比
		 * \param m_Tcp TCP 线速度
		 * \param m_ori 空间旋转速度
		 * \param m_exj_l 外部轴线速度
		 * \param m_exj_r 外部轴角速度
		 */
		E_ROB_SPEED(double m_Per,
			double m_Tcp,
			double m_ori,
			double m_exj_l,
			double m_exj_r);

		/**
		 * 拷贝构造函数.
		 * 
		 * \param t 机器人运动速度
		 */
		E_ROB_SPEED(const E_ROB_SPEED& t);

		/**
		 * 析构函数.
		 * 
		 */
		~E_ROB_SPEED();

		/**
		 * 赋值运算符.
		 * 
		 * \param t 机器人运动速度
		 * \return 机器人运动速度
		 */
		E_ROB_SPEED& operator=(const E_ROB_SPEED& t);

		/**
		 * 获取关节速度百分比.
		 * 
		 * \return 关节速度百分比
		 */
		double get_Per() const;

		/**
		 * 设置关节速度百分比.
		 * 
		 * \param val 关节速度百分比
		 */
		void set_Per(double val);

		/**
		 * 获取TCP 线速度.
		 * 
		 * \return TCP 线速度
		 */
		double get_Tcp() const;

		/**
		 * 设置TCP 线速度.
		 * 
		 * \param val TCP 线速度
		 */
		void set_Tcp(double val);

		/**
		 * 获取空间旋转速度.
		 * 
		 * \return 空间旋转速度
		 */
		double get_ori() const;

		/**
		 * 设置空间旋转速度.
		 * 
		 * \param val 空间旋转速度
		 */
		void set_ori(double val);

		/**
		 * 外部轴线速度.
		 * 
		 * \return 外部轴线速度
		 */
		double get_exj_l() const;

		/**
		 * 外部轴线速度.
		 * 
		 * \param val 外部轴线速度
		 */
		void set_exj_l(double val);

		/**
		 * 外部轴角速度.
		 * 
		 * \return 外部轴角速度
		 */
		double get_exj_r() const;

		/**
		 * 外部轴角速度.
		 * 
		 * \param val 外部轴角速度
		 */
		void set_exj_r(double val);

	private:
		struct RobotSpeedPrivate;
		RobotSpeedPrivate* _p;
	};

	/**
	 * 机器人转弯区类.
	 */
	class ESTUNREMOTEAPILIBAPI E_ROB_ZONE : public IObject
	{
	public:
		/**
		 * 默认构造，值无效.
		 *
		 */
		E_ROB_ZONE();

		/**
		 * 基本构造函数.
		 * 
		 * \param m_ZoneType 过渡类型(0-无过渡/1-绝对过渡/2-相对过渡);
		 * \param m_Per 关节轨迹转弯百分比
		 * \param m_dis 笛卡尔空间转弯区大小
		 * \param m_vConst 恒速过渡使能状态
		 */
		E_ROB_ZONE(int8_t m_ZoneType, double m_Per, double m_dis, int m_vConst);

		/**
		 * 拷贝构造函数.
		 * 
		 * \param t 机器人转弯区
		 */
		E_ROB_ZONE(const E_ROB_ZONE& t);

		/**
		 * 析构函数.
		 * 
		 */
		~E_ROB_ZONE();

		/**
		 * 赋值运算符.
		 * 
		 * \param t
		 * \return 
		 */
		E_ROB_ZONE& operator=(const E_ROB_ZONE& t);

		/**
		 * 获取过渡类型.
		 * 
		 * \return 0-无过渡，1-绝对过渡，2-相对过渡
		 */
		int8_t get_ZoneType() const;

		/**
		 * 设置获取过渡类型.
		 * 
		 * \param val 0-无过渡，1-绝对过渡，2-相对过渡
		 */
		void set_ZoneType(int8_t val);

		/**
		 * 获取关节轨迹转弯百分比.
		 * 
		 * \return 关节轨迹转弯百分比
		 */
		double get_Per() const;

		/**
		 * 设置关节轨迹转弯百分比.
		 * 
		 * \param val 关节轨迹转弯百分比
		 */
		void set_Per(double val);

		/**
		 * 获取笛卡尔空间转弯区大小.
		 * 
		 * \return 笛卡尔空间转弯区大小
		 */
		double get_Dis() const;

		/**
		 * 设置笛卡尔空间转弯区大小.
		 * 
		 * \param val 笛卡尔空间转弯区大小
		 */
		void set_Dis(double val);

		/**
		 * 恒速过渡使能状态.
		 * 
		 * \return 恒速过渡使能状态
		 */
		int get_vConst() const;

		/**
		 * 设置恒速过渡使能状态.
		 * 
		 * \param val 恒速过渡使能状态
		 */
		void set_vConst(int val);

	private:
		struct RobotZonePrivate;
		RobotZonePrivate* _p;
	};

	/**
	 * 机器人负载类.
	 */
	class ESTUNREMOTEAPILIBAPI E_ROB_PAYLOAD : public IObject
	{
	public:
		/**
		 * 默认构造，值无效.
		 *
		 */
		E_ROB_PAYLOAD();

		/**
		 * 基本构造函数.
		 * 
		 * \param m_M 工具&负载的重量， 单位是 kg
		 * \param m_Mx 安装的工具或装夹的负载在坐标系 OTool-XYZ 的 X 方向上的偏移量，单位是 mm
		 * \param m_My 安装的工具或装夹的负载在坐标系 OTool-XYZ 的 Y 方向上的偏移量，单位是 mm
		 * \param m_Mz 安装的工具或装夹的负载在坐标系 OTool-XYZ 的 Z 方向上的偏移量，单位是 mm
		 * \param m_Ixx 安装的工具或装夹的负载在重心位臵的 X 方向回转的惯量矩，单位是 kg〮 mm2
		 * \param m_Iyy 安装的工具或装夹的负载在重心位臵的 Y 方向回转的惯量矩，单位是 kg〮 mm2
		 * \param m_Izz 安装的工具或装夹的负载在重心位臵的 Z 方向回转的惯量矩，单位是 kg〮 mm2
		 * \param m_Ixy 安装的工具或装夹的负载在重心位臵的 XY 交叉方向的惯量积，单位是 kg〮 mm2
		 * \param m_Ixz 安装的工具或装夹的负载在重心位臵的 XZ 交叉方向的惯量积，单位是 kg〮 mm2
		 * \param m_Iyz 安装的工具或装夹的负载在重心位臵的 YZ 交叉方向的惯量积，单位是 kg〮 mm2
		 */
		E_ROB_PAYLOAD(double m_M, double m_Mx, double m_My, double m_Mz, double m_Ixx = 0,
			double m_Iyy = 0, double m_Izz = 0, double m_Ixy = 0, double m_Ixz = 0, double m_Iyz = 0);

		/**
		 * 数组构造函数，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 负载数据数组，{m_M,m_Mx,m_My,m_Mz,m_Ixx,m_Iyy,m_Izz,m_Ixy,m_Ixz,m_Iyz}
		 */
		E_ROB_PAYLOAD(const std::vector<double>& array);
		E_ROB_PAYLOAD(double array[10]);

		/**
		 * 拷贝构造函数.
		 * 
		 * \param t 机器人负载
		 */
		E_ROB_PAYLOAD(const E_ROB_PAYLOAD& t);

		/**
		 * 析构函数.
		 * 
		 */
		~E_ROB_PAYLOAD();

		/**
		 * 赋值运算符.
		 * 
		 * \param t 机器人负载
		 * \return 机器人负载
		 */
		E_ROB_PAYLOAD& operator=(const E_ROB_PAYLOAD& t);

		/**
		 * 设置负载的值，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 负载数据数组，{m_M,m_Mx,m_My,m_Mz,m_Ixx,m_Iyy,m_Izz,m_Ixy,m_Ixz,m_Iyz}
		 */
		void setPayloadByArray(const std::vector<double>& array);
		void setPayloadByArray(double array[10]);

		/**
		 * 获取工具&负载的重量.
		 * 
		 * \return 工具&负载的重量
		 */
		double get_M() const;

		/**
		 * 设置工具&负载的重量.
		 * 
		 * \param val 工具&负载的重量
		 */
		void set_M(double val);

		/**
		 * 获取 X 方向上的偏移量.
		 * 
		 * \return X 方向上的偏移量
		 */
		double get_Mx() const;

		/**
		 * 设置 X 方向上的偏移量.
		 * 
		 * \param val X 方向上的偏移量
		 */
		void set_Mx(double val);

		/**
		 * 获取 Y 方向上的偏移量.
		 * 
		 * \return Y 方向上的偏移量
		 */
		double get_My() const;

		/**
		 * 设置 Y 方向上的偏移量.
		 * 
		 * \param val Y 方向上的偏移量
		 */
		void set_My(double val);

		/**
		 * 获取 Z 方向上的偏移量.
		 * 
		 * \return Z 方向上的偏移量
		 */
		double get_Mz() const;

		/**
		 * 设置 Z 方向上的偏移量.
		 * 
		 * \param val Z 方向上的偏移量
		 */
		void set_Mz(double val);

		/**
		 * 获取 X 方向回转的惯量矩.
		 * 
		 * \return X 方向回转的惯量矩
		 */
		double get_Ixx() const;

		/**
		 * 设置 X 方向回转的惯量矩.
		 * 
		 * \param val X 方向回转的惯量矩
		 */
		void set_Ixx(double val);

		/**
		 * 获取 Y 方向回转的惯量矩.
		 * 
		 * \return Y 方向回转的惯量矩
		 */
		double get_Iyy() const;

		/**
		 * 设置 Y 方向回转的惯量矩.
		 * 
		 * \param val Y 方向回转的惯量矩
		 */
		void set_Iyy(double val);

		/**
		 * 获取 Z 方向回转的惯量矩.
		 * 
		 * \return Z 方向回转的惯量矩
		 */
		double get_Izz() const;

		/**
		 * 设置 Z 方向回转的惯量矩.
		 * 
		 * \param val Z 方向回转的惯量矩
		 */
		void set_Izz(double val);

		/**
		 * 获取 XY 方向回转的惯量矩.
		 * 
		 * \return XY 方向回转的惯量矩
		 */
		double get_Ixy() const;

		/**
		 * 设置 XY 方向回转的惯量矩.
		 * 
		 * \param val XY 方向回转的惯量矩
		 */
		void set_Ixy(double val);

		/**
		 * 获取 YZ 方向回转的惯量矩.
		 * 
		 * \return YZ 方向回转的惯量矩
		 */
		double get_Iyz() const;

		/**
		 * 设置 YZ 方向回转的惯量矩.
		 * 
		 * \param val YZ 方向回转的惯量矩
		 */
		void set_Iyz(double val);

		/**
		 * 设置 XZ 方向回转的惯量矩.
		 * 
		 * \return XZ 方向回转的惯量矩
		 */
		double get_Ixz() const;

		/**
		 * 获取 XZ 方向回转的惯量矩.
		 * 
		 * \param val XZ 方向回转的惯量矩
		 */
		void set_Ixz(double val);

	private:
		struct RobotPayloadPrivate;
		RobotPayloadPrivate* _p;
	};

	/**
	 * 用户坐标系类.
	 */
	class ESTUNREMOTEAPILIBAPI E_ROB_COORD : public IObject
	{
	public:
		/**
		 * 默认构造，值无效.
		 *
		 */
		E_ROB_COORD();

		/**
		 * 基本构造函数.
		 * 
		 * \param x TCP 相对于世界（基） 坐标系在 x 方向的位移偏移量，单位是 mm
		 * \param y TCP 相对于世界（基） 坐标系在 y 方向的位移偏移量，单位是 mm
		 * \param z TCP 相对于世界（基） 坐标系在 z 方向的位移偏移量，单位是 mm
		 * \param a TCP 相对于世界（基） 坐标系 z 轴旋转的欧拉角，单位是 deg
		 * \param b TCP 相对于世界（基） 坐标系 y’轴旋转的欧拉角，单位是 deg
		 * \param c TCP 相对于世界（基） 坐标系 x’’轴旋转的欧拉角，单位是 deg
		 */
		E_ROB_COORD(double x, double y, double z, double a, double b, double c);

		/**
		 * 数组构造函数，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 坐标系值数组，{x,y,z,a,b,c}
		 */
		E_ROB_COORD(const std::vector<double>& array);
		E_ROB_COORD(double array[6]);

		/**
		 * 拷贝构造函数.
		 * 
		 * \param t 用户坐标系
		 */
		E_ROB_COORD(const E_ROB_COORD& t);

		/**
		 * 析构函数.
		 * 
		 */
		~E_ROB_COORD();

		/**
		 * 赋值运算符.
		 * 
		 * \param t 用户坐标系
		 * \return 用户坐标系
		 */
		E_ROB_COORD& operator=(const E_ROB_COORD& t);

		/**
		 * 设置用户坐标系的值，debug模式下不可以，建议使用下面的接口.
		 * 
		 * \param array 坐标系值数组，{x,y,z,a,b,c}
		 */
		void setCoordByArray(const std::vector<double>& array);
		void setCoordByArray(double array[6]);

		/**
		 * 获取TCP 点在参考坐标系上 x 方向的坐标.
		 *
		 * \return 坐标值
		 */
		double getX() const;

		/**
		 * 设置TCP 点在参考坐标系上 x 方向的坐标.
		 *
		 * \param val 坐标值
		 */
		void setX(double val);

		/**
		 * 获取TCP 点在参考坐标系上 y 方向的坐标.
		 *
		 * \return 坐标值
		 */
		double getY() const;

		/**
		 * 设置TCP 点在参考坐标系上 y 方向的坐标.
		 *
		 * \param val 坐标值
		 */
		void setY(double val);

		/**
		 * 获取TCP 点在参考坐标系上 z 方向的坐标.
		 *
		 * \return 坐标值
		 */
		double getZ() const;

		/**
		 * 设置TCP 点在参考坐标系上 z 方向的坐标.
		 *
		 * \param val 坐标值
		 */
		void setZ(double val);

		/**
		 * 获取TCP 点相对于参考坐标系 z 轴旋转的欧拉角.
		 *
		 * \return 角度值
		 */
		double getA() const;

		/**
		 * 设置TCP 点相对于参考坐标系 z 轴旋转的欧拉角.
		 *
		 * \param val 角度值
		 */
		void setA(double val);

		/**
		 * 获取TCP 点相对于参考坐标系 y 轴旋转的欧拉角.
		 *
		 * \return 角度值
		 */
		double getB() const;

		/**
		 * 设置TCP 点相对于参考坐标系 y 轴旋转的欧拉角.
		 *
		 * \param val 角度值
		 */
		void setB(double val);

		/**
		 * 获取TCP 点相对于参考坐标系 x 轴旋转的欧拉角.
		 *
		 * \return 角度值
		 */
		double getC() const;

		/**
		 * 设置TCP 点相对于参考坐标系 x 轴旋转的欧拉角.
		 *
		 * \param val 角度值
		 */
		void setC(double val);

	private:
		struct RobotCoordPrivate;
		RobotCoordPrivate* _p;
	};
}
#endif // !ESTUNREMOTEAPILIB_STRUCT_H

