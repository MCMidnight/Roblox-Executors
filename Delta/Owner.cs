using System;

namespace Delta
{
	public class Owner
	{
		public string _id { get; set; }

		public string username { get; set; }

		public bool verified { get; set; }

		public string role { get; set; }

		public bool isBanned { get; set; }

		public string profilePicture { get; set; }

		public DateTime createdAt { get; set; }

		public DateTime lastActive { get; set; }

		public bool online { get; set; }

		public bool idle { get; set; }

		public bool offline { get; set; }

		public string id { get; set; }
	}
}
