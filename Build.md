Проект содержит:
- тестовую программу MainCClient.NET.exe (работает совместно с программой MainCSimulator, MainCSimulator.NET)


Для сборки программы MainCClient.NET.exe необходимо выполнить pbuild.bat (запускается по двойному щелчку мышкой).
После окончания сборки, MainCClient.NET.exe и сопуствующие файлы будут лежать в директории .\Redist\
 

Программа MainCClient.NET.exe имеет конфигурационный файл MainCClient.NET.exe.config, 
в котором задаются параметры сетевого подключения, по умолчанию, а так же язык интерфейса.

 ```
 <appSettings>
    <add key="IP" value="localhost" />
    <add key="Port" value="9996" />

    <!-- Language: -->
    <!-- ru = Russian -->
    <!-- en = English -->
    <add key="Language" value="ru" />
 </appSettings>
 ```

