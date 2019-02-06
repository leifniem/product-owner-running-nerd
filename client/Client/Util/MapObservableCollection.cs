using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace LoadRunnerClient.Util
{
	/// <summary>
	/// New Collection, that is Used like the <see cref="ObservableCollection{T}"/>, with the difference,
	/// that Lists of Elements can be added ad Once
	/// After adding, the <see cref="NotifyCollectionChangedEventHandler"/> is called just once with <see cref="NotifyCollectionChangedAction.Add"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MapObservableCollection<T> : Collection<T>, INotifyCollectionChanged
	{
		/// <summary>
		/// intern representation of Collection
		/// </summary>
		private IList<T> list = new List<T>();

		/// <summary>
		/// notifies all Observers
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Adds a List to the Collection 
		/// calls <see cref="CollectionChanged"/>
		/// </summary>
		/// <param name="list">Liste die hinzugefügt werden soll</param>
		public void AddAll(List<T> list)
		{
			foreach (T element in list)
			{
				this.list.Add(element);
			}
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list);
			if (CollectionChanged != null) CollectionChanged(this,args);
		}
		
		/// <summary>
		/// Adds a single element to the Collection
		/// </summary>
		/// <param name="t">element to be added</param>
		public new void Add(T t)
		{
			list.Add(t);
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t);
			if (CollectionChanged != null) CollectionChanged(this,args);
		}

		/// <summary>
		/// Removes one Element from List, in case it is in it
		/// </summary>
		/// <param name="t">element, to be removed</param>
		/// <returns>bool if removing was successfull</returns>
		public new bool Remove(T t)
		{
			bool removed = list.Remove(t);
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, t);
			if (CollectionChanged != null) CollectionChanged(this,args);
			return removed;
		}

		/// <summary>
		/// removes all elements in <paramref name="list"/>
		/// </summary>
		/// <param name="list">elemnts, to be removed</param>
		public void RemoveAll(List<T> list)
		{
			foreach (T element in list)
			{
				this.list.Remove(element);
			}
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list);
			if (CollectionChanged != null) CollectionChanged(this,args);
		}

		/// <summary>
		/// clears the hole Collection
		/// </summary>
		public new void Clear()
		{
			list.Clear();
			var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			if (CollectionChanged != null) CollectionChanged(this,args);
		}

		/// <summary>
		/// Removes all elements that fullfill <paramref name="condition"/> 
		/// </summary>
		/// <param name="condition">function, that returns bool and selects the elements to be deleted</param>
		/// <returns>count of all deleted elements</returns>
		public int RemoveAll(Func<T, bool> condition)
		{
			var itemsToRemove = this.list.Where(condition).ToList();
			RemoveAll(itemsToRemove);
			return itemsToRemove.Count();
		}

		/// <summary>
		/// Selects the first element that fullfills <paramref name="condition"/> 
		/// </summary>
		/// <param name="condition">function, that returns bool and selects the elements that fullfill <paramref name="condition"/></param>
		/// <returns>first element of selection</returns>
		public T First(Func<T, bool> condition)
		{
			var itemList = this.list.Where(condition).ToList();
			return itemList.First();
		}

		/// <summary>
		/// Returns all elements that fullfill <paramref name="condition"/> 
		/// </summary>
		/// <param name="condition">function, that returns bool and selects the elements to be returned</param>
		/// <returns>All elements that fullfill <paramref name="condition"/></returns>
		public IEnumerable<T> Where(Func<T,bool> condition)
		{
			return this.list.Where(condition);
		}


        /// <summary>
        /// Returns if element <paramref name="ele"/> is in Collection
        /// </summary>
        /// <param name="ele"></param>
        /// <returns></returns>
        public new bool Contains(T ele)
        {
            return list.Contains(ele);
        }



	}
}
