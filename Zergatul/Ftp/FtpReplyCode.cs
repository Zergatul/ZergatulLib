using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zergatul.Ftp
{
    public enum FtpReplyCode
    {
        NoCode = 0,

        /// <summary>
        /// Restart marker reply.
        ///     In this case, the text is exact and not left to the
        ///     particular implementation; it must read:
        ///          MARK yyyy = mmmm
        ///     Where yyyy is User-process data stream marker, and mmmm
        ///     server's equivalent marker (note the spaces between markers
        ///     and "=").
        /// </summary>
        RestartMarkerReply = 110,

        /// <summary>
        /// Service ready in nnn minutes.
        /// </summary>
        ServiceReadyInMinutes = 120,

        /// <summary>
        /// Data connection already open; transfer starting.
        /// </summary>
        DataConnectionAlreadyOpen = 125,

        /// <summary>
        /// File status okay; about to open data connection.
        /// </summary>
        FileStatusOkay = 150,

        /// <summary>
        /// Command okay.
        /// </summary>
        CommandOkay = 200,

        /// <summary>
        /// Command not implemented, superfluous at this site.
        /// </summary>
        CommandNotImplementedSuperfluous = 202,

        /// <summary>
        /// System status, or system help reply.
        /// </summary>
        SystemStatus = 211,

        /// <summary>
        /// Directory status.
        /// </summary>
        DirectoryStatus = 212,

        /// <summary>
        /// File status.
        /// </summary>
        FileStatus = 213,

        /// <summary>
        /// Help message.
        ///     On how to use the server or the meaning of a particular
        ///     non-standard command.  This reply is useful only to the
        ///     human user.
        /// </summary>
        HelpMessage = 214,

        /// <summary>
        /// NAME system type.
        ///     Where NAME is an official system name from the list in the
        ///     Assigned Numbers document.
        /// </summary>
        SystemType = 215,

        /// <summary>
        /// Service ready for new user.
        /// </summary>
        ServiceReadyForNewUser = 220,

        /// <summary>
        /// Service closing control connection.
        ///     Logged out if appropriate.
        /// </summary>
        ServiceClosingControlConnection = 221,

        /// <summary>
        /// Data connection open; no transfer in progress.
        /// </summary>
        DataConnectionOpen = 225,

        /// <summary>
        /// Closing data connection.
        ///     Requested file action successful (for example, file
        ///     transfer or file abort).
        /// </summary>
        ClosingDataConnection = 226,

        /// <summary>
        /// Entering Passive Mode (h1,h2,h3,h4,p1,p2).
        /// </summary>
        EnteringPassiveMode = 227,

        /// <summary>
        /// User logged in, proceed.
        /// </summary>
        UserLoggedIn = 230,

        /// <summary>
        /// Requested file action okay, completed.
        /// </summary>
        RequestedFileActionOkay = 250,

        /// <summary>
        /// "PATHNAME" created.
        /// </summary>
        PathnameCreated = 257,

        /// <summary>
        /// User name okay, need password.
        /// </summary>
        UserNameOkayNeedPassword = 331,

        /// <summary>
        /// Need account for login.
        /// </summary>
        NeedAccountForLogin = 332,

        /// <summary>
        /// Requested file action pending further information.
        /// </summary>
        RequestedFileActionPendingInformation = 350,

        /// <summary>
        /// Service not available, closing control connection.
        ///     This may be a reply to any command if the service knows it
        ///     must shut down.
        /// </summary>
        ServiceNotAvailable = 421,

        /// <summary>
        /// Can't open data connection.
        /// </summary>
        CantOpenDataConnection = 425,

        /// <summary>
        /// Connection closed; transfer aborted.
        /// </summary>
        ConnectionClosed = 426,

        /// <summary>
        /// Requested file action not taken.
        ///     File unavailable (e.g., file busy).
        /// </summary>
        RequestedFileActionNotTakenFileUnavailable = 450,

        /// <summary>
        /// Requested action aborted. Local error in processing.
        /// </summary>
        RequestedActionAbortedLocalError = 451,

        /// <summary>
        /// Requested action not taken.
        ///     Insufficient storage space in system.
        /// </summary>
        RequestedActionNotTakenInsufficientStorage = 452,

        /// <summary>
        /// Syntax error, command unrecognized.
        /// </summary>
        SyntaxError = 500,

        /// <summary>
        /// Syntax error in parameters or arguments.
        /// </summary>
        SyntaxErrorInParameters = 501,

        /// <summary>
        /// Command not implemented.
        /// </summary>
        CommandNotImplemented = 502,

        /// <summary>
        /// Bad sequence of commands.
        /// </summary>
        BadSequence = 503,

        /// <summary>
        /// Command not implemented for that parameter.
        /// </summary>
        CommandNotImplementedForThatParameter = 504,

        /// <summary>
        /// Not logged in.
        /// </summary>
        NotLoggedIn = 530,

        /// <summary>
        /// Need account for storing files.
        /// </summary>
        NeedAccountForStoringFiles = 532,

        /// <summary>
        /// Requested action not taken.
        ///     File unavailable (e.g., file not found, no access).
        /// </summary>
        RequestedActionNotTakenFileUnavailable2 = 550,

        /// <summary>
        /// Requested action aborted. Page type unknown.
        /// </summary>
        RequestedActionAbortedPageTypeUnknown = 551,

        /// <summary>
        /// Requested file action aborted.
        ///     Exceeded storage allocation (for current directory or
        ///     dataset).
        /// </summary>
        RequestedFileActionAborted = 552,

        /// <summary>
        /// Requested action not taken.
        ///     File name not allowed.
        /// </summary>
        RequestedActionNotTakenInvalidFileName = 553
    }
}
