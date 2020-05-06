CREATE TABLE "BtkKbob" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"SortCode"	TEXT,
	"IdKbob"	INTEGER,
	"Thickness"	REAL,
	"ThermalCond"	REAL,
	"Percentage"	REAL,
	"MinThickness"	REAL,
	"MaxThickness"	REAL,
	"Increment"	REAL,
	"Layer"	INTEGER
);