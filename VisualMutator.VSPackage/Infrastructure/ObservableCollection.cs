﻿namespace PiotrTrzpil.VisualMutator_VSPackage.Infrastructure
{
    #region Usings

    using System.Collections.Generic;
    using System.Collections.Specialized;

    #endregion

    /// <summary>
    ///   Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    public class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        /// <summary>
        ///   Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class.
        /// </summary>
        public ObservableCollection()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name = "collection">collection: The collection from which the elements are copied.</param>
        /// <exception cref = "System.ArgumentNullException">The collection parameter cannot be null.</exception>
        public ObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        ///   Adds the elements of the specified collection to the end of the ObservableCollection(Of T).
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T i in collection)
            {
                Items.Add(i);
            }
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///   Removes the first occurence of each item in the specified collection from ObservableCollection(Of T).
        /// </summary>
        public void RemoveRange(IEnumerable<T> collection)
        {
            foreach (T i in collection)
            {
                Items.Remove(i);
            }
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///   Clears the current collection and replaces it with the specified item.
        /// </summary>
        public void Replace(T item)
        {
            ReplaceRange(new[] { item });
        }

        /// <summary>
        ///   Clears the current collection and replaces it with the specified collection.
        /// </summary>
        public void ReplaceRange(IEnumerable<T> collection)
        {
            var old = new List<T>(Items);
            Items.Clear();
            foreach (T i in collection)
            {
                Items.Add(i);
            }
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}