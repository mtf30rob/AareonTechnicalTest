using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AareonTechnicalTest.Models;
using AareonTechnicalTest.JsonConfiguration;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace AareonTechnicalTest.Tests.Controllers
{
    public class TicketControllerTest : ControllerTestBase
    {
        public TicketControllerTest(CustomWebApplicationFactory<AareonTechnicalTest.Startup> factory) : base(factory)
        { }

        [Fact]
        public async Task Get_ShouldReturnRows()
        {
            var response = await _client.GetAsync("Ticket");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.DeserialiseContentAsync<List<Ticket>>();

            data.Should().HaveCountGreaterThan(0, "the test DB should contain at least one ticket");
            data.Exists(d => d.Id == 1).Should().BeTrue("a ticket with id = 1 should exsit in the test DB");
        }

        [Fact]
        public async Task GetOne_ValidTicketId_ShouldReturnTicket()
        {
            var response = await _client.GetAsync("Ticket/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var data = await response.DeserialiseContentAsync<Ticket>();

            data.Should().NotBeNull("a ticket with id = 1 should exsit in the test DB");
            data.Id.Should().Be(1, "id 1 was requested from the API");
        }

        [Fact]
        public async Task GetOne_InvalidTicketId_ShouldReturnBadRequest()
        {
            var response = await _client.GetAsync("Ticket/0");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest, "0 is not a valid ticket id");
        }

        [Fact]
        public async Task GetOne_ValidTicketIdNotInDb_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync("Ticket/1000");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        public async Task Create_ValidTicketObject_ShouldReturnCreatedTicket()
        {
            var ticketToAdd = new Ticket() { Content = "Test ticket to add", PersonId = 1 };
            var content = new ObjectJsonContent(ticketToAdd);
            var response = await _client.PostAsync("Ticket", content);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var data = await response.DeserialiseContentAsync<Ticket>();

            data.Should().NotBeNull("as the API has indicated that the create request was successful");
            data.Id.Should().BeGreaterThan(0, "the created ticket should now have a valid db generated id");
            data.Content.Should().Be(ticketToAdd.Content);
            data.PersonId.Should().Be(ticketToAdd.PersonId);

            // Check the location header matches the returned id
            response.Headers.Location.Should().Be($"/Ticket/{data.Id}");
        }


        /*
         // This test does not work using a in memory DB as referencial constraints are not checked
        [Fact]
        public async Task Create_InvalidPersonId_ShouldReturnConflict()
        {
            var ticketToAdd = new Ticket() { Content = "Test ticket to add", PersonId = 1000 };
            var content = new ObjectJsonContent(ticketToAdd);
            var response = await _client.PostAsync("Ticket", content);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }*/



        [Fact]
        public async Task Update_ValidTicketObject_ShouldReturnUpdatedTicket()
        {
            // Must create ticket first so we can then updated it
            var ticketToAdd = new Ticket() { Content = "Test ticket to update", PersonId = 1 };
            var contentToAdd = new ObjectJsonContent(ticketToAdd);
            var addedResponse = await _client.PostAsync("Ticket", contentToAdd);

            addedResponse.StatusCode.Should().Be(HttpStatusCode.Created, "we need to create a ticekt before we can update it");

            var createdTicket = await addedResponse.DeserialiseContentAsync<Ticket>();

            createdTicket.Should().NotBeNull("as the API has indicated that the create request was successful");
            createdTicket.Id.Should().BeGreaterThan(0, "the created ticket should now have a valid db generated id");

            // Now perform the update
            var additionalContent = " - ticket updated!";
            createdTicket.Content = $"{createdTicket.Content}{additionalContent}";

            var contentToUpdate = new ObjectJsonContent(createdTicket);
            var updatedTicketResponse = await _client.PutAsync($"Ticket/{createdTicket.Id}", contentToUpdate);

            updatedTicketResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "as this indicates the update request was successful");

            var getUpdatedTicketRespose = await _client.GetAsync($"Ticket/{createdTicket.Id}");
            var updatedTicket = await getUpdatedTicketRespose.DeserialiseContentAsync<Ticket>();

            updatedTicket.Should().NotBeNull();
            updatedTicket.Content.Should().Be($"{ticketToAdd.Content}{additionalContent}");
        }

        [Fact]
        public async Task Update_InvalidTicketId_ShouldReturnBadRequest()
        {
            var ticket= new Ticket() { Content = "Test ticket to update", PersonId = 1 };
            var content= new ObjectJsonContent(ticket);
            var addedResponse = await _client.PutAsync("Ticket/0", content);

            addedResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "0 is an invalid ticket id");
        }

        [Fact]
        public async Task Update_ValidTicketIdNotInDB_ShouldReturnNotFound()
        {
            var ticket = new Ticket() { Content = "Test ticket to update", PersonId = 1 };
            var content = new ObjectJsonContent(ticket);
            var addedResponse = await _client.PutAsync("Ticket/1000", content);

            addedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }




        [Fact]
        public async Task Delete_ValidTicketId_ShouldReturnDeleteTicket()
        {
            // Must create ticket first so we can then delete it
            var ticketToAdd = new Ticket() { Content = "Test ticket to delete", PersonId = 1 };
            var contentToAdd = new ObjectJsonContent(ticketToAdd);
            var addedResponse = await _client.PostAsync("Ticket", contentToAdd);

            addedResponse.StatusCode.Should().Be(HttpStatusCode.Created, "we need to create a ticekt before we can delete it");

            var createdTicket = await addedResponse.DeserialiseContentAsync<Ticket>();

            createdTicket.Should().NotBeNull("as the API has indicated that the create request was successful");
            createdTicket.Id.Should().BeGreaterThan(0, "the created ticket should now have a valid db generated id");

            // Now perform the delete
            var deletedTicketResponse = await _client.DeleteAsync($"Ticket/{createdTicket.Id}");

            deletedTicketResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, "as this indicates the delete request was successful");

            // Check ticket is no longer there
            var getUpdatedTicketRespose = await _client.GetAsync($"Ticket/{createdTicket.Id}");
            getUpdatedTicketRespose.StatusCode.Should().Be(HttpStatusCode.NotFound, "the ticket should have be deleted");
        }

        [Fact]
        public async Task Delete_InvalidTicketId_ShouldReturnBadRequest()
        {
            var addedResponse = await _client.DeleteAsync("Ticket/0");

            addedResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "0 is an invalid ticket id");
        }

        [Fact]
        public async Task Delete_ValidTicketIdNotInDB_ShouldReturnNotFound()
        {
            var addedResponse = await _client.DeleteAsync("Ticket/1000");

            addedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

