namespace TelegramBotHost.Configurations {
    public class BotConfiguration {
        public string BotToken { get; set; } = string.Empty;
        public bool DropPendingUpdates { get; set; } = true;
    }
}
