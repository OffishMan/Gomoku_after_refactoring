USE ScoreTable
GO

CREATE PROCEDURE AddResult @Nickname nvarchar (50), @score int
AS
INSERT INTO Scores
VALUES (@Nickname, @score)
SELECT SCOPE_IDENTITY()
GO

--CREATE PROCEDURE ShowSomeScores @id int, @Nickname nvarchar (50), @score int
--AS
--select * from
--(select row_number() over (order by Scores) as n, * from Scores) x
--where n = @score

CREATE PROCEDURE ShowPlace @score int
AS
SELECT TOP 1 Sorted.pos as pos
FROM (SELECT Score as scr, RANK() OVER (ORDER BY Score DESC) AS pos
FROM Scores) AS Sorted
WHERE scr = @score
GO

CREATE PROCEDURE ShowMyPosition @score int
AS
SELECT TOP 10 * 
FROM (SELECT RANK() OVER (ORDER BY Score DESC) AS pos, Nickname AS nick, Score as scr
FROM Scores) AS Sorted
WHERE scr <= @score
GO

CREATE PROCEDURE ShowAllResults 
AS
SELECT * FROM Scores
GO