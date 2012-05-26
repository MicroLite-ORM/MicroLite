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
        private static readonly IList<Type> listenerTypes = new List<Type>();
        private static readonly IList<Func<IListener>> listenerFactories = new List<Func<IListener>>();

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
                Func<IListener> listenerFactory = () =>
                {
                    return new T();
                };

                listenerFactories.Add(listenerFactory);

                listenerTypes.Add(listenerType);
            }
        }

        internal static IEnumerable<IListener> CreateListeners()
        {
            return listenerFactories.Select(x => x()).ToArray();
        }

        internal static void ClearListeners()
        {
            listenerFactories.Clear();
            listenerTypes.Clear();
        }
    }
}