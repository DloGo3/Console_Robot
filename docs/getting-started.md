# 快速开始

本项目需要提前安装：

1. 开发工具：VS2017以上  
2. .NET SDK 版本：6.1以上
3. Seq数据库，[Seq数据库官网](https://datalust.co/seq)，用于日志记录，便于查看日志（非常好用建议安装）

## 项目配置

Estun的API库相关配置理论上项目已在WebBackend.csproj中配置好，不要轻易调整项目结构，若需调整详见<a href="ER系列工业机器人API使用手册_RCS2_V1.3.pdf" target="_blank">ER系列工业机器人API使用手册_RCS2_V1.3.pdf</a>  

MySQL数据库需要给指定局域网网段访问权限：  

```sql
CREATE USER 'root'@'192.168.1.%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON *.* TO 'root'@'192.168.1.%' WITH GRANT OPTION;
FLUSH PRIVILEGES;
```

## 网页版文档查看

目前该文档以通过Github Actions部署到 develop.ustbhyy.top 下，将来可能无法访问此网页，可以克隆此项目，在项目根目录下执行 `docfx docfx.json --serve` 命令，访问 [http://localhost:8080](http://localhost:8080) 进行查看