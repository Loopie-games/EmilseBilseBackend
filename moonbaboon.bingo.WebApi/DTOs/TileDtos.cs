namespace moonbaboon.bingo.WebApi.DTOs
{
    public class CreateResponse
    {
        public string Id;
        public string AddedByUsername;
        public string Action;
        public string AboutUsername;

        public CreateResponse(string addedByUsername, string action, string aboutUsername, string id)
        {
            Action = action;
            AboutUsername = aboutUsername;
            Id = id;
            AddedByUsername = addedByUsername;
        }
    }
}