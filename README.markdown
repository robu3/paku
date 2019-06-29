# paku (パク)

![Paku](/Images/Paku.gif?raw=true "Yummy!")

*paku* is a little utility designed to keep a folder clean by "eating" the files inside of it.
The "eating" action does not necessarily mean simple file deletion; zipping and encryption are among the
other options available.

## Process

The *paku* process consists of three steps:

1. select: Select files to eat.
2. filter: Filter the selected files to only the ones we desire.
3. paku: "Eats" the files by deleting, zipping, etc.

For example, let's say that you have an ETL process that loads hundreds of raw CSV files per day, and you want to prevent the drive
from filling up over time. Since the data is loaded into a database, you only really need to keep one week's worth of files around
in case there are any issues/bugs in the ETL process. You could use *paku* to keep the directory clean:

 dotnet.exe .\Paku.dll -d "C:\ETL\processing" -s=pattern:*.csv -f=age:cdate<7d -p=delete

Of course, there are several other options available for selection, filtering, and *paku*-ing. You can see the full list with:

 dotnet.exe .\Paku.dll --help


