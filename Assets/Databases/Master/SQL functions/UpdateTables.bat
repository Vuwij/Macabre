REM Reset the entire table
sqlite3 ../MacabreDB.master.db3 < CreateAllTables.sql

REM Run the imports
sqlite3 ../MacabreDB.master.db3 < UpdateTablesScript.txt

REM Remove table headings
sqlite3 ../MacabreDB.master.db3 < DeleteTableHeaders.sql

PAUSE