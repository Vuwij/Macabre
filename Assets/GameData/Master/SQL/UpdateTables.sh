# Delete the entire file
rm ../MacabreDB.master.db3

# Reset the entire table
sqlite3 ../MacabreDB.master.db3 < CreateAllTables.sql

# Run the imports
sqlite3 ../MacabreDB.master.db3 < UpdateTablesScript.txt

# Remove table headings
sqlite3 ../MacabreDB.master.db3 < DeleteTableHeaders.sql