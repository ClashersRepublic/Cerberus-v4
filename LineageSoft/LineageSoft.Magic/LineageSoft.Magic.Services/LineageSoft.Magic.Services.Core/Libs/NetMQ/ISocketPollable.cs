namespace LineageSoft.Magic.Services.Core.Libs.NetMQ
{
    using JetBrains.Annotations;

    /// <summary>
    ///     Implementations provide a <see cref="NetMQSocket" /> via the <see cref="Socket" /> property.
    /// </summary>
    public interface ISocketPollable
    {
        /// <summary>
        ///     Gets a <see cref="NetMQSocket" /> instance.
        /// </summary>
        [NotNull]
        NetMQSocket Socket { get; }
    }
}