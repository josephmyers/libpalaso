using System;
using System.Collections.Generic;
using System.Linq;
using SIL.Code;

namespace SIL.WritingSystems
{
	/// <summary>
	/// This class forms the bases for managing collections of WritingSystemDefinitions. WritingSystemDefinitions
	/// can be registered and then retrieved and deleted by ID. The preferred use when editting a WritingSystemDefinition stored
	/// in the WritingSystemRepository is to Get the WritingSystemDefinition in question and then to clone it either via the
	/// Clone method on WritingSystemDefinition or via the MakeDuplicate method on the WritingSystemRepository. This allows
	/// changes made to a WritingSystemDefinition to be registered back with the WritingSystemRepository via the Set method,
	/// or to be discarded by simply discarding the object.
	/// Internally the WritingSystemRepository uses the WritingSystemDefinition's StoreId property to establish the identity of
	/// a WritingSystemDefinition. This allows the user to change the IETF language tag components and thereby the ID of a
	/// WritingSystemDefinition and the WritingSystemRepository to update itself and the underlying store correctly.
	/// </summary>
	public abstract class WritingSystemRepositoryBase : IWritingSystemRepository
	{

		private readonly Dictionary<string, WritingSystemDefinition> _writingSystems;
		private readonly Dictionary<string, DateTime> _writingSystemsToIgnore;

		private readonly Dictionary<string, string> _idChangeMap;

		public event EventHandler<WritingSystemIdChangedEventArgs> WritingSystemIdChanged;
		public event EventHandler<WritingSystemDeletedEventArgs> WritingSystemDeleted;
		public event EventHandler<WritingSystemConflatedEventArgs> WritingSystemConflated;

		/// <summary>
		/// </summary>
		protected WritingSystemRepositoryBase()
		{
			_writingSystems = new Dictionary<string, WritingSystemDefinition>(StringComparer.OrdinalIgnoreCase);
			_writingSystemsToIgnore = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
			_idChangeMap = new Dictionary<string, string>();
			//_sharedStore = LdmlSharedWritingSystemRepository.Singleton;
		}

		/// <summary>
		/// Gets the writing systems to ignore.
		/// </summary>
		protected IDictionary<string, DateTime> WritingSystemsToIgnore
		{
			get
			{
				return _writingSystemsToIgnore;
			}
		}

		/// <summary>
		/// Gets the changed IDs mapping.
		/// </summary>
		protected IDictionary<string, string> ChangedIds
		{
			get { return _idChangeMap; }
		}

		public virtual WritingSystemDefinition CreateNew()
		{
			return new WritingSystemDefinition();
		}

		public virtual WritingSystemDefinition CreateNew(string ietfLanguageTag)
		{
			return new WritingSystemDefinition(ietfLanguageTag);
		}

		public virtual void Conflate(string wsToConflate, string wsToConflateWith)
		{
			WritingSystemDefinition ws = _writingSystems[wsToConflate];
			RemoveDefinition(ws);
			if (WritingSystemConflated != null)
				WritingSystemConflated(this, new WritingSystemConflatedEventArgs(wsToConflate, wsToConflateWith));
		}

		/// <summary>
		/// Remove the specified WritingSystemDefinition.
		/// </summary>
		/// <param name="id">the StoreId of the WritingSystemDefinition</param>
		/// <remarks>
		/// Note that ws.StoreId may differ from ws.Id.  The former is the key into the
		/// dictionary, but the latter is what gets persisted to disk (and shown to the
		/// user).
		/// </remarks>
		public virtual void Remove(string id)
		{
			if (id == null)
				throw new ArgumentNullException("id");
			if (!_writingSystems.ContainsKey(id))
				throw new ArgumentOutOfRangeException("id");

			WritingSystemDefinition ws = _writingSystems[id];
			RemoveDefinition(ws);
			if (WritingSystemDeleted != null)
				WritingSystemDeleted(this, new WritingSystemDeletedEventArgs(id));
			//TODO: Could call the shared store to advise that one has been removed.
			//TODO: This may be useful if writing systems were reference counted.
		}

		protected virtual void RemoveDefinition(WritingSystemDefinition ws)
		{
			_writingSystems.Remove(ws.Id);
			if (_writingSystemsToIgnore.ContainsKey(ws.Id))
				_writingSystemsToIgnore.Remove(ws.Id);
			if (_writingSystemsToIgnore.ContainsKey(ws.IetfLanguageTag))
				_writingSystemsToIgnore.Remove(ws.IetfLanguageTag);
		}

		public abstract string WritingSystemIdHasChangedTo(string id);

		protected virtual void LastChecked(string id, DateTime dateModified)
		{
			_writingSystemsToIgnore[id] = dateModified;
		}

		public virtual bool CanSave(WritingSystemDefinition ws, out string path)
		{
			path = string.Empty;
			return true;
		}

		/// <summary>
		/// Removes all writing systems.
		/// </summary>
		protected void Clear()
		{
			_writingSystems.Clear();
		}

		public abstract bool WritingSystemIdHasChanged(string id);

		public bool Contains(string id)
		{
			// identifier should not be null, but some unit tests never define StoreId
			// on their temporary WritingSystemDefinition objects.
			return id != null && _writingSystems.ContainsKey(id);
		}

		public bool CanSet(WritingSystemDefinition ws)
		{
			if (ws == null)
			{
				return false;
			}
			return !(_writingSystems.Keys.Any(id => id.Equals(ws.IetfLanguageTag, StringComparison.OrdinalIgnoreCase)) &&
				ws.Id != _writingSystems[ws.IetfLanguageTag].Id);
		}

		public virtual void Set(WritingSystemDefinition ws)
		{
			if (ws == null)
			{
				throw new ArgumentNullException("ws");
			}

			//Check if this is a new writing system with a conflicting id
			if (!CanSet(ws))
				throw new ArgumentException(String.Format("Unable to set writing system '{0}' because this id already exists. Please change this writing system id before setting it.", ws.IetfLanguageTag));

			string oldId = _writingSystems.Where(kvp => kvp.Value.Id == ws.Id).Select(kvp => kvp.Key).FirstOrDefault();
			//??? How do we update
			//??? Is it sufficient to just set it, or can we not change the reference in case someone else has it too
			//??? i.e. Do we need a ws.Copy(WritingSystemDefinition)?
			if (!string.IsNullOrEmpty(oldId) && _writingSystems.ContainsKey(oldId))
				_writingSystems.Remove(oldId);
			_writingSystems[ws.IetfLanguageTag] = ws;

			if (!string.IsNullOrEmpty(oldId) && (oldId != ws.IetfLanguageTag))
			{
				UpdateChangedIds(oldId, ws.IetfLanguageTag);
				if (WritingSystemIdChanged != null)
					WritingSystemIdChanged(this, new WritingSystemIdChangedEventArgs(oldId, ws.IetfLanguageTag));
			}

			ws.Id = ws.IetfLanguageTag;
		}

		/// <summary>
		/// Updates the changed IDs mapping.
		/// </summary>
		protected void UpdateChangedIds(string oldId, string newId)
		{
			if (_idChangeMap.ContainsValue(oldId))
			{
				// if the oldid is in the value of key/value, then we can update the cooresponding key with the newId
				string keyToChange = _idChangeMap.First(pair => pair.Value == oldId).Key;
				_idChangeMap[keyToChange] = newId;
			}
			else if (_idChangeMap.ContainsKey(oldId))
			{
				// if oldId is already in the dictionary, set the result to be newId
				_idChangeMap[oldId] = newId;
			}
		}

		/// <summary>
		/// Loads the changed IDs mapping from the existing writing systems.
		/// </summary>
		protected void LoadChangedIdsFromExistingWritingSystems()
		{
			_idChangeMap.Clear();
			foreach (var pair in _writingSystems)
				_idChangeMap[pair.Key] = pair.Key;
		}

		public bool TryGet(string id, out WritingSystemDefinition ws)
		{
			if (Contains(id))
			{
				ws = Get(id);
				return true;
			}

			ws = null;
			return false;
		}

		public string GetNewIdWhenSet(WritingSystemDefinition ws)
		{
			if (ws == null)
			{
				throw new ArgumentNullException("ws");
			}
			return String.IsNullOrEmpty(ws.Id) ? ws.IetfLanguageTag : ws.Id;
		}

		public WritingSystemDefinition Get(string id)
		{
			if (id == null)
				throw new ArgumentNullException("id");
			if (!_writingSystems.ContainsKey(id))
				throw new ArgumentOutOfRangeException("id", String.Format("Writing system id '{0}' does not exist.", id));
			return _writingSystems[id];
		}

		public int Count
		{
			get
			{
				return _writingSystems.Count;
			}
		}

		public virtual void Save()
		{
		}

		protected virtual void OnChangeNotifySharedStore(WritingSystemDefinition ws)
		{
			DateTime lastDateModified;
			if (_writingSystemsToIgnore.TryGetValue(ws.IetfLanguageTag, out lastDateModified) && ws.DateModified > lastDateModified)
				_writingSystemsToIgnore.Remove(ws.IetfLanguageTag);
		}

		protected virtual void OnRemoveNotifySharedStore()
		{
		}

		protected virtual IEnumerable<WritingSystemDefinition> WritingSystemsNewerIn(IEnumerable<WritingSystemDefinition> rhs)
		{
			if (rhs == null)
			{
				throw new ArgumentNullException("rhs");
			}
			var newerWritingSystems = new List<WritingSystemDefinition>();
			foreach (WritingSystemDefinition ws in rhs)
			{
				Guard.AgainstNull(ws, "ws in rhs");
				if (_writingSystems.ContainsKey(ws.IetfLanguageTag))
				{
					DateTime lastDateModified;
					if ((!_writingSystemsToIgnore.TryGetValue(ws.IetfLanguageTag, out lastDateModified) || ws.DateModified > lastDateModified)
						&& (ws.DateModified > _writingSystems[ws.IetfLanguageTag].DateModified))
					{
						newerWritingSystems.Add(ws.Clone());
					}
				}
			}
			return newerWritingSystems;
		}

		public IEnumerable<WritingSystemDefinition> AllWritingSystems
		{
			get
			{
				return _writingSystems.Values;
			}
		}
	}
}
