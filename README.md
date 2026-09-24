# AccidentesMadrid 

Práctica que analiza accidentes de tráfico de Madrid de 2024, 2025 y 2026 usando LINQ, PLINQ y DataFrames en C#.

## Cómo ponerlo en marcha

Los 3 CSV ya están metidos en `AccidentesMadrid.Back/Data/`, así que no hay que descargar nada, tiran directos.

- Local: `dotnet run` desde `AccidentesMadrid.Back`
- Docker: `docker compose up --build` desde la raíz

## Tecnologías

- C# 14 / .NET
- LINQ y PLINQ
- `Microsoft.Data.Analysis` (DataFrames)
- CsvHelper para leer los CSV
- CSharpFunctionalExtensions (`Result<T, DomainError>` en vez de excepciones)
- NUnit + FluentAssertions para los tests

## Estructura del proyecto

```
AccidentesMadrid.Back/
├── Models/          → el modelo de dominio (Accidente)
├── Mappers/         → AccideneCsv (lectura cruda del CSV) + AccidenteMapper (CSV → dominio)
├── Errors/          → errores de dominio tipados (DomainError, AccidenteError)
├── Repositories/     → AccidentesRepository (lee y combina los 3 CSV)
├── Services/         → AccidentesLinq (30 consultas LINQ) + AccidentesDataFrame (las mismas 30 con DataFrame)
└── Program.cs        → orquesta todo y mide tiempos
AccidentesMadrid.Tests/  → tests de Mapper, Repository y de las consultas
```

## Decisiones que he tomado (y por qué)

### Sin DTO intermedio

Solo tengo una fuente de datos (el CSV de Madrid), un DTO tiene sentido cuando hay varios formatos.
Si mañana tuviera que leer de otro formato (Json), añadiría el mapper nuevo sin tocar lo que ya funciona (abierto/cerrado, vamos).

### Caché de fechas/horas ya parseadas

Como muchas de las 30 consultas del DataFrame necesitan la fecha o la hora ya convertidas, y parsear ~130.000 filas cada vez sale caro, guardo el resultado la primera vez que lo necesito (`_filasCache`) y lo reutilizo en las siguientes consultas. Se nota bastante en los tiempos: la primera consulta que toca fecha es la más lenta, las que vienen después van más rápidas porque ya no repiten el parseo.

### PLINQ

Solo tengo una consulta con PLINQ (la de "lesión más frecuente por año"), PLINQ **no** mejoraba los tiempos de forma relevante.

## Resultados y tiempos

Ejecución real (`dotnet run`), 3 CSV fusionados, 130.864 filas leídas.

### Lectura de los 3 CSV

| Modo | Tiempo |
|---|---|
| Secuencial | 1261 ms |
| Paralela (`Task.WhenAll`) | 399 ms |

~3.2x más rápido en paralelo.

### Las 30 consultas: LINQ vs DataFrame (ms)

| # | Consulta | LINQ | DataFrame |
|---|---|---|---|
| 1 | Total accidentes | 23 | 24 |
| 2 | Accidentes por distrito (top 5) | 44 | 60 |
| 3 | Accidentes por tipo | 35 | 50 |
| 4 | Accidentes por estado meteorológico | 34 | 50 |
| 5 | Accidentes por sexo | 28 | 23 |
| 6 | Accidentes por rango de edad | 29 | 25 |
| 7 | Positivos en alcohol | 5 | 16 |
| 8 | Positivos en drogas | 5 | 3 |
| 9 | Accidentes por día de la semana | 32 | 230 |
| 10 | Accidentes por mes | 50 | 32 |
| 11 | Hora con más accidentes | 29 | 36 |
| 12 | Lesiones más frecuentes | 28 | 20 |
| 13 | Tipo de vehículo más implicado | 32 | 26 |
| 14 | Accidentes con peatones | 14 | 8 |
| 15 | Proporción hombre/mujer | 28 | 31 |
| 16 | Distritos con más peatones | 14 | 32 |
| 17 | Fin de semana vs entre semana | 27 | 28 |
| 18 | Media de accidentes por día | 36 | 51 |
| 19 | Accidentes con alcohol + droga | 5 | 33 |
| 20 | Rangos de edad más vulnerables (peatones) | 14 | 30 |
| 21 | Distritos con más positivos en alcohol | 9 | 31 |
| 22 | Accidentes por código de distrito | 30 | 50 |
| 23 | Accidentes por año | 33 | 30 |
| 24 | Evolución mensual por año | 38 | 35 |
| 25 | Distrito con más accidentes por año | 49 | 43 |
| 26 | Tendencia de alcohol por año | 10 | 16 |
| 27 | Comparativa fin de semana vs entre semana por año | 37 | 28 |
| 28 | Hora pico por año | 37 | 27 |
| 29 | Lesión más frecuente por año | 49 | 21 |
| 29b | Lesión más frecuente por año (PLINQ) | 41 | — |
| 30 | Evolución de peatones por año | 14 | 8 |
| | **Total (30 consultas)** | **818 ms** | **1097 ms** |

### Lo que se ve en los números

- LINQ gana: 818 ms vs 1097 ms de DataFrame.
- La consulta 9 (día de la semana) es la que más tarda en DataFrame, 230 ms, porque es la primera que toca fechas y ahí se rellena la caché. Las siguientes consultas con fecha ya van rápidas (16-51 ms) porque se reaprovecha.
- Si quito esa consulta, DataFrame baja a 867 ms, casi empata con LINQ.
- PLINQ (29b) tarda 41 ms, la versión normal 49 ms. Mejora poco, no merece la pena para 1 consulta con solo 3 particiones.

## Resumen 

DataFrame como en los apuntes empieza a rendir a partir de los 150.000 filas, pero en la práctica, para este caso LINQ sigue yendo bastante bien y a mí me parece más simple de leer.
Y con PLINQ me pasa algo parecido: hay que medir antes de meterlo porque se me complica el código sin ganar nada a cambio.
