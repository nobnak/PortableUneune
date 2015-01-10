using UnityEngine;
using System.Collections;
using nobnak.Timer;

namespace UneuneSpots {

	public static class TimeUtil {
		public static readonly System.DateTime BASE_TIME = new System.DateTime(2014, 1, 1, 0, 0, 0, System.DateTimeKind.Local);

		public static double Time2014 {
			get { return (HighResTime.LocalNow - BASE_TIME).TotalSeconds; }
		}
		public static System.TimeSpan Span2014 {
			get { return HighResTime.LocalNow - BASE_TIME; }
		}
		public static Vector4 ShaderTime2014 {
			get {
				var from2014 = Span2014;
				var today = HighResTime.LocalNow - System.DateTime.Today;
				var totalYears = from2014.TotalDays / 365.0;
				return new Vector4((float)(totalYears * 13.0), (float)(totalYears * 61.0), (float)today.TotalHours, 0f);
			}
		}
	}
}
