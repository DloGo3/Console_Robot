# 项目改进指导

这个项目可能后续由其他人接收，在添加新功能或改进原始功能时请遵循下面几个规范：  

1. 遵循XML格式的注释习惯：当使用Visual Studio来开发代码时，项目设置了在生成解决方案时会生成一个xml格式的注释文档，该文档被用于后期docfx工具生成html格式的API文档，具体的注释规范可以参考[这篇文章](https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/xmldoc/)。
2. 日志记录：日志是通过Serilog结合Seq数据库实现的，Seq数据库直接按照[快速开始](getting-started.md)中提到的链接安装就行，Serilog通过NuGet安装（在VS中右键项目有一个`管理NuGet程序包`的选项，搜索安装即可），安装好之后通过 [http://localhost:5341](http://localhost:5341) 可以访问到Seq数据库的前端，在Setting中添加两个API KEY，一个用于记录Controller层的日志，一个用于记录非Controller层的日志，然后在WebBackend文件夹的config.yaml设置对应的字段即可。
3. 异常处理：后端的服务器内部错误暴露给前端是软件开发一大忌，现有项目绝大部分异常都被捕获和处理，同时通过Serilog进行日志记录。Controller层和Service层都有日志记录，Controller层注重记录请求参数，Service层注重记录代码内部问题。
4. 版本控制：项目通过Git进行版本控制，在拉取该项目之后，请不要在**master**分支下进行代码修改，需新建一个属于自己的分支（based on master or dev）后，在测试完成功能后再merge到master分支。提交前，使用 `git config` 命令配置好自己的用户名和邮箱后再提交。每次切换分支时**务必**提交当前分支的所有修改（可暂时不同步到远程仓库），否则会丢失当前已保存的修改。每次拉取项目时可能会产生冲突，请小心谨慎地处理这些冲突。

> 有其他问题请发邮件给我！  
> 联系邮箱：hosealle@outlook.com