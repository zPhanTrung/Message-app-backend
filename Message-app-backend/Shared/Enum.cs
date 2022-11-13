using System.ComponentModel;

namespace Message_app_backend.Shared
{
    public enum GenderEnum
    {
        [Description("Male")]
        Male = 0,
        [Description("Female")]
        Female = 1
    }

    public enum ActiveStatusEnum
    {
        [Description("Active")]
        Active = 0,
        [Description("Inactive")]
        Inactive = 1,
    }

    public enum ConnectStausEnum
    {
        [Description("Connect")]
        Connect = 1,
        [Description("Disconnect")]
        Disconnect = 0,
    }

    public enum MessageTypeEnum
    {
        [Description("Message")]
        Message = 0,
        [Description("Call")]
        Call = 1,
    }

    public enum JoinGroupChatTypeEnum
    {
        [Description("Invited")]
        Invited = 0,
        [Description("Request")]
        Request = 1,
    }

    public enum RoomTypeEnum
    {
        [Description("Personal")]
        Personal = 0,
        [Description("GroupChat")]
        GroupChat = 1,
    }
}
