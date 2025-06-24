
WITH Sesiones AS (
    SELECT 
        l.User_id,
        l.Fecha AS FechaLogin,
        o.Fecha AS FechaLogout,
        DATEDIFF(SECOND, l.Fecha, o.Fecha) AS DuracionSegundos
    FROM ccLogLogin l

    -- Emparejamos cada login con su primer logout posterior del MISMO usuario
    OUTER APPLY (
        SELECT TOP 1 o.*
        FROM ccLogLogin o
        WHERE o.User_id = l.User_id           -- mismo usuario
          AND o.TipoMov = 0                   -- logout
          AND o.Fecha > l.Fecha               -- posterior al login
        ORDER BY o.Fecha
    ) o

    WHERE l.TipoMov = 1                       -- solo registros de login
      AND o.Fecha IS NOT NULL                 -- aseguramos que hubo logout
),
PromedioPorMes AS (
    SELECT 
        User_id,
        FORMAT(FechaLogin, 'yyyy-MM') AS Mes,
        AVG(DuracionSegundos) AS PromedioSegundos
    FROM Sesiones
    GROUP BY User_id, FORMAT(FechaLogin, 'yyyy-MM')
)
SELECT 
    User_id,
    Mes,
    FORMAT(PromedioSegundos / 86400, '0') + ' días, ' +
    FORMAT((PromedioSegundos % 86400) / 3600, '00') + ' horas, ' +
    FORMAT((PromedioSegundos % 3600) / 60, '00') + ' minutos, ' +
    FORMAT(PromedioSegundos % 60, '00') + ' segundos' AS PromedioLogueo
FROM PromedioPorMes
ORDER BY User_id, Mes;