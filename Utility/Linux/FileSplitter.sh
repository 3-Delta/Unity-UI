::https://blog.csdn.net/weixin_30510153/article/details/99023584?utm_medium=distribute.pc_relevant_download.none-task-blog-2~default~BlogCommendFromBaidu~default-2.test_version_3&depth_1-utm_source=distribute.pc_relevant_download.none-task-blog-2~default~BlogCommendFromBaidu~default-2.test_version_
:: 文件存在：if exist %src%
:: 文件夹存在：if exist %destDir%
:: md, rd，rmdir /S 创建，删除空文件夹， 删除文件夹
:: touch, del 创建删除文件

set src=%1
set destDir=%2
:: 字节数
set size=%3

echo source: %src%
echo destination：%destDir%
echo size：%size%

:: 如果destDir已经存在，则先删除老的
if exist %destDir% (
	rmdir %destDir%
)

split -b %size% destDir

echo Press any key to exit
pause
