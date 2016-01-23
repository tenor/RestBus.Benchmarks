## SEND-ONLY BENCHMARKS CHART DATA ##

**One Publisher, One Consumer**  
**Non-Persistent Messaging** *  
**Message Size:** 2048 bytes (roughly)  
**Messages Per Thread**: 5000  
**Serialization Format**: JSON  
**Publisher Confirms:** Off *  
**Consumer No-Ack:** False *  
**Consumer Prefetch:** 50   
**Client logging:** Off



| *                                        | AVERAGE TIME PER THREAD |          |           |           |   | THROUGHPUT (Messages Per Second) |          |          |          |   | THROUGHPUT (KB Per Second) |          |          |          |
|------------------------------------------|-------------------------|----------|-----------|-----------|---|----------------------------------|----------|----------|----------|---|----------------------------|----------|----------|----------|
|                                          |                         |          |           |           |   |                                  |          |          |          |   |                            |          |          |          |
|                                          | 10                      | 20       | 40        | 80        |   | 10                               | 20       | 40       | 80       |   | 10                         | 20       | 40       | 80       |
| RestBus (Web API)                        | 2.713775                | 5.4293   | 11.18335  | 28.91775  |   | 18424.52                         | 18418.58 | 17883.73 | 13832.33 |   | 36849.04                   | 36837.16 | 35767.46 | 27664.67 |
| RestBus (ASP.NET 5)                      | 2.98415                 | 7.38205  | 12.013    | 28.95435  |   | 16755.19                         | 13546.37 | 16648.63 | 13814.85 |   | 33510.38                   | 27092.75 | 33297.26 | 27629.7  |
| RestBus (ASP.NET 5 -- Bare to the metal) | 2.86255                 | 6.20995  | 11.2422   | 28.1336   |   | 17466.94                         | 16103.19 | 17790.11 | 14217.87 |   | 34933.89                   | 32206.38 | 35580.22 | 28435.75 |
| EasyNetQ                                 | 2.6086                  | 5.33835  | 11.131275 | 22.006625 |   | 19167.37                         | 18732.38 | 17967.39 | 18176.34 |   | 38334.74                   | 37464.76 | 35934.79 | 36352.69 |
| MassTransit                              | 25.4128                 | 33.71485 | 50.0037   | 86.6202   |   | 1967.51                          | 2966.05  | 3999.7   | 4617.86  |   | 3935.02                    | 5932.1   | 7999.41  | 9235.72  |
| NServiceBus                              | 19.14465                | 33.5144  | 66.1058   | 132.45355 |   | 2611.7                           | 2983.79  | 3025.45  | 3019.93  |   | 5223.39                    | 5967.58  | 6050.91  | 6039.85  |    



**Notes**  
- Queues are drained between tests.  
- The message size is doubled for the first two RestBus and EasyNetQ tests, so that the test period is not too short (<3 secs) and the results are more reliable.