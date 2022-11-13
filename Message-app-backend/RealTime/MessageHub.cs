using Message_app_backend.Entities;
using Message_app_backend.Repository;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using Message_app_backend.RealTime.Dto;
using StackExchange.Redis;
using Message_app_backend.Dto.Chat;

namespace Message_app_backend.RealTime
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MessageHub : Hub
    {

        MessageRepository messageRepository;
        ReactionRepository reactionRepository;
        UserInfoRepository userInfoRepository;
        GroupMemberRepository groupMemberRepository;
        GroupChatRepository groupChatRepository;
        RoomMemberRepository roomMemberRepository;
        RoomRepository roomRepository;
        IConnectionMultiplexer redis;
        public MessageHub(
            ReactionRepository reactionRepository,
            MessageRepository messageRepository,
            UserInfoRepository userInfoRepository,
            GroupMemberRepository groupMemberRepository,
            GroupChatRepository groupChatRepository,
            RoomMemberRepository roomMemberRepository,
            RoomRepository roomRepository,
            IConnectionMultiplexer redis)
        {
            this.messageRepository = messageRepository;
            this.reactionRepository = reactionRepository;
            this.userInfoRepository = userInfoRepository;
            this.groupMemberRepository = groupMemberRepository;
            this.groupChatRepository = groupChatRepository;
            this.roomMemberRepository = roomMemberRepository;
            this.roomRepository = roomRepository;
            this.redis = redis;
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("on connect");
            base.OnConnectedAsync();
            var userId = GetUserId();

            var db = redis.GetDatabase();
            var key1 = "disconnectTime_" + userId.ToString();
            var key2 = "connectionId_" + userId.ToString();
            db.StringGetDelete(key1);
            db.StringSet(key2, Context.ConnectionId);

            return Task.CompletedTask;
        }


        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //base.OnDisconnectedAsync(exception);
            //Console.WriteLine("disconnect" + Context.ConnectionId);
            var userId = GetUserId();
            var db = redis.GetDatabase();
            var key1 = "disconnectTime_" + userId.ToString();
            var disconnectTime = DateTime.UtcNow.ToString();
            db.StringSet(key1, disconnectTime);

            return base.OnDisconnectedAsync(exception); ;
        }

        public async Task SendMessage(string messageContent, int roomId, int userReceiveId, RoomTypeEnum roomType, string messageType, string time)
        {
            //save message
            var userId = GetUserId();
            var userInfo = userInfoRepository.FindById(userId);
            var  message = await SaveMessage(messageContent, roomId, userReceiveId, messageType, time);
            var receiveMessageDto = new MessageDto();
            //send message
            if (roomType == RoomTypeEnum.Personal)
            {
                var connectionId = GetConnectionIdByUserId(userReceiveId);
                receiveMessageDto = new MessageDto
                {
                    MessageId = message.MessageId,
                    MessageContent = message.MessageContent,
                    UserId = message.UserId,
                    RoomId = message.RoomId,
                    Recall = message.Recall,
                    MessageType = message.MessageType,
                    SendTime = message.SendTime.ToString(),
                };

                if (connectionId != "")
                {
                    await Clients.Clients(connectionId).SendAsync("ReceiveMessage", receiveMessageDto);
                }
            }
            else
            {
                var groupChat = groupChatRepository.FindByRoomId(roomId);
                var groupMembers = groupMemberRepository.FindByGroupId(groupChat.GroupChatId);
                var userIds = groupMembers.Select(e => e.UserId).ToList();
                var connectionIds = GetConnectionIdsByUserIds(userIds);

                receiveMessageDto = new MessageDto
                {
                    MessageContent = message.MessageContent,
                    UserId = message.UserId,
                    RoomId = message.RoomId,
                    Recall = message.Recall,
                    MessageType = message.MessageType,
                    SendTime = message.SendTime.ToString(),
                };

                if (connectionIds.Count > 0)
                {
                    await Clients.Clients(connectionIds).SendAsync("ReceiveMessage", receiveMessageDto);
                }
            }

            // send response
            await Clients.Clients(Context.ConnectionId).SendAsync("SendMessageResponse", new { data = receiveMessageDto, status = "success" });
        }

        private async Task<Message> SaveMessage(string messageContent, int roomId, int userReceiveId, string messageType, string time)
        {
            using (var transaction = roomRepository.Database().BeginTransaction())
            {
                try
                {
                    Room room = null;
                    if (roomId == 0)
                    {
                        //Create room
                        var newRoom = new Room { RoomType = RoomTypeEnum.Personal };
                        room = roomRepository.Create(newRoom);

                        var roomMembers = new List<RoomMember>
                        {
                            new RoomMember { UserId = userReceiveId, RoomId  = newRoom.RoomId },
                            new RoomMember { UserId = GetUserId(), RoomId  = newRoom.RoomId }
                        };

                        roomMembers.AddRange(roomMembers);
                        roomMemberRepository.CreateRange(roomMembers);
                    }
                    else
                    {
                        room = roomRepository.FindById(roomId);
                    }

                    DateTime sendTime = DateTime.Parse(time);

                    Message message = new Message
                    {
                        RoomId = room.RoomId,
                        UserId = GetUserId(),
                        MessageContent = messageContent,
                        MessageType = (MessageTypeEnum)Enum.Parse(typeof(MessageTypeEnum), messageType),
                        SendTime = sendTime
                    };

                    messageRepository.Create(message);

                    transaction.Commit();

                    return message;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return null;
                }
            }

        }

        //private int GetUserId()
        //{
        //    var userId = ClaimsPrincipal.Current?.Claims.Where(claim => claim?.Subject?.Name == "UserId").FirstOrDefault()?.Value;
        //    return int.Parse(userId);
        //}

        private int GetUserId()
        {
            return int.Parse(Context.User.Claims.First(claim => claim.Type == "UserId").Value);
        }

        private string GetConnectionIdByUserId(int userId)
        {
            var db = redis.GetDatabase();
            var key = "connectionId_" + userId.ToString();
            var value = db.StringGet(key);

            if (value.HasValue)
            {
                return value;
            }
            return "";
        }

        private List<string> GetConnectionIdsByUserIds(List<int> userIds)
        {
            List<string> connectionIds = new List<string>();
            var db = redis.GetDatabase();

            foreach (var userId in userIds)
            {
                var key = "connectionId_" + userId.ToString();
                var value = db.StringGet(key);

                if (value.HasValue)
                {
                    connectionIds.Add(value);
                }
            }

            return connectionIds;
        }
    }
}
