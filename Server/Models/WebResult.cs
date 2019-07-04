using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Models
{
	public class WebResult<T>
	{
		public bool Success { get; set; }

		public string Message { get; set; }

		public T Value { get; set; }
	}
}