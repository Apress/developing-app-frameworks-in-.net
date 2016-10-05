using System;
using System.Collections;

namespace SAF.Cache
{
	/// <summary>
	/// the sample cache strategy implementation which 
	/// shows how you create an pluggable component for SAF.Cache 
	/// to customize the way object is cahced and retrieved.
	/// </summary>
	public class DefaultCacheStrategy : ICacheStrategy
	{
		private Hashtable objectTable;
		
		/// <summary>
		/// constructor to instantiate the internal hashtable.
		/// </summary>
		public DefaultCacheStrategy()
		{
			objectTable = new Hashtable();
		}

		/// <summary>
		/// Add an object to the underlying storage
		/// </summary>
		/// <param name="objId">key for the object</param>
		/// <param name="o">object</param>
		public void AddObject(string objId, object o)
		{
			objectTable.Add(objId,o);
		}
		/// <summary>
		/// Remove an object from the underlying storage
		/// </summary>
		/// <param name="objId">key for the object</param>
		public void RemoveObject(string objId)
		{
			objectTable.Remove(objId);
		}
		/// <summary>
		/// Retrieve an object from the underlying storage
		/// </summary>
		/// <param name="objId">key for the object</param>
		/// <returns>object</returns>
		public object RetrieveObject(string objId)
		{

			return objectTable[objId];
		}
	}
}
