using Verse;
using UnityEngine;
using System.Text;

namespace flangoCore
{
	public enum TimeFormat
    {
		Seconds,
		Minutes,
		Hours,
		Days,
		Quadrums,
		Years,
		RealSeconds,
		RealMinutes,
		RealHours,
		RealDays
    }

	public class HediffCompProperties_SeverityPerDayExtra : HediffCompProperties
	{
		public float severityPerDay;
		public bool showTimeToRecover;
		public bool onlyShowInInfo;
		public TimeFormat timeFormat = TimeFormat.Hours;
		public int decimalPlaces = -1;
		public int updateInterval = 200; // Use 60 for every second, 30 for every 0.5 sec, 6 for 0.1 sec

		public HediffCompProperties_SeverityPerDayExtra()
		{
			compClass = typeof(HediffComp_SeverityPerDayExtra);
		}
	}

	public class HediffComp_SeverityPerDayExtra : HediffComp
	{
		//protected const int SeverityUpdateInterval = 200;

		private HediffCompProperties_SeverityPerDayExtra Props => (HediffCompProperties_SeverityPerDayExtra)props;

		public override string CompLabelInBracketsExtra
		{
			get
			{
				if (!Props.onlyShowInInfo && props is HediffCompProperties_SeverityPerDayExtra && SeverityChangePerDay() < 0f)
				{
					var (multiplier, timeString, format) = GetMultiplierTimeFormat(Props.timeFormat, Props.decimalPlaces);

					return (parent.Severity / Mathf.Abs(SeverityChangePerDay()) * multiplier).ToString(format) + timeString.Translate();
				}
				return null;
			}
		}

		public override string CompTipStringExtra
		{
			get
			{
				if (Props.onlyShowInInfo && props is HediffCompProperties_SeverityPerDayExtra && SeverityChangePerDay() < 0f)
				{
					var (multiplier, timeString, format) = GetMultiplierTimeFormat(Props.timeFormat, Props.decimalPlaces);

					return timeString.Translate((parent.Severity / Mathf.Abs(SeverityChangePerDay()) * multiplier).ToString(format));
				}
				return null;
			}
		}

		public (float, string, string) GetMultiplierTimeFormat(TimeFormat timeFormat, int decimalPlaces)
        {
			float multiplier;
			string timeString;
			string format = "0.";
			switch (timeFormat)
			{
				case TimeFormat.Seconds:
					multiplier = 86400f;
					timeString = Props.onlyShowInInfo ? "SecondsToRecover" : "LetterSecond";
					break;
				case TimeFormat.Minutes:
					multiplier = 1440f;
					timeString = Props.onlyShowInInfo ? "MinutesToRecover" : "LetterMinute";
					break;
				case TimeFormat.Hours:
					multiplier = 24f;
					timeString = Props.onlyShowInInfo ? "HoursToRecover" : "LetterHour";
					break;
				case TimeFormat.Days:
					multiplier = 1f;
					timeString = Props.onlyShowInInfo ? "DaysToRecover" : "LetterDay";
					break;
				case TimeFormat.Quadrums:
					multiplier = 0.066667f;
					timeString = Props.onlyShowInInfo ? "QuadrumsToRecover" : "LetterQuadrum";
					break;
				case TimeFormat.Years:
					multiplier = 0.016667f;
					timeString = Props.onlyShowInInfo ? "YearsToRecover" : "LetterYear";
					break;

				case TimeFormat.RealSeconds:
					multiplier = 1000f;
					timeString = Props.onlyShowInInfo ? "RealSecondsToRecover" : "LetterSecond";
					break;
				case TimeFormat.RealMinutes:
					multiplier = 16.66667f;
					timeString = Props.onlyShowInInfo ? "RealMinutesToRecover" : "LetterMinute";
					break;
				case TimeFormat.RealHours:
					multiplier = 0.277778f;
					timeString = Props.onlyShowInInfo ? "RealHoursToRecover" : "LetterHour";
					break;
				case TimeFormat.RealDays:
					multiplier = 0.011574074f; // i hope i got this right
					timeString = Props.onlyShowInInfo ? "RealDaysToRecover" : "LetterDay";
					break;

				default:
					multiplier = 24f;
					timeString = "LetterHour";
					break;
			}
			if (decimalPlaces == 0) format = "0";
			else if (decimalPlaces <= -1) format = "0.0";
			else format = format.PadRight(decimalPlaces + 2, '0');

			return (multiplier, timeString, format);
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (Pawn.IsHashIntervalTick(Props.updateInterval))
			{
				float num = SeverityChangePerDay();
				num *= 0.00333333341f/200*Props.updateInterval;
				severityAdjustment += num;
			}
		}

		public virtual float SeverityChangePerDay()
		{
			return Props.severityPerDay;
		}

		public override string CompDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.CompDebugString());
			if (!Pawn.Dead)
			{
				stringBuilder.AppendLine("severity/day: " + SeverityChangePerDay().ToString("F3"));
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}
	}

}
