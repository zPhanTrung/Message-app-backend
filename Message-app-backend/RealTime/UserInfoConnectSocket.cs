namespace Message_app_backend.RealTime
{
    public class UserInfoConnectSocket
    {
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
    }

    public static class MangeUserConnectSocket
    {
        public static List<UserInfoConnectSocket> userInfoConnectSockets { get; set; } = new List<UserInfoConnectSocket>();

        public static string GetConnectionId(int userId)
        {
            var user = userInfoConnectSockets.FirstOrDefault(user => user.UserId == userId);
            if( user != null)
            {
                return user.ConnectionId;
            }

            return "";
        }

        public static UserInfoConnectSocket? GetByConnectionId(string connnectionId)
        {
            return userInfoConnectSockets.Where(e => e.ConnectionId == connnectionId).FirstOrDefault();
        }

        public static void UpdateConnectionId(string connectionIdOld, string connectionIdNew)
        {
            userInfoConnectSockets.Where(e => e.ConnectionId.Equals(connectionIdOld)).First().ConnectionId = connectionIdNew;
        }

        public static List<string> GetConnectionIds(List<int> userIds)
        {
            var result = new List<string>();
            foreach(var userId in userIds)
            {
                var userInfoConnectSocket = userInfoConnectSockets.Where(e => e.UserId == userId).FirstOrDefault();
                if(userInfoConnectSocket!=null && userInfoConnectSocket.ConnectionId!="")
                {
                    result.Add(userInfoConnectSocket.ConnectionId);
                }
            }

            return result;
        }
    }
}
