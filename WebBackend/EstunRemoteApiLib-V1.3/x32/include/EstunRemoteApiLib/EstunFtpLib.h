#ifndef __ESTUNFTPLIB_H__
#define __ESTUNFTPLIB_H__

#include "EstunRemoteApiLib_global.h"
#include "EstunRemoteApiLib_struct.h"
using namespace EstunRobot;

struct ESTUNREMOTEAPILIBAPI FtpFileInfo
{
	FtpFileInfo()
	{
		name = "";
		isFolder = false;
	}

	FtpFileInfo(const FtpFileInfo& t)
	{
		this->name = t.name;
		this->isFolder = t.isFolder;
	}

	FtpFileInfo& operator = (const FtpFileInfo& t)
	{
		if (this != &t)
		{
			this->name = t.name;
			this->isFolder = t.isFolder;
		}
		return *this;
	}

	//文件名称
	ESString name;
	//是否是目录
	bool isFolder;
};

class ESTUNREMOTEAPILIBAPI EstunFtpLib
{
public:
	EstunFtpLib();
	~EstunFtpLib();

	/**
	 * 连接Ftp服务.
	 * 
	 * \param ip_port 服务IP:port
	 * \return 成功返回true
	 */
	bool Connect(ESString ip_port);

	/**
	 * 断开连接.
	 * 
	 * \return 成功返回true
	 */
	bool DisConnect();

	/**
	 * 进入到目录并将工作目录修改到当前.
	 * 
	 * \param path 远端目录
	 * \return 成功返回true
	 */
	bool EntryFolder(ESString path);

	/**
	 * 获取当前远端路径.
	 * 
	 * \param char数组地址，由调用者申请
	 * \param char数组容量
	 * \return 成功返回true
	 */
	bool PWD(char* path, int max);

	/**
	 * 获取从当前目录开始的相对路径下的内容列表.
	 * 
	 * \param path 相对路径
	 * \param infoArray 文件列表数组指针，包含当前和上级描述符
	 * \param infoNum 文件列表数组元素个数
	 * \return 获取成功返回true
	 */
	bool ListDir(ESString path, FtpFileInfo*& infoArray, int& infoNum);

	/**
	 * 创建远端路径.
	 * 
	 * \param path 相对路径
	 * \return 成功返回true
	 */
	bool CreateDir(ESString path);

	/**
	 * 删除远端文件.
	 * 
	 * \param path 相对路径
	 * \return 成功返回true
	 */
	bool RemoveFile(ESString path);

	/**
	 * 删除远端目录.
	 *
	 * \param path 相对路径
	 * \return 成功返回true
	 */
	bool RemoveDir(ESString path);

	/**
	 * 拉取远端文件到本地.
	 * 
	 * \param localPath 本地保存文件路径，带文件名
	 * \param remotePath 远端文件路径
	 * \return 成功返回true
	 */
	bool GetFile(ESString localPath, ESString remotePath);

	/**
	 * 拉取远端目录内容到本地目录.
	 * 
	 * \param localPath 本地保存路径
	 * \param remotePath 远端路径
	 * \return 成功返回true
	 */
	bool GetFolder(ESString localPath, ESString remotePath);

	/**
	 * 上传本地文件到远端.
	 * 
	 * \param localPath 本地文件路径
	 * \param remotePath 远端保存文件路径，带文件名
	 * \return 成功返回true
	 */
	bool PutFile(ESString localPath, ESString remotePath);

	/**
	 * 上传本地目录内容到远端目录.
	 * 
	 * \param localPath 本地路径
	 * \param remotePath 远端保存路径
	 * \return 成功返回true
	 */
	bool PutFolder(ESString localPath, ESString remotePath);

	/**
	 * 获取最近一次执行接口错误信息.
	 * 
	 * \return 错误信息
	 */
	ESString GetLastErr();

private:
	struct EstunFtpLibPrivate;
	EstunFtpLibPrivate* _p;
};

#endif