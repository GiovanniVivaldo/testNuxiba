WITH Sesiones AS (
    SELECT 
        l.User_id,
        l.Fecha AS FechaLogin,
        o.Fecha AS FechaLogout,
        DATEDIFF(SECOND, l.Fecha, o.Fecha) AS DuracionSegundos
    FROM ccLogLogin l

    -- Emparejamos cada login con su primer logout posterior (del mismo usuario)
    OUTER APPLY (
        SELECT TOP 1 o.*
        FROM ccLogLogin o
        WHERE o.User_id = l.User_id         -- 🔒 mismo usuario
          AND o.TipoMov = 0                 -- logout
          AND o.Fecha > l.Fecha             -- posterior
        ORDER BY o.Fecha
    ) o

    WHERE l.TipoMov = 1                     -- solo logins
      AND o.Fecha IS NOT NULL              -- asegurar que tenga logout
),
Totales AS (
    SELECT 
        User_id,
        SUM(DuracionSegundos) AS TotalSegundos
    FROM Sesiones
    GROUP BY User_id
)
SELECT TOP 1
    User_id,
    FORMAT(TotalSegundos / 86400, '0') + ' días, ' +
    FORMAT((TotalSegundos % 86400) / 3600, '00') + ' horas, ' +
    FORMAT((TotalSegundos % 3600) / 60, '00') + ' minutos, ' +
    FORMAT(TotalSegundos % 60, '00') + ' segundos' AS TiempoTotal
FROM Totales
ORDER BY TotalSegundos ASC;