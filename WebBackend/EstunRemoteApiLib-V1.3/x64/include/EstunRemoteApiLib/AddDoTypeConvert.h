/*****************************************************************//**
 * \file   AddDoTypeConvert.h
 * \brief  
 * 
 * \author wanglei64101
 * \date   May 2022
 *********************************************************************/
#ifndef ADDDOTYPECONVERT_H
#define ADDDOTYPECONVERT_H

#include "EstunRemoteApiLib_struct.h"
using namespace EstunRobot;

class ESTUNREMOTEAPILIBAPI AddDoTypeConvert
{
public:
	/**
	 * 将E_ROB_ADDDO_IO类型转换为E_ROB_APIBUFF.
	 * 
	 * \param from ADD_DO_IO参数
	 * \param to ADD_DO结构，引用类型
	 */
	static void ADDDO_IO_2_E_ROB_APIBUFF(const E_ROB_ADDDO_IO& from, E_ROB_APIBUFF& to);
};

#endif