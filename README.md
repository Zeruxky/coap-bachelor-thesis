# Bachelor thesis about CoAP.NET
The requirements of today's web servers are wide-ranging. A server should withstand a huge amount of requests, process them in a short period and use resources as few as possible. One possible solution is to increase the number of servers as the load increases, but this only reduces the problem and does not fully eliminate it. Another way is to increase the throughput of server applications by using the asynchronous pattern. In this thesis a proof of concept is being evaluated by fully rewriting the C# CoAP library CoAP.NET and using the mentioned pattern. Due to the limited ability to reuse parts of the old library, retransmission and block-wise transfer of messages will not be covered by this thesis. To measure the impact of the pattern, the old library and its newly created counterpart will be compared to each other on several scenarios by using automated tests.

The resulting library is going to be build with C#. It will be based on the Task-based asynchronous pattern (TAP) from Microsoft and published under an Open-Source license on GitHub.

Disclaimer: This thesis will be developed in cooperation with World-Direct eBusiness solutions GmbH.

GitHub-Repository Link: https://github.com/world-direct/CoAP.NET
