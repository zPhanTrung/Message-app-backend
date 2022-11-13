using AutoMapper;
using Message_app_backend.Dto.Contacs;
using Message_app_backend.Entities;
using Message_app_backend.Repository;
using Message_app_backend.Shared;
using StackExchange.Redis;
using static StackExchange.Redis.Role;

namespace Message_app_backend.Service
{
    public class ContactsService : BaseService
    {
        UserInfoRepository userInfoRepository;
        FriendRepository friendRepository;
        FriendRequestRepository friendRequestRepository;
        GroupChatRepository groupChatRepository;
        GroupMemberRepository groupMemberRepository;
        InviteAndRequestJoinGroupChatRepository inviteAndRequestJoinGroupChatRepository;
        RoomMemberRepository roomMemberRepository;
        IConnectionMultiplexer redis;
        RoomRepository roomRepository;
        IMapper mapper;
        public ContactsService(
            UserInfoRepository userInfoRepository,
            FriendRepository friendRepository,
            IMapper mapper,
            FriendRequestRepository friendRequestRepository,
            RoomRepository roomRepository,
            RoomMemberRepository roomMemberRepository,
            IConnectionMultiplexer redis)
        {
            this.userInfoRepository = userInfoRepository;
            this.friendRepository = friendRepository;
            this.friendRequestRepository = friendRequestRepository;
            this.roomRepository = roomRepository;
            this.roomMemberRepository = roomMemberRepository;
            this.redis = redis;
            this.mapper = mapper;
        }
        public List<ContactsOfFriendsDto> GetFriends(int userId)
        {
            var friends = friendRepository.GetFriends(userId);
            var contactDtoList = new List<ContactsOfFriendsDto>();

            if (friends.Count > 0)
            {
                var ids = friends.Select(friend => friend.FriendId).ToList();
                List<UserInfo> users = userInfoRepository.FindUserInfoByIds(ids);

                foreach (var user in users)
                {
                    var roomId = roomMemberRepository.FindRoomIdByUserId(user.UserId, userId);

                    var contactsDto = new ContactsOfFriendsDto()
                    {
                        UserId = user.UserId,
                        DisplayName = user.DisplayName,
                        Avatar = user.Avatar,
                        RoomId = roomId,
                        UserCurrentId = userId
                    };

                    var db = redis.GetDatabase();
                    var key = "disconnectTime_" + user.UserId.ToString();
                    var key2 = "connectionId_" + user.UserId.ToString();
                    var value = db.StringGet(key);
                    var connectionId = db.StringGet(key2);

                    if (connectionId.HasValue)
                    {
                        contactsDto.ConnectStatus = ConnectStausEnum.Connect;
                    }
                    else
                    {
                        contactsDto.ConnectStatus = ConnectStausEnum.Disconnect;
                    }

                    if (value.HasValue)
                    {
                        contactsDto.DisconnectTime = value;
                    }
                    else
                    {
                        contactsDto.DisconnectTime = "";
                    }

                    contactDtoList.Add(contactsDto);
                }
            }

            return contactDtoList;
        }

        public UserInfo? FindContacts(string phoneNumber)
        {
            try
            {
                return userInfoRepository.FindByPhoneNumber(phoneNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendFriendRequest(SendFriendRequestDto friendRequestDto)
        {
            using (var transaction = friendRepository.Database().BeginTransaction())
            {
                try
                {
                    var friend = friendRepository.CheckExistFriend(friendRequestDto);

                    if (friend != null)
                    {
                        throw new Exception("Have been friends");
                    }

                    if (friendRequestRepository.CheckExistOfRequest(friendRequestDto) == null)
                    {
                        var friendRequest = new FriendRequest()
                        {
                            UserSendId = friendRequestDto.UserSendId,
                            UserReceiveId = friendRequestDto.UserReceiveId,
                            Message = friendRequestDto.Message
                        };
                        friendRequestRepository.Create(friendRequest);

                        var user = userInfoRepository.FindById((int)friendRequestDto.UserReceiveId);
                        user.FriendRequest += 1;
                        userInfoRepository.Update(user);

                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception("Request existed");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public void ApproveFriendRequest(int friendRequestId)
        {
            using (var transaction = friendRepository.Database().BeginTransaction())
            {
                try
                {
                    var request = friendRequestRepository.FindById(friendRequestId);

                    if (request != null)
                    {
                        var friend1 = new Friend() { UserId = request.UserReceiveId, FriendId = request.UserSendId };
                        var friend2 = new Friend() { UserId = request.UserSendId, FriendId = request.UserReceiveId };
                        var friends = new List<Friend>();

                        friends.Add(friend1);
                        friends.Add(friend2);

                        friendRequestRepository.Delete(request);
                        friendRepository.CreateRange(friends);

                        var user = userInfoRepository.FindById((int)request.UserReceiveId);
                        user.FriendRequest -= 1;
                        userInfoRepository.Update(user);

                        transaction.Commit();
                    }
                    else
                    {
                        throw new Exception("Request not exist");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

        }

        public void RemoveFriend(int userId, int friendId)
        {
            try
            {
                var friend1 = friendRepository.FindByFriendId(userId, friendId);
                var friend2 = friendRepository.FindByFriendId(friendId, userId);

                if (friend1 != null && friend2 != null)
                {
                    var friends = new List<Friend>();
                    friends.Add(friend1);
                    friends.Add(friend2);
                    friendRepository.DeleteRange(friends);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ReceiveFriendRequestDto> GetFriendRequest(int userId)
        {
            var user = userInfoRepository.FindById(userId);

            if (user.FriendRequest > 0)
            {
                var requests = friendRequestRepository.FindAllRequestByUserReceiveId(userId);
                var friendsRequestDtos = new List<ReceiveFriendRequestDto>();
                if (requests.Any())
                {
                    foreach (var request in requests)
                    {
                        var userSend = userInfoRepository.FindById((int)request.UserSendId);
                        friendsRequestDtos.Add(new ReceiveFriendRequestDto
                        {
                            FriendRequestId = request.FriendRequestId,
                            Message = request.Message,
                            UserSendId = userSend.UserId,
                            DisplayName = userSend.DisplayName,
                            Avatar = userSend.Avatar
                        });
                    }
                }
                return friendsRequestDtos;
            }

            return new List<ReceiveFriendRequestDto>();
        }

        public void CreateGroupChat(int masterId, string name, List<int> memberIds)
        {
            using (var transaction = groupChatRepository.Database().BeginTransaction())
            {
                try
                {
                    var newGroup = new GroupChat() { Name = name, MasterId = masterId };
                    groupChatRepository.Create(newGroup);
                    var members = new List<GroupMember>();
                    members.Add(new GroupMember { UserId = masterId, GroupChatId = newGroup.GroupChatId });

                    var invites = new List<InviteAndRequestJoinGroupChat>();
                    var usersUpdate = new List<UserInfo>();

                    foreach (var userId in memberIds)
                    {
                        var friend = friendRepository.FindByFriendId(masterId, userId);
                        if (friend != null)
                        {
                            members.Add(new GroupMember { UserId = userId, GroupChatId = newGroup.GroupChatId });
                            newGroup.QuanityMember += 1;
                        }
                        else
                        {
                            var user = userInfoRepository.FindById(userId);
                            user.InviteJoinGroupChat += 1;
                            usersUpdate.Add(user);

                            invites.Add(new InviteAndRequestJoinGroupChat
                            {
                                GroupChatId = newGroup.GroupChatId,
                                UserSendId = masterId,
                                UserReceiveId = userId,
                                Type = JoinGroupChatTypeEnum.Invited
                            });
                        }
                    }

                    groupMemberRepository.CreateRange(members);

                    if (invites.Count > 0)
                    {
                        inviteAndRequestJoinGroupChatRepository.CreateRange(invites);
                        userInfoRepository.UpdateRange(usersUpdate);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public void InviteJoinGroupChat(int userSendId, int userReceiveId, int groupChatId)
        {
            using (var transaction = userInfoRepository.Database().BeginTransaction())
            {
                try
                {
                    var user = userInfoRepository.FindById(userReceiveId);
                    user.InviteJoinGroupChat += 1;

                    var invite = new InviteAndRequestJoinGroupChat
                    {
                        GroupChatId = groupChatId,
                        UserSendId = userSendId,
                        UserReceiveId = userReceiveId,
                        Type = JoinGroupChatTypeEnum.Invited
                    };

                    inviteAndRequestJoinGroupChatRepository.Create(invite);
                    userInfoRepository.Update(user);

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void ApproveInviteJoinGroupChat(int userId, int inviteId)
        {
            using (var transaction = userInfoRepository.Database().BeginTransaction())
            {
                try
                {
                    var invite = inviteAndRequestJoinGroupChatRepository.FindById(inviteId);
                    var user = userInfoRepository.FindById(userId);
                    user.InviteJoinGroupChat -= 1;
                    var groupChat = groupChatRepository.FindById((int)invite.GroupChatId);
                    groupChat.QuanityMember += 1;

                    inviteAndRequestJoinGroupChatRepository.Delete(invite);
                    groupMemberRepository.Create(new GroupMember { GroupChatId = (int)invite.GroupChatId, GroupMemberId = invite.UserReceiveId });
                    userInfoRepository.Update(user);
                    groupChatRepository.Update(groupChat);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
