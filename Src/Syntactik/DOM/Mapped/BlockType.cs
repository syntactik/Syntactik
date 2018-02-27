namespace Syntactik.DOM.Mapped
{
    /// <summary>
    /// Represents type of the pair value.
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// Regular body.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Block is defined with {}
        /// </summary>
        JsonObject,
        /// <summary>
        /// Block is defined with []
        /// </summary>
        JsonArray

    }
}