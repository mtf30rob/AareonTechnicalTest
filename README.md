
# Requirements

<span style="color:darkgreen"><b>Note</b>: I have added my comments after each requirement below and a developer notes section underneath.</span>
### Aareon Technical Test - Ticketing System

We need you to extend the current ticketing system that will allow additional resources to be included.

1. Implement RESTful endpoint(s) that will allow all Create, Read, Update, and Delete (CRUD) actions on a Ticket. <span style="color:darkgreen">API controller for ticket CRUN operations added.</span>
2. Now amend the solution to allow the addition of Notes to a Ticket. <span style="color:darkgreen">'TicketNotes' database table and API controller for ticket note CRUN operations added.</span>
3. Amend the solution to track any data manipulation and actions. Auditing could happen out of process. <span style="color:darkgreen">The scope for this point is quite broad so I opted for automatically auditing database table changes in an audit table and also logging controller action calls (only implemented for the tickets controller). Ideally this would include the authenticated users information but authentication was not specified in the requirements so I have not implemented it. Additionally logging may not be the most suitable solution for auditing controller actions depending on the business requirements. If logging was to be used I would link it with a structured logging provider like Azure application insights or Seq.</span>
4. Create a Pull Request on github.

Requirements:
- A note is created by a Person to log additional information against a ticket.
- A note can be created, updated, or removed by anyone, but only an Administrator may delete an existing note. <span style="color:darkgreen">As no authentication was specified in the requirements I crudely implemented this using the isAdmin column on the Person table.</span>
- Any actions that are taken against records in the ticketing system are subject to monitoring and auditing.
- This application will be deployed automatically using a CI/CD pipeline. <span style="color:darkgreen">I have added a devops pipeline yaml file to show the start of the [devops pipeline](../AareonApiTechTest/AareonTechnicalTest/Pipelines/devops-pipeline.yaml) implementation.</span>

These tasks should take no longer than 4 hours


# Developer Notes

### Real world application development

In a real application I would typically:
- Have the API secured against an identity service provider like Identity Server or Azure AD and ensure all controllers require authentication
- Add middleware to log and translate exceptions so 500 responses are not returned to the consumer of the API
- Inject a repository or service class into the controllers rather than the DB context directly
- Convert the EF Entity class to new POCOs / DTOs specifically for the contract of the API (so the API requests and responses can be changed independently of each other)
- Document the API in the Swagger UI giving descriptions for actions and examples for parameters
- Add tests for all controllers and specific tests for complex or critical application logic. I have only implemented tests for the tickets controller in this solution.


### EF Core commands

The following ef commands were used to update the db:
`dotnet ef database update --project AareonTechnicalTest/AareonTechnicalTest.csproj`
`dotnet ef migrations add MigrationName  --project AareonTechnicalTest/AareonTechnicalTest.csproj`