CHECKPOINT; DBCC DROPCLEANBUFFERS; DBCC FREEPROCCACHE;
SET STATISTICS IO ON
SET STATISTICS TIME ON

SELECT   *
FROM     (
            SELECT   ROW_NUMBER() OVER (ORDER BY Title DESC) rn,
                     *
            FROM     (
                        SELECT   TOP 100
                                 *
                        FROM     Item
                        ORDER BY Title DESC
                     ) dv
         ) dv
WHERE    rn BETWEEN 81 AND 100
