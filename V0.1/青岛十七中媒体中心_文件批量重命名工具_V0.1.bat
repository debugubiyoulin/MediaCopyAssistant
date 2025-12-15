@echo off
chcp 65001 >nul
:start
cls
echo === 文件批量重命名工具 ===
echo 功能：为文件批量添加前缀和时间戳
echo 格式：前缀_时间戳_原文件名
echo V0.1    ©青岛十七中媒体中心
echo ==========================
echo.

set /p "folderPath=请输入文件夹路径: "

:: 检查路径是否存在
if not exist "%folderPath%" (
    echo 错误：文件夹不存在！
    echo 按任意键返回...
    pause >nul
    goto :start
)

set /p "prefix=请输入要添加的前缀: "

:: 使用WMIC获取标准格式的日期和时间
for /f "tokens=1,2 delims==" %%a in ('wmic OS Get localdatetime /value') do (
    if "%%a"=="LocalDateTime" set datetime=%%b
)

:: 提取日期和时间部分
set year=%datetime:~0,4%
set month=%datetime:~4,2%
set day=%datetime:~6,2%
set hour=%datetime:~8,2%
set minute=%datetime:~10,2%
set second=%datetime:~12,2%

:: 创建时间戳
set timestamp=%year%%month%%day%_%hour%%minute%%second%

cd /d "%folderPath%"
set count=0

echo.
echo 时间戳: %timestamp%
echo 开始处理文件...
echo ==================

:: 使用简单的循环和计数
for %%f in (*) do (
    if not "%%f"=="%~nx0" (
        call :process_file "%%f"
    )
)

echo ==================
echo 处理完成! 成功重命名 %count% 个文件
echo.
echo 按任意键继续处理下一个文件夹...
pause >nul
goto :start

:process_file
set "file=%~1"
if "%prefix%"=="" (
    set "newname=%timestamp%_%file%"
) else (
    set "newname=%prefix%_%timestamp%_%file%"
)

ren "%file%" "%newname%" >nul 2>&1
if %errorlevel% equ 0 (
    echo 成功: "%file%" -^> "%newname%"
    set /a count+=1
) else (
    echo 失败: "%file%" (可能文件名已存在或文件被占用)
)
goto :eof