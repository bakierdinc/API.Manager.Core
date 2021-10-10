# API.Manager
### Api Manager for manage availability of REST services.


## Implementation

Firstly you should add ApiManagerOptions to configuration file.

Example
![options](https://user-images.githubusercontent.com/55300546/136687127-33800fc4-1313-4907-9501-36c0e7a8b5d0.PNG)

**ApiManagerOptions fields meaning;**

- Schema : Schema name of the tables to be created your database.
- CreateTablesIfNeccassary : If the CreateTablesIfNeccassary value is true, the application creates a table in the database while standing up. If the CreateTablesIfNeccassary -value is false, the application will not create table in the database. 
- HeaderKey : Requests must have a header value to be distinguishable.
- Channels : Allows to manage the availability of services for multiple platforms.
- IsServiceable : Specifies the default availability of services.
- NotAcceptableMessage : If services are not available, 406 status code is returned and indicates the content message.


Secondly you should add to Service Collection

Example
![ServiceCollection](https://user-images.githubusercontent.com/55300546/136687172-386f7e31-6443-4ad5-8609-a9164832cea9.PNG)

Done...
