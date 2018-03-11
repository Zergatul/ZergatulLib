namespace Zergatul.Network.Smtp
{
    public enum SmtpReplyCode
    {
        /// <summary>
        /// Service ready
        /// </summary>
        ServiceReady = 220,

        /// <summary>
        /// Service closing transmission channel
        /// </summary>
        ChannelClosing = 221,

        /// <summary>
        /// Authentication Succeeded
        /// </summary>
        AuthSucceded = 235,

        /// <summary>
        /// Requested mail action okay, completed
        /// </summary>
        OK = 250,

        /// <summary>
        /// Start mail input
        /// </summary>
        StartInput = 354,

        /// <summary>
        /// Authentication mechanism is too weak
        /// </summary>
        AuthenticationMechanismTooWeak = 534,

        /// <summary>
        /// Authentication credentials invalid
        /// </summary>
        InvalidCredentials = 535,

        /// <summary>
        /// Requested action not taken: mailbox unavailable (e.g., mailbox not found, no access, or command rejected for policy reasons)
        /// </summary>
        RequestedActionNotTaken = 550
    }
}