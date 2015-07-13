using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Forcefield.Forcefields;

namespace Forcefield
{
	public class ForceFieldUser
	{
		public bool Enabled { get; set; }
		private List<IForcefield> Fields { get; set; }
		public int FieldCount { get { return Fields.Count; } }

		private readonly Dictionary<string, object> _extensions = new Dictionary<string, object>();

		public bool HasField(IForcefield field)
		{
			return Fields.Contains(field);
		}

		public void AddField(IForcefield field)
		{
			Fields.Add(field);
		}

		public void RemoveField(IForcefield field)
		{
			Fields.Remove(field);
		}

		public void ClearFields()
		{
			Fields.Clear();
		}

		public object this[string name]
		{
			get { return _extensions[name]; }
			set { _extensions[name] = value; }
		}

		public bool HasProperty(string name)
		{
			return _extensions.Keys.Contains(name);
		}

		public void SetProperty(string name, object value)
		{
			_extensions[name] = value;
		}

		public ForceFieldUser()
		{
			Enabled = false;
			Fields = new List<IForcefield>();
		}
	}
}