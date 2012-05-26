namespace MicroLite
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The class which manages extensions to the MicroLite ORM framework.
    /// </summary>
    internal static class ExtensionManager
    {
        private static readonly IList<Func<IListener>> listenerFactories = new List<Func<IListener>>();
        private static readonly IList<Type> listenerTypes = new List<Type>();

        internal static void ClearListeners()
        {
            listenerFactories.Clear();
            listenerTypes.Clear();
        }

        internal static IEnumerable<IListener> CreateListeners()
        {
            return listenerFactories.Select(x => x()).ToArray();
        }

        /// <summary>
        /// Registers the listener.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IListener"/> to register.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The syntax is accepted practice for registering types.")]
        internal static void RegisterListener<T>()
            where T : IListener, new()
        {
            var listenerType = typeof(T);

            if (!listenerTypes.Contains(listenerType))
            {
                RegisterListener(() =>
                {
                    return new T();
                });

                listenerTypes.Add(listenerType);
            }
        }

        internal static void RegisterListener(Func<IListener> listenerFactory)
        {
            listenerFactories.Add(listenerFactory);
        }
    }
}