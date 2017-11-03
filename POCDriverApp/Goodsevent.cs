using System;
using Newtonsoft.Json;

namespace POCDriverApp
{
	public class Goodsevent
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "consignmentitemnumber")]
		public string Consignmentitemnumber { get; set; }

		[JsonProperty(PropertyName = "eventcode")]
		public string Eventcode { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }

    }

	public class GoodseventWrapper : Java.Lang.Object
	{
		public GoodseventWrapper (Goodsevent item)
		{
			Goodsevent = item;
		}

		public Goodsevent Goodsevent { get; private set; }
	}
}

