--******************************************************************--
--**************************错误信息对应表**************************--
--******************************************************************--
----------------------------------------------------------------------
-------------------------系统错误信息[201,300]------------------------
----------------------------------------------------------------------
201 <CH>设置系统模式失败，不支持该模式！</CH> <EN>Set system mode failed，this mode is not supported!</EN>
202 <CH>系统模式切换导致程序报警停止！</CH> <EN>Switching system mode causes the program to stop!</EN>
203 <CH>系统切换到远程模式，自动加载程序失败！</CH> <EN>System was switched to remote mode, program auto-load failed!</EN>
204 <CH>系统切换到远程模式，启动设置全局速度失败！</CH> <EN>System was switched to remote mode, set global velocity failed!</EN>
205 <CH>远程模式下，启动程序失败！</CH> <EN>Start program failed in remote mode!</EN>
206 <CH>远程模式下，停止程序失败！</CH> <EN>Stop program failed in remote mode!</EN>
207 <CH>安全门打开，程序转到暂停状态！</CH> <EN>The safe-door is open, program paused!</EN>
208 <CH>多工位加载程序失败！请检查预约程序的正确性！</CH> <EN>Loading Multi-station program failed! System execute failed!</EN>
209 <CH>与示教器断开连接，暂停运动！</CH> <EN>Network interruption, pause current motion!</EN>

251 <CH>主管理模块1运行异常！</CH> <EN>An exception occurred in the main management module 1!</EN>
252 <CH>运动管理模块2运行异常！</CH> <EN>An exception occurred in the motion management module 2!</EN>
253 <CH>指令管理模块3运行异常！</CH> <EN>An exception occurred in the instruction management module 3!</EN>
254 <CH>通讯模块4运行异常！</CH> <EN>An exception occurred in the linker module 4!</EN>
255 <CH>PLC管理模块5运行异常！</CH> <EN>An exception occurred in the PLC management module 5!</EN>
256 <CH>视觉管理模块6运行异常！</CH> <EN>An exception occurred in the vision management module 6!</EN>
257 <CH>现场总线Modbus模块7运行异常！</CH> <EN>An exception occurred in the ModbusTCP management module 7!</EN>

-------------------------程序执行错误[601,900]------------------------
601  <CH>configTable.cfg文件加载失败！</CH> <EN>Load file "configTable.cfg" failed!</EN>
602  <CH>userTable.cfg文件加载失败！</CH> <EN>Load file "userTable.cfg" failed!</EN>
603  <CH>执行逻辑控制指令失败！</CH> <EN>Execute logical control instructions failed!</EN>
604  <CH>命令执行失败！</CH> <EN>Execute command failed!</EN>
605  <CH>执行失败！run程序中存在禁止运行的指令在运行！</CH> <EN>Program execute failed, there is prohibited instructions in "run" program!</EN>
606  <CH>判断指令中有变量不存在！</CH> <EN>There is invalid variable in judge instruction!</EN>
607  <CH>加载指令字符串失败！</CH> <EN>Load instruction string failed!</EN>
608  <CH>运动内核缓存已满！</CH> <EN>The buffer of motion kernel is full!</EN>
609  <CH>加载程序数据失败！</CH> <EN>Load program data failed!</EN>
610  <CH>加载run程序数据失败！</CH> <EN>Load run program data failed!</EN>
611  <CH>加载触发程序数据失败！</CH> <EN>Load trig-data of program failed!</EN>
612  <CH>解析失败！程序未加载！</CH> <EN>Parse instruction failed! No program has been loaded!</EN>
613  <CH>解析逻辑控制指令失败！</CH> <EN>Parse logical control instructions failed!</EN>
614  <CH>call指令行变更,请重新载入工程！</CH> <EN>The CALL instruction line change.Please reload project!</EN>
615  <CH>执行call时获取call程序文件失败！</CH> <EN>Get "call" program file failed when executing "call" instruction!</EN>
616  <CH>需要call的程序文件不存在！</CH> <EN>The program file of "call" is not found!</EN>
617  <CH>圆弧中间停止,不允许直接再启动！</CH> <EN>Stop at the middle of arc path, restarting the program is forbidden!</EN>
618  <CH>系统最多同时支持八个程序run！</CH> <EN>The system supports up to eight "Run" programs!</EN>
619  <CH>kill与run程序不匹配！</CH> <EN>"Kill" program does not match "run" program!</EN>
620  <CH>表达式指令执行失败！</CH> <EN>Execute expression instruction failed!</EN>
621  <CH>机器人零点已丢失！当前仅允许执行RefRobotAxis指令！</CH> <EN>The robot's home-pos of driver has lost! Only "RefRobotAxis" can execute!</EN>
622  <CH>触发指令缓冲已满,请确保程序结构正确！</CH> <EN>The buffer of trigger instructions is full, please make sure the program structure is correct!</EN>
623  <CH>变量类型不存在！</CH> <EN>The variable type does not exist!</EN>
624  <CH>变量类型不匹配！</CH> <EN>The variable type mismatch!</EN>
625  <CH>单步模式下，不允许执行run指令！</CH> <EN>"Run" instruction cannot be executed in single-step mode!</EN>
626  <CH>程序处于run时，不允许再次run！</CH> <EN>Program in running state is not allowed to run again!</EN>
627  <CH>单步模式下，不允许执行触发指令！</CH> <EN>Trigger instruction cannot be executed in single-step mode!</EN>
628  <CH>触发指令执行失败！请检查指令是否合法！</CH> <EN>Failed to execute trigger instruction, please check the legality of the instruction!</EN>
629  <CH>OnParameter触发指令不支持该运动指令！</CH> <EN>The OnParameter instruction dose not support current motion instruction!</EN>
630  <CH>触发指令与运动指令之间不允许使用等待指令！</CH> <EN>The wait instruction cannot be used between trigger instruction and motion instruction!</EN>
631  <CH>软浮动指令之间，不允许运动指令！</CH> <EN>Motion instructions is forbidden between soft-floating instructions!</EN>
632  <CH>软浮动停止指令之前缺少软浮动启动指令！</CH> <EN>The start command of soft-floating function is missing before the soft-floating instruction!</EN>
633  <CH>多个软浮动指令嵌套使用！</CH> <EN>Multiple soft-floating instructions are nested!</EN>
634  <CH>缺少软浮动结束指令！</CH> <EN>The end command of soft-floating function is missing!</EN>
635  <CH>该区域变量未被激活，不允许条件判断！</CH> <EN>This area variable is not activated, condition judge failed!</EN>
636  <CH>单条轨迹的触发数量超过最大限制数量，执行失败！</CH> <EN>Trig count of single line is out of max count, execute failed!</EN>
637	 <CH>获取程序变量文件失败！</CH> <EN>Failed to obtain program variable file!</EN>
638	 <CH>BendTrack过程中停止，不允许再启动！</CH> <EN>System execute stop command during BendTrack process, restarting the program is forbidden!</EN>
639	 <CH>OnDistance触发指令不支持该运动指令！</CH> <EN>The OnDistance instruction dose not support current motion instruction!</EN>
640	 <CH>触发指令执行失败！</CH> <EN>The trigger instruction implement error!</EN>
641	 <CH>当前指令不支持倒退执行！</CH> <EN>The current instruction dose not support backward execution!</EN>
642	 <CH>待跳转的label不存在！</CH> <EN>The label is not existed!</EN>
643	 <CH>跟随状态下有禁止的指令在运行！</CH> <EN>Some command is forbidden during the follow motion!</EN>
644	 <CH>跟随过程中停止，请pc到带有用户坐标系的运动指令上再启动！</CH> <EN>System execute stop command during ConveyorTrack process, restarting the program is forbidden!</EN>
645	 <CH>跟随失败后跳转的label不能为空！</CH> <EN>The label which systen need skip is empty when conveyorTrack is failed!</EN>
646	 <CH>全局程序中不支持RUN、KILL指令！</CH> <EN>Global program is forbid to execute RUN/KILL command!</EN>
647	 <CH>全局程序中CALL指令不支持call到非全局程序以外的程序！</CH> <EN>The CALL command of Global program should be use local program!</EN>

651  <CH>指令中参数数量异常！</CH> <EN>Invalid number of parameters in instruction!</EN>
652  <CH>指令中位置点数据异常！</CH> <EN>Invalid Pos data in the instruction!</EN>
653  <CH>指令参数中位置点类型不匹配！</CH> <EN>Mismatch Pos data type in instruction!</EN>
654  <CH>指令参数中位置点类型无法找到！</CH> <EN>Pos data type cannot be found in instruction!</EN>
655  <CH>指令参数中位置点中某项的值找不到！</CH> <EN> Element cannot be found in Pos data!</EN>	
656  <CH>指令参数中速度项参数异常！</CH> <EN>Invalid velocity parameters of instruction!</EN>
657  <CH>指令参数中过渡类型参数异常！</CH> <EN>Invalid blend type of instruction parameters!</EN>
658  <CH>指令参数中触发参数异常！</CH> <EN>Invalid trigger parameter in instruction!</EN>
659  <CH>指令参数中AddDO类型参数异常！</CH> <EN>Invalid "AddDo" parameter in instruction!</EN>
660  <CH>指令参数中速度变量数据异常！</CH> <EN> Abnormal speed variable data in instruction parameters!</EN>
661  <CH>指令参数中速度变量类型不匹配！</CH> <EN>Speed variable type mismatch in instruction parameters!</EN>
662  <CH>指令参数中速度变量类型无法找到！</CH> <EN>Speed variable type cannot be found in instruction parameters!</EN>
663  <CH>指令参数中速度变量中某项的值找不到！</CH> <EN>Some value of Speed variable cannot be found in instruction parameters!</EN>
664  <CH>指令参数中过渡变量数据异常！</CH> <EN>There is some abnormal data of blend variable in instruction parameters!</EN>
665  <CH>指令参数中过渡变量类型不匹配！</CH> <EN>Blend variable type mismatch in instruction parameters!</EN>
666  <CH>指令参数中过渡变量类型无法找到！</CH> <EN>Blend variable type missing in instruction parameters!</EN>
667  <CH>指令参数中过渡变量中某项的值找不到！</CH> <EN>Some value of Blend variable cannot be found in instruction parameters!</EN>
668  <CH>指令参数POS点中MODE类型找不到！</CH> <EN>The "Mode" type cannot be found in "POS" parameters!</EN>
669  <CH>指令参数POS点中MODE中某项的值找不到！</CH> <EN>Some value of "Mode" type cannot be found!</EN>
670  <CH>指令参数中socket名字异常！</CH> <EN>Invalid socket name in instruction!</EN>
671 <CH>指令参数中IP地址不合法！</CH> <EN>Invalid IP address in instruction!</EN>
672 <CH>指令参数中tool坐标系参数异常！</CH> <EN>Invalid parameters in tool coordinate system!</EN>
673 <CH>指令参数中tool坐标系类型不匹配！</CH> <EN>The tool coordinate system type mismatch in instruction parameters!</EN>
674 <CH>指令参数中tool坐标系类型无法找到！</CH> <EN>The tool coordinate system type cannot be found in instruction parameters!</EN>
675 <CH>指令参数中tool坐标系中某项的值找不到！</CH> <EN>Some value of tool coordinate system cannot be found in instruction parameters!</EN>
676 <CH>指令参数中用户坐标系参数异常！</CH> <EN>There have exceptions in the user coordinate system!</EN>
677 <CH>指令参数中用户坐标系类型不匹配！</CH> <EN>The user coordinate system type mismatch in instruction parameters!</EN>
678 <CH>指令参数中用户坐标系类型无法找到！</CH> <EN>The user coordinate system type cannot be found in instruction parameters!</EN>
679 <CH>指令参数中用户坐标系中某项的值找不到！</CH> <EN>Some value of user coordinate system cannot be found in instruction parameters!</EN>
680 <CH>指令参数中负载惯量类型不匹配！</CH> <EN>The payload system type mismatch in instruction parameters!</EN>
681 <CH>指令参数中负载惯量类型无法找到！</CH> <EN>The payload system type cannot be found in instruction parameters!</EN>
682 <CH>指令参数中负载惯量中某项找不到！</CH> <EN>Some value of payload system cannot be found in instruction parameters!</EN>
683 <CH>指令参数中摆弧类型不匹配！</CH> <EN>The weave type mismatch in instruction parameters!</EN>
684 <CH>指令参数中摆弧类型无法找到！</CH> <EN>The weave type cannot be found in instruction parameters!</EN>
685 <CH>指令参数中摆弧中某项找不到！</CH> <EN>Some value of weave cannot be found in instruction parameters!</EN>
686 <CH>指令参数中外部TCP坐标系参数异常！</CH> <EN>There have exceptions in the external tcp coordinate system!</EN>
687 <CH>指令参数中外部TCP坐标系类型不匹配！</CH> <EN>The external tcp coordinate type mismatch in instruction parameters!</EN>
688 <CH>指令参数中外部TCP坐标系类型无法找到！</CH> <EN>The external tcp coordinate type cannot be found in instruction parameters!</EN>
689 <CH>指令参数中外部TCP坐标系中某项的值找不到！</CH> <EN>Some value of external tcp coordinate system cannot be found in instruction parameters!</EN>
690  <CH>指令中坐标系参数数据异常！</CH> <EN>Invalid Coordinate data in the instruction!</EN>
691  <CH>指令参数中坐标系类型不匹配！</CH> <EN>Mismatch Coordinate data type in instruction!</EN>
692  <CH>指令参数中坐标系类型无法找到！</CH> <EN>Coordinate data type cannot be found in instruction!</EN>
693  <CH>指令参数中坐标系中某项的值找不到！</CH> <EN> Element cannot be found in Coordinate data!</EN>	
694 <CH>指令参数中变位机坐标系参数异常！</CH> <EN>There have exceptions in the positioner coordinate system!</EN>
695 <CH>指令参数中变位机坐标系类型不匹配！</CH> <EN>The positioner coordinate type mismatch in instruction parameters!</EN>
696 <CH>指令参数中变位机坐标系类型无法找到！</CH> <EN>The positioner coordinate type cannot be found in instruction parameters!</EN>
697 <CH>指令参数中变位机坐标系中某项的值找不到！</CH> <EN>Some value of positioner coordinate system cannot be found in instruction parameters!</EN>
698 <CH>指令中待跳转的label格式不正确！</CH> <EN>The label system type mismatch in instruction parameters</EN>
699 <CH>指令参数中动坐标系参数异常！</CH> <EN>There have exceptions in the dyn coordinate system!</EN>
700 <CH>指令参数中动坐标系类型不匹配！</CH> <EN>The dyn coordinate system type mismatch in instruction parameters!</EN>
701 <CH>指令参数中动坐标系类型无法找到！</CH> <EN>The dyn coordinate system type cannot be found in instruction parameters!</EN>
702 <CH>指令参数中动坐标系中某项的值找不到！</CH> <EN>Some value of dyn coordinate system cannot be found in instruction parameters!</EN>
703 <CH>指令参数中速率类型参数异常！</CH> <EN>Invalid override type of instruction parameters!</EN>
704 <CH>指令参数中扫频参数异常！</CH> <EN>There have exceptions in the chirp parameter!</EN>
705 <CH>指令参数中扫频类型不匹配！</CH> <EN>The chirp type mismatch in instruction parameters!</EN>
706 <CH>指令参数中扫频类型无法找到！</CH> <EN>The chirp type cannot be found in instruction parameters!</EN>

--Cam2D3D操作错误[901,1000]-----
901 <CH>3D视觉信息发送失败，请确认相机已连接！</CH> <EN>Failed to send 3D-vision message, please confirm the camera is connected!</EN>
902 <CH>2D视觉拍照命令触发失败！</CH> <EN>Failed to trig 2D-vision photograph command!</EN>
903 <CH>计算失败，请确认点位数据正确！</CH> <EN>Failed to Calculate, please make sure the point data is correct!</EN>
----------------------------------------------------------------------
--configure脚本执行错误[951,1000]-----
951 <CH>APOS转CPOS失败！</CH> <EN>Conversion from APOS to CPOS failed!</EN>
952 <CH>CPOS转APOS失败！</CH> <EN>Conversion from CPOS to APOS failed!</EN>
953 <CH>CPOS转CPOS失败！</CH> <EN>Conversion from CPOS to CPOS failed!</EN>
954 <CH>位运算失败！不能对负数进行位操作！</CH> <EN>Bit operation failed, the variable cannot be set to a negative number!</EN>
955 <CH>当前码垛计数已经超过最大码垛数量！</CH> <EN>The current pallet count exceeds the maximum number of pallets!</EN>
956 <CH>放置失败,当前放置已满！</CH> <EN>Placement failed, the current placement is full!</EN>
957 <CH>抓取失败,当前可抓取工件为空！</CH> <EN>Failed to grab, there are no artifacts to grab currently!</EN>
958 <CH>码垛参考位置点不存在！</CH> <EN>The reference position point of pallet does not exist!</EN>
959 <CH>批量IO操作失败，起始端口号大于结束端口号！</CH> <EN>Batch IO operation failed, the start port number is bigger than the end port number!</EN>
960 <CH>Hand指令答复失败！</CH> <EN>Hand instruction reply failed!</EN>
961 <CH>1~8号输出口为系统输出口，禁止用户操作！</CH> <EN>User is not allowed to operate system output, these output is 1,2,3,4,5,6,7,8!</EN>
962 <CH>读取的数据个数不能超过数组的大小！</CH> <EN>Data count which need read cannot exceed the array size</EN>
963 <CH>IO类型的触发值只能为0或者1！</CH> <EN>The trigger value of type IO can only be 0 or 1!</EN>
964 <CH>AreaActivate 或 AreaDeactivate 指令失败！</CH> <EN>AreaActivate or AreaDeactivate failed!</EN>
965 <CH>参数异常，请检查参数是否合法！</CH> <EN>Parameter Error! Please check whether parameter is truely!</EN>
966 <CH>无法获取明确的回退位置点，不能继续！</CH> <EN>Unable to get clear back position point,unable to continue!</EN>
967 <CH>SetTargetPos指令仅能在传送带为无视觉触发模式下运行！</CH> <EN>SetTargetPos is forbid execute in current mode!</EN>
968 <CH>动坐标系对应的传送带未使能，SetTargetPos指令执行失败！</CH> <EN>SetTargetPos execute failed! Conveyor is not in enable status!</EN>
969 <CH>根据动坐标系的值计算用户坐标系失败！</CH> <EN>Failed to convert the syncoord to usercoord!</EN>
970 <CH>伺服增益参数设置失败！</CH> <EN>The Pn gain Param set failed!</EN>
971 <CH>阵列指令:行数及列数必须为大于0的整数！</CH> <EN>Row and Column must be over than zero!</EN>
972 <CH>阵列指令:行号或列号大于设置值！</CH> <EN>Row or Column larger than settings!</EN>
973 <CH>阵列指令:行数和列数乘积需要小于限定值(32767)！</CH> <EN>(Row * Column) cannot larger than settings(32767)!</EN>
974 <CH>当前运动指令选择不受全局速度影响模式时需设置运行速度！</CH> <EN>Please set motion speed when OverrideType is UNCONTROLLED!</EN>
975 <CH>Damping加速度限制系数设置失败！</CH> <EN>The Damping acc limit param set failed!</EN>
976 <CH>Damping滤波器参数设置失败！</CH> <EN>The Damping filter param set failed!</EN>
977 <CH>同一时刻执行的脉冲指令数量超过了最大限制！</CH> <EN>The pulse count which system execute is out of max limit!</EN>
978 <CH>传入的变量类型不合法！</CH> <EN>Inst execute failed! Please check whether parameter is truely!</EN>
979 <CH>模型辨识数据采集失败！</CH> <EN>Failed to collect MDL data!</EN>
980 <CH>传送带使能状态设置失败！</CH> <EN>Failed to set virual conveyor enable status!</EN>
981 <CH>MovArch指令中下降高度未设置！</CH> <EN>The drop height is not set in the MovArch instruction!</EN>

---******************************************************************--
---****************命令响应失败信息对应表***************--
---******************************************************************--
-----------------ER_Manager操作错误[50001,50999]----------------------
----------------------------------------------------------------------
50001 <CH>设置系统模式的参数异常！</CH> <EN>Invalid parameter of setting system mode!</EN>
50002 <CH>设置系统模式时，程序由执行转到停止失败！</CH> <EN>Failed to switch the program state from "run" to "stop" when setting system mode!</EN>
50003 <CH>获取系统中运动相关配置参数失败！</CH> <EN>Get motion configuration failed!</EN>
50004 <CH>程序执行中，不允许伺服励磁操作！</CH> <EN>Servo motivate operation is not allowed when program is running!</EN>
50005 <CH>系统不在自动模式，不允许伺服励磁操作！</CH> <EN>Servo motivate operation is not allowed when system is not in automatic mode!</EN> 
50006 <CH>系统下发运动控制复位命令失败！</CH> <EN>System failed to issue reset instruction of motion control!</EN>
50007 <CH>系统下发指令处理复位命令失败！</CH> <EN>System failed to issue reset instruction of instruction processing!</EN>
50008 <CH>系统清理错误列表失败！</CH> <EN>System failed to clean up the error list!</EN>
50009 <CH>系统错误清空复位命令执行失败！</CH> <EN>System error! Executing instruction of emptying reset command failed!</EN>
50010 <CH>获取到系统远程速度参数异常，超过[1,100]！</CH> <EN>Invalid remote velocity parameter of system, above [1,100]!</EN>
50011 <CH>设置系统远程速度参数异常，超过[1,100]！</CH> <EN>Invalid remote velocity parameter to be set, above [1,100]!</EN>
50012 <CH>设置自启动中工程或程序名存在空值！</CH> <EN>Project or program name is empty when setting auto-start option!</EN>		
50013 <CH>取消自启动中工程或程序名与原本不对应！</CH> <EN>Project or program name mismatch when canceling auto-start option!</EN>
50014 <CH>程序执行中，不允许设置系统IP！</CH> <EN>Operation of setting system IP is not allowed when program is running!</EN>
50015 <CH>设置系统IP参数中端口号值异常！</CH> <EN>The port value parameter of  setting system IP is abnormal!</EN>
50016 <CH>加密信息不合法，加密失败！</CH> <EN>The encrypt information is not valid, failed to encrypt!</EN>
50017 <CH>解密密码错误，解密失败！</CH> <EN>Password error, failed to decrypt!</EN>
50018 <CH>加密数据被其他线程占用，请稍后重试！</CH> <EN>The encryption data is occupied by another thread, please try again later!</EN>
50019 <CH>系统当前没有加密，无需解密！</CH> <EN>The system is not encrypted and dose not require decryption!</EN>
50020 <CH>系统执行期限已过，请联系厂家进行解密！</CH> <EN>System execution period expired, please contact the manufacturer to decrypt!</EN>
50021 <CH>系统时间被篡改，运行不合法！</CH> <EN>System time was tampered!</EN>
50022 <CH>系统时间获取失败，操作频繁，请重试！</CH> <EN>Failed to acquire system time, please try again later!</EN>
50023 <CH>程序由暂停转到停止状态出错！</CH> <EN>The program failed to switch from pause to stop state!</EN>
50024 <CH>多工位参数设置失败，参数不合法！</CH> <EN>Parameter is error! Set multi-station parameter failed!</EN>
50025 <CH>设置远程速度失败，请切换到其它模式下设置！</CH> <EN>Failed to set remote speed, please switch to other mode settings!</EN>
50026 <CH>被使能的多工位输入口/输出口/程序名不能为空或无效值！</CH> <EN>Multi-station param is illegal, please ensure param is truely!</EN>
50027 <CH>更新加密数据到文件中失败！</CH> <EN>Update the encrypt information to file failed!</EN>
50028 <CH>设置系统模式时，程序倒序切换到单步失败！</CH> <EN>Failed to switch the program step from "step_back" to "step" when setting system mode!</EN>
50029 <CH>该系统不支持设置外部通讯网络信息！</CH> <EN>Failed to set Ext_network when system not support!</EN>
50030 <CH>非停止状态下禁止设置API模式！</CH> <EN>It is frobid to set api parameter when system is not stopping status!</EN>

-----------------ER_MovManager操作错误[51001,52000]-------------------
51001 <CH>程序执行中，不允许进行回零操作！</CH> <EN>Homing operation is not allowed when program is running!</EN>
51002 <CH>伺服不在励磁状态，不允许回零操作！</CH> <EN>Homing operation is not allowed when servo is not in excitation state!</EN>
51003 <CH>回零命令执行失败！</CH> <EN>Failed to execute homing instruction!</EN>
51004 <CH>全局速度命令设置失败！</CH> <EN>Failed to execute setting override instruction!</EN>
51005 <CH>程序运行中，不允许设置点动参考坐标系！</CH> <EN>Setting jog reference coordinate system is not allowed when program is running!</EN>
51006 <CH>点动坐标系命令设置失败！</CH> <EN>Failed to execute setting jog coordinate system!</EN>
51007 <CH>程序运行中，不允许机器人点动！</CH> <EN>Jog operation is not allowed when program is running!</EN>
51008 <CH>伺服不在励磁状态，不允许机器人点动！</CH> <EN>Jog operation is not allowed when servo is not in excitation state!</EN>
51009 <CH>自动模式下，不允许机器人点动！</CH> <EN>Jog operation is not allowed when system is in automatic state!</EN>
51010 <CH>当前系统模式下，不允许机器人点动操作！</CH> <EN>Jog operation is not allowed when system is in current state!</EN>
51011 <CH>程序运行中，不允许切换点动模式！</CH> <EN>Switching to jog mode is not allowed when program is running!</EN>
51012 <CH>点动命令执行失败！</CH> <EN>Failed to execute jog instruction!</EN>
51013 <CH>设置DOUT命令执行失败！</CH> <EN>Failed to execute setting DOUT instruction!</EN>
51014 <CH>设置AOUT命令执行失败！</CH> <EN>Failed to execute setting AOUT instruction!</EN>
51015 <CH>设置SIMDOUT命令执行失败！</CH> <EN>Failed to execute setting SIMDOUT instruction!</EN>
51016 <CH>设置SIMAOUT命令执行失败！</CH> <EN>Failed to execute setting SIMAOUT instruction!</EN>
51017 <CH>系统接口异常，重启系统后再尝试！</CH> <EN>The system interface is abnormal, try again after restarting the system!</EN>
51018 <CH>命令参数超限定范围，命令执行失败！</CH> <EN>Command parameters exceed the limit, failed to execute the command!</EN>
51019 <CH>程序运行中，不允许标定工具参数！</CH> <EN>Calibrating tool parameters is not allowed when program is running!</EN>
51020 <CH>程序运行中，不允许标定用户坐标系！</CH> <EN>Calibrating user coordinate system is not allowed when program is running!</EN>
51021 <CH>设置传送带参数命令执行失败！</CH> <EN>Failed to execute setting conveyor parameters instruction!</EN>
51022 <CH>设置跟随参数命令执行失败！</CH> <EN>Failed to execute setting follow parameters instruction!</EN>
51023 <CH>刷新单圈值失败！</CH>  <EN>Failed to refresh single turn value!</EN>
51024 <CH>映射运动内核接口数据失败，检查运动内核组件是否正常启动！</CH> <EN>Failed to map motion kernel interface data，check whether RTMC.rta starts normally!</EN>
51025 <CH>起终点跨奇异，不支持一点到达运动！</CH> <EN>Path pass singularity!</EN>
51026 <CH>设置的高度值不合法！</CH> <EN>The height value is illegal!</EN>
51027 <CH>当前机型不支持一点到达操作(拱形运动)！</CH> <EN>Current model does not support go targetPos in this way(arch motion)!</EN>
51028 <CH>当前模式不支持一点到达操作(拱形运动)！</CH> <EN>Current mode does not support go targetPos in this way(arch motion)!</EN>
51029 <CH>运动启动失败！</CH> <EN>Failed to start motion!</EN>
51030 <CH>运动缓存已满！</CH> <EN>The motion buffer is full!</EN>
51031 <CH>程序运行中，不允许激活一点对正！</CH> <EN>The program is running, activating one-click alignment is not allowed!</EN>
51032 <CH>程序运行中，不允许进行一点对正操作！</CH> <EN>The program is running, one-click alignment operation is not allowed!</EN>
51033 <CH>一点对正操作失败！</CH> <EN>Failed to execute one-click alignment operation!</EN>
51034 <CH>未激活一点对正，不允许操作！</CH> <EN>One-click alignment operation is not activated, cannot be executed!</EN>
51035 <CH>程序暂停状态中，不允许进行回零操作！</CH> <EN>The program is in paused state, homing operation is not allowed!</EN>
51036 <CH>程序暂停状态中，不允许设置点动参考坐标系！</CH> <EN>The program is in paused state, cannot set the reference coordinate system of jog-motion!</EN>
51037 <CH>程序暂停状态中，不允许机器人点动！</CH> <EN>The program is in paused state, jog-motion is not allowed!</EN>
51038 <CH>程序暂停状态中，不允许切换点动模式！</CH> <EN>The program is in paused state, switching jog-motion mode is not allowed!</EN>
51039 <CH>程序暂停状态中，不允许标定工具参数！</CH> <EN>The program is in paused state, calibrating the tool parameters is not allowed!</EN>
51040 <CH>程序暂停状态中，不允许标定用户坐标系！</CH> <EN>The program is in paused state, calibrating the user coordinate system is not allowed!</EN>
51041 <CH>程序暂停状态中，不允许激活一点对正！</CH> <EN>The program is in paused state, activating one-click alignment is not allowed!</EN>
51042 <CH>程序暂停状态中，不允许进行一点对正操作！</CH> <EN>The program is in paused state, one-click alignment operation is not allowed!</EN>
51043 <CH>设置DI命令执行失败！</CH> <EN>Failed to set DI!</EN>
51044 <CH>设置AI命令执行失败！</CH> <EN>Failed to set AI!</EN>
51045 <CH>设置SIMDI命令执行失败！</CH> <EN>Failed to set SIMAI!</EN>
51046 <CH>设置SIMAI命令执行失败！</CH> <EN>Failed to set SIMDI!</EN>
51047 <CH>伺服励磁状态，不允许清除伺服报警！</CH> <EN>Clearing servo alarms is not allowed when the servo is in an excited state!</EN>
51048 <CH>清除伺服报警或伺服历史报警失败！</CH> <EN>Failed to clear servo alarms or servo history alarms!</EN>
51049 <CH>获取伺服Pn参数失败！</CH> <EN>Failed to get the servo PN parameters!</EN>
51050 <CH>设置伺服Pn参数失败！</CH> <EN>Failed to set the servo PN parameters!</EN>
51051 <CH>软浮动期间，不允许机器人点动！</CH> <EN>Jog operation is not allowed during in SoftFloat!</EN>
51052 <CH>内核由暂停转空闲状态失败！</CH> <EN>Kernel state from pause to idle failed!</EN>
51053 <CH>设置诊断功能项失败！</CH> <EN>Set function item of Diagnose failed!</EN>
51054 <CH>设置诊断参数失败！</CH> <EN>Set parameters of Diagnose failed!</EN>
51055 <CH>程序运行中，不允许标定外部坐标系！</CH> <EN>Calibrating extend tcp system is not allowed when program is running!</EN>
51056 <CH>程序暂停状态中，不允许标定外部坐标系！</CH> <EN>The program is in paused state, calibrating the extend tcp system is not allowed!</EN>
51057 <CH>映射运动内核接口数据错误，版本不匹配！</CH> <EN>Mapping motion kernel interface data error, version mismatch!</EN>
51058 <CH>变位机参数设置失败,请检查输入参数的合法性！</CH> <EN>It is failed to set positioner parameter, Please ensure parameter is truely!</EN>
51059 <CH>变位机参数读取失败，请检查输入参数的合法性！</CH> <EN>It is failed to get positioner parameter, Please ensure parameter is truely!</EN>
51060 <CH>程序运行中，不允许标定变位机坐标系！</CH> <EN>Calibrating positioner system is not allowed when program is running!</EN>
51061 <CH>程序暂停状态中，不允许标定变位机坐标系！</CH> <EN>The program is in paused state, calibrating the positioner system is not allowed!</EN>
51062 <CH>程序运行中，不允许设置变位机坐标系参数！</CH> <EN>Set positioner param error! Program is in running!</EN>
51063 <CH>第一个变位机轴不在零点位置！标定失败！</CH> <EN>The first external axis is not at zero! Calibration positioner param failed!</EN>
51064 <CH>第二个变位机轴不在零点位置！标定失败！</CH> <EN>The second external axis is not at zero! Calibration positioner param failed!</EN>
51065 <CH>激活或冻结区域命令失败！</CH> <EN>AreaActivateOrNot cmd failed!</EN>
51066 <CH>传入的传送带ID错误！</CH> <EN>The conveyor index is out of limit!</EN>
51067 <CH>程序运行中，不允许标定传送带脉冲当量！</CH> <EN>Calibrating Conveyor Equivalent system is not allowed when program is running!</EN>
51068 <CH>程序暂停状态中，不允许标定传送带脉冲当量！</CH> <EN>The program is in paused state, calibrating the Conveyor Equivalent system is not allowed!</EN>
51069 <CH>当前点与上一次记录的点相同！</CH> <EN>The current point is samed to last point!</EN>
51070 <CH>计算失败！两个点之间的编码器脉冲差值太小或者相等！</CH> <EN>The encoder value between last point and current point is equal or too short!</EN>
51071 <CH>传入的动坐标系未与传送带绑定，运行失败！</CH> <EN>Please ensure the dyn coordinate is bound to the conveyor!</EN>
51072 <CH>设置的传送带输出口超过设定范围！</CH> <EN>The trig output of conveyor is out of limit!</EN>
51073 <CH>设置的传送带输入口超过设定范围！</CH> <EN>The trig input of conveyor is out of limit!</EN>
51074 <CH>设置的传送带输出口跟其他传送带的输出口重复！</CH> <EN>The trig output of conveyor is used by other conveyor!</EN>
51075 <CH>设置的传送带输入口跟其他传送带的输入口重复！</CH> <EN>The trig input of conveyor is used by other conveyor!</EN>
51076 <CH>未获取到视觉点信息，运行失败！</CH> <EN>Please ensure the vision message is received before execute MovLSync instruction!</EN>
51077 <CH>设置的传送带坐标系跟其他传送带的动坐标系重复！</CH> <EN>The trig syncoord of conveyor is used by other conveyor!</EN>
51078 <CH>动坐标系下，不允许机器人点动！</CH> <EN>Jog operation is not allowed when current coodrate is syncoord!</EN>
51079 <CH>动坐标系下，不允许切换坐标系！</CH> <EN>Coordinate switch operation is not allowed when current coodrate is syncoord!</EN>
51080 <CH>动坐标系下，禁止标定传送带的脉冲当量！</CH> <EN>Calibrating conveyor equivalent operation is not allowed when current coodrate is syncoord!</EN>
51081 <CH>动坐标系下，禁止设置碰撞检测参数！</CH> <EN>Set CollideValue parameter operation is not allowed when current coodrate is syncoord!</EN>
51082 <CH>动坐标系下，禁止设置振动抑制参数！</CH> <EN>Set vibraBatement parameter operation is not allowed when current coodrate is syncoord!</EN>
51083 <CH>计算失败！两个点之间的距离太近！</CH> <EN>The distance between last point and current point is too short!</EN>
51084 <CH>恢复参数完成！但是，以下参数无法恢复：</CH> <EN>Recover servo parameter done!But,below paramertes cannot recover:</EN>
51085 <CH>设置机械臂参数失败！</CH> <EN>Failed to set mechanical arm parameters!</EN>
51086 <CH>更改轨迹纠偏使能状态失败！</CH> <EN>Failed to change the track correction enable state!</EN>
51087 <CH>设置轨迹纠偏参数失败！</CH> <EN>Failed to set correction parameter!</EN>
51088 <CH>设置区域参数失败！</CH> <EN>Failed to set area parameter!</EN>
51089 <CH>备份参数失败！</CH> <EN>Failed to backup servo parameter!</EN>
51090 <CH>恢复参数失败！</CH> <EN>Failed to recover servo parameter!</EN>
51091 <CH>当前动坐标系与待设置参数的传送带动坐标系不一致！</CH> <EN>Current syncoord should be corresponding to current conveyor syncoord!!</EN>
51092 <CH>当前操作只能在动坐标系下进行！</CH> <EN>Record follow pos should be in syncoord!</EN>
51093 <CH>设置多边体区域参数失败！</CH> <EN>Failed to set polyhedron area parameter!</EN>
51094 <CH>获取多边体区域参数失败！</CH> <EN>Failed to get polyhedron area parameter!</EN>
51095 <CH>删除多边体区域参数失败！</CH> <EN>Failed to delete polyhedron area parameter!</EN>
51096 <CH>激活或冻结多边体区域命令失败！</CH> <EN>Failed to activate or deactivate polyhedron cmd!</EN>

----------------------------------------------------------------------
--------------------Instruction操作错误[52001,52999]------------------
52001  <CH>系统处于运行状态，操作被禁止！</CH><EN>The system is in running state, the operation is forbidden!</EN>
52002  <CH>请先加载工程或程序！</CH><EN>Please load a project or program at first!</EN>
------------------工程文件操作错误(52003~52030)------------------
52003  <CH>工程创建失败！</CH> <EN>Create project failed!</EN>
52004  <CH>工程加载失败！</CH> <EN>Load project failed!</EN>
52005  <CH>工程注销失败！</CH> <EN>Logout project failed!</EN>
52006  <CH>工程复制失败！</CH> <EN>Copy project failed!</EN>
52007  <CH>工程粘贴失败！</CH> <EN>Paste project failed!</EN>
52008  <CH>工程删除失败！</CH> <EN>Delete project failed!</EN>
52009  <CH>工程重命名失败！</CH> <EN>Rename project failed!</EN>
52010  <CH>工程刷新失败！</CH> <EN>Refresh project failed!</EN>
52011  <CH>程序创建失败！</CH> <EN>Create program failed!</EN>
52012  <CH>程序加载失败！</CH> <EN>Load program failed!</EN>
52013  <CH>程序注销失败！</CH> <EN>Logout program failed!</EN>
52014  <CH>程序打开失败！</CH> <EN>Open program failed!</EN>
52015  <CH>程序关闭失败！</CH> <EN>Close program failed!</EN>
52016  <CH>程序复制失败！</CH> <EN>Copy program failed!</EN>
52017  <CH>程序粘贴失败！</CH> <EN>Paste program failed!</EN>
52018  <CH>程序被调用，删除失败！</CH> <EN>Delete program failed!</EN>
52019  <CH>程序被调用，重命名失败！</CH> <EN>Rename program failed!</EN>
52020  <CH>程序刷新失败！</CH> <EN>Refresh program failed!</EN>
52021  <CH>程序列表获取失败！</CH> <EN>Get program list failed!</EN>
52022  <CH>工程名过长或为空！</CH> <EN>The project name is too long or empty!</EN>
52023  <CH>工程名已存在！</CH> <EN>The project name already exists!</EN>
52024  <CH>工程数量超出限制！</CH> <EN>The number of projects exceeds the limit!</EN>
52025  <CH>创建工程变量数据文件失败！</CH> <EN>Failed to create variable data file of project!</EN>
52026  <CH>创建工程后获取工程列表失败！</CH> <EN>Failed to get Project list after creating the project!</EN>
52027  <CH>工程文件异常！</CH> <EN>Project file exception!</EN>
52028  <CH>该工程不存在！</CH> <EN>The project does not exist!</EN>
52029  <CH>获取全局数据失败！</CH> <EN>Failed to get the global speed!</EN>
52030  <CH>加载工程数据失败！</CH> <EN>Failed to load the data file of project!</EN>
52031  <CH>工程没有被加载！</CH> <EN>The project is not loaded!</EN>
52032  <CH>非法的程序名！</CH> <EN>The program name is illegal!</EN>
52033  <CH>程序名过长或为空！</CH> <EN>The program name is too long or empty!</EN>
52034  <CH>程序名已存在！</CH> <EN>The variable name already exists!</EN>
52035  <CH>程序数量超出限制！</CH> <EN>The number of programs exceeds the limit!</EN>
52036  <CH>创建程序变量文件失败！</CH> <EN>Failed to create variable data file of program!</EN>
52037  <CH>创建程序文件失败！</CH> <EN>Failed to create program file!</EN>
52038  <CH>获取选定工具坐标系参数失败！</CH> <EN>Failed to get tool coordinate system parameters!</EN>
52039  <CH>获取选定用户坐标系参数失败！</CH> <EN>Failed to get user coordinate system parameters!</EN>
52040  <CH>工程名或程序名为空！</CH> <EN>Project name or program name is empty!</EN>
52041  <CH>程序文件无法打开！</CH> <EN>Cannot open the program file!</EN>
52042  <CH>该程序没有在工程中找到或没有被加载！</CH> <EN>The program was not found or was not be loaded!</EN>
52043  <CH>程序文件不存在！</CH> <EN>The program file does not exist!</EN>
52044  <CH>删除数据文件失败！</CH> <EN>Failed to delete data file!</EN>
52045  <CH>删除程序文件失败！</CH> <EN>Failed to delete program file!</EN>
52046  <CH>获取程序列表失败！</CH> <EN>Failed to get program list!</EN>
52047  <CH>无法删除当前加载的工程！</CH> <EN>Current loaded project cannot be deleted!</EN>
52048  <CH>从程序文件中获取全部指令失败！</CH> <EN>Failed to get all instructions from program file!</EN>
52049  <CH>程序文件格式转换失败！</CH> <EN>Failed to convert program file format!</EN>
52050  <CH>当前打开的程序无法关闭！</CH> <EN>Current opened program cannot be closed!</EN>
52051  <CH>程序数据文件大小超出限制！</CH> <EN>The size of program file exceeds limit!</EN>
52052  <CH>恢复运行的数据失败！</CH> <EN>Failed to resume running data!</EN>
52053  <CH>当前加载程序的程序变量数据获取失败！</CH> <EN>Failed to get all variable data of current loaded program!</EN>
52054  <CH>当前加载程序的程序变量数据加载失败！</CH> <EN>Failed to load all variable data of current loaded program!</EN>
52055  <CH>当前加载程序的程序变量数据保存失败！</CH> <EN>Failed to save all variable data of current loaded program!</EN>
52056  <CH>没有被复制的程序，程序粘贴失败！</CH> <EN>No program was copied, pasting program failed!</EN>
52057  <CH>没有被复制的工程，工程粘贴失败！</CH> <EN>No project was copied, pasting project failed!</EN>
52058  <CH>要复制的工程不存在，工程粘贴失败！</CH> <EN>The project to be copied does not exist, pasting project failed!</EN>
52059  <CH>工程文件目录获取失败！</CH> <EN>Failed to get the project file directory!</EN>
52060  <CH>工程被加载、程序被加载或打开时，不允许重命名操作！</CH> <EN>Renaming operation are not allowed when a project is loaded or a program is loaded or opened!</EN>
52061  <CH>程序数据文件重命名失败！</CH> <EN>Failed to rename the program data file!</EN>
52062  <CH>程序指令文件重命名失败！</CH> <EN>Failed to rename the program instruction file!</EN>
52063  <CH>工程文件目录拷贝失败！</CH> <EN>Failed to copy the project file directory!</EN>
52064  <CH>工程文件目录删除失败！</CH> <EN>Failed to delete the project file directory!</EN>
52065  <CH>拷贝工程目录失败！</CH> <EN>Failed to copy the project directory!</EN>
52066  <CH>工程加载状态下不允许删除工程中的程序！</CH> <EN>Program of one project cannot be deleted when the project loaded!</EN>
52067  <CH>工程加载状态下不允许重命名工程中的程序！</CH> <EN>Program of one project cannot be renamed when the project loaded!</EN>
52068  <CH>变量名长度超限！</CH> <EN>Variable name overrun!</EN>
52069  <CH>IF命令携带设置坐标系或工具参数！</CH> <EN>IF command carries set coordinates or tool parameters!</EN>
52070  <CH>程序行数超限！</CH> <EN>Program line number overrun!</EN>
52071  <CH>打开程序文本失败！</CH> <EN>Open program file error!</EN>
52072  <CH>平移命令下发错误！</CH> <EN>Send Error!</EN>
52073  <CH>平移命令获取结果超时！</CH> <EN>Time Out!</EN>
52074  <CH>镜像命令下发错误！</CH> <EN>Send Error!</EN>
52075  <CH>镜像命令获取结果超时！</CH> <EN>Time Out!</EN>
52076  <CH>更新变量管理表失败！</CH> <EN>Update Variable false!</EN>
52077  <CH>操作失败！系统初始化未完成，请稍后重试！</CH> <EN>Operate failed! System initialization is unfinished! Please try again later!</EN>
52078  <CH>获取选定外部TCP坐标系参数失败！</CH> <EN>Failed to get externaltcp coordinate system parameters!</EN>
52079  <CH>获取选定变位机坐标系参数失败！</CH> <EN>Failed to get positioner coordinate system parameters!</EN>
52080  <CH>获取选定负载惯量参数失败！</CH> <EN>Failed to get payload parameters!</EN>
52081  <CH>传入的label名不存在，获取label所在行失败 ！</CH> <EN>The label name is not existed!</EN>
52082  <CH>程序删除成功！</CH> <EN>Program deleted successfully!</EN>
52083  <CH>工程删除成功！</CH> <EN>Project deleted successfully!</EN>
52084  <CH>API模式下不支持该操作！</CH> <EN>API mode is not support to this operation!</EN>
52085  <CH>全局工程不支持该操作：复制、删除、重命名！</CH> <EN>It is forbid to copy/delete/rename Global program!</EN>

------------------指令操作错误(52301~52500)------------------
52301  <CH>枚举字符串未找到！</CH> <EN>Enumeration string not found!</EN>
52302  <CH>指令校验不合法！</CH> <EN>Instruction verification is illegal!</EN>
52303  <CH>获取完整的指令内容失败！</CH> <EN>Failed to get complete instruction content!</EN>
52304  <CH>获取新建的指令列表失败！</CH> <EN>Failed to get new instruction list!</EN>
52305  <CH>获取修改的指令列表失败！</CH> <EN>Failed to get modified instruction list!</EN>
52306  <CH>获取新建的小指令列表失败！</CH> <EN>Failed to get the  new "add do" instruction list!</EN>
52307  <CH>获取修改的小指令列表失败！</CH> <EN>Failed to get the "add do" instruction list modified!</EN>
52308  <CH>小功能指令创建失败！</CH> <EN>Failed to create "add do" instruction!</EN>
52309  <CH>指令创建失败！</CH> <EN>Create instruction failed!</EN>
52310  <CH>指令修改失败！</CH> <EN>Modify instruction failed!</EN>
52311  <CH>指令复制失败！</CH> <EN>Copy instruction failed!</EN>
52312  <CH>指令剪切失败！</CH> <EN>Cut instruction failed!</EN>
52313  <CH>指令粘贴失败！</CH> <EN>Paste instruction failed!</EN>
52314  <CH>指令删除失败！</CH> <EN>Delete instruction failed!</EN>
52315  <CH>指令注释失败！</CH> <EN>Annotate instruction failed!</EN>
52316  <CH>指令恢复失败！</CH> <EN>Restore instruction failed!</EN>
52317  <CH>当前行号超过了最大数量限制！</CH> <EN>Current line number exceeds the maximum number of restriction!</EN>
52318  <CH>变量数据超过了最大限制！</CH> <EN>Data in Variable exceeds limitation!</EN> 
52319  <CH>不支持的指令类型！</CH> <EN>Instruction type unsupported!</EN>
52320  <CH>指令参数缺失，添加AddDo指令失败！</CH> <EN>Some instruction parameters are missing, failed to create "AddDo" instruction!</EN>
52321  <CH>指令转译失败！</CH> <EN>Instruction translation failed!</EN>
52322  <CH>指令校验时获取全部指令失败！</CH> <EN>Failed to get all instructions when the instructions are verified!</EN>
52323  <CH>指令MD5校验失败！</CH> <EN>Instructions MD5 check failed!</EN>
52324  <CH>工程名或程序名错误！</CH> <EN>The project name or program name is incorrect!</EN>
52325  <CH>程序为空，或结束行号大于起始行号！</CH> <EN>The program is empty or the starting line number bigger than ending line number!</EN>
52326  <CH>存在特殊字符或不闭合的语句，指令批量操作失败！</CH> <EN>There are special characters or logic instructions not closed, batch operation failed!</EN>
52327  <CH>更新程序文件失败！</CH> <EN>Failed to update program file!</EN>
52328  <CH>无法删除当前加载的程序！</CH> <EN>Current loaded program cannot be deleted!</EN>
52329  <CH>程序指令数量超出最大限制！</CH> <EN>The number of the program instruction exceed the limit!</EN>
52330  <CH>内存插入指令操作失败！</CH> <EN>Failed to insert an instruction into memory failed!</EN>
52331  <CH>指令文件打开错误！</CH> <EN>Failed to open the instruction file of the program!</EN>
52332  <CH>指令行数超出最大限制！</CH> <EN>The number of instruction lines exceeds the maximum limit!</EN>
52333  <CH>指令文件为空！</CH> <EN>The instruction file is empty!</EN>
52334  <CH>指令文件损坏，系统已从备份文件恢复该文件！</CH> <EN>The instruction file is corrupt, system has restored the file from the backup file!</EN>
52335  <CH>单行指令长度超出限制！</CH> <EN>The length of a single line instruction exceeds the limit!</EN>
52336  <CH>获取工程下的程序文件列表失败！</CH> <EN>Failed to get the list of program files under the project!</EN>
52337  <CH>指令粘贴失败，没有被复制或剪切的指令！</CH> <EN>There are no instruction to be copied or cut, failed to paste instruction!</EN>
52338  <CH>没有待粘贴的指令，指令粘贴失败！</CH> <EN>There are no instructions to be pasted!</EN>
52339  <CH>撤销失败！</CH> <EN>Undo operation failed!</EN>
52340  <CH>没有可撤销的操作！</CH> <EN>There are no operation can be undone!</EN>
52341  <CH>撤销文件名错误！</CH> <EN>Undo operation of file name error!</EN>
52342  <CH>触发操作不能为空！</CH> <EN>TrigDo param is empty! Please set it!</EN>
52343  <CH>操作失败！程序文件与指令系统版本不匹配，两者分别为:</CH> <EN>The version of program and inst version is not matching! They are </EN>
52344  <CH>操作的行号超过当前的程序总数，请刷新界面后重试！</CH> <EN>The line number of operation is out of the total number of current program!</EN>
52345  <CH>反向粘贴不能包含IF或WHILE指令！</CH> <EN>Reverse paste cannot cantain IF or WHILE!</EN>

------------------变量操作错误(52501~52700)------------------
52501  <CH>获取所有的变量值域失败！</CH> <EN>Failed to get all variable scopes!</EN>
52502  <CH>变量列表获取失败！</CH> <EN>Failed to get variable list!</EN>
52503  <CH>变量名为空或超过最大长度！</CH> <EN>Variable name is empty or exceeds the maximum length!</EN>
52504  <CH>变量作用域错误！</CH> <EN>Variable scope error!</EN>
52505  <CH>变量名已存在！</CH> <EN>Variable name have existed!</EN>
52506  <CH>变量创建失败！</CH> <EN>Create variable failed!</EN>
52507  <CH>变量删除失败！</CH> <EN>Delete variable failed!</EN>
52508  <CH>变量修改失败！</CH> <EN>Modify variable failed!</EN>
52509  <CH>变量值获取失败！</CH> <EN>Get variable value failed!</EN>
52510  <CH>变量名获取失败！</CH> <EN>Get variable name failed!</EN>
52511  <CH>变量未找到！</CH> <EN>Variable not fount!</EN>
52512  <CH>变量类型不合法！</CH> <EN>Variable type is illegal!</EN>
52513  <CH>示教点坐标获取失败！</CH> <EN>Failed to get teaching point!</EN>
52514  <CH>程序域变量掉电保存属性不允许修改！</CH> <EN>Program domain variables power-down protection attribute does not allow change!</EN>
52515  <CH>安全区域不是全局变量或者变量不存在！</CH> <EN>Safe-area is not a global variable or the variable does not exist!</EN>
52516  <CH>变量新建写入文件失败！</CH> <EN>Variable creation failed to write to file!</EN>
52517  <CH>变量修改写入文件失败！</CH> <EN>Variable modification failed to write to file!</EN>
52518  <CH>变量示教写入文件失败！</CH> <EN>Variable teaching failed to write to file!</EN>
52519  <CH>变量值不合法或超过最大长度！</CH> <EN>The variable value is invalid or the value's length exceeds the limit!</EN>
52520  <CH>PLC变量索引号不能相同！</CH> <EN>The indexes of different PLC variables cannot be the same!</EN>
52521  <CH>PLC变量已达最大数量！</CH> <EN>The number of PLC variable exceeds the maximum number limit!</EN>
52522  <CH>PLC变量正在被其他应用占用！</CH> <EN>PLC variable is using by other applications!</EN>
52523  <CH>变量重命名失败！</CH> <EN>Failed to rename the variable!</EN>
52524  <CH>系统变量不允许修改！</CH> <EN>The system variables cannot be modified!</EN>
52525  <CH>系统变量不允许删除！</CH> <EN>The system variables cannot be deleted!</EN>
52526  <CH>系统变量不允许重命名！</CH> <EN>The system variables cannot be renamed!</EN>
52527  <CH>全局变量内部处理错误！</CH> <EN>Global variable internal processing error!</EN>
52528  <CH>工程变量内部处理错误！</CH> <EN>Project variable internal processing error!</EN>
52529  <CH>程序变量内部处理错误！</CH> <EN>Program variable internal processing error!</EN>
52530  <CH>Run/Kill数据内部处理错误！</CH> <EN>Run/Kill data internal processing error!</EN>
52531  <CH>删除全局变量执行失败！</CH> <EN>Failed to delete the global variable!</EN>
52532  <CH>删除工程变量执行失败！</CH> <EN>Failed to delete the project variable!</EN>
52533  <CH>删除程序变量执行失败！</CH> <EN>Failed to delete the program variable!</EN>
52534  <CH>安全区域变量数据获取失败！</CH> <EN>Failed to get data of the area variable!</EN>
52535  <CH>安全区域变量不存在或者不是全局变量！</CH> <EN>The area variable does not exist or is not a global variable!</EN>
52536  <CH>系统变量不允许删除！</CH> <EN>The system variable is not allowed to be delete!</EN>
52537  <CH>当前加载的工程或程序与变量所在工程或程序不匹配！</CH> <EN>The currently loaded project or program does not match which the variable in!</EN>
52538  <CH>没有找到该用户坐标系变量！</CH> <EN>The user coordinate system variable was not found!</EN>
52539  <CH>没有找到该工具坐标系变量！</CH> <EN>The tool coordinate system variable was not found!</EN>
52540  <CH>该变量在程序指令中已经被引用！</CH> <EN>The variable has been referenced in the program!</EN>
52541  <CH>系统变量不允许创建！</CH> <EN>The system variables cannot be created!</EN>
52542  <CH>变量所在程序与当前打开的程序不一致！</CH> <EN>Variable is not in the current open program!</EN>
52543  <CH>程序变量文本打开失败！</CH> <EN>Failed to open program variable text!</EN>
52544  <CH>禁止对已被设为系统工具坐标系的tool参数进行修改/删除/重命名操作！</CH> <EN>This Variable is being used by the system tool, operate failed!</EN>
52545  <CH>禁止对已被设为系统用户坐标系的user参数进行修改/删除/重命名操作！</CH> <EN>This Variable is being used by the system user, operate failed!</EN>
52546  <CH>禁止对已被设为系统工件负载的payload参数进行修改/删除/重命名操作！</CH> <EN>This Variable is being used by the system payload, operate failed!</EN>
52547  <CH>当前域变量总数达到上限！</CH> <EN>The total number of variables in the current domain has reached the upper limit!</EN>
52548  <CH>变量创建失败,个数超出！</CH> <EN>Create variable failed,number of variables has reached the upper limit!</EN>
52549  <CH>禁止对使用中的extTcp参数进行修改/删除/重命名操作！</CH> <EN>This Variable is being used by the system, operate failed!</EN>
52550  <CH>禁止对使用中的变位机参数进行修改/删除/重命名操作！</CH> <EN>This Variable is being used by the system, operate failed!</EN>
52551  <CH>修改t_x域变量失败！</CH> <EN>Failed to modify t_x domain variable!</EN>
52552  <CH>修改t_xb域变量失败！</CH> <EN>Failed to modify t_xb domain variable!</EN>
52553  <CH>修改临时域变量失败！</CH> <EN>Failed to modify t_tmp domain variable!</EN>
52554  <CH>临时域变量转字符串失败！</CH> <EN>Conversion of t_tmp domain variable to string failed!</EN>
52555  <CH>更新变量文本失败！</CH> <EN>Failed to update variable text!</EN>
52556  <CH>禁止对已设为默认工具坐标系的tool参数进行删除/重命名操作！</CH> <EN>This Variable is being used by the default tool, operate failed!</EN>
52557  <CH>禁止对已设为默认用户坐标系的user参数进行删除/重命名操作！</CH> <EN>This Variable is being used by the default user, operate failed!</EN>
52558  <CH>禁止对已设为默认工件负载的payload参数进行删除/重命名操作！</CH> <EN>This Variable is being used by the default payload, operate failed!</EN>
52559  <CH>此区域变量处于激活状态，请冻结区域后再删除此变量！</CH> <EN>Failed to delete the variable because the variable is activated,please deactivate the variable then delete the variable!</EN>
52560  <CH>禁止对已被设为动坐标系的syncoord参数进行修改/删除/重命名操作！</CH> <EN>This Variable is being used by the system synccoord, operate failed!</EN>
52561  <CH>多边体区域变量数据获取失败！</CH> <EN>Failed to get data of the polyhedron area variable!</EN>
52562  <CH>多边体区域变量不存在或者不是全局变量！</CH> <EN>The polyhedron area variable does not exist or is not a global variable!</EN>
52563  <CH>全局工程中无工程变量 ！</CH> <EN>It is forbid to use project var in Global program!</EN>

------------------其他功能操作错误(52701~53000)------------------
52701  <CH>“停止”命令写入到解析器或者内核中失败！</CH> <EN>The stop command failed to be written to parser or motion kernel!</EN> 
52702  <CH>启动解析器或者内核失败！</CH> <EN>Failed to start parser or motion kernel!</EN>
52703  <CH>程序指针设置失败！</CH> <EN>Set point cursor failed!</EN>
52704  <CH>程序模式设置失败！</CH> <EN>Set program mode failed!</EN>
52705  <CH>工具坐标系变量获取失败！</CH> <EN>Failed to get the tool coordinate system variable!</EN>
52706  <CH>工具坐标系设置失败！</CH> <EN>Failed to set the tool coordinate system!</EN>
52707  <CH>用户坐标系获取失败！</CH> <EN>Failed to get the user coordinate system!</EN>
52708  <CH>用户坐标系设置失败！</CH> <EN>Failed to set the user coordinate system!</EN>
52709  <CH>程序没有加载！</CH> <EN>The program has not been loaded!</EN>
52710  <CH>PLC变量属性有变更，更改将在重启后生效！</CH> <EN>The "PLC" variable attributes have changed, the changes will take effect after restart!</EN>
52711  <CH>当前程序仅为打开状态，不允许启动！</CH> <EN>Current program is only open and not allowed to start!</EN>
52712  <CH>程序不处于停止或暂停状态，启动程序失败！</CH> <EN>The program is not in pause or stop state, failed to start the program!</EN>
52713  <CH>启动运动内核失败，无法启动程序！</CH> <EN>Failed to start the motion kernel, cannot start the program!</EN>
52714  <CH>停止指令下发失败！</CH> <EN>The stop command failed to be delivered!</EN>
52715  <CH>当前无工程加载，或打开的程序与加载程序不匹配！</CH> <EN>There is no project loaded, or the program opened does not match the loaded one!</EN>
52716  <CH>暂停程序失败！</CH> <EN>Failed to pause the program!</EN>
52717  <CH>恢复启动内核失败，无法启动程序！</CH> <EN>Failed to restart the motion kernel ,cannot start the program!</EN>
52718  <CH>恢复指令解析失败，无法启动程序！</CH> <EN>Recovery parsing instructions failed, cannot start the program!</EN>
52719  <CH>启动指令解析失败，无法启动程序！</CH> <EN>Failed to start parsing instruction, cannot start the program!</EN>
52720  <CH>“停止”命令写入解析器失败！</CH> <EN>The stop command failed to be written to the parser!</EN>
52721  <CH>“暂停”命令写入解析器失败！</CH> <EN>The pause command failed to be written to the parser!</EN>
52722  <CH>“暂停”命令写入内核失败！</CH> <EN>The pause command failed to be written to the kernel!</EN>
52723  <CH>激活区域命令写入解析器失败！</CH> <EN>Activating area command failed to be written to parser!</EN>
52724  <CH>冻结区域命令写入内核失败！</CH> <EN>Freezing area command failed to be written to motion kernel!</EN>
52725  <CH>此次工具参数标定错误，请重新标定！</CH> <EN>Failed to calculate TOOL param, please calculate it again!</EN>
52726  <CH>此次用户坐标系标定错误，请重新标定！</CH> <EN>Failed to calculate UserCoord param, please calculate it again!</EN>
52727  <CH>获取暂停点与当前点是否一致的状态失败！</CH> <EN>Failed to get pause point state!</EN>
52728  <CH>请切换到手动模式，再设置为倒序执行！</CH> <EN>Please set Back_Step in Teach mode!</EN>
52729  <CH>设置的变量类型不合法！</CH> <EN>Please ensure the default param type is legal!</EN>
52730  <CH>一点到达只支持手动模式！</CH> <EN>The function which system can mov to targetPos only support jog mode!</EN>
52731  <CH>机器人未完全停止状态，不响应GO操作，请增大抬起与按下的时间间隔！</CH> <EN>The robot is not completely stopped and does not respond to GO operation.Please increase the time interval between lifting and pressing!</EN>

------ER_ModbusTcpIp操作错误[53001,53199]--------
53001  <CH>modbustcp 通讯中断！</CH> <EN>Modbustcp communication is interrupted!</EN>
53002  <CH>网络连接断开超过时限，系统加密！</CH> <EN>Network disconnected beyond time limit, system encryption!</EN>
53003  <CH>公网下发命令，系统加密！</CH> <EN>Public network issuing commands, system encryption!</EN>
53004  <CH>公网下发命令, 功能使能！</CH><EN>Public network issuing commands, open the function!</EN>
53005  <CH>公网下发命令，永久解锁！</CH> <EN>Public network issuing commands, permanently unlock!</EN>
53006  <CH>已超过保养周期或日期，建议及时保养！</CH> <EN>It has exceeded the maintenance period or date, please maintain it in time!</EN>
53007  <CH>当前机型与保养机型不匹配，请重置保养信息！</CH> <EN>The current robot type does not match the maintenance robot type,please reset the maintenance information !</EN>
53008  <CH>示教器设置Modbus寄存器失败！</CH> <EN>Teaching-Programming pendant write ModubsTcp register failed！</EN>
53009  <CH>示教器读取Modbus寄存器失败！</CH> <EN>Teaching-Programming pendant read ModubsTcp register failed！</EN>