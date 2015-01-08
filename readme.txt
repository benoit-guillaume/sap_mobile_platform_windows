Windows Store Flight Sample Application Series consists of 10 complete applications (hereinafter referred to as "solutions") and 10 incomplete applications (hereinafter referred to as "exercises") targeting the Windows Store and Windows Desktop.  These 10 exercises are identical to the 10 solutions, except certain key code snippets are marked with TODO comments.  The objective is for the end user to fill in the TODO sections with the appropriate code snippets – which will be described in detail in the How To… guides.  

The "master" branch contains the exercises.
The "solution" branch contains the solutions.  

The 10 solutions that target the Windows Store and Windows Desktop are listed below.  Each solution builds on top of the previous solution.  Incrementally building 10 separate solutions based on functionality makes it easier for the end user to understand key concepts of the SAP Mobile Platform SDK.  Each solution will have its own How To… guide located in the same folder.  Additionally, all the How To... guides associated with each project can be found on http://scn.sap.com/docs/DOC-58677#Windows.
   
	1.  Onboarding – Onboards a device (Windows Store and Windows Desktop)
	2.  GETFlight – Gets Flight data from the SAP backend (Windows Store and Windows Desktop)
	3.  JSONService – Gets airport weather conditions from an external JSON service (Windows Store and Windows Desktop)
	4.  POSTFlightBooking – Books a flight (Windows Store and Windows Desktop)
	5.  E2ETracing – End to End tracing of log messages (Windows Store and Windows Desktop)

SAP Mobile Platform Windows SDK (hereinafter referred to as "SDK") hides all the complexities involved in consuming an OData Service.  The SDK leverages the power and simplicity of Microsoft’s asynchronous programming, dependency properties and other key concepts.  

The 10 solutions uses the ubiquitous Flight database that is shipped with SAP systems to register a device with the SMP Server and retrieve flight information and also book flights.  The OData Service exposed by NetWeaver Gateway is consumed by the solutions by utilizing the methods provided in the SDK.  In addition, weather conditions at the destination airport are also retrieved using an externally available JSON Service.  
