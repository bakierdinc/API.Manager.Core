# API.Manager.Core
### API.Manager.Core for manage availability of REST services.


## Implementation

**Firstly you should add ApiManagerOptions to configuration file.**

**Example**<br>
![options](https://user-images.githubusercontent.com/55300546/137203490-1257c39a-7cd3-4b61-a632-0dc478b0d73f.PNG)

**ApiManagerOptions fields meaning;**

- Schema : Schema name of the tables to be created your database. Default value will be "ApiManager" when Schema field was null.
- CreateTableIfNeccassary: When the CreateTablesIfNeccassary value is true, the application creates a table in the database if available, otherwise application will not create table in the database. Default value will be true when CreateTablesIfNeccassary field was null.
- HeaderKey : Requests must have a header value to be distinguishable. Default value will be "Channel" when HeaderKey field was null.
- Channels : Allows to manage the availability of services for multiple platforms. Default value will be "Default" when Channels field was null.
- IsServiceable : Specifies the default availability of services. Default value will be true when IsServiceable field was null.
- NotAcceptableMessage : If services are not available, 406 status code is returned and indicates the content message.


**Secondly you should add to Service Collection**

**Example**<br>
![ServiceCollection](https://user-images.githubusercontent.com/55300546/136687172-386f7e31-6443-4ad5-8609-a9164832cea9.PNG)


**Finally you should use OnlyServiceable attribute your controllers**

**Example**<br>
![attribute](https://user-images.githubusercontent.com/55300546/136688278-acbe01e5-ea5a-41d3-b4df-9e8b38476945.PNG)

## API.Manager.Core Services ##

![API.ManagerServices](https://user-images.githubusercontent.com/55300546/136690340-1571593f-ab31-49f2-afe9-9f2f996b4355.PNG)

