/*****************************************************************//**
 * \file   EstunRemoteApiLib_global.h
 * \brief  
 * 
 * \author wanglei64101
 * \date   May 2022
 *********************************************************************/
#ifndef ESTUNREMOTEAPILIB_GLOBAL_H
#define ESTUNREMOTEAPILIB_GLOBAL_H

#ifdef _WIN32
#define ESTUN_EXPORT_API __declspec(dllexport)
#define ESTUN_IMPORT_API __declspec(dllimport)
#else
#define ESTUN_EXPORT_API __attribute__((visibility("default")))
#define ESTUN_IMPORT_API
#endif

#ifdef ESTUNREMOTEAPILIB_EXPORTS
#define ESTUNREMOTEAPILIBAPI ESTUN_EXPORT_API
#else
#ifdef ESTUNREMOTEAPILIB_STATIC
#define ESTUNREMOTEAPILIBAPI
#else
#define ESTUNREMOTEAPILIBAPI ESTUN_IMPORT_API
#ifdef _AUTO_LINK_
#define __LIB_NAME__ "EstunRemoteApiLib"
#include <EasyFrameRuntime/EFAutoLink.h>
#endif
#endif
#endif

#include <iostream>
using namespace std;

#ifndef _DEBUG
#define _SECURE_SCL 0
#endif // !_DEBUG


#endif
