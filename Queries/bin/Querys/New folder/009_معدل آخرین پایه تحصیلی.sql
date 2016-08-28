﻿/*	009	[AVG]	*/
IF COL_LENGTH('TABLENAME','AVG') IS NULL BEGIN ALTER TABLE [TABLENAME] ADD [AVG] FLOAT END
GO
IF COL_LENGTH('TABLENAME','AVG') IS NOT NULL BEGIN UPDATE [TABLENAME] SET [AVG]=NULL END
UPDATE c SET c.[AVG]=d.[AVG] FROM [TABLENAME] c
JOIN 
	(	SELECT b.StudentID,a.MaxGRD,b.AVG FROM (SELECT StudentID,MAX(Grade)MaxGRD FROM [AmarPartDB].dbo.TBL_Arziabi GROUP BY StudentID) a
		INNER JOIN (SELECT * FROM [AmarPartDB].dbo.TBL_Arziabi WHERE AVG IS NOT NULL AND AVG>0)b
		ON b.StudentID = a.StudentID AND a.MaxGRD=b.Grade
	)d ON d.StudentID = c.StudentID
