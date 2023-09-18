using System;
using System.Collections.Generic;

namespace Delta
{
	public class Script
	{
		public string _id { get; set; }

		public string title { get; set; }

		public Game game { get; set; }

		public string features { get; set; }

		public List<object> tags { get; set; }

		public string script { get; set; }

		public Owner owner { get; set; }

		public string slug { get; set; }

		public bool verified { get; set; }

		public int views { get; set; }

		public string scriptType { get; set; }

		public bool isUniversal { get; set; }

		public bool isPatched { get; set; }

		public string visibility { get; set; }

		public int rawCount { get; set; }

		public bool showRawCount { get; set; }

		public DateTime createdAt { get; set; }

		public DateTime updatedAt { get; set; }

		public int __v { get; set; }

		public int likeCount { get; set; }

		public int dislikeCount { get; set; }

		public string id { get; set; }

		public bool liked { get; set; }

		public bool disliked { get; set; }

		public bool isFav { get; set; }
	}
}
