namespace MicroLite.Syntax
{
    /// <summary>
    /// The interface which specifies a parameter and argument for a stored procedure in the fluent sql builder syntax.
    /// </summary>
    public interface IWithParameter : IToSqlQuery, IHideObjectMembers
    {
        /// <summary>
        /// Specifies that the stored procedure should be executed the supplied parameter and argument.
        /// </summary>
        /// <param name="parameter">The parameter to be added.</param>
        /// <param name="arg">The argument value for the parameter.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWithParameter WithParameter(string parameter, object arg);
    }
}