using Microsoft.AspNetCore.SignalR.Client;
using ReenbitTest.UI.Interfaces;
using ReenbitTest.UI.Models;

namespace ReenbitTest.UI.Services
{
    public class ChatHubService : IChatHubService
    {
        private readonly string _hubUrl;
        private HubConnection? _hubConnection;
        private Timer? _heartbeatTimer;
        private readonly ILogger<ChatHubService> _logger;

        public event Action<Message> OnReceiveMessage = _ => { };
        public event Action<List<Message>> OnLoadRecentMessages = _ => { };
        public event Action<UserTypingInfo> OnUserTyping = _ => { };
        public event Action<UserTypingInfo> OnUserStoppedTyping = _ => { };
        public event Action<int, string, string> OnUserJoined = (_, _, _) => { };
        public event Action<int, string, string> OnUserLeft = (_, _, _) => { };
        public event Action<string> OnError = _ => { };
        public event Action<int> OnJoinedChatRoom = _ => { };
        public event Action<int> OnLeftChatRoom = _ => { };

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public ChatHubService(IConfiguration configuration, ILogger<ChatHubService> logger)
        {
            _hubUrl = configuration["ApiEndpoints:ChatHub"]!;
            _logger = logger;
        }

        public async Task StartConnectionAsync(string token)
        {
            if (_hubConnection != null)
            {
                return;
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options => 
                {
                    options.AccessTokenProvider = () => Task.FromResult(token)!;
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            RegisterHandlers();

            try
            {
                await _hubConnection.StartAsync();
                _logger.LogInformation("Hub connection started");
                
                // Start heartbeat timer
                _heartbeatTimer = new Timer(async _ => await SendHeartbeatAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting hub connection: {ex.Message}");
                throw;
            }
        }

        public async Task StopConnectionAsync()
        {
            if (_hubConnection != null)
            {
                // Stop heartbeat
                _heartbeatTimer?.Dispose();
                _heartbeatTimer = null;
                
                try
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                    _logger.LogInformation("Hub connection stopped");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error stopping hub connection: {ex.Message}");
                }
                finally
                {
                    // Ensure connection is null even if an exception occurred
                    _hubConnection = null;
                }
            }
        }

        public void ResetConnectionState()
        {
            _hubConnection = null;
            _heartbeatTimer?.Dispose();
            _heartbeatTimer = null;
            _logger.LogInformation("Connection state has been reset");
        }

        private void RegisterHandlers()
        {
            if (_hubConnection == null)
                return;

            _hubConnection.On<Message>("ReceiveMessage", message => 
            {
                _logger.LogInformation($"Message received from {message.SenderUserName}");
                OnReceiveMessage.Invoke(message);
            });

            _hubConnection.On<List<Message>>("LoadRecentMessages", messages => 
            {
                _logger.LogInformation($"Loaded {messages.Count} recent messages");
                OnLoadRecentMessages.Invoke(messages);
            });

            _hubConnection.On<UserTypingInfo>("UserTyping", typingInfo => 
            {
                _logger.LogInformation($"User typing: {typingInfo.UserName}");
                OnUserTyping.Invoke(typingInfo);
            });

            _hubConnection.On<UserTypingInfo>("UserStoppedTyping", typingInfo => 
            {
                _logger.LogInformation($"User stopped typing: {typingInfo.UserName}");
                OnUserStoppedTyping.Invoke(typingInfo);
            });

            _hubConnection.On<dynamic>("UserStoppedTyping", info => 
            {
                var typingInfo = new UserTypingInfo
                {
                    UserId = info.userId,
                    UserName = info.userName,
                    ChatRoomId = info.chatRoomId
                };
                OnUserStoppedTyping.Invoke(typingInfo);
            });

            _hubConnection.On<dynamic>("UserJoined", info => 
            {
                OnUserJoined.Invoke(
                    (int)info.chatRoomId,
                    (string)info.userId,
                    (string)info.userName
                );
            });

            _hubConnection.On<dynamic>("UserLeft", info => 
            {
                OnUserLeft.Invoke(
                    (int)info.chatRoomId,
                    (string)info.userId,
                    (string)info.userName
                );
            });

            _hubConnection.On<string>("Error", error => 
            {
                _logger.LogError($"Hub error: {error}");
                OnError.Invoke(error);
            });

            _hubConnection.On<int>("JoinedChatRoom", chatRoomId => 
            {
                _logger.LogInformation($"Joined chat room {chatRoomId}");
                OnJoinedChatRoom.Invoke(chatRoomId);
            });

            _hubConnection.On<int>("LeftChatRoom", chatRoomId => 
            {
                _logger.LogInformation($"Left chat room {chatRoomId}");
                OnLeftChatRoom.Invoke(chatRoomId);
            });

            _hubConnection.On<DateTime>("HeartbeatResponse", _ => 
            {
                _logger.LogDebug("Received heartbeat response");
            });
        }

        public async Task JoinChatRoomAsync(int chatRoomId)
        {
            if (_hubConnection == null)
                throw new InvalidOperationException("Hub connection is not initialized");

            try
            {
                await _hubConnection.InvokeAsync("JoinChatRoom", chatRoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error joining chat room: {ex.Message}");
                throw;
            }
        }

        public async Task LeaveChatRoomAsync(int chatRoomId)
        {
            if (_hubConnection == null)
                throw new InvalidOperationException("Hub connection is not initialized");

            try
            {
                await _hubConnection.InvokeAsync("LeaveChatRoom", chatRoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error leaving chat room: {ex.Message}");
                throw;
            }
        }

        public async Task SendMessageAsync(CreateMessage message)
        {
            if (_hubConnection == null)
                throw new InvalidOperationException("Hub connection is not initialized");

            try
            {
                await _hubConnection.InvokeAsync("SendMessage", message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message: {ex.Message}");
                throw;
            }
        }

        public async Task NotifyUserTypingAsync(int chatRoomId)
        {
            if (_hubConnection == null)
                throw new InvalidOperationException("Hub connection is not initialized");

            try
            {
                await _hubConnection.InvokeAsync("UserIsTyping", chatRoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error notifying typing: {ex.Message}");
                // Don't throw as this is not critical
            }
        }

        public async Task NotifyUserStoppedTypingAsync(int chatRoomId)
        {
            if (_hubConnection == null)
                throw new InvalidOperationException("Hub connection is not initialized");

            try
            {
                await _hubConnection.InvokeAsync("UserStoppedTyping", chatRoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error notifying stopped typing: {ex.Message}");
                // Don't throw as this is not critical
            }
        }

        public async Task<ConnectionInfo> GetConnectionInfoAsync()
        {
            if (_hubConnection == null)
                throw new InvalidOperationException("Hub connection is not initialized");

            try
            {
                return await _hubConnection.InvokeAsync<ConnectionInfo>("GetConnectionInfo");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting connection info: {ex.Message}");
                throw;
            }
        }

        public async Task SendHeartbeatAsync()
        {
            if (_hubConnection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.InvokeAsync("Heartbeat");
                    _logger.LogDebug("Heartbeat sent");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending heartbeat: {ex.Message}");
                }
            }
        }
    }
}