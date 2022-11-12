namespace Message_app_backend.Dto.Contacs
{
    public class CreateGroupChatDto
    {
        public string Name { get; set; }
        public List<int> MemberIds { get; set; }
    }
}
