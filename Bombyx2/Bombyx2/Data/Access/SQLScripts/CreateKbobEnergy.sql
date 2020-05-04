CREATE TABLE "KbobEnergy" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"NameEnglish"	TEXT,
	"NameGerman"	TEXT,
	"NameFrench"	TEXT,
	"IdKbob"	TEXT,
	"Size"	TEXT,
	"Unit"	TEXT,
	"UBP"	REAL,
	"PeTotal"	REAL,
	"PeRenewable"	REAL,
	"PeNonRenewable"	REAL,
	"PeRenewableAtLocation"	REAL,
	"GHG"	REAL
);