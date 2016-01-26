### Test Date
January 16 2016.

### Test Environment

AWS East Region.  
Three m4.4xlarge (16 vcpus, 64 GiB memory, 2000 Mbps network throughput) Windows Server 2012 R2 EC2 instances.  
Machine 1: Running RabbitMQ server version 3.6.0 (with 6 default management related plugins)  
Machine 2: Runs test servers.  
Machine 3: Runs test clients.

### Library Versions

| Nuget Library        | Version     |
|----------------------|-------------|
| RestBus.RabbitMQ     | 0.6.9       |
| RestBus.WebAPI       | 0.5.8       |
| RestBus.AspNet       | 0.6.2-rc2   |
| EasyNetQ             | 0.53.6.418  |
| MassTransit          | 3.1.2       |
| MassTransit.RabbitMQ | 3.1.2       |
| NServiceBus          | 5.2.12      |
| NServiceBus.RabbitMQ | 3.0.1       |


### Test Setup 

- All tests have one publisher and one consumer.
- Transient messaging is used when possible.
- Publisher confirms is turned off when possible.  
- 5000 messages are sent by the client per thread. *(See Send Only Notes below)*  
- The serialization format is JSON.  
- Consumer Prefetch is 50.
- Client/Server logging: Off

**IMPORTANT**:
   
* Only use Release builds of Test servers and clients.  
* Run test clients and servers from the cmd.exe command prompt. Running from the Windows desktop may lead to a slight degradation in performance.

### Test Procedure

1. The test server is started.
2. The client is ran three times in quick succession.
3. The first measurement is discarded (The broker might be starting resources). The second and third measurements are recorded.

**Ease of use rating is determined by:**

1. Intuitiveness of the library.
2. How quickly a working RPC topology can be set up using official documentation, stackoverflow, and other online resources.
3. Ease of configuration.

### RPC Test Notes 

Set app.config *MessageSize* setting to 2048 for client and server.  
Set client app.config *ExpectReply* to true.  
Set server app.config *Reply* to true. *(not applicable for RestBus servers)*  

### Send Only Test Notes 

Set app.config *MessageSize* setting to 2048 for client and server.  
Set client app.config *ExpectReply* to false.  
Set server app.config *Reply* to false. *(not applicable for RestBus servers)*  

For the 10 and 20 thread RestBus and EasyNetQ tests, a higher number of messages sent per thread (*NoOfThreads* setting) is needed since the tests complete too quickly to be reliable.

Queues must be drained before moving to a new set of tests. For example, after running a test (3 times) at a certain thread level, make sure the queue is drained or purged before running the next set of tests.

### Payload Test Notes

Set client app.config *NoOfThreads* setting to 20.    
Set client app.config *ExpectReply* to true.  
Set server app.config *Reply* to true. *(not applicable for RestBus servers)* 