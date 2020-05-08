CREATE TABLE "BtkWindows" (
	"Id"	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,
	"ComponentCode"	TEXT,
	"SortCode"	TEXT,
	"CategoryEnglish"	TEXT,
	"CategoryGerman"	TEXT,
	"CategoryFrench"	TEXT,
	"CategoryTextEnglish"	TEXT,
	"CategoryTextGerman"	TEXT,
	"CategoryTextFrench"	TEXT,
	"PictureURL"	TEXT,
	"RSL"	INTEGER,
	"Uvalue"	REAL,
	"Gvalue"	REAL,
	"Cost"	REAL,
	"CostUnit"	TEXT
);