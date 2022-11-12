using AutoMapper;
using Message_app_backend.Dto.Auth;
using Message_app_backend.Dto.Contacs;
using Message_app_backend.Entities;
using Message_app_backend.Repository;
using Message_app_backend.Service;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Net;

namespace Message_app_backend.Controllers
{
    [ApiController]
    public class ContactsController : ControllerBase
    {
        ContactsService contactsService;
        IMapper mapper;
        public ContactsController(ContactsService contactsService, IMapper mapper)
        {
            this.mapper = mapper;
            this.contactsService = contactsService;
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/GetFriends")]
        public MessageResponse<List<ContactsOfFriendsDto>> GetFriends()
        {
            try
            {
                var userId = GetUserId();
                var data = contactsService.GetFriends(userId);

                return new MessageResponse<List<ContactsOfFriendsDto>> { Code = HttpStatusCode.OK, Message = "", Data = data };
            }
            catch (Exception ex)
            {
                return new MessageResponse<List<ContactsOfFriendsDto>> { Code = HttpStatusCode.NotFound, Message = "Error:" + ex.Message };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/FindContactsInfo")]
        public MessageResponse<AddFirendContactsInfoDto> FindContactsInfo(string phoneNumber)
        {
            try
            {
                var user = contactsService.FindContacts(phoneNumber);

                if (user == null)
                {
                    throw new Exception("Not found conacts info");
                }

                var data = mapper.Map<AddFirendContactsInfoDto>(user);

                return new MessageResponse<AddFirendContactsInfoDto> { Code = HttpStatusCode.OK, Message = "", Data = data };
            }
            catch (Exception ex)
            {
                return new MessageResponse<AddFirendContactsInfoDto> { Code = HttpStatusCode.NotFound, Message = "Error:" + ex.Message };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/[controller]/GetMyQR")]
        public MessageResponse<object> GetMyQR()
        {
            try
            {
                var data = User.Claims.ToList().Where(c => c.Type == "PhoneNumber").FirstOrDefault()?.Value;

                return new MessageResponse<object> { Code = HttpStatusCode.OK, Message = "", Data = new { phoneNumber = data } };
            }
            catch (Exception ex)
            {
                return new MessageResponse<object> { Code = HttpStatusCode.NotFound, Message = "Error:" + ex.Message };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/SendFriendRequest")]
        public MessageResponse<bool> SendFriendRequest(SendFriendRequestDto friendRequestDto)
        {
            try
            {
                friendRequestDto.UserSendId = GetUserId();
                contactsService.SendFriendRequest(friendRequestDto);
                return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "Send friend request success", Data = true };
            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/ApproveFriendRequest")]
        public MessageResponse<bool> ApproveFriendRequest(int friendRequestId)
        {
            try
            {
                contactsService.ApproveFriendRequest(friendRequestId);

                return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "Add friend success", Data = true };
            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/RemoveFriend")]
        public MessageResponse<bool> RemoveFriend(int friendId)
        {
            try
            {
                var userId = GetUserId();
                contactsService.RemoveFriend(userId, friendId);

                return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "Add friend success", Data = true };
            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/GetFriendRequest")]
        public MessageResponse<List<ReceiveFriendRequestDto>> GetFriendRequest()
        {
            try
            {
                var userId = GetUserId();
                var data = contactsService.GetFriendRequest(userId);

                return new MessageResponse<List<ReceiveFriendRequestDto>> { Code = HttpStatusCode.OK, Message = "", Data = data };
            }
            catch (Exception ex)
            {
                return new MessageResponse<List<ReceiveFriendRequestDto>> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        [HttpPost]
        [Authorize]
        [Route("[controller]/CreateGroupChat")]
        public MessageResponse<bool> CreateGroupChat(CreateGroupChatDto createGroupChatDto)
        {
            try
            {
                var userId = GetUserId();
                contactsService.CreateGroupChat(userId, createGroupChatDto.Name, createGroupChatDto.MemberIds);

                return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "", Data = true };
            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        [HttpGet]
        [Authorize]
        [Route("[controller]/ApproveInviteJoinGroupChat")]
        public MessageResponse<bool> ApproveInviteJoinGroupChat(int inviteId)
        {
            try
            {
                var userId = GetUserId();
                contactsService.ApproveInviteJoinGroupChat(userId, inviteId);

                return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "", Data = true };
            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        // pending
        [HttpGet]
        [Authorize]
        [Route("[controller]/GetInviteJoinGroupChat")]
        public MessageResponse<bool> GetInviteJoinGroupChat(int inviteId)
        {
            try
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "", Data = true };
            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        private int GetUserId()
        {
            return int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "UserId").Value);
        }

    }
}
