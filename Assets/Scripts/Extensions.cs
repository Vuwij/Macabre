using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

public static class Extensions {

	/// <summary>
	/// Determines whether the collection is null or contains no elements.
	/// </summary>
	/// <typeparam name="T">The IEnumerable type.</typeparam>
	/// <param name="enumerable">The enumerable, which may be null or empty.</param>
	/// <returns>
	///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) {
		if (enumerable == null) {
			return true;
		}
		/* If this is a list, use the Count property for efficiency. 
	         * The Count property is O(1) while IEnumerable.Count() is O(N). */
		var collection = enumerable as ICollection<T>;
		if (collection != null) {
			return collection.Count < 1;
		}
		return false; 
	}
}

[XmlRoot ("dictionary")]
public class SDictionary<TKey, TValue>
		: Dictionary<TKey, TValue>, IXmlSerializable
{
	#region IXmlSerializable Members
	public System.Xml.Schema.XmlSchema GetSchema ()
	{
		return null;
	}

	public void ReadXml (System.Xml.XmlReader reader)
	{
		XmlSerializer keySerializer = new XmlSerializer (typeof (TKey));
		XmlSerializer valueSerializer = new XmlSerializer (typeof (TValue));

		bool wasEmpty = reader.IsEmptyElement;
		reader.Read ();

		if (wasEmpty)
			return;

		while (reader.NodeType != System.Xml.XmlNodeType.EndElement) {
			reader.ReadStartElement ("item");

			reader.ReadStartElement ("key");
			TKey key = (TKey)keySerializer.Deserialize (reader);
			reader.ReadEndElement ();

			reader.ReadStartElement ("value");
			TValue value = (TValue)valueSerializer.Deserialize (reader);
			reader.ReadEndElement ();

			this.Add (key, value);

			reader.ReadEndElement ();
			reader.MoveToContent ();
		}
		reader.ReadEndElement ();
	}

	public void WriteXml (System.Xml.XmlWriter writer)
	{
		XmlSerializer keySerializer = new XmlSerializer (typeof (TKey));
		XmlSerializer valueSerializer = new XmlSerializer (typeof (TValue));

		foreach (TKey key in this.Keys) {
			writer.WriteStartElement ("item");

			writer.WriteStartElement ("key");
			keySerializer.Serialize (writer, key);
			writer.WriteEndElement ();

			writer.WriteStartElement ("value");
			TValue value = this [key];
			valueSerializer.Serialize (writer, value);
			writer.WriteEndElement ();

			writer.WriteEndElement ();
		}
	}
	#endregion
}