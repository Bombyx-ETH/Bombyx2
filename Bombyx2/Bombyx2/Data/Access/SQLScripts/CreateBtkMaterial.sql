CREATE TABLE "BtkMaterial" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"SortCode"	TEXT,
	"NameEnglish"	TEXT,
	"NameGerman"	TEXT,
	"NameFrench"	TEXT,
	"EcoDevisEval"	TEXT,
	"EcoDevisCode"	TEXT,
	"Thickness"	REAL,
	"Lambda"	REAL,
	"LifeSpan"	INTEGER,
	"Mass"	REAL,
	"Quantity"	REAL,
	"Unit"	TEXT
);