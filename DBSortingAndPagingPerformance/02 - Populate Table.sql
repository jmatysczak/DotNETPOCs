DECLARE @i INT = 0
WHILE @i < 500000 BEGIN
   DECLARE @title VARCHAR(25) = ''
   WHILE LEN(@title) < 20 SET @title = @title + CHAR(48 + (RAND() * 74))
   
   SET @title = STUFF(@title, CAST(RAND() * 10 AS INT) + 6, 0, ' ')
   SET @title = STUFF(@title, CAST(RAND() * 10 AS INT) + 6, 0, ' ')
   
   INSERT INTO Item(Title) VALUES (@title)

   SET @i = @i + 1
END
GO
