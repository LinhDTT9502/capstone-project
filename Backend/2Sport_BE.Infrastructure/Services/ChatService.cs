using _2Sport_BE.Infrastructure.Hubs;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Services.Caching;
using Microsoft.CodeAnalysis.Semantics;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2Sport_BE.Service.Services
{
    public interface IChatService
    {
        Task SendMessageFromCustomerToCoordinator(int senderId, int receiverId,
                                                    string message, string imgUrl);
        Task SendMessageFromCoordinatorToCustomer(Guid chatSessionId,
                                                               int senderId, int receiverId,
                                                               string message, string imgUrl);
        Task SendMessageFromCoordinatorToManager(int senderId, int receiverId,
                                                               string message, string imgUrl);
        Task SendMessageFromManagerToCoordinator(Guid chatSessionId,
                                                       int senderId, int receiverId,
                                                       string message, string imgUrl);
        Task<ChatVM> GetMessagesOfCustomer(int customerId);
    }

    public class ChatService : IChatService
    {
        private readonly IChatHub _chatHub;
        private readonly IRedisCacheService _redisCacheService;
        private readonly string _chatSessionsKey;
        private readonly string _messagesKey;
        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IChatHub chatHub,
                          IRedisCacheService redisCacheService,
                          IConfiguration configuration,
                          IUnitOfWork unitOfWork)
        {
            _chatHub = chatHub;
            _redisCacheService = redisCacheService;
            _unitOfWork = unitOfWork;
            _chatSessionsKey = configuration.GetValue<string>("RedisKeys:ChatSessions");
            _messagesKey = configuration.GetValue<string>("RedisKeys:Messages");
        }

        public async Task SendMessageFromCustomerToCoordinator(int senderId, int receiverId, 
                                                    string message, string imgUrl)
        {
            try
            {
                var listChatSessions = _redisCacheService.GetData<List<ChatSession>>(_chatSessionsKey)
                                                                ?? new List<ChatSession>();
                var chatSessionOfCustomer = listChatSessions.Find(_ => _.CustomerId == senderId);


                var listMessages = _redisCacheService.GetData<List<Message>>(_messagesKey)
                                                                ?? new List<Message>();


                if (chatSessionOfCustomer is not null)
                {
                    var chatSessionId = chatSessionOfCustomer.Id;

                    var messageObject = new Message()
                    {
                        Id = Guid.NewGuid(),
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Content = message,
                        ImageUrl = imgUrl,
                        ChatSessionId = chatSessionId,
                    };
                    await _chatHub.SendMessageToSpecificUser(receiverId.ToString(), message);

                    listMessages.Add(messageObject);
                    _redisCacheService.SetData(_messagesKey, listMessages, TimeSpan.FromDays(30));
                } else
                {
                    var chatSessionId = Guid.NewGuid();
                    var chatSession = new ChatSession()
                    {
                        Id = chatSessionId,
                        ManagerId = null,
                        CustomerId = senderId,
                        CoordinatorId = receiverId,
                    };
                    listChatSessions.Add(chatSession);
                    _redisCacheService.SetData(_chatSessionsKey, listChatSessions, TimeSpan.FromDays(30));


                    var messageObject = new Message()
                    {
                        Id = Guid.NewGuid(),
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Content = message,
                        ImageUrl = imgUrl,
                        ChatSessionId = chatSessionId,
                    };
                    await _chatHub.SendMessageToSpecificUser(receiverId.ToString(), message);

                    listMessages.Add(messageObject);
                    _redisCacheService.SetData(_messagesKey, listMessages, TimeSpan.FromDays(30));
                }

            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendMessageFromCoordinatorToCustomer(Guid chatSessionId,
                                                               int senderId, int receiverId,
                                                               string message, string imgUrl)
        {
            try
            {
                var listMessages = _redisCacheService.GetData<List<Message>>(_messagesKey)
                                                                ?? new List<Message>();

                var messageObject = new Message()
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = message,
                    ImageUrl = imgUrl,
                    ChatSessionId = chatSessionId,
                };
                await _chatHub.SendMessageToSpecificUser(receiverId.ToString(), message);
                listMessages.Add(messageObject);
                _redisCacheService.SetData(_messagesKey, listMessages, TimeSpan.FromDays(30));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendMessageFromCoordinatorToManager(int senderId, int receiverId,
                                                               string message, string imgUrl)
        {
            try
            {
                var listChatSessions = _redisCacheService.GetData<List<ChatSession>>(_chatSessionsKey)
                                                                ?? new List<ChatSession>();
                var chatSessionOfCoordinator = listChatSessions.Find(_ => _.CoordinatorId == senderId);


                var listMessages = _redisCacheService.GetData<List<Message>>(_messagesKey)
                                                                ?? new List<Message>();

                if (chatSessionOfCoordinator is not null)
                {
                    var chatSessionId = chatSessionOfCoordinator.Id;

                    var messageObject = new Message()
                    {
                        Id = Guid.NewGuid(),
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Content = message,
                        ImageUrl = imgUrl,
                        ChatSessionId = chatSessionId,
                    };
                    await _chatHub.SendMessageToSpecificUser(receiverId.ToString(), message);

                    listMessages.Add(messageObject);
                    _redisCacheService.SetData(_messagesKey, listMessages, TimeSpan.FromDays(30));
                }
                else
                {
                    var chatSessionId = Guid.NewGuid();

                    var chatSession = new ChatSession()
                    {
                        Id = chatSessionId,
                        ManagerId = null,
                        CustomerId = senderId,
                        CoordinatorId = receiverId,
                    };
                    listChatSessions.Add(chatSession);
                    _redisCacheService.SetData(_chatSessionsKey, listChatSessions, TimeSpan.FromDays(30));

                    var messageObject = new Message()
                    {
                        Id = Guid.NewGuid(),
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        Content = message,
                        ImageUrl = imgUrl,
                        ChatSessionId = chatSessionId,
                    };
                    await _chatHub.SendMessageToSpecificUser(receiverId.ToString(), message);

                    listMessages.Add(messageObject);
                    _redisCacheService.SetData(_messagesKey, listMessages, TimeSpan.FromDays(30));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task SendMessageFromManagerToCoordinator(Guid chatSessionId,
                                                       int senderId, int receiverId,
                                                       string message, string imgUrl)
        {
            try
            {
                var listMessages = _redisCacheService.GetData<List<Message>>(_messagesKey)
                                                                ?? new List<Message>();

                var messageObject = new Message()
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = message,
                    ImageUrl = imgUrl,
                    ChatSessionId = chatSessionId,
                };
                await _chatHub.SendMessageToSpecificUser(receiverId.ToString(), message);
                listMessages.Add(messageObject);
                _redisCacheService.SetData(_messagesKey, listMessages, TimeSpan.FromDays(30));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<ChatVM> GetMessagesOfCustomer(int customerId)
        {
            try
            {
                var listChatSessions = _redisCacheService.GetData<List<ChatSession>>(_chatSessionsKey)
                                                                                ?? new List<ChatSession>();
                var chatSessionOfCustomer = listChatSessions.Find(_ => _.CustomerId == customerId);

                if (chatSessionOfCustomer is null)
                {
                    return null;
                }

                var listMessages = _redisCacheService.GetData<List<Message>>(_messagesKey)
                                                ?? new List<Message>();
                var listMessagesOfCustomer = listMessages.Where(_ => _.ChatSessionId.Equals(chatSessionOfCustomer.Id))
                                                    .ToList();

                var messagesVMs = new List<MessageVM>();
                foreach (var message in listMessagesOfCustomer)
                {
                    var messageVM = new MessageVM()
                    {
                        MessageId = message.Id,
                        SenderId = message.SenderId,
                        SenderName = (await _unitOfWork.UserRepository.FindAsync(message.SenderId)).FullName,
                        ReceiverId = message.ReceiverId,
                        ReceiverName = (await _unitOfWork.UserRepository.FindAsync(message.ReceiverId)).FullName,
                        Content = message.Content,
                        ImageUrl = message.ImageUrl,
                        Timestamp = message.Timestamp
                    };
                    messagesVMs.Add(messageVM);
                }

                var chatVM = new ChatVM()
                {
                    ChatSessionId = chatSessionOfCustomer.Id,
                    CustomerId = customerId,
                    CustomerName = (await _unitOfWork.UserRepository.FindAsync(customerId)).FullName,
                    CoordinatorId = chatSessionOfCustomer.CoordinatorId,
                    CoordinatorName = (await _unitOfWork.UserRepository
                                                .FindAsync(chatSessionOfCustomer.CoordinatorId)).FullName,
                    MessageVMs = messagesVMs,
                };
                return chatVM;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }

    public class ChatSession()
    {
        public Guid Id { get; set; }
        public int? CustomerId { get; set; }
        public int CoordinatorId { get; set; }
        public int? ManagerId { get; set; }
    }

    public class Message()
    {
        public Guid Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public Guid ChatSessionId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
