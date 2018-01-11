namespace Syntactik.DOM
{
    /// <summary>
    /// Enumerates possible values for assignment of the <see cref="Pair"/>.
    /// </summary>
    public enum AssignmentEnum
    {
        /// <summary>
        /// The <see cref="Pair"/> has no assignment.
        /// </summary>
        None,
        /// <summary>
        /// The <see cref="Pair"/> has a string assignment <c>=</c>
        /// </summary>
        E,
        /// <summary>
        /// The <see cref="Pair"/> has a string assignment <c>==</c>
        /// </summary>
        EE,
        /// <summary>
        /// The <see cref="Pair"/> has an object assignment <c>:</c>
        /// </summary>
        C,
        /// <summary>
        /// The <see cref="Pair"/> has a choice assignment <c>::</c>
        /// </summary>
        CC,
        /// <summary>
        /// The <see cref="Pair"/> has an array assignment <c>:::</c>
        /// </summary>
        CCC,
        /// <summary>
        /// The <see cref="Pair"/> has concatenation assignment <c>=:</c>
        /// </summary>
        EC,  // =:
        /// <summary>
        /// The <see cref="Pair"/> has a literal choice assignment <c>=::</c>
        /// </summary>
        ECC, // =::
        /// <summary>
        /// The <see cref="Pair"/> has a pair value delimeter <c>:=</c>
        /// </summary>
        CE, // :=
    }
}